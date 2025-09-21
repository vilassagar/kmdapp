using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Data;
using Microsoft.AspNetCore.Mvc;
using ExcelDataReader;
using KMDRecociliationApp.Domain.Entities;
using System.Data;
using ClosedXML.Excel;

namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BulkReconcilationController : ApiBaseController
    {
            
        private readonly ApplicationDbContext context;    
        private readonly BulkReconcilationRepository _reconcilationRepository;
       
        public BulkReconcilationController(ILoggerFactory logger
            , ApplicationDbContext _context, BulkReconcilationRepository reconcilationRepository
            ) : base(context: _context)
        {
            _reconcilationRepository= reconcilationRepository;
            context = _context;
            
        }

        [HttpPost("BulkReconcilationCheque")]
        public async Task<IActionResult> BulkReconcilationCheque([FromForm] ReconcilationChequeTemplate chequeTemplate)
        {
            CurrentUser = HttpContext.User;
            List<string> messages = new List<string>();
            if (chequeTemplate.template == null || chequeTemplate.template.Length <= 0)
            {
                messages.Add("File should not be empty.");
                return BadRequest(messages);
            }

            if (chequeTemplate.template.Length > 0)
            {
                var stream = chequeTemplate.template.OpenReadStream();
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

                if (ds == null || ds.Tables.Count != 1)
                {
                    messages.Add("Invalid Template");
                    return BadRequest(messages);
                }

               
                messages =await _reconcilationRepository.BulkReconcilationCheque(ds,LoggedInUserId, chequeTemplate.CampaignId);
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

        [HttpPost("BulkReconcilationNEFT")]
        public async Task<IActionResult> BulkReconcilationNEFT([FromForm] ReconcilationNEFTTemplate neftTemplate)
        {
            CurrentUser = HttpContext.User;
            List<string> messages = new List<string>();
            if (neftTemplate.template == null || neftTemplate.template.Length <= 0)
            {
                messages.Add("File should not be empty.");
                return BadRequest(messages);
            }

            if (neftTemplate.template.Length > 0)
            {
                var stream = neftTemplate.template.OpenReadStream();
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

                if (ds == null || ds.Tables.Count != 1)
                {
                    messages.Add("Invalid Template");
                    return BadRequest(messages);
                }


                messages = await _reconcilationRepository.BulkReconcilationNEFT(ds, LoggedInUserId, neftTemplate.CampaignId);
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
        [Route("DownloadReconcilationChequeTemplate")]
        public IActionResult DownloadReconcilationChequeTemplate()
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ReconcilationChequeTemplate.xlsx");

                // Load the template Excel file using ClosedXML
                using (var workbook = new XLWorkbook(filePath))
                {
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

        [HttpPost]
        [Route("DownloadReconcilationNEFTTemplate")]
        public IActionResult DownloadReconcilationNEFTTemplate()
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ReconcilationChequeTemplate.xlsx");

                // Load the template Excel file using ClosedXML
                using (var workbook = new XLWorkbook(filePath))
                {
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
    }
}
