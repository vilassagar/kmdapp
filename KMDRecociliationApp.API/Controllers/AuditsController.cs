using KMDRecociliationApp.Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Data;
using KMDRecociliationApp.Domain.Results;
using KMDRecociliationApp.API.Common;
namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuditsController : ControllerBase
    {

        private readonly AuditRepository _auditRepo;
        private readonly ApplicationDbContext context;
        public AuditsController(ApplicationDbContext _context, AuditRepository AuditRepository)
        {
            _auditRepo = AuditRepository;
            context = _context;
        }
        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery] int PageNumber, [FromQuery] int Limit, [FromQuery] AuditLogResult auditlogResult, [FromQuery] DataSorting Sorting)
        {
            DataReturn<AuditLogResult> nVReturn = new DataReturn<AuditLogResult>();
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
                {
                    Sorting = new DataSorting() { SortName = "Auditid", SortDirection = "desc" };
                }
                if (PageNumber <= 0) PageNumber = 1;
                DataFilter<AuditLogResult> filter = new DataFilter<AuditLogResult>()
                {
                    PageNumber = PageNumber,
                    Limit = Limit
                    ,
                    Filter = auditlogResult,
                    SortName = Sorting.SortName
                    ,
                    SortDirection = Sorting.SortDirection
                };

                nVReturn = await _auditRepo.GetAll(filter);
                return await Task.FromResult(Ok(nVReturn));

            }
            else
            {
                DataReturn<AuditLogResult> result = new DataReturn<AuditLogResult>();
                result.ErrorMessage = "You dont have access to permissions";
                result.StatusCode = (int)HttpStatusCode.Unauthorized;
                Log.Warning($"Error in Audit controller Status Code : {result.StatusCode}, Message: {result.ErrorMessage}");
                return Unauthorized(result);
            }
        }



        [HttpGet("GetLogByDate")]
        public IActionResult GetLogByDate(string dateTime)
        {
            DateTime lDate = Convert.ToDateTime(dateTime);
            if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
                string logFilePath = $"{CommonHelper.LogFolderPath}/apphublog{lDate.ToString("yyyyMMdd")}.json";
                string todayLogFilePath = $"{CommonHelper.LogFolderPath}/copy_of_log{lDate.ToString("yyyyMMdd")}.json";

                if (lDate.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
                {
                    using (FileStream inf = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (FileStream outf = new FileStream(todayLogFilePath, FileMode.Create))
                        {
                            int a;
                            while ((a = inf.ReadByte()) != -1)
                            {
                                outf.WriteByte((byte)a);
                            }
                        }

                    }
                    if (System.IO.File.Exists(todayLogFilePath))
                    {
                        return File(System.IO.File.OpenRead(todayLogFilePath), "application/octet-stream", Path.GetFileName(todayLogFilePath));
                    }
                }
                else
                {
                    if (System.IO.File.Exists(logFilePath))
                    {
                        return File(System.IO.File.OpenRead(logFilePath), "application/octet-stream", Path.GetFileName(logFilePath));
                    }
                }
                return NotFound();

            }

            return Unauthorized();
        }

        [HttpGet("{Id:int}")]
        public async Task<IActionResult> Getbyid(int Id)
        {
            if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
                var result = _auditRepo.GetByID(Id).Result;
                return await Task.FromResult(Ok(result));
            }

            else
                return Unauthorized();
        }

        [HttpGet("GetAuditLogsByEmail")]
        public async Task<IActionResult> GetAuditLogsByEmail(string email)
        {
            if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
                var result = _auditRepo.GetAuditLogsByEmail(email).Result;
                return await Task.FromResult(Ok(result));
            }

            else
                return Unauthorized();
        }

    }
}
