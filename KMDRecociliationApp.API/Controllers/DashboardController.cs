using FluentValidation;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Data;
using KMDRecociliationApp.Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using KMDRecociliationApp.Domain.DTO.Dashboard;
using KMDRecociliationApp.Domain.ReportParamModels;
using KMDRecociliationApp.Domain.ReportDataModels;

namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ApiBaseController
    {
        private readonly UserRepository _userRepo;
        private readonly PaymentRepository _paymentRepository;
        private readonly RefundRequestRepository _refundRequestRepository;
        private readonly CommonHelperRepository _commonHelperRepository;
        private readonly ApplicationDbContext context;
        private readonly IValidator<UserDTO> _validator;
        private readonly RoleRepository _roleRepository;
        private readonly ProductRepository _productRepository;
        private readonly ProductPolicyRepository _productPolicyRepository;
        private readonly DashboardRepository _dashboardRepository;
        public DashboardController(ILoggerFactory logger
            , ApplicationDbContext _context, UserRepository userRepository
            , CommonHelperRepository commonHelperRepository
            , IValidator<UserDTO> validator
            , RoleRepository roleRepository
            , ProductRepository productRepository
            , ProductPolicyRepository productPolicyRepository
            , PaymentRepository paymentRepository
            , RefundRequestRepository refundRequestRepository
            , DashboardRepository dashboardRepository

            ) : base(_context)
        {
            _userRepo = userRepository;
            context = _context;
            _commonHelperRepository = commonHelperRepository;
            _validator = validator;
            _roleRepository = roleRepository;
            _productRepository = productRepository;
            _productPolicyRepository = productPolicyRepository;
            _paymentRepository = paymentRepository;
            _refundRequestRepository = refundRequestRepository;
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet("GetAssociationWisePayment")]
        public async Task<IActionResult> GetAssociationWisePayment([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
        {
            if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
            {
                Sorting = new DataSorting() { SortName = "id", SortDirection = "desc" };
            }
            DataFilter<DTOAssociationPaymentStatus> filter =
                new DataFilter<DTOAssociationPaymentStatus>();
            filter.PageNumber = searchDTO.Page;
            filter.Limit = searchDTO.pageSize;
            filter.Filter = null;
            filter.SortName = Sorting.SortName;
            filter.SortDirection = Sorting.SortDirection;
            if (searchDTO.AssociationId > 0)
                filter.AssociationId = searchDTO.AssociationId;
            

            DataReturn<DTOAssociationPaymentStatus> dataReturn = new DataReturn<DTOAssociationPaymentStatus>();
            dataReturn =await _dashboardRepository.GetAssociationWisePaymentStatus(filter);
            return await Task.FromResult(Ok(dataReturn));
          
        }

        [HttpGet("getofflinepayments")]
        public async Task<IActionResult> GetOfflinePayments(int associationId)
        {
            var obj=await _dashboardRepository.GetOfflinePayments(associationId);
            return await Task.FromResult(Ok(obj));
        }
        
        [HttpGet("getCompletedpayments")]
        public async Task<IActionResult> getCompletedpayments(int associationId)
        {
            var obj = await _dashboardRepository.getCompletedpayments(associationId);
            return await Task.FromResult(Ok(obj));
        }

        [HttpGet("getpaymentmodes")]
        public async Task<IActionResult> getPaymentModes(int associationId)
        {
            var obj =await _dashboardRepository.getPaymentModes(associationId);
            return await Task.FromResult(Ok(obj));
        }

        [HttpGet("getUserCount")]
        public async Task<IActionResult> getUserCount(int associationId)
        {
            var obj = await _dashboardRepository.getUserCount(associationId);
            return await Task.FromResult(Ok(obj));
        }
        [HttpGet("getcampaigns")]
        public async Task<IActionResult> getcampaigns(int associationId)
        {
            var obj =await _dashboardRepository.getCampaignsCount(associationId);
            return await Task.FromResult(Ok(obj));
        }

        [HttpGet("getDashboardData")]
        public async Task<ActionResult<FormatedDashboardDataDataModel>> getDashboardData([FromQuery] DashboardDataParamModel dashboardDataParamModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var obj = await _dashboardRepository.getDashboardDataAsync(dashboardDataParamModel);
                 var _obj=_dashboardRepository.ConvertToFormattedModel(obj);
                return Ok(_obj);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        //[HttpGet("getDashboardDataDetails")]
        //public async Task<ActionResult<IEnumerable<DTOPensioneer>>> getDashboardDataDetails([FromQuery] DashboardDataDetailsParamModel dashboardDataDetailsParamModel)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);

        //        var objList = await _dashboardRepository.getDashboardDataDetailsAsync(dashboardDataDetailsParamModel);
        //        // var formattedList = _dashboardRepository.ConvertToFormattedModel(objList); // If needed
        //        return Ok(objList);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        Console.WriteLine(ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        //    }
        //}

        
        [HttpGet("getdashboardfilterdata")]
        //public async Task<ActionResult<IEnumerable<OrganisationDTO>>> getDashboardDataDetails([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting, DashboardDataDetailsParamModel dashboardDataDetailsParamModel)
        public async Task<IActionResult> getDashboardFilterData([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
            //, DashboardDataDetailsParamModel dashboardDataDetailsParamModel)
        {
            if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
            {
                Sorting = new DataSorting() { SortName = "id", SortDirection = "desc" };
            }
            DataFilter<DTOOfflinePayments> filter = new DataFilter<DTOOfflinePayments>();

            filter.PageNumber = searchDTO.Page;
            filter.Limit = searchDTO.pageSize;
            filter.Filter = null;
            filter.SortName = Sorting.SortName;
            filter.SortDirection = Sorting.SortDirection;
            filter.Search = searchDTO.Search;
            filter.BaseFilter = searchDTO.BaseFilter;

            if (searchDTO.AssociationId > 0)
                filter.AssociationId = searchDTO.AssociationId;
            if (searchDTO.CampaignId > 0)
                filter.CampaignId = searchDTO.CampaignId;

            DataReturn<DTOOfflinePayments> dataReturn = new DataReturn<DTOOfflinePayments>();
            dataReturn = _dashboardRepository.getDashboardDataDetailsAsync(filter);
            return await Task.FromResult(Ok(dataReturn));
        }



    }
}
