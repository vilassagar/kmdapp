using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using System.Data;

namespace KMDRecociliationApp.API.Common
{
    public class Constants
    {
        public const string CORS_ORIGINS = "CorsOrigins";
    }
    public class CommonHelper
    {
        public const string RetireeRoleName = "Retiree";
        public const string AssociationRoleName = "Association";
        public const string AdminRoleName = "Admin";
        public const string LogFolderPath = "C:\\logs\\Api";
        public const string KMDSECRETAPIKEY = "SecretAPIKey";
        public static DataTable ConvertListToDataTable<T>(List<T> dataList)
        {
            DataTable dataTable = new DataTable();

            // Get the properties of the object type
            var properties = typeof(T).GetProperties();

            // Create columns in DataTable based on the properties of the object
            foreach (var property in properties)
            {
                dataTable.Columns.Add(property.Name, property.PropertyType);
            }

            // Populate the DataTable with data from the list
            foreach (var item in dataList)
            {
                DataRow row = dataTable.NewRow();

                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item);
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
        public static ApplicationUser CopyUser(UserDTO userDTO)
        {
            var copy = new ApplicationUser();
            copy.FirstName = userDTO.FirstName;
            copy.LastName = userDTO.LastName;
            copy.Email = userDTO.Email;
            copy.CountryCode = userDTO.CountryCode;
            copy.MobileNumber = userDTO.MobileNumber;
           // copy.Gender = userDTO.Gender;
            copy.DOB = userDTO.DateOfBirth;
            return copy;
        }
    }
}
