using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Data;
using KMDRecociliationApp.Domain.Results;
using Microsoft.AspNetCore.Authorization;
namespace KMDRecociliationApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class ErrorlogController : ApiBaseController
    {
        private readonly AuditRepository _auditRepo;
        private readonly ApplicationDbContext _context;
      
        public ErrorlogController(AuditRepository auditRepo, ApplicationDbContext appHubContext) : base(appHubContext)
        {
            _auditRepo = auditRepo;
            _context = appHubContext;
        }
       
        [AllowAnonymous]
        [HttpGet("GetAllAuditLog")]
        public async Task<IActionResult> GetAllAuditLog([FromQuery] int PageNumber, [FromQuery] int Limit, [FromQuery] AuditLogResult auditlogResult, [FromQuery] DataSorting Sorting)
        {
            DataReturn<AuditLogResult> nVReturn = new DataReturn<AuditLogResult>();

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

        [AllowAnonymous]
        [HttpGet("GetAllAppLog")]
        public async Task<IActionResult> GetAllAppLog(DateTime dateTime)
        {
            try
            {
               var data= _context.AppLogs.Where(x=>x.Auditdate.Value.Date==dateTime.Date).ToList();
                return Ok(data);

            }
            catch (Exception ex)
            {
                return Problem("Something went wrong",ex.Message);
            }
        }



    }
}
