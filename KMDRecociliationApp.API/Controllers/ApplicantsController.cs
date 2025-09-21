using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using ExcelDataReader;
using KMDRecociliationApp.API.Common.Filters;
using KMDRecociliationApp.Data;
using KMDRecociliationApp.Data.Exceptions;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Data.Services;
using KMDRecociliationApp.Data.Services.Interfaces;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.DTO.Common;
using KMDRecociliationApp.Domain.DTO.InsurerData;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using KMDRecociliationApp.Domain.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Data;
using System.IO;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicantsController : ApiBaseController
    {
        private readonly IApplicantService _applicantService;
        private readonly IDependentService _dependentService;
        private readonly IBankDetailsService _bankDetailsService;
        private readonly CommonHelperRepository _commonHelperRepository;
        private readonly IImportExportService _importExportService;
        private readonly ReportsRepository _reportsRepository;
        private readonly ApplicationDbContext _context;
        public ApplicantsController(IApplicantService applicantService
            , IDependentService dependentService,
        IBankDetailsService bankDetailsService, CommonHelperRepository commonHelperRepository
            , IImportExportService importExportService, ApplicationDbContext context
            , ReportsRepository reportsRepository
            ) : base(context)
        {
            _applicantService = applicantService;
            _dependentService = dependentService;
            _bankDetailsService = bankDetailsService;
            _commonHelperRepository = commonHelperRepository;
            _importExportService = importExportService;
            _context = context;
            _reportsRepository = reportsRepository;
        }
        // GET: api/Applicants/filter
        [HttpGet("filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilteredApplicants([FromQuery] ApplicantFilterDto filterDto, [FromQuery] DataSorting Sorting)
        {
            try
            {
                if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
                {
                    Sorting = new DataSorting() { SortName = "Id", SortDirection = "desc" };
                }
                DataFilter<ApplicantFilterDto> filter = new DataFilter<ApplicantFilterDto>()
                {
                    PageNumber = filterDto.Page,
                    Limit = filterDto.PageSize,
                    Filter = null,
                    SortName = Sorting.SortName,
                    Search = filterDto.Search,
                    SortDirection = Sorting.SortDirection
                };
                DataReturn<ApplicantDto> dataReturn = new DataReturn<ApplicantDto>();

                dataReturn = await _applicantService.GetFilteredApplicantsAsync(filterDto);
                return Ok(dataReturn);

            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetAssociations {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }

        }
        // GET: api/Applicants
        [HttpGet("Applicants")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApplicantDto>>> GetApplicants()
        {
            var applicants = await _applicantService.GetAllApplicantsAsync();
            return Ok(applicants);
        }

        // GET: api/Applicants/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApplicantDto>> GetApplicant(int id)
        {
            var applicant = await _applicantService.GetApplicantByIdAsync(id);
            if (applicant == null)
                return NotFound();

            return Ok(applicant);
        }

        // GET: api/Applicants/identifier/ABC123_JOHN
        [HttpGet("identifier/{uniqueIdentifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApplicantDto>> GetApplicantByUniqueIdentifier(string uniqueIdentifier)
        {
            var applicant = await _applicantService.GetApplicantByUniqueIdentifierAsync(uniqueIdentifier);
            if (applicant == null)
                return NotFound();

            return Ok(applicant);
        }

        // POST: api/Applicants
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApplicantDto>> CreateApplicant(ApplicantDto applicantDto)
        {
            try
            {
                var (createdApplicant, errors) = await _applicantService.CreateApplicantAsync(applicantDto);
                if (errors.Count > 0)
                {
                    return BadRequest(new { Errors = errors });
                }
                return CreatedAtAction(nameof(GetApplicant), new { id = createdApplicant.Id }, createdApplicant);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create applicant: {ex.Message}");
            }
        }

        // PUT: api/Applicants/5
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateApplicant(int id, ApplicantDto applicantDto)
        {

            // Validate input
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if ID in route matches ID in body (if included)
            if (applicantDto.Id != 0 && applicantDto.Id != id)
            {
                return BadRequest("ID in the URL does not match the ID in the request body.");
            }
            try
            {
                var updatedApplicant = await _applicantService.UpdateApplicantAsync(id, applicantDto);
                if (updatedApplicant == null)
                    return NotFound();

                return Ok(updatedApplicant);
            }
            catch (NotFoundException ex)
            {
                // Log the exception (optional)
                Log.Warning(ex, "Resource not found: {Message}", ex.Message);

                // Return 404 Not Found with the exception message
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update applicant: {ex.Message}");
            }
        }

        // DELETE: api/Applicants/5
        [HttpPost("DeleteApplicant/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteApplicant(int id)
        {
            var applicant = await _applicantService.GetApplicantByIdAsync(id);
            if (applicant == null)
                return NotFound();

            await _applicantService.DeleteApplicantAsync(id);
            return NoContent();
        }

        // POST: api/applicants/import
        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportApplicants([FromForm] ApplicantsTemplate template)
        {
            List<string> messages = new List<string>();
            if (template.template == null || template.template.Length == 0)
            {
                messages.Add("No file was uploaded");
                return BadRequest(new { Errors = messages });

            }

            var allowedExtensions = new[] { ".csv", ".xlsx", ".xls" };
            var fileExtension = Path.GetExtension(template.template.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                messages.Add("Invalid file format. Supported formats: CSV, XLSX, XLS");
                return BadRequest(new { Errors = messages });

            }

            try
            {
                var stream1 = template.template.OpenReadStream();
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using var reader = ExcelReaderFactory.CreateReader(stream1); // No need to cast to IExcelDataReader

                DataSet ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    UseColumnDataType = false,
                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
                if (ds == null)
                {
                    messages.Add("Invalid Template");
                    return BadRequest(new { Errors = messages });

                }

                DataTable dt = ds.Tables[0].DropBlankRows();
                var messageList = validateRequiredFields(dt);
                if (messageList.Count > 0)
                {
                    messages.AddRange(messageList);
                    return BadRequest(new { Errors = messages });
                }
                dt.Rows.RemoveAt(0);
                if (dt.Rows.Count > 501)
                {
                    messages.Add("Maximum 500 records are allowed");
                    return BadRequest(new { Errors = messages });
                }
                var importResult = await _importExportService.ImportApplicantsAsync(dt);
                messages.Add("File imported successfully");
                return Ok(new { Errors = messages });


            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during file import");
                return BadRequest($"Error processing file: {ex.Message}");
            }
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
        [HttpGet("export")]
        [IpAuthorizationFilter]
        public IActionResult ExportApplicants([FromQuery] ApplicantFilterDto filterDto, [FromQuery] string format = "xlsx"
               )
        {

            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"Applicants{DateTime.Now:yyyyMMdd}.xlsx";
            var table = _reportsRepository.GetApplicantData(filterDto);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Cell(1, 1).InsertTable(table); // Insert DataTable to worksheet

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                                contentType,
                                fileName);
                }
            }



        }

        [HttpPost]
        [Route("DownloadTemplate")]
        public IActionResult DownloadTemplate()
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ApplicantsTemplate.xlsx");

                // Load the template Excel file using ClosedXML
                using (var workbook = new XLWorkbook(filePath))
                {

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
    }
}

