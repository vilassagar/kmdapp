using DocumentFormat.OpenXml.InkML;
using Irony.Ast;
using KMDRecociliationApp.Data.Helpers;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Metadata;
using Microsoft.AspNetCore.Http;
//using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SqlHelpers;

using System.Data;
using System.Data.SqlClient;


namespace KMDRecociliationApp.Data.Repositories
{
    public class ImportDataRepository : MainHeaderRepo<ApplicationUser>
    {
        private IMsSqlHelper msSqlHelper;
        ApplicationDbContext context = null;
        private readonly ILogger _logger;

        private readonly IConfiguration _configuration;
        public ImportDataRepository(ILoggerFactory logger
            , ApplicationDbContext appContext, IConfiguration configuration) : base(appContext)
        {
            context = appContext;
            _logger = logger.CreateLogger("ImportDataRepository");
            _configuration= configuration;
           msSqlHelper = new MsSqlHelper(_configuration.GetConnectionString("constr"));
        }

        public List<string> BulkUploadApplicationUsers(DataSet ds, string userName, string displayName)
        {
            List<string> message = new List<string>();

            // Remove blank rows and validate required fields
            DataTable applicationUserDataTable = ds.Tables[0].DropBlankRows();
            message = validateRequiredFields(applicationUserDataTable);

            if (message.Count > 0)
                return message;

            // Remove the first row if needed
            applicationUserDataTable = RemoveFirstRow(ds.Tables[0]);
            // Format the Date of Birth column
            foreach (DataRow row in applicationUserDataTable.AsEnumerable())
            {
                var date = row["Date of birth"];
                row["Date of birth"] = Convert.ToDateTime(date).ToString("MM/dd/yyyy");
            }
            // Fetch existing users from the database (synchronously)
            var existingUsers = GetExistingUsers();

            // Remove rows where the combination of FirstName, LastName, and MobileNumber already exists
            DataTable filteredTable = RemoveExistingRows(applicationUserDataTable, existingUsers);

            filteredTable.Columns.Add("Id", typeof(int));

            // Add sequential values to the new column
            int seqValue = 1; // Starting sequential number
            foreach (DataRow row in filteredTable.Rows)
            {
                row["Id"] = seqValue;
                seqValue++;
            }




            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(filteredTable.CreateSqlParameter("@ApplicationUser"));
            parameters.Add(userName.CreateSqlParameter("@UserName"));
            parameters.Add(displayName.CreateSqlParameter("@DisplayName"));

            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                 DBConstant.USPBULKUPLOADAPPLICATIONUSERS, parameters);
           

            // Optionally, add a success message
            message.Add("The Application Users have been successfully created.");

                return message;
        }
        
        public Dictionary<(string FirstName, string LastName, string MobileNumber), bool> GetExistingUsers()
        {

            var existingUsers = context.Applicationuser.AsNoTracking()
                .Select(u => new { u.FirstName, u.LastName, u.MobileNumber })
                .ToList();

            return existingUsers.ToDictionary(
                u => (u.FirstName, u.LastName, u.MobileNumber),
                u => true
            );

        }
        
