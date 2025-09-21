using KMDRecociliationApp.Data;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class FinancialYearController : ApiBaseController
    {
        //private readonly TokenService _tokenService;
        private readonly ILogger<UserController> _logger;
        private readonly FinancialYearRepository _financialYearRepository;
        private readonly ApplicationDbContext _context;
        //private readonly IValidator<DTOAssociation> _validator;
        //private readonly RoleRepository _roleRepository;
        // private readonly RolePermissionRepository _rolePermissionRepo;
        public FinancialYearController(FinancialYearRepository financialYearRepository,
           ApplicationDbContext context, ILogger<UserController> logger
            //, IValidator<DTOAssociation> validator, RoleRepository roleRepository
            ):base(context)
        {
            // _tokenService = tokenService;
            _logger = logger;
            _financialYearRepository = financialYearRepository;
            _context = context;
            //_validator = validator;
            //_kMDAPISecretKey = kmdapikey.Value;
            //_roleRepository = roleRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetFinancialYears([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
        {
            //if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
                {
                    Sorting = new DataSorting() { SortName = "Id", SortDirection = "desc" };
                }
                DataFilter<FinancialYearResult> filter = new DataFilter<FinancialYearResult>()
                {
                    PageNumber = searchDTO.Page,
                    Limit = searchDTO.pageSize,
                    Filter = null,
                    SortName = Sorting.SortName,
                    Search = searchDTO.Search,
                    SortDirection = Sorting.SortDirection
                };
                DataReturn<FinancialYearResult> dataReturn = new DataReturn<FinancialYearResult>();
                dataReturn = _financialYearRepository.GetFinancialYears(filter);
                return await Task.FromResult(Ok(dataReturn));
            }
            //else
            //{
            //    DataReturn<AssociationResult> dataReturn = new DataReturn<AssociationResult>();
            //    dataReturn.ErrorMessage = "You dont have access to permissions";
            //    dataReturn.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    Log.Warning($"Error Association controller -Get All Association method, Status Code : {dataReturn.StatusCode}, Message: {dataReturn.ErrorMessage}");
            //    return Unauthorized(dataReturn);
            //}
        }

        [HttpPost("CreateFinancialYear")]
        public async Task<IActionResult> CreateFinancialYear(DTOFinancialYear dtoFinancialYear)
        {
            // if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
                _financialYearRepository.CurrentUser = HttpContext.User;
                FinancialYear retFinancialYear = null;
                if (dtoFinancialYear.Id == null || dtoFinancialYear.Id == 0)
                {
                    var tempFinancialYear = await _financialYearRepository.CheckFinancialYearLabel(dtoFinancialYear);
                    //_financialYearRepository.UserEmail = Convert.ToString(User.FindFirst("email")?.Value);
                    // _rolePermissionRepo.UserEmail = _associationRepository.UserEmail;



                    if (tempFinancialYear == null)
                    {
                        retFinancialYear = await _financialYearRepository.CreateFinancialYearAsync(dtoFinancialYear,1/* _financialYearRepository.UserId*/);
                        if (retFinancialYear != null && retFinancialYear.Id > 0)
                        {
                            return Ok(new { Message = "Finiancial year created successfully" });
                        }
                        else
                        {
                            return BadRequest(new { Message = "Finiancial year create Failed" });
                        }
                        ////if (retAssociation != null && retAssociation.Id > 0)
                        ////{
                        ////    //Add premimum chart data
                        ////}

                    }
                    else
                    {
                        string error = $"Finiancial Year Lable: {dtoFinancialYear.FinancialYearLabel} already exists";
                        return Conflict(new { Message = error });
                         
                    }
                }

                FinancialYearResult financialYearResult = new FinancialYearResult().Copy(retFinancialYear);
                return await Task.FromResult(Ok(financialYearResult));

                // return await Task.FromResult(Ok());
            }
            //else
            //{
            //    return Unauthorized();
            //}
        }

        // GET: api/FinancialYear/5
        [HttpGet("{finiancialYearId:int}")]
        public async Task<IActionResult> GetFinancialYearById(int finiancialYearId)
        {
            try
            {
                var organisation = await _financialYearRepository.GetFinancialYearByIdAsync(finiancialYearId);
                if (organisation == null)
                {
                    return NotFound(new { Message = $"FinancialYear with ID {finiancialYearId} not found" });
                }

                return Ok(organisation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Get Financial Year By Id: {ex.Message}", ex);
                return Problem(detail: "An error occurred while retrieving the Financial Year.");
            }
        }

        [HttpPatch("UpdateFiniancialYear/{finiancialYearId:int}")]
        public async Task<IActionResult> UpdateAssociation(DTOFinancialYear financialYear, int finiancialYearId)
        {
            // if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
                _financialYearRepository.CurrentUser = HttpContext.User;
                FinancialYear retFinancialYear = null;
                if (financialYear.Id != null || financialYear.Id != 0)
                {
                    var tempFinancialYear = await _financialYearRepository.CheckFinancialYearLabel(financialYear, true);


                    if (tempFinancialYear == null)
                    {
                        retFinancialYear = await _financialYearRepository.UpdateFinancialYearAsync(finiancialYearId, financialYear, 1/*_financialYearRepository.UserId*/);
                        if (retFinancialYear != null && retFinancialYear.Id > 0)
                        {
                            return Ok(new { Message = "FinancialYear updated successfully" });
                        }
                        else
                        {
                            return BadRequest(new { Message = "FinancialYear update Failed" });
                        }
                        ////if (retAssociation != null && retAssociation.Id > 0)
                        ////{
                        ////    //Add premimum chart data
                        ////}

                    }
                    else
                    {
                        string error = $"Financial Year Lable: {financialYear.FinancialYearLabel} already exists";
                        return Conflict(new { Message = error });
                    }
                }

                FinancialYearResult financialYearResult = new FinancialYearResult().Copy(retFinancialYear);
                return await Task.FromResult(Ok(financialYearResult));

                // return await Task.FromResult(Ok());
            }
            //else
            //{
            //    return Unauthorized();
            //}
        }



        // DELETE: api/Organisation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFinancialYear(int id)
        {
            bool deleted = await _financialYearRepository.DeleteFinancialYearAsync(id);
            if (!deleted)
            {
                return NotFound(new { Message = $"FinancialYear with ID {id} not found." });
            }

            return Ok(new { Message = "FinancialYear Deleted successfully" });
        }




    }
}
