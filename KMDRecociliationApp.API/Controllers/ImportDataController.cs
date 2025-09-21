using ExcelDataReader;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Data;

using KMDRecociliationApp.Data;
//using Aspose.Cells;
using KMDRecociliationApp.Domain.Metadata;
using KMDRecociliationApp.Domain.Enum;
using ClosedXML.Excel;


namespace KMDRecociliationApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportDataController : ControllerBase
    {
        private readonly ImportDataRepository _importDataRepository;

        public ImportDataController(ImportDataRepository importDataRepository)
        {
            _importDataRepository = importDataRepository;
        }

        [HttpPost]
        [Route("BulkUploadApplicationUsers")]
        public IActionResult BulkUploadApplicationUsers([FromForm] ApplicationUserTemplate applicationUserTemplate)
        {
            List<string> messages = new List<string>();
            if (applicationUserTemplate.template == null || applicationUserTemplate.template.Length <= 0)
            {
                messages.Add("File should not be empty.");
                return BadRequest(messages);
            }

            if (applicationUserTemplate.template.Length > 0)
            {
                var stream = applicationUserTemplate.template.OpenReadStream();
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using var reader = ExcelReaderFactory.CreateReader(stream); // No need to cast to IExcelDataReader

                DataSet ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    UseColumnDataType = false,
                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                if (ds == null || ds.Tables.Count != 2)
                {
                    messages.Add("Invalid Template");
                    return BadRequest(messages);
                }

                bool isvalid = ds.Tables.Count == 2 &&
                    ds.Tables[0].TableName == "User" && ds.Tables[0].Columns.Count == 13 &&
                    ds.Tables[1].TableName == "Metadata";

                if (!isvalid)
                {
                    messages.Add("Invalid Template");
                    return BadRequest(messages);
                }

                messages = _importDataRepository.BulkUploadApplicationUsers(ds, "", "");
                if (messages.Count == 1 && messages.FirstOrDefault().Contains("successfully"))
                {
                    return Ok(messages);
                }
                else
                {
                    return BadRequest(messages);
                }
            }
            else
            {
                return BadRequest(messages);
            }
        }

        [HttpPost]
        [Route("BulkUploadAssociation")]
        public IActionResult BulkUploadAssociation([FromForm] AssociationTemplate associationTemplate)
        {
            List<string> messages = new List<string>();
            if (associationTemplate.template == null || associationTemplate.template.Length <= 0)
            {
                messages.Add("File should not be empty.");
                return BadRequest(messages);
            }

            if (associationTemplate.template.Length > 0)
            {
                var stream = associationTemplate.template.OpenReadStream();
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using var reader = ExcelReaderFactory.CreateReader(stream); // No need to cast to IExcelDataReader

                DataSet ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    UseColumnDataType = false,
                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                if (ds == null || ds.Tables.Count != 4)
                {
                    messages.Add("Invalid Template");
                    return BadRequest(messages);
                }

                bool isvalid = ds.Tables.Count == 4 &&
                    ds.Tables[0].TableName == "Association" && ds.Tables[0].Columns.Count == 17 &&
                    ds.Tables[1].TableName == "Contact" && ds.Tables[1].Columns.Count == 5 &&
                    ds.Tables[2].TableName == "Message" && ds.Tables[2].Columns.Count == 3 &&
                    ds.Tables[3].TableName == "Metadata";

                if (!isvalid)
                {
                    messages.Add("Invalid Template");
                    return BadRequest(messages);
                }

                messages = _importDataRepository.BulkUploadAssociation(ds, "", "");
                if (messages.Count == 1 && messages.FirstOrDefault().Contains("successfully"))
                {
                    return Ok(messages);
                }
                else
                {
                    return BadRequest(messages);
                }
            }
            else
            {
                return BadRequest(messages);
            }
        }

        [HttpPost]
        [Route("BulkUploadProducts")]
        public IActionResult BulkUploadBaseProducts([FromForm] ProductTemplate productTemplate)
        {
            List<string> messages = new List<string>();
            if (productTemplate.template == null || productTemplate.template.Length <= 0)
            {
                messages.Add("File should not be empty.");
                return BadRequest(messages);
            }

            if (productTemplate.template.Length > 0)
            {
                var stream = productTemplate.template.OpenReadStream();
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using var reader = ExcelReaderFactory.CreateReader(stream); // No need to cast to IExcelDataReader

                DataSet ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    UseColumnDataType = false,
                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                if (ds == null || ds.Tables.Count != 3)
                {
                    messages.Add("Invalid Template");
                    return BadRequest(messages);
                }

                bool isvalid = ds.Tables.Count == 3 &&
                    ds.Tables[0].TableName == "Product" && ds.Tables[0].Columns.Count == 9 &&
                    ds.Tables[1].TableName == "PremiumChart" && ds.Tables[1].Columns.Count == 6 &&
                    ds.Tables[2].TableName == "Metadata" && ds.Tables[2].Columns.Count == 2;

                if (!isvalid)
                {
                    messages.Add("Invalid Template");
                    return BadRequest(messages);
                }

                messages = _importDataRepository.BulkUploadBaseProducts(ds, "", "");
                if (messages.Count == 1 && messages.FirstOrDefault().Contains("successfully"))
                {
                    return Ok(messages);
                }
                else
                {
                    return BadRequest(messages);
                }
            }
            else
            {
                return BadRequest(messages);
            }
        }
        

        // Your existing controller method
        [HttpPost]
        [Route("DownloadApplicationUsersTemplate")]
        public IActionResult DownloadApplicationUsersTemplate()
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "UserTemplate.xlsx");

                // Load the template Excel file using ClosedXML
                using (var workbook = new XLWorkbook(filePath))
                {
                    UpdateMetadataCell(workbook);

                    // Save the workbook to a memory stream
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UserTemplate.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new { result = false, msg = ex.Message });
            }
        }

        private void UpdateMetadataCell(XLWorkbook workbook)
        {
            var metadata = _importDataRepository.GetAllApplicationUserMetaData();
            var metadataList = _importDataRepository.GetTemplateAndMetadataList("UserTemplate");

            foreach (var item in metadataList)
            {
                var worksheet = workbook.Worksheet(item.Worksheet);
                var metaDataValues = GetMetadataValues(metadata, item.ColumnKey);

                // Import the metadata values into the worksheet starting from row 2 (assuming row 1 is header)
                for (int i = 0; i < metaDataValues.Count; i++)
                {
                    worksheet.Cell(i + 2, item.ColumnNumber + 1).Value = metaDataValues[i]; // ClosedXML uses 1-based indexing
                }
            }
        }

        private static List<string> GetMetadataValues(ApplicationUserMetaData applicationUserMetaData, string columnKey)
        {
            var key = Enum.Parse(typeof(ApplicationUserMetadataKeys), columnKey);

            switch (key)
            {
                case ApplicationUserMetadataKeys.UserType:
                    return applicationUserMetaData.UserType.Select(a => a.Name).ToList();
                case ApplicationUserMetadataKeys.Gender:
                    return applicationUserMetaData.Gender.Select(a => a.Name).ToList();
                case ApplicationUserMetadataKeys.CountryCode:
                    return applicationUserMetaData.CountryCode.Select(a => a.Name).ToList();
                case ApplicationUserMetadataKeys.State:
                    return applicationUserMetaData.State.Select(a => a.Name).ToList();
                case ApplicationUserMetadataKeys.Association:
                    return applicationUserMetaData.Association.Select(a => a.Name).ToList();
                case ApplicationUserMetadataKeys.Organisations:
                    return applicationUserMetaData.Organisations.Select(a => a.Name).ToList();
                default:
                    throw new Exception("Data not found for " + key);
            }
        }

        [HttpPost]
        [Route("DownloadAssociationTemplate")]
        public IActionResult DownloadAssociationTemplate()
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "AssociationTemplate.xlsx");

                // Load the template Excel file using ClosedXML
                using (var workbook = new XLWorkbook(filePath))
                {
                    UpdateAssociationMetadataCell(workbook);

                    // Save the workbook to a memory stream
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AssociationTemplate.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new { result = false, msg = ex.Message });
            }
        }

        private void UpdateAssociationMetadataCell(XLWorkbook workbook)
        {
            var metadata = _importDataRepository.GetAllAssociationMetaData();
            var metadataList = _importDataRepository.GetTemplateAndMetadataList("AssociationTemplate");

            foreach (var item in metadataList)
            {
                var worksheet = workbook.Worksheet(item.Worksheet);
                var metaDataValues = GetAssociationMetadataValues(metadata, item.ColumnKey);

                // Import the metadata values into the worksheet starting from row 2 (assuming row 1 is header)
                for (int i = 0; i < metaDataValues.Count; i++)
                {
                    worksheet.Cell(i + 2, item.ColumnNumber + 1).Value = metaDataValues[i]; // ClosedXML uses 1-based indexing
                }
            }
        }

        private static List<string> GetAssociationMetadataValues(AssociationMetadata associationMetadata, string columnKey)
        {
            var key = Enum.Parse(typeof(AssociationMetadataKeys), columnKey);

            switch (key)
            {
                case AssociationMetadataKeys.YesNo:
                    return associationMetadata.YesNo.Select(a => a.Name).ToList();
                case AssociationMetadataKeys.States:
                    return associationMetadata.States.Select(a => a.Name).ToList();
                case AssociationMetadataKeys.Countries:
                    return associationMetadata.Countries.Select(a => a.Name).ToList();
                case AssociationMetadataKeys.Organisations:
                    return associationMetadata.Organisations.Select(a => a.Name).ToList();
                case AssociationMetadataKeys.Association:
                    return associationMetadata.Association.Select(a => a.Name).ToList();
                default:
                    throw new Exception("Data not found for " + key);
            }
        }


        //[HttpPost]
        //[Route("DownloadTopupProductTemplate")]
        //public IActionResult DownloadBaseProductTemplate()
        //{
        //    try
        //    {
        //        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "TopupProductTemplate.xlsx");

        //        // Load the template Excel file using ClosedXML
        //        using (var workbook = new XLWorkbook(filePath))
        //        {
        //            UpdateTopupProductMetadataCell(workbook);

        //            // Save the workbook to a memory stream
        //            using (var stream = new MemoryStream())
        //            {
        //                workbook.SaveAs(stream);
        //                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TopupProductTemplate.xlsx");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { result = false, msg = ex.Message });
        //    }
        //}

        //private void UpdateTopupProductMetadataCell(XLWorkbook workbook)
        //{
        //    var metadata = _importDataRepository.GetAllTopupProductMetaData();
        //    var metadataList = _importDataRepository.GetTemplateAndMetadataList("TopupProductTemplate");

        //    foreach (var item in metadataList)
        //    {
        //        var worksheet = workbook.Worksheet(item.Worksheet);
        //        var metaDataValues = GetTopupProductMetadataValues(metadata, item.ColumnKey);

        //        // Import the metadata values into the worksheet starting from row 2 (assuming row 1 is header)
        //        for (int i = 0; i < metaDataValues.Count; i++)
        //        {
        //            worksheet.Cell(i + 2, item.ColumnNumber + 1).Value = metaDataValues[i]; // ClosedXML uses 1-based indexing
        //        }
        //    }
        //}

        //private static List<string> GetTopupProductMetadataValues(BaseProductMetadata baseProductMetadata, string columnKey)
        //{
        //    var key = Enum.Parse(typeof(TopupProductMetadataKeys), columnKey);

        //    switch (key)
        //    {
        //    //    case AssociationMetadataKeys.YesNo:
        //    //        return associationMetadata.YesNo.Select(a => a.Name).ToList();
        //    //    case AssociationMetadataKeys.States:
        //    //        return associationMetadata.States.Select(a => a.Name).ToList();
        //    //    case AssociationMetadataKeys.Countries:
        //    //        return associationMetadata.Countries.Select(a => a.Name).ToList();
        //    //    case AssociationMetadataKeys.Organisations:
        //    //        return associationMetadata.Organisations.Select(a => a.Name).ToList();
        //    //    case AssociationMetadataKeys.Association:
        //    //        return associationMetadata.Association.Select(a => a.Name).ToList();
        //        default:
        //            throw new Exception("Data not found for " + key);
        //    }
        //}



    }
}