        private List<string> validateRequiredFields(DataTable dt)
        {
            List<string> message = new List<string>();

            for (int j = 0; j < dt.Columns.Count; j++)
            {
                List<string> s = new List<string>();
                string Col = string.Empty;
                if (dt.Rows.Count > 1)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Col = dt.Rows[0][j].ToString();
                        s.Add(dt.Rows[i][j].ToString());
                    }
                    if (Col.Trim() == "Mandatory")
                    {
                        var finlst = s.Select(p => p.ToString()).Where(x => string.IsNullOrWhiteSpace(x)).ToList();

                        if (finlst.Count > 0)
                            message.Add($" {dt.Columns[j].ColumnName} is Required.");
                    }
                    //if (dt.Columns[j].ColumnName.Equals("Leagal Entity Name") || dt.Columns[j].ColumnName.Equals("Entity Code") || dt.Columns[j].ColumnName.Equals("Company Registration Number"))
                    //{
                    //    for (int i = 0; i < dt.Rows.Count; i++)
                    //    {
                    //        //Col = dt.Rows[0][j].ToString();
                    //        if (!dt.Rows[i][j].ToString().IsValidAlphanumeric())
                    //        {
                    //            message.Add($" {dt.Columns[j].ColumnName} accepts only Alphanumeric characters with -,_,\\,/ and space in the middle.");
                    //        }
                    //    }

                    //}

                }
                else
                {
                    message.Add($" {dt.Columns[j].ColumnName} is Required.");
                }
            }
            return message;
        }

        public async Task<Dictionary<(string FirstName, string LastName, string MobileNumber), bool>> GetExistingUsersAsync()
        {

            var existingUsers = await context.Applicationuser.AsNoTracking()
                .Select(u => new { u.FirstName, u.LastName, u.MobileNumber })
                .ToListAsync();

            return existingUsers.ToDictionary(
                u => (u.FirstName, u.LastName, u.MobileNumber),
                u => true
            );

        }

        public DataTable RemoveExistingRows(DataTable dataTable, Dictionary<(string FirstName, string LastName, string MobileNumber), bool> existingUsers)
        {
            DataTable filteredTable = dataTable.Clone();

            foreach (DataRow row in dataTable.Rows)
            {
                string firstName = row["First Name"].ToString();
                string lastName = row["Last Name"].ToString();
                string mobileNumber = row["Mobile Number"].ToString();

                if (!existingUsers.ContainsKey((firstName, lastName, mobileNumber)))
                {
                    filteredTable.ImportRow(row);
                }
            }

            return filteredTable;
        }

        DataTable RemoveFirstRow(DataTable dataTable)
        {
            /* var cols = dataTable.Columns.Cast<DataColumn>().ToArray()*/
            DataTable dest = dataTable.Clone();
            //DataTable dest = DropBlankRows( dataTable);
            foreach (DataColumn dataColumn in dest.Columns)
            {
                dataColumn.DataType = typeof(string);
            }
            //dest = DropBlankRows(dest);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {

                if (i != 0)
                {
                    DataRow drNew = dest.NewRow();
                    drNew.ItemArray = dataTable.Rows[i].ItemArray;
                    dest.Rows.Add(drNew);
                }
            }
            return dest;
        }

        public List<string> BulkuploadImportData(DataSet dataSet, IFormFile uploadedTemplate)
        {
            var messages = new List<string>();

            try
            {
                // Validate the uploaded template
                if (uploadedTemplate == null || uploadedTemplate.Length == 0)
                {
                    messages.Add("Uploded file is Empty");
                    return messages;
                }

                // Process the DataSet (if required in addition to the file)
                if (dataSet == null || dataSet.Tables.Count == 0)
                {
                    messages.Add("No data found in the DataSet.");
                    return messages;
                }

                foreach (DataTable table in dataSet.Tables)
                {
                    // Validate column structure
                    if (!table.Columns.Contains("First Name") || !table.Columns.Contains("Last Name"))
                    {
                        messages.Add("Invalid table structure. 'First Name' or 'Last Name' column is missing.");
                        continue;
                    }

                    foreach (DataRow row in table.Rows)
                    {
                        // Validate required fields
                        if (row["First Name"] == DBNull.Value || row["Last Name"] == DBNull.Value)
                        {
                            messages.Add("Invalid data: 'First Name' or 'Last Name' is missing in one or more rows.");
                            continue;
                        }

                        // Create entity object from DataRow and file upload
                        var importData = new ApplicationUserTemplate
                        {
                            template = uploadedTemplate // Assign the uploaded file to the template property
                        };

                        // Here you would typically save the importData to the database, along with other related data
                        bool isSaved = AddImportDataTemplate(importData);

                        if (isSaved)
                        {
                            messages.Add($"ImportData '{row["First Name"]}' uploaded successfully along with the template.");
                        }
                        else
                        {
                            messages.Add($"Failed to upload ImportData '{row["First Name"]}'.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle accordingly
                messages.Add($"An error occurred: {ex.Message}");
            }

            return messages;
        }

        private bool AddImportDataTemplate(ApplicationUserTemplate importData)
        {
            // Logic to save the entity to the database, including the uploaded file
            try
            {
                // Example of saving the file to the server
                var filePath = Path.Combine("path_to_save", importData.template.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    importData.template.CopyTo(stream);
                }

                // Additional logic to save importData information to the database

                return true;
            }
            catch
            {
                // Handle exceptions or errors during the save
                return false;
            }
        }

        public List<string> BulkUploadAssociation(DataSet ds, string userName, string displayName)
        {
            
            List<string> message = new List<string>();

            // Remove blank rows and validate required fields
            DataTable associationDataTable = ds.Tables[0].DropBlankRows();
            message = validateRequiredFields(associationDataTable);
            if (message.Count > 0) 
            { 
                return message;
            }
            // Remove the first row if needed
            associationDataTable = RemoveFirstRow(ds.Tables[0]);
            foreach (DataRow row in associationDataTable.Rows)
            {
                // Check if the value in the 'Accept Onepay Payment' column is 'Yes'
                if (row["Accept Onepay Payment"].ToString().ToLower() == "yes")
                {
                    row["Accept Onepay Payment"] = 1;
                }
                else
                {
                    row["Accept Onepay Payment"] = 0;
                }
            }

            // After modifying the DataTable, if you want to update the changes to the database,
            // you'll need to use a data adapter or a custom method depending on your setup.


            var contactDataTable = CloneTable(ds.Tables[1]);
            contactDataTable = contactDataTable.DropBlankRows();
            message = ValidateContactDataTable(associationDataTable, contactDataTable);
            if (message.Count > 0)
                return message;
            message = validateRequiredFields(contactDataTable);
            if (message.Count > 0)
                return message;
            // Remove the first row if needed
            contactDataTable = RemoveFirstRow(ds.Tables[1]);

            var messageDataTable = CloneTable(ds.Tables[2]);
            messageDataTable = messageDataTable.DropBlankRows();
            message = ValidateMessageDataTable(associationDataTable, messageDataTable);
            if (message.Count > 0)
                return message;
            message = validateRequiredFields(messageDataTable);
            if (message.Count > 0)
                return message;
            messageDataTable = RemoveFirstRow(ds.Tables[2]);

            
            // Fetch existing users from the database (synchronously)
            var existingAssociation = GetExistingAssociationNames();

            // Remove rows where the combination of FirstName, LastName, and MobileNumber already exists
            DataTable filteredTable = RemoveExistingAssociation(associationDataTable, existingAssociation);


            
            //var returnValue = new SqlParameter
            //{
            //    ParameterName = "@ReturnValue",
            //    SqlDbType = System.Data.SqlDbType.Int,
            //    Direction = System.Data.ParameterDirection.Output
            //};


            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(filteredTable.CreateSqlParameter("@Association"));
            parameters.Add(contactDataTable.CreateSqlParameter("@Contact"));
            parameters.Add(messageDataTable.CreateSqlParameter("@Message"));
            parameters.Add(userName.CreateSqlParameter("@UserName"));
            parameters.Add(displayName.CreateSqlParameter("@DisplayName"));

            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                    DBConstant.USPBULKUPLOADASSOCIATION, parameters);
            if (!result.IsIsNullOrEmpty())
            {
                foreach (DataRow dataRow in result.Tables[0].Rows)
                {
                    message.Add(dataRow.Field<string>("message"));
                }
            }


            // Optionally, add a success message
            message.Add("The Association have been successfully created.");

            return message;
            

        }

        public List<string> BulkUploadBaseProducts(DataSet ds, string userName, string displayName)
        {
            List<string> message = new List<string>();

            // Remove blank rows and validate required fields
            DataTable baseProductsDataTable = ds.Tables[0].DropBlankRows();
            message = validateRequiredFields(baseProductsDataTable);
            if (message.Count > 0)
            {
                return message;
            }
            // Remove the first row if needed
            baseProductsDataTable = RemoveFirstRow(ds.Tables[0]);
            
            // After modifying the DataTable, if you want to update the changes to the database,
            // you'll need to use a data adapter or a custom method depending on your setup.


            var premimumChartDataTable = CloneTable(ds.Tables[1]);
            premimumChartDataTable = premimumChartDataTable.DropBlankRows();
            message = ValidateBaseProductDataTable(baseProductsDataTable, premimumChartDataTable);
            if (message.Count > 0)
                return message;
            message = validateRequiredFields(premimumChartDataTable);
            if (message.Count > 0)
                return message;
            // Remove the first row if needed
            premimumChartDataTable = RemoveFirstRow(ds.Tables[1]);

            // Fetch existing users from the database (synchronously)
            var existingBaseProduct = GetExistingBaseProductNames();

            // Remove rows where the combination of FirstName, LastName, and MobileNumber already exists
            DataTable filteredTable = RemoveExistingBaseProduct(baseProductsDataTable, existingBaseProduct);

            IList<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(filteredTable.CreateSqlParameter("@BaseProduct"));
            parameters.Add(baseProductsDataTable.CreateSqlParameter("@PremimumChart"));
            parameters.Add(userName.CreateSqlParameter("@UserName"));
            parameters.Add(displayName.CreateSqlParameter("@DisplayName"));

            var result = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                    DBConstant.USPBULKUPLOADBASEPRODUCT, parameters);
            if (!result.IsIsNullOrEmpty())
            {
                foreach (DataRow dataRow in result.Tables[0].Rows)
                {
                    message.Add(dataRow.Field<string>("message"));
                }
            }


            // Optionally, add a success message
            message.Add("The Base product have been successfully created.");

            return message;

        }

        private DataTable RemoveExistingBaseProduct(DataTable baseProductsDataTable, List<string> existingBaseProduct)
        {
            // Create a copy of the original DataTable structure
            DataTable filteredTable = baseProductsDataTable.Clone();

            foreach (DataRow row in baseProductsDataTable.Rows)
            {
                string productName = row["Product Name"].ToString();

                // Check if the association name exists in the list of existing associations
                if (!existingBaseProduct.Contains(productName, StringComparer.OrdinalIgnoreCase))
                {
                    filteredTable.ImportRow(row);
                }
            }

            return filteredTable;
        }

        private List<string> ValidateBaseProductDataTable(DataTable dataTable, DataTable premimumChartDataTable)
        {
            //DataTable dataTable, DataTable contactDataTable
            List<string> message = new List<string>();
            if (dataTable.IsIsNullOrEmpty())
            {
                message.Add($"At least one Base Product is required!.");
                return message;
            }
            var table1 = (from row in dataTable.AsEnumerable()
                          select new
                          {
                              ProductName = row.Field<string>("Product Name")
                          }).ToList();
            var table2 = (from row in premimumChartDataTable.AsEnumerable()
                          select new
                          {
                              ProductName = row.Field<string>("Product Name")
                          }).ToList();

            var columns1NotRight = table1.Where(x => !table2.Any(c => c.ProductName == x.ProductName)).ToList();
            if (columns1NotRight.Count != 0)
            {
                message.Add($"Product Name mistmatch in Product sheet and Premium Chart sheet!.");
                return message;
            }
            return message;
        }

        private List<string> GetExistingBaseProductNames()
        {
            // Fetch all association names from the database
            return context.Product
                           .Select(a => a.ProductName)
                           .Where(name => !string.IsNullOrEmpty(name))
                           .ToList();
        }

        DataTable CloneTable(DataTable dataTable)
        {
            /* var cols = dataTable.Columns.Cast<DataColumn>().ToArray()*/

            DataTable dest = dataTable.Clone();
            foreach (DataColumn dataColumn in dest.Columns)
            {
                dataColumn.DataType = typeof(string);
            }

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {


                DataRow drNew = dest.NewRow();
                drNew.ItemArray = dataTable.Rows[i].ItemArray;
                dest.Rows.Add(drNew);

            }
            return dest;
        }

        private List<string> ValidateContactDataTable(DataTable dataTable, DataTable contactDataTable)
        {
            List<string> message = new List<string>();
            if (dataTable.IsIsNullOrEmpty())
            {
                message.Add($"At least one contact is required!.");
                return message;
            }
            var table1 = (from row in dataTable.AsEnumerable()
                          select new
                          {
                              AssociationName = row.Field<string>("Association Name")
                          }).ToList();
            var table2 = (from row in contactDataTable.AsEnumerable()
                          select new
                          {
                              AssociationName = row.Field<string>("Association Name")
                          }).ToList();

            var columns1NotRight = table1.Where(x => !table2.Any(c => c.AssociationName == x.AssociationName)).ToList();
            if (columns1NotRight.Count != 0)
            {
                message.Add($"Association Name mistmatch in Association sheet and Contact sheet!.");
                return message;
            }
            return message;
        }

        private List<string> ValidateMessageDataTable(DataTable dataTable, DataTable messageDataTable)
        {
            List<string> message = new List<string>();
            if (dataTable.IsIsNullOrEmpty())
            {
                message.Add($"At least one contact is required!.");
                return message;
            }
            var table1 = (from row in dataTable.AsEnumerable()
                          select new
                          {
                              AssociationName = row.Field<string>("Association Name")
                          }).ToList();
            var table2 = (from row in messageDataTable.AsEnumerable()
                          select new
                          {
                              AssociationName = row.Field<string>("Association Name")
                          }).ToList();

            var columns1NotRight = table1.Where(x => !table2.Any(c => c.AssociationName == x.AssociationName)).ToList();
            if (columns1NotRight.Count != 0)
            {
                message.Add($"Association Name mistmatch in Association sheet and Message sheet!.");
                return message;
            }
            return message;
        }

        public List<string> GetExistingAssociationNames()
        {
            // Fetch all association names from the database
            return context.Association
                           .Select(a => a.AssociationName)
                           .Where(name => !string.IsNullOrEmpty(name))
                           .ToList();
        }

        private DataTable RemoveExistingAssociation(DataTable associationDataTable, List<string> existingAssociations)
        {
            // Create a copy of the original DataTable structure
            DataTable filteredTable = associationDataTable.Clone();

            foreach (DataRow row in associationDataTable.Rows)
            {
                string associationName = row["Association Name"].ToString();

                // Check if the association name exists in the list of existing associations
                if (!existingAssociations.Contains(associationName, StringComparer.OrdinalIgnoreCase))
                {
                    filteredTable.ImportRow(row);
                }
            }

            return filteredTable;
        }

        public ApplicationUserMetaData GetAllApplicationUserMetaData()
        {
            var dataSet = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                    DBConstant.GETALLAPPLICATIONUSERMETADATA, null);

            var applicationUserMetaData = new ApplicationUserMetaData();

            if (dataSet.IsIsNullOrEmpty())
            {
                return applicationUserMetaData;
            }
            else
            {
                applicationUserMetaData.UserType= dataSet.Tables[0].MapDataTableToObjectList<CommonMetadataModel>();
                applicationUserMetaData.Gender = dataSet.Tables[1].MapDataTableToObjectList<CommonMetadataModel>();
                applicationUserMetaData.CountryCode = dataSet.Tables[2].MapDataTableToObjectList<CommonMetadataModel>();
                applicationUserMetaData.State = dataSet.Tables[3].MapDataTableToObjectList<CommonMetadataModel>();
                applicationUserMetaData.Association = dataSet.Tables[4].MapDataTableToObjectList<CommonMetadataModel>();
                applicationUserMetaData.Organisations = dataSet.Tables[5].MapDataTableToObjectList<CommonMetadataModel>();
                    
                return applicationUserMetaData;
            }
            
        }

        public List<MetaDataList> GetTemplateAndMetadataList(string templateName)
        {
            //public List<MetaDataList> GetTemplateAndMetadataList(string templateName)
            {
                IList<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(templateName.CreateSqlParameter("@TemplateName"));

                var dataSet = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                     DBConstant.GETALLMETADATALIST, parameters);

                List<MetaDataList> templateMetadataList = new();

                if (dataSet.IsIsNullOrEmpty())
                {
                    return templateMetadataList;
                }
                else
                {
                    return dataSet.Tables[0].MapDataTableToObjectList<MetaDataList>();
                }
            }

        }

        public AssociationMetadata GetAllAssociationMetaData()
        {
            var dataSet = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                    DBConstant.GETALLASSOCIATIONMETADATA, null);

            var associationMetadata = new AssociationMetadata();

            if (dataSet.IsIsNullOrEmpty())
            {
                return associationMetadata;
            }
            else
            {
                associationMetadata.YesNo = dataSet.Tables[0].MapDataTableToObjectList<CommonMetadataModel>();
                associationMetadata.States = dataSet.Tables[1].MapDataTableToObjectList<CommonMetadataModel>();
                associationMetadata.Countries = dataSet.Tables[2].MapDataTableToObjectList<CommonMetadataModel>();
                associationMetadata.Organisations = dataSet.Tables[3].MapDataTableToObjectList<CommonMetadataModel>();
                associationMetadata.Association = dataSet.Tables[4].MapDataTableToObjectList<CommonMetadataModel>();

                return associationMetadata;
            }

        }

        public List<MetaDataList> GetAssociationTemplateAndMetadataList(string templateName)
        {
            //public List<MetaDataList> GetAssociationTemplateAndMetadataList(string templateName)
            {
                IList<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(templateName.CreateSqlParameter("@TemplateName"));

                var dataSet = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                     DBConstant.GETALLMETADATALIST, parameters);

                List<MetaDataList> templateMetadataList = new();

                if (dataSet.IsIsNullOrEmpty())
                {
                    return templateMetadataList;
                }
                else
                {
                    return dataSet.Tables[0].MapDataTableToObjectList<MetaDataList>();
                }
            }
        }

        public BaseProductMetadata GetAllTopupProductMetaData()
        {            
            var dataSet = msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                    DBConstant.GETALLASSOCIATIONMETADATA, null);

            var baseProductMetadata = new BaseProductMetadata();

            if (dataSet.IsIsNullOrEmpty())
            {
                return baseProductMetadata;
            }
            else
            {
                //baseProductMetadata.YesNo = dataSet.Tables[0].MapDataTableToObjectList<CommonMetadataModel>();
                //baseProductMetadata.States = dataSet.Tables[1].MapDataTableToObjectList<CommonMetadataModel>();
                //baseProductMetadata.Countries = dataSet.Tables[2].MapDataTableToObjectList<CommonMetadataModel>();
                //baseProductMetadata.Organisations = dataSet.Tables[3].MapDataTableToObjectList<CommonMetadataModel>();
                //baseProductMetadata.Association = dataSet.Tables[4].MapDataTableToObjectList<CommonMetadataModel>();

                return baseProductMetadata;
            }           

        }

       
    }
}
