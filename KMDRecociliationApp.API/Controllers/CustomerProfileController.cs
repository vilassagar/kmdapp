using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Data;
using Microsoft.AspNetCore.Mvc;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Results;
using Serilog;
using FluentValidation;
using FluentValidation.Results;
using KMDRecociliationApp.Domain.Enum;
using KMDRecociliationApp.API.Common;
using System.Data;
using Microsoft.EntityFrameworkCore;
using KMDRecociliationApp.Data.Helpers;
using KMDRecociliationApp.Domain.Common;
using System.Linq.Dynamic.Core;
using KMDRecociliationApp.Domain.PaymentDTO;
using Newtonsoft.Json;



namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerProfileController : ApiBaseController
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
        public CustomerProfileController(ILoggerFactory logger
            , ApplicationDbContext _context, UserRepository userRepository
            , CommonHelperRepository commonHelperRepository
            , IValidator<UserDTO> validator
            , RoleRepository roleRepository
            , ProductRepository productRepository
            , ProductPolicyRepository productPolicyRepository
            , PaymentRepository paymentRepository
            , RefundRequestRepository refundRequestRepository
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



        }

        #region ----User  


        [HttpPost]
        [Route("DeleteUser/{userId:int}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {

            List<string> messages = new List<string>();
            try
            {
                _userRepo.CurrentUser = HttpContext.User;
                var policy = context.PolicyHeader
                     .AsNoTracking()
                     .Where(x => x.UserId == userId &&
                     (x.PaymentStatus != PaymentStatus.Completed
                     )).FirstOrDefault();
                if (policy != null && policy.PaymentStatus == PaymentStatus.Completed)
                    return Conflict(new { Message = "payment complted user can't be deleted" });


                if (policy != null)
                {


                    var paymentDetail = context.PaymentDetails
                                         .AsNoTracking()
                                         .Where(x => x.PolicyId == policy.Id).FirstOrDefault();
                    if (paymentDetail != null)
                    {
                        var chequeDetails = context.PaymentModeCheque
                                        .AsNoTracking()
                                        .Where(x => x.PaymentDetailId == paymentDetail.Id).FirstOrDefault();
                        if (chequeDetails != null)
                        {
                            context.PaymentModeCheque.Remove(chequeDetails);
                            await context.SaveChangesAsync();
                            AppLogs appLogs = new AppLogs();
                            appLogs.Auditdate = DateTime.Now;
                            appLogs.UserId = _userRepo.UserId;
                            appLogs.Recordtype = ApplogType.Other;
                            appLogs.Comment = $"PaymentModeCheque Delete: {JsonConvert.SerializeObject(chequeDetails)} ";
                            context.AppLogs.Add(appLogs);
                            await context.SaveChangesAsync();

                        }
                        var neft = context.PaymentModeNEFT
                                       .AsNoTracking()
                                       .Where(x => x.PaymentDetailId == paymentDetail.Id).FirstOrDefault();
                        if (neft != null)
                        {
                            AppLogs appLogs = new AppLogs();
                            appLogs.Auditdate = DateTime.Now;
                            appLogs.UserId = _userRepo.UserId;
                            appLogs.Recordtype = ApplogType.Other;
                            appLogs.Comment = $"PaymentModeNEFT Delete: {JsonConvert.SerializeObject(neft)} ";
                            context.AppLogs.Add(appLogs);
                            await context.SaveChangesAsync();


                            context.PaymentModeNEFT.Remove(neft);
                            await context.SaveChangesAsync();
                        }
                        AppLogs appLogs1 = new AppLogs();
                        appLogs1.Auditdate = DateTime.Now;
                        appLogs1.UserId = _userRepo.UserId;
                        appLogs1.Recordtype = ApplogType.Other;
                        appLogs1.Comment = $"PaymentDetails Delete: {JsonConvert.SerializeObject(paymentDetail)} ";
                        context.AppLogs.Add(appLogs1);
                        await context.SaveChangesAsync();

                        context.PaymentDetails.Remove(paymentDetail);
                        await context.SaveChangesAsync();
                    }
                    var payment = context.PaymentHeader
                                         .AsNoTracking()
                                         .Where(x => x.PolicyId == policy.Id).FirstOrDefault();
                    if (payment != null)
                    {
                        AppLogs appLogs = new AppLogs();
                        appLogs.Auditdate = DateTime.Now;
                        appLogs.UserId = _userRepo.UserId;
                        appLogs.Recordtype = ApplogType.Other;
                        appLogs.Comment = $"PaymentHeader Delete: {JsonConvert.SerializeObject(payment)} ";
                        context.AppLogs.Add(appLogs);
                        await context.SaveChangesAsync();

                        context.PaymentHeader.Remove(payment);
                        await context.SaveChangesAsync();
                    }

                    AppLogs appLogs3 = new AppLogs();
                    appLogs3.Auditdate = DateTime.Now;
                    appLogs3.UserId = _userRepo.UserId;
                    appLogs3.Recordtype = ApplogType.Other;
                    appLogs3.Comment = $"PolicyHeader Delete: {JsonConvert.SerializeObject(policy)} ";
                    context.AppLogs.Add(appLogs3);
                    await context.SaveChangesAsync();
                    var userObj = context.Applicationuser
                   .AsNoTracking()
                   .Where(x => x.Id == userId).FirstOrDefault();
                    if (userObj != null)
                    {
                        AppLogs appLogs2 = new AppLogs();
                        appLogs2.Auditdate = DateTime.Now;
                        appLogs2.UserId = _userRepo.UserId;
                        appLogs2.Recordtype = ApplogType.Other;
                        appLogs2.Comment = $"Applicationuser Delete: {JsonConvert.SerializeObject(userObj)} ";
                        context.AppLogs.Add(appLogs2);
                        await context.SaveChangesAsync();
                    }
                    context.PolicyHeader.Remove(policy);
                    await context.SaveChangesAsync();

                    var userObj1 = context.Applicationuser
                    .AsNoTracking()
                    .Where(x => x.Id == userId).FirstOrDefault();
                    if (userObj1 != null)
                    {


                        context.Applicationuser.Remove(userObj1);
                        await context.SaveChangesAsync();

                    }


                }
                else
                {
                    var userObj1 = context.Applicationuser
                        .AsNoTracking()
                        .Where(x => x.Id == userId).FirstOrDefault();
                    if (userObj1 != null)
                    {
                        AppLogs appLogs2 = new AppLogs();
                        appLogs2.Auditdate = DateTime.Now;
                        appLogs2.UserId = _userRepo.UserId;
                        appLogs2.Recordtype = ApplogType.Other;
                        appLogs2.Comment = $"Applicationuser Delete: {JsonConvert.SerializeObject(userObj1)} ";
                        context.AppLogs.Add(appLogs2);
                        await context.SaveChangesAsync();

                        context.Applicationuser.Remove(userObj1);
                        await context.SaveChangesAsync();

                    }
                    else
                        return NotFound(new { Message = "User not found" });

                }
                return await Task.FromResult(Ok(new { Message = "User deleted " }));


            }
            catch (Exception ex)
            {

                return await Task.FromResult(this.InternalServerError(new { Message = $"Something went wrong!" }));
            }

            //}
            //else
            //{
            //    return Unauthorized();
            //}


        }



        [HttpGet("")]
        public async Task<IActionResult> GetUserList([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
        {
            if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
            {
                Sorting = new DataSorting() { SortName = "UpdatedAt", SortDirection = "desc" };
            }
            DataFilter<UserlistResult> filter = new DataFilter<UserlistResult>()
            {
                PageNumber = searchDTO.Page,
                Limit = searchDTO.pageSize
                ,
                Filter = null,
                SortName = Sorting.SortName,
                SortDirection = Sorting.SortDirection,
                AssociationId = searchDTO.AssociationId

            };

            filter.Search = searchDTO.Search;
            filter.userType = searchDTO.userTypeId;
            DataReturn<UserlistResult> dataReturn = new DataReturn<UserlistResult>();
            dataReturn = _userRepo.GetAll(filter);
            return await Task.FromResult(Ok(dataReturn));
        }

        [HttpPatch("updateuser/{userId}")]
        public async Task<IActionResult> updateuser(UserDTO userDTO, int userId)
        {
            if (userId <= 0 || userDTO == null)
                return BadRequest();
            try
            {
                ValidationResult validationResult = _validator.Validate(userDTO);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                ApplicationUser retApplicationUser = null;
                var result = await _userRepo.IsUserExists(userDTO, true);

                if (result.Id == 0 && userDTO != null)
                {
                    _userRepo.CurrentUser = HttpContext.User;
                    retApplicationUser = await _userRepo.UpdateUserAsync(userId, userDTO, updatedBy: _userRepo.UserId);
                    if (retApplicationUser != null && retApplicationUser.Id > 0)
                    {
                        return Ok(new { Message = "User added successfully" });
                    }
                    else
                    {
                        return BadRequest(new { Message = "User Add Failed" });
                    }


                }
                else
                {
                    string error = $"Mobile number {userDTO.MobileNumber} already in use.";
                    return Conflict(new { Message = error });
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Exception occured" + ex.Message);
                return Problem(ex.Message);
            }

        }

        [HttpPost("Createuser")]
        public async Task<IActionResult> Createuser(UserDTO userDTO)
        {
            try
            {
                ValidationResult validationResult = _validator.Validate(userDTO);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                ApplicationUser retApplicationUser = null;
                var result = await _userRepo.IsUserExists(userDTO);

                if (result != null && result.Id == 0)
                {
                    var role = new List<ApplicationRole>();
                    if (userDTO.UserTypeId > 0)
                    {
                        if (userDTO.UserTypeId.Value == (int)UserType.Pensioner||
                            userDTO.UserTypeId.Value == (int)UserType.Community)
                        {
                            role = await _roleRepository.GetByName(CommonHelper.RetireeRoleName);
                        }
                        else if (userDTO.UserTypeId.Value == (int)UserType.Association)
                        {
                            role = await _roleRepository.GetByName(CommonHelper.AssociationRoleName);
                        }
                        else
                        {
                            if (userDTO.RoleIds == null || !userDTO.RoleIds.Any())
                                role = await _roleRepository.GetByName(CommonHelper.AdminRoleName);
                        }
                        if (role != null && (userDTO.RoleIds == null || userDTO.RoleIds.Count == 0))
                        {
                            userDTO.RoleIds = [role.FirstOrDefault().Id];
                        }

                    }

                    _userRepo.CurrentUser = HttpContext.User;
                    retApplicationUser = await _userRepo.RegisterUserAsync(userDTO, updatedBy: _userRepo.UserId);
                    if (retApplicationUser != null && retApplicationUser.Id > 0)
                    {
                        return Ok(new { Message = "User added successfully" });
                    }
                    else
                    {
                        return BadRequest(new { Message = "User Add Failed" });
                    }


                }
                else
                {
                    string error = $"Mobile number {userDTO.MobileNumber} already in use.";
                    return Conflict(new { Message = error });
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Exception occured" + ex.Message);
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("getuser")]
        public async Task<IActionResult> Getbyid(int Id)
        {

            if (Id == 0)
                return NotFound();
            try
            {
                var user = _userRepo.GetByID(Id).Result;
                if (user == null)
                    return NotFound();
                var idTypes = _commonHelperRepository.GetPensionerIdType();
                user.PensionerIdType = idTypes.FirstOrDefault();
                if (user.pensioneridtypeId == 0 && user.PensionerIdType != null)
                    user.pensioneridtypeId = user.PensionerIdType.Id;
                return await Task.FromResult(Ok(user));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in Getbyid {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }

        }


        #endregion



        #region------ Common
        [HttpGet("GetOrganisations")]
        public async Task<IActionResult> GetOrganisations()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var obj = _commonHelperRepository.GetOrganisations();
                return await Task.FromResult(Ok(obj));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetOrganisations {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }

        [HttpGet("GetAssociations")]
        public async Task<IActionResult> GetAssociations(int associationId, string filter)
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var obj = _commonHelperRepository.GetAssociations(associationId, filter);
                return await Task.FromResult(Ok(obj));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetAssociations {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }

        [HttpGet("GetAssociationById")]
        public async Task<IActionResult> GetAssociationById(int associationId)
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var obj = _commonHelperRepository.GetAssociations(associationId, "");
                return await Task.FromResult(Ok(obj));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetAssociations {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }

        [HttpGet("GetGenders")]
        public async Task<IActionResult> GetGenders()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var obj = _commonHelperRepository.GetGenders();
                return await Task.FromResult(Ok(obj));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetGenders {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }
      
        [HttpGet("GetIdCardTypes")]
        public async Task<IActionResult> GetIdCardTypes()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var obj = _commonHelperRepository.GetIdTypes();
                return await Task.FromResult(Ok(obj));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetGenders {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }
       
        [HttpGet("getApplicantOrganizations")]
        public async Task<IActionResult> getApplicantOrganizations()
        {

            try
            {
                var obj = _commonHelperRepository.getApplicantOrganizations();
                return await Task.FromResult(Ok(obj));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in getApplicantOrganizations {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
        }

        [HttpGet("GetNomineeRelations")]
        public async Task<IActionResult> GetNomineeRelations()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var obj = _commonHelperRepository.GetNomineeRelations();
                return await Task.FromResult(Ok(obj));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetNomineeRelations {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }

        [HttpGet("GetUserTypes")]
        public async Task<IActionResult> GetUserTypes(int AssociationId)
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var user = _commonHelperRepository.GetUserTypes(AssociationId);
                return await Task.FromResult(Ok(user));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetUserTypes {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }

        [HttpGet("GetPensionerIdTypes")]
        public async Task<IActionResult> getPensionerIdTypes()
        {

            try
            {
                var result = _commonHelperRepository.GetPensionerIdType();
                return await Task.FromResult(Ok(result));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetUserTypes {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }

        }

        [HttpGet("GetPaymentStatus")]
        public async Task<IActionResult> GetPaymentStatus()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var obj = _commonHelperRepository.GetPaymentStatus();
                return await Task.FromResult(Ok(obj));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetPaymentStatus {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }
        [HttpGet("GetStates")]
        public async Task<IActionResult> GetStates()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var obj = _commonHelperRepository.GetStates();
                return await Task.FromResult(Ok(obj));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetStates {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }

        [HttpGet("GetContries")]
        public async Task<IActionResult> GetContries()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var obj = _commonHelperRepository.GetStates();
                return await Task.FromResult(Ok(obj));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetContries {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }

        #endregion

        [HttpGet("ValidatePolicyPurchase")]
        public async Task<IActionResult> ValidatePolicyPurchase(int userId)
        {

            if (userId == 0)
                return NotFound();
            try
            {
                var user = context.Applicationuser.AsNoTracking().
                    FirstOrDefault(x => x.Id == userId
                    );
                if (user == null)
                {
                    return await Task.FromResult(NotFound());
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(user.PensionerIdNumber)&&(int)user.UserType==1)
                        return await Task.FromResult(Ok(new 
                        { Message = "A unique ID number is required. Please visit your profile to complete the information." }));
                    else
                        return await Task.FromResult(Ok(new { Message = "ok" }));

                }

            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in ValidatePolicyPurchase {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }

        }


        #region -- Policy---

        [HttpPost]
        [Route("FreezPolicyOrder")]
        public async Task<IActionResult> FreezPolicyOrder([FromQuery] int policyId, [FromQuery] bool IsFreez)
        {
            List<string> messages = new List<string>();
            try
            {
                // Assuming _context is your database context for EF Core
                var policyHeader = await context.PolicyHeader.Where(x => x.Id == policyId).FirstOrDefaultAsync();

                if (policyHeader == null)
                {
                    return NotFound(new { Message = "Policy Order not found" });
                }

                // Update the IsFreezed field
                policyHeader.IsProfilePreez = !IsFreez;

                // Save changes to the database
                await context.SaveChangesAsync();

                return Ok(new { Message = $"Policy {(IsFreez ? "Freezed" : "Unfreezed")}" });
            }
            catch (Exception ex)
            {
                return this.InternalServerError(new { Message = "Something went wrong!" });
            }
        }



        [HttpGet("GetProductList")]
        public async Task<IActionResult> GetProductList(int userId)
        {

            try
            {
                if (userId < 0)
                    return BadRequest(new { errors = "User Date Of Birth is Invalid " });

                var userObj =await  context.Applicationuser.AsNoTracking()
                    .Where(x => x.Id == userId&& x.UserType==UserType.Community).FirstOrDefaultAsync();
                int age = 0;
                if (userObj != null && userObj.DOB != null)
                {
                     age = Data.Common.CommonHelper.CalculateAge(userObj.DOB.Value);
                    if (age < 1)
                        return BadRequest(new { errors = "User Date Of Birth is Invalid " });
                }
              
                //else
                //{
                //    return BadRequest(new { errors = "Please complete your profile " });
                //}

                    var dtoproduct = _productRepository.GetProductListByUser(userId, age);
                return await Task.FromResult(Ok(dtoproduct));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetProductList {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }

        }

        private PolicyProductPremimum CreatePremimumChartObj(DTOPolicyProductPremimumChart premimum
             , PolicyProductPremimum premimumChart, bool isTopupOption = false)
        {
            //var premimumChart = new PolicyProductPremimum();
            premimumChart.ProductPremimumId = premimum.ProductPremiumId;
            premimumChart.SumInsured = premimum.SumInsured;
            premimumChart.SpousePremium = premimum.SpousePremium;
            premimumChart.SelfSpousePremium = premimum.SelfSpousePremium;
            premimumChart.SelfSpouse2ChildrenPremium = premimum.SelfSpouse2ChildrenPremium;
            premimumChart.SelfSpouse1ChildrenPremium = premimum.SelfSpouse1ChildrenPremium;
            premimumChart.Self1ChildrenPremium = premimum.Self1ChildrenPremium;
            premimumChart.Self2ChildrenPremium = premimum.Self2ChildrenPremium;

            premimumChart.SelfOnlyPremium = premimum.SelfOnlyPremium;
            premimumChart.Child1Premium = premimum.Child1Premium;
            premimumChart.Child2Premium = premimum.Child2Premium;
            premimumChart.Parent1Premium = premimum.Parent1Premium;
            premimumChart.Parent2Premium = premimum.Parent2Premium;
            premimumChart.InLaw1Premium = premimum.InLaw1Premium;
            premimumChart.InLaw2Premium = premimum.InLaw2Premium;
            premimumChart.ProductId = premimum.ProductId;
            premimumChart.ParentProductPremimumId = premimum.ParentProductPremiumId;

            premimumChart.IsChild1PremiumSelected = premimum.IsChild1PremiumSelected;
            premimumChart.IsChild2PremiumSelected = premimum.IsChild2PremiumSelected;
            premimumChart.IsInLaw1PremiumSelected = premimum.IsInLaw1PremiumSelected;
            premimumChart.IsInLaw2PremiumSelected = premimum.IsInLaw2PremiumSelected;
            premimumChart.IsParent1PremiumSelected = premimum.IsParent1PremiumSelected;
            premimumChart.IsParent2PremiumSelected = premimum.IsParent2PremiumSelected;
            premimumChart.IsSpousePremiumSelected = (premimum.IsTopUpSpousePremiumSelected || premimum.IsSpousePremiumSelected);
            premimumChart.IsSelfSpousePremiumSelected = (premimum.IsTopUpSelfSpousePremiumSelected || premimum.IsSelfSpousePremiumSelected);
            premimumChart.IsSelfPremiumSelected = (premimum.IsTopUpSelfPremiumSelected || premimum.IsSelfPremiumSelected);

            premimumChart.IsSelf1ChildrenPremiumSelected = premimum.IsSelf1ChildrenPremiumSelected;
            premimumChart.IsSelf2ChildrenPremiumSelected = premimum.IsSelf2ChildrenPremiumSelected;
            premimumChart.IsSelfSpouse1ChildrenPremiumSelected = premimum.IsSelfSpouse1ChildrenPremiumSelected;
            premimumChart.IsSelfSpouse2ChildrenPremiumSelected = premimum.IsSelfSpouse2ChildrenPremiumSelected;
            premimumChart.AgeBandPremiumRateId = premimum.AgeBandPremiumRateId!=null? premimum.AgeBandPremiumRateId.Value:0;
            premimumChart.AgeBandValue = premimum.AgeBandPremiumRateValue;
            return premimumChart;
        }

        async Task<int?> ProcessBeneficiary(DTOPolicy dTOPolicy, DTOBeneficiaryPerson beneficiary, int policyHeaderId, bool hasDisabilityCert = false)
        {
            if (beneficiary == null || string.IsNullOrWhiteSpace(beneficiary.Name))
                return null;

            var person = dTOPolicy.IsUpdate && beneficiary.Id > 0
                ? await context.BeneficiaryPerson.FirstOrDefaultAsync(x => x.Id == beneficiary.Id)
                : new BeneficiaryPerson();

            person ??= new BeneficiaryPerson();

            // Set common properties
            person.Name = beneficiary.Name;
            person.Gender = (Gender)beneficiary.Gender.Id;
            person.DateOfBirth = beneficiary.DateOfBirth;
            person.PolicyId = dTOPolicy.PolicyId==0? policyHeaderId: dTOPolicy.PolicyId;
            person.UserId = dTOPolicy.UserId;

            // Save person
            if (dTOPolicy.IsUpdate && person.Id > 0)
                context.BeneficiaryPerson.Update(person);
            else
                context.BeneficiaryPerson.Add(person);

            await context.SaveChangesAsync();

            // Handle disability certificate if needed
            if (hasDisabilityCert && beneficiary.DisabilityCertificate?.File != null)
            {
                var commonFile = await new DataHelpers().UploadFile(
                    beneficiary.DisabilityCertificate.File,
                    DataHelpers.DISABILITYCERTIFICATE,
                    person.Id
                );

                if (commonFile != null && !string.IsNullOrWhiteSpace(commonFile.Name) && !string.IsNullOrWhiteSpace(commonFile.Url))
                {
                    person.DisabilityDocumentName = commonFile.Name;
                    person.DisabilityDocumentUrl = commonFile.Url;
                    context.BeneficiaryPerson.Update(person);
                    await context.SaveChangesAsync();
                }
            }

            return person.Id;
        }
    
        [HttpPost("AddProductPolicy")]
        public async Task<IActionResult> AddProductPolicy([FromForm] DTOPolicy dTOPolicy, int step)
        {

            if (step != 1 &&(dTOPolicy.PolicyId==null|| dTOPolicy.PolicyId < 0))
                return BadRequest();
            if(dTOPolicy.UserId<0)
                return BadRequest();
            int? campaignId = null;
            try
            {
                _paymentRepository.CurrentUser = HttpContext.User;
                int policyHeaderId = 0, BeneficiaryId = 0, isUpdatepolicy = 0;


                isUpdatepolicy = 1;

                PolicyHeader policyHeader = new PolicyHeader();
                policyHeader.TotalPremimum = dTOPolicy.TotalPremium;
                if (dTOPolicy.PolicyId != null && dTOPolicy.PolicyId != 0)
                    policyHeaderId = dTOPolicy.PolicyId.Value;
                if (dTOPolicy.Products != null && dTOPolicy.Products.Any())
                {
                    campaignId = dTOPolicy.Products.Where(x => x.IsProductSelected == true).FirstOrDefault().CampaignId;
                }


                policyHeader.TotalPaidPremimum = dTOPolicy.TotalPaidPremium;
                policyHeader.ChildPremium = dTOPolicy.ChildPremium;
                bool iscommit = false;
                using (var transaction = context.Database.BeginTransaction())
                {

                    try
                    {


                        if (step == 1 && isUpdatepolicy == 1)
                        {
                            iscommit = true;
                            if (dTOPolicy.PolicyId == 0)
                            {
                                policyHeader.UserId = dTOPolicy.UserId;
                                policyHeader.CampaignId = campaignId;
                                policyHeader.CreatedAt = DateTime.Now;
                                policyHeader.UpdatedAt = DateTime.Now;
                                policyHeader.CreatedBy = _paymentRepository.UserId;
                                policyHeader.UpdatedBy = _paymentRepository.UserId;
                                context.PolicyHeader.Add(policyHeader);
                                await context.SaveChangesAsync();
                            }
                            else
                            {
                                var policyheaderobj = context.PolicyHeader.AsNoTracking()
                                    .Where(x => x.Id == dTOPolicy.PolicyId.Value).FirstOrDefault();
                                if (policyheaderobj != null)
                                {
                                    policyHeader = policyheaderobj;
                                    policyHeader.UpdatedAt = DateTime.Now;
                                    policyHeader.UpdatedBy = _paymentRepository.UserId;
                                    policyHeader.Id = dTOPolicy.PolicyId.Value;
                                    if (policyheaderobj.PaymentStatus != PaymentStatus.Completed)
                                        policyHeader.PaymentStatus = policyheaderobj.PaymentStatus;

                                    policyHeader.IsActive = true;
                                    policyHeader.TotalPremimum = dTOPolicy.TotalPremium;

                                    context.PolicyHeader.Update(policyHeader);
                                    await context.SaveChangesAsync();

                                }
                            }
                            // Ensure user.Id is not 0 and was inserted successfully
                            if (policyHeader.Id == 0)
                            {
                                throw new Exception("policy Insert failed.");
                            }
                            policyHeaderId = policyHeader.Id;
                            if (policyHeader.Id > 0 && dTOPolicy.Products != null &&
                              dTOPolicy.Products.Count > 0)
                            {

                                foreach (var product in dTOPolicy.Products.Where(x => x.IsProductSelected == true))
                                {
                                    var policyProductDetails = new PolicyProductDetails();
                                    var policyProductDetailsobj = context.PolicyProductDetails.AsNoTracking()
                                                .Where(x => x.Id == product.Id).FirstOrDefault();
                                    if (policyProductDetailsobj != null && dTOPolicy.IsUpdate)
                                    {
                                        policyProductDetails = policyProductDetailsobj;
                                    }
                                    policyProductDetails.ProductId = product.ProductId;
                                    policyProductDetails.IsDisclaimerAccepted = product.IsDisclaimerAccepted;
                                    policyProductDetails.PolicyHeaderId = policyHeader.Id;
                                    policyProductDetails.TotalProductPremimum = product.TotalProductPremium;
                                    if (product.SelectedSumInsured != null)

                                    {
                                        policyProductDetails.SumInsured = product.SelectedSumInsured.Name;
                                        policyProductDetails.SumInsuredPremimumId = product.SelectedSumInsured.Id;

                                    }
                                    var IsTopUpSelected = false;
                                    if (product.PremiumChart != null)
                                        IsTopUpSelected = product.PremiumChart.Any(x => x.IsTopUpSelected == true);
                                    policyProductDetails.IsTopUpSelected = IsTopUpSelected;
                                    if (IsTopUpSelected == true && product.SelectedTopUpOption != null)
                                    {
                                      
                                        policyProductDetails.TopupSumInsured = product.SelectedTopUpOption.Name;
                                        policyProductDetails.TopupSumInsuredPremimumId = product.SelectedTopUpOption.Id;
                                    }

                                    if (product != null)
                                    {
                                        policyProductDetails.CampaignId = product.CampaignId;
                                        policyProductDetails.UserId = dTOPolicy.UserId;

                                        var isPrpoductexists = context.PolicyProductDetails.AsNoTracking()
                                           .FirstOrDefault(x => x.PolicyHeaderId == policyHeader.Id
                                           && x.ProductId == product.ProductId);
                                        if (dTOPolicy.IsUpdate&& isPrpoductexists!=null&& isPrpoductexists.Id>0)
                                        {
                                            policyProductDetails.Id = product.Id;

                                            context.PolicyProductDetails.Update(policyProductDetails);
                                            await context.SaveChangesAsync();
                                        }
                                        else
                                        {

                                            await context.PolicyProductDetails.AddAsync(policyProductDetails);
                                            await context.SaveChangesAsync();
                                        }

                                        if (policyProductDetails.Id == 0)
                                        {
                                            throw new Exception("Policy Product Details failed.");
                                        }

                                        var policyProductDetailsId = policyProductDetails.Id;
                                        if (policyProductDetailsId != 0 && product.PremiumChart != null)
                                        {
                                            foreach (var premimum in product.PremiumChart)
                                            {
                                                var premimumChart = new PolicyProductPremimum();
                                                if (dTOPolicy.PolicyId > 0 && dTOPolicy.IsUpdate)
                                                {
                                                    var premimumChartobj = context.PolicyProductPremimum
                                                         .Where(x => x.Id == premimum.Id).FirstOrDefault();
                                                    if (premimumChartobj != null)
                                                    {
                                                        premimumChart = premimumChartobj;
                                                    }
                                                }
                                                premimumChart = CreatePremimumChartObj(premimum, premimumChart, isTopupOption: false);

                                                premimumChart.PolicyHeaderId = policyHeader.Id;
                                                premimumChart.PolicyProductDetailsId = policyProductDetails.Id;
                                                premimumChart.CampaignId = product.CampaignId;
                                                premimumChart.UserId = dTOPolicy.UserId;
                                                var isPolicyProductPremimumexists = context.PolicyProductPremimum.AsNoTracking()
                                          .FirstOrDefault(x => x.PolicyHeaderId == policyHeader.Id
                                          && x.ProductId == product.ProductId &&x.Id== premimum.Id);
                                                if (dTOPolicy.PolicyId > 0 && dTOPolicy.IsUpdate
                                                    && isPolicyProductPremimumexists!=null
                                                    && isPolicyProductPremimumexists.Id>0)
                                                {
                                                    premimumChart.Id = premimum.Id;
                                                    context.PolicyProductPremimum.Update(premimumChart);
                                                    await context.SaveChangesAsync();
                                                }
                                                else
                                                {
                                                    await context.PolicyProductPremimum.AddAsync(premimumChart);
                                                    await context.SaveChangesAsync();
                                                }
                                                if (premimumChart.Id == 0)
                                                {
                                                    throw new Exception("Policy Product Premimum insert failed.");
                                                }

                                                var parentId = policyProductDetails.Id;

                                                if (premimum.TopUpOptions != null && premimum.TopUpOptions.Count > 0)
                                                {
                                                    foreach (var topups in premimum.TopUpOptions)
                                                    {

                                                        if (topups != null)
                                                        {
                                                            bool isTopUpseleccted = false;
                                                            if (product.SelectedTopUpOption != null && topups.Id == product.SelectedTopUpOption.Id)
                                                                isTopUpseleccted = true;

                                                            var topupspremimumChart = new PolicyProductPremimum();
                                                            if (dTOPolicy.PolicyId > 0 && dTOPolicy.IsUpdate)
                                                            {
                                                                var premimumChartobj = context.PolicyProductPremimum
                                                                     .Where(x => x.Id == premimum.Id).FirstOrDefault();
                                                                if (premimumChartobj != null)
                                                                {
                                                                    topupspremimumChart = premimumChartobj;
                                                                }
                                                            }
                                                            topupspremimumChart = CreatePremimumChartObj(topups, topupspremimumChart, isTopUpseleccted);
                                                            topupspremimumChart.PolicyHeaderId = policyHeader.Id;
                                                            topupspremimumChart.PolicyProductDetailsId = policyProductDetails.Id;
                                                            topupspremimumChart.CampaignId = product.CampaignId;
                                                            topupspremimumChart.UserId = dTOPolicy.UserId;
                                                            if (dTOPolicy.PolicyId > 0 && dTOPolicy.IsUpdate && topups.Id > 0)
                                                            {
                                                                topupspremimumChart.Id = topups.Id;
                                                                context.PolicyProductPremimum.Update(topupspremimumChart);
                                                                await context.SaveChangesAsync();
                                                            }
                                                            else
                                                            {
                                                                await context.PolicyProductPremimum.AddAsync(topupspremimumChart);
                                                                await context.SaveChangesAsync();
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }


                        }
                        else if (step == 2)
                        {
                            // Main method
                            iscommit = true;
                            if (dTOPolicy.beneficiaries != null)
                            {
                                var beneficiaryIds = new Dictionary<string, int?>
                                {
                                    ["Spouse"] = await ProcessBeneficiary(dTOPolicy, dTOPolicy.beneficiaries.Spouse, policyHeaderId,false),
                                    ["Child1"] = await ProcessBeneficiary(dTOPolicy, dTOPolicy.beneficiaries.Child1, policyHeaderId, true),
                                    ["Child2"] = await ProcessBeneficiary(dTOPolicy, dTOPolicy.beneficiaries.Child2, policyHeaderId, true),
                                    ["Parent1"] = await ProcessBeneficiary(dTOPolicy, dTOPolicy.beneficiaries.Parent1, policyHeaderId, false),
                                    ["Parent2"] = await ProcessBeneficiary(dTOPolicy, dTOPolicy.beneficiaries.Parent2, policyHeaderId, false),
                                    ["InLaw1"] = await ProcessBeneficiary(dTOPolicy, dTOPolicy.beneficiaries.InLaw1, policyHeaderId, false),
                                    ["InLaw2"] = await ProcessBeneficiary(dTOPolicy, dTOPolicy.beneficiaries.InLaw2, policyHeaderId, false)
                                };

                                var beneficiaryDetails = await context.BeneficiaryDetails
                                    .FirstOrDefaultAsync(x => x.PolicyId == dTOPolicy.PolicyId)
                                    ?? new BeneficiaryDetails();

                                beneficiaryDetails.PolicyId = dTOPolicy.PolicyId;
                                beneficiaryDetails.UserId = dTOPolicy.UserId;
                                beneficiaryDetails.Child1 = beneficiaryIds["Child1"];
                                beneficiaryDetails.Child2 = beneficiaryIds["Child2"];
                                beneficiaryDetails.Parent1 = beneficiaryIds["Parent1"];
                                beneficiaryDetails.Parent2 = beneficiaryIds["Parent2"];
                                beneficiaryDetails.InLaw1 = beneficiaryIds["InLaw1"];
                                beneficiaryDetails.InLaw2 = beneficiaryIds["InLaw2"];
                                beneficiaryDetails.Spouse = beneficiaryIds["Spouse"];

                                if (beneficiaryDetails.Id > 0)
                                    context.BeneficiaryDetails.Update(beneficiaryDetails);
                                else
                                    context.BeneficiaryDetails.Add(beneficiaryDetails);

                                await context.SaveChangesAsync();
                                BeneficiaryId = beneficiaryDetails.Id;

                                //Update the User Spuse details table User
                                if (dTOPolicy.beneficiaries.Spouse != null)
                                {
                                    var spouseDetailUser = await context.UserSpouseDetail
                                     .FirstOrDefaultAsync(x => x.UserId == dTOPolicy.UserId);
                                    if (spouseDetailUser != null)
                                    {
                                        spouseDetailUser.Name = dTOPolicy.beneficiaries.Spouse.Name;
                                        spouseDetailUser.Gender = (Gender)dTOPolicy.beneficiaries.Spouse.Gender.Id;
                                        spouseDetailUser.DateOfBirth = dTOPolicy.beneficiaries.Spouse.DateOfBirth;
                                        spouseDetailUser.UserId = dTOPolicy.UserId;
                                       
                                        context.UserSpouseDetail.Update(spouseDetailUser);
                                    }
                                    else
                                    {
                                        spouseDetailUser = new UserSpouseDetail();
                                        spouseDetailUser.Name = dTOPolicy.beneficiaries.Spouse.Name;
                                        spouseDetailUser.Gender = (Gender)dTOPolicy.beneficiaries.Spouse.Gender.Id;
                                        spouseDetailUser.DateOfBirth = dTOPolicy.beneficiaries.Spouse.DateOfBirth;
                                        spouseDetailUser.UserId = dTOPolicy.UserId;
                                        context.UserSpouseDetail.Add(spouseDetailUser);
                                    }
                                    await context.SaveChangesAsync();
                                }
                                if (dTOPolicy.beneficiaries.Child1 != null)
                                {
                                    var userchild1Detail = await context.UserChild1
                                     .FirstOrDefaultAsync(x => x.UserId == dTOPolicy.UserId);
                                    if (userchild1Detail != null)
                                    {
                                        userchild1Detail.Name = dTOPolicy.beneficiaries.Spouse.Name;
                                        userchild1Detail.Gender = (Gender)dTOPolicy.beneficiaries.Spouse.Gender.Id;
                                        userchild1Detail.DateOfBirth = dTOPolicy.beneficiaries.Spouse.DateOfBirth;
                                        userchild1Detail.UserId = dTOPolicy.UserId;
                                        userchild1Detail.DisabilityDocumentName = "";
                                        userchild1Detail.DisabilityDocumentUrl = "";

                                        context.UserChild1.Update(userchild1Detail);
                                    }
                                    else
                                    {
                                        userchild1Detail = new UserChild1();
                                        userchild1Detail.Name = dTOPolicy.beneficiaries.Spouse.Name;
                                        userchild1Detail.Gender = (Gender)dTOPolicy.beneficiaries.Spouse.Gender.Id;
                                        userchild1Detail.DateOfBirth = dTOPolicy.beneficiaries.Spouse.DateOfBirth;
                                        userchild1Detail.UserId = dTOPolicy.UserId;
                                        userchild1Detail.DisabilityDocumentName = "";
                                        userchild1Detail.DisabilityDocumentUrl = "";
                                        context.UserChild1.Add(userchild1Detail);
                                    }
                                    await context.SaveChangesAsync();
                                }
                                if (dTOPolicy.beneficiaries.Child2 != null)
                                {
                                    var userchild2Detail = await context.UserChild2
                                     .FirstOrDefaultAsync(x => x.UserId == dTOPolicy.UserId);
                                    if (userchild2Detail != null)
                                    {
                                        userchild2Detail.Name = dTOPolicy.beneficiaries.Spouse.Name;
                                        userchild2Detail.Gender = (Gender)dTOPolicy.beneficiaries.Spouse.Gender.Id;
                                        userchild2Detail.DateOfBirth = dTOPolicy.beneficiaries.Spouse.DateOfBirth;
                                        userchild2Detail.UserId = dTOPolicy.UserId;
                                        userchild2Detail.DisabilityDocumentName = "";
                                        userchild2Detail.DisabilityDocumentUrl = "";

                                        context.UserChild2.Update(userchild2Detail);
                                    }
                                    else
                                    {
                                        userchild2Detail = new UserChild2();
                                        userchild2Detail.Name = dTOPolicy.beneficiaries.Spouse.Name;
                                        userchild2Detail.Gender = (Gender)dTOPolicy.beneficiaries.Spouse.Gender.Id;
                                        userchild2Detail.DateOfBirth = dTOPolicy.beneficiaries.Spouse.DateOfBirth;
                                        userchild2Detail.UserId = dTOPolicy.UserId;
                                        userchild2Detail.DisabilityDocumentName = "";
                                        userchild2Detail.DisabilityDocumentUrl = "";
                                        context.UserChild2.Add(userchild2Detail);
                                    }
                                    await context.SaveChangesAsync();
                                }

                            }
                        }
                        else if (step == 3)
                        {

                            iscommit = true;
                            int nomineeId = 0;

                            if (dTOPolicy.Nominee != null && !string.IsNullOrWhiteSpace(dTOPolicy.Nominee.Name))
                            {
                                var objNominee = dTOPolicy.Nominee;
                                if (objNominee.NomineeRelation == null)
                                    objNominee.NomineeRelation = new CommonNameDTO();

                                // Handle nominee person
                                var nominee = dTOPolicy.IsUpdate && dTOPolicy.Nominee.Id > 0
                                    ? await context.BeneficiaryPerson.FirstOrDefaultAsync(x => x.Id == dTOPolicy.Nominee.Id)
                                    : new BeneficiaryPerson();

                                if (nominee == null)
                                    nominee = new BeneficiaryPerson();

                                // Set nominee properties
                                nominee.Name = objNominee.Name;
                                nominee.Gender = (Gender)objNominee.Gender.Id;
                                nominee.NomineeRelation = (NomineeRelation)objNominee.NomineeRelation.Id;
                                nominee.DateOfBirth = objNominee.DateOfBirth;
                                nominee.PolicyId = dTOPolicy.PolicyId;
                                nominee.UserId = dTOPolicy.UserId;

                                // Save nominee
                                if (dTOPolicy.IsUpdate && nominee.Id > 0)
                                    context.BeneficiaryPerson.Update(nominee);
                                else
                                    context.BeneficiaryPerson.Add(nominee);

                                await context.SaveChangesAsync();
                                nomineeId = nominee.Id;
                                var nomineeUser = await context.UserNomineeDetail
                                 .FirstOrDefaultAsync(x => x.UserId == dTOPolicy.UserId);
                                if (nomineeUser != null)
                                {
                                    nomineeUser.Name = objNominee.Name;
                                    nomineeUser.Gender = (Gender)objNominee.Gender.Id;
                                    nomineeUser.NomineeRelation = (NomineeRelation)objNominee.NomineeRelation.Id;
                                    nomineeUser.DateOfBirth = objNominee.DateOfBirth;
                                    nomineeUser.UserId = dTOPolicy.UserId;
                                    context.UserNomineeDetail.Update(nomineeUser);
                                }
                                else
                                {
                                    nomineeUser = new UserNomineeDetail();
                                    nomineeUser.Name = objNominee.Name;
                                    nomineeUser.Gender = (Gender)objNominee.Gender.Id;
                                    nomineeUser.NomineeRelation = (NomineeRelation)objNominee.NomineeRelation.Id;
                                    nomineeUser.DateOfBirth = objNominee.DateOfBirth;
                                    nomineeUser.UserId = dTOPolicy.UserId;
                                    context.UserNomineeDetail.Add(nomineeUser);
                                }
                                await context.SaveChangesAsync();


                                // Handle beneficiary details
                                var beneficiaryDetails = dTOPolicy.BeneficiaryId > 0
                                    ? await context.BeneficiaryDetails
                                        .FirstOrDefaultAsync(x => x.Id == dTOPolicy.BeneficiaryId)
                                    : new BeneficiaryDetails();

                                if (beneficiaryDetails != null)
                                {
                                    beneficiaryDetails.PolicyId = dTOPolicy.PolicyId;
                                    beneficiaryDetails.UserId = dTOPolicy.UserId;
                                    beneficiaryDetails.Nominee = nomineeId;

                                    if (dTOPolicy.BeneficiaryId > 0)
                                    {
                                        beneficiaryDetails.Id = dTOPolicy.BeneficiaryId.Value;
                                        context.BeneficiaryDetails.Update(beneficiaryDetails);
                                    }
                                    else
                                        context.BeneficiaryDetails.Add(beneficiaryDetails);

                                    await context.SaveChangesAsync();
                                }
                            }

                        }
                        else if (step == 4)
                        {
                            iscommit = true;
                            if (dTOPolicy.PaymentDetails != null)
                            {
                                var payment = await context.PaymentHeader.AsNoTracking()
                                    .Where(x => x.PolicyId == dTOPolicy.PolicyId).FirstOrDefaultAsync();
                                int paymentHeaderId = 0;
                                if (payment != null)
                                {
                                    paymentHeaderId = payment.Id;
                                    PaymentHeader paymentHeader = new PaymentHeader();
                                    paymentHeader.PolicyId = dTOPolicy.PolicyId;
                                    paymentHeader.UserId = dTOPolicy.UserId;
                                    paymentHeader.PaidAmount = dTOPolicy.AmountPaid + payment.PaidAmount;
                                    paymentHeader.UpdatedBy = _paymentRepository.UserId;
                                    paymentHeader.TotalPremimumAmount = dTOPolicy.TotalPremium;
                                    paymentHeader.UpdatedAt = DateTime.Now;
                                    context.PaymentHeader.Update(paymentHeader);
                                    await context.SaveChangesAsync();
                                }
                                else
                                {
                                    PaymentHeader paymentHeader = new PaymentHeader();
                                    paymentHeader.PolicyId = dTOPolicy.PolicyId;
                                    paymentHeader.UserId = dTOPolicy.UserId;
                                    paymentHeader.CampaignId = campaignId;
                                    paymentHeader.TotalPremimumAmount = dTOPolicy.TotalPaidPremium;
                                    paymentHeader.PaidAmount = dTOPolicy.AmountPaid;
                                    paymentHeader.CreatedBy = _paymentRepository.UserId;
                                    paymentHeader.CreatedAt = DateTime.Now;
                                    paymentHeader.UpdatedAt = DateTime.Now;
                                    paymentHeader.UpdatedBy = _paymentRepository.UserId;
                                    paymentHeader.TotalPremimumAmount = dTOPolicy.TotalPremium;
                                    context.PaymentHeader.Add(paymentHeader);
                                    await context.SaveChangesAsync();
                                    paymentHeaderId = paymentHeader.Id;
                                    paymentHeaderId = paymentHeader.Id;
                                }


                                var paymentDetailsobj = await context.PaymentDetails.AsNoTracking()
                                    .Where(x => x.PolicyId == dTOPolicy.PolicyId).FirstOrDefaultAsync();

                                PaymentDetails paymentDetails = new PaymentDetails();
                                if (paymentDetailsobj != null)
                                {
                                    paymentDetails = paymentDetailsobj;
                                    paymentDetails.PaymentHeaderId = paymentHeaderId;
                                    paymentDetails.PaymentDate = DateTime.Now;
                                    paymentDetails.PaymentMode = PaymentMode.Offline;
                                    if (dTOPolicy.PaymentDetails.PaymentTypeId == (int)PaymentTypes.Cheque)

                                    {
                                        paymentDetails.PaymentType = PaymentTypes.Cheque;
                                        paymentDetails.PaymentStatus = PaymentStatus.Initiated;
                                    }
                                    else if (dTOPolicy.PaymentDetails.PaymentTypeId == (int)PaymentTypes.NEFT
                                       )
                                    {
                                        paymentDetails.PaymentType = PaymentTypes.NEFT;
                                        paymentDetails.PaymentStatus = PaymentStatus.Completed;
                                    }
                                    else if ( dTOPolicy.PaymentDetails.PaymentTypeId == (int)PaymentTypes.UPI)
                                    {
                                        paymentDetails.PaymentType = PaymentTypes.UPI;
                                        paymentDetails.PaymentStatus = PaymentStatus.Completed;
                                    }

                                        paymentDetails.TotalPremimumAmount = dTOPolicy.TotalPremium;
                                    paymentDetails.PayableAmount = dTOPolicy.AmountPaid;
                                    paymentDetails.AmountPaid = dTOPolicy.AmountPaid;
                                    paymentDetails.PolicyId = dTOPolicy.PolicyId;
                                    paymentDetails.UserId = dTOPolicy.UserId;
                                    paymentDetails.CampaignId = campaignId;
                                    paymentDetails.UpdatedAt = DateTime.Now;
                                    paymentDetails.UpdatedBy = _paymentRepository.UserId;
                                    context.PaymentDetails.Update(paymentDetails);
                                    context.SaveChanges();

                                }
                                else
                                {

                                    paymentDetails.PaymentHeaderId = paymentHeaderId;
                                    paymentDetails.IsActive = true;
                                    paymentDetails.PaymentDate = DateTime.Now;
                                    paymentDetails.PaymentMode = PaymentMode.Offline;
                                    if (dTOPolicy.PaymentDetails.PaymentTypeId == (int)PaymentTypes.Cheque)

                                    {
                                        paymentDetails.PaymentType = PaymentTypes.Cheque;
                                        paymentDetails.PaymentStatus = PaymentStatus.Initiated;
                                    }
                                    else if (dTOPolicy.PaymentDetails.PaymentTypeId == (int)PaymentTypes.NEFT)
                                    {
                                        paymentDetails.PaymentType = PaymentTypes.NEFT;
                                        paymentDetails.PaymentStatus = PaymentStatus.Completed;
                                    }
                                    else if (dTOPolicy.PaymentDetails.PaymentTypeId == (int)PaymentTypes.UPI)
                                    {
                                        paymentDetails.PaymentType = PaymentTypes.UPI;
                                        paymentDetails.PaymentStatus = PaymentStatus.Completed;
                                    }

                                    paymentDetails.TotalPremimumAmount = dTOPolicy.TotalPremium;
                                    paymentDetails.PayableAmount = dTOPolicy.AmountPaid;
                                    paymentDetails.AmountPaid = dTOPolicy.TotalPremium;
                                    paymentDetails.PolicyId = dTOPolicy.PolicyId;
                                    paymentDetails.UserId = dTOPolicy.UserId;
                                    paymentDetails.CampaignId = campaignId;
                                    paymentDetails.CreatedBy = _paymentRepository.UserId;
                                    paymentDetails.CreatedAt = DateTime.Now;
                                    paymentDetails.UpdatedAt = DateTime.Now;
                                    paymentDetails.UpdatedBy = _paymentRepository.UserId;
                                    context.PaymentDetails.Add(paymentDetails);
                                    context.SaveChanges();

                                }

                                int paymentId = paymentDetails.Id;
                                var policyHeadersetp4 = context.PolicyHeader.AsNoTracking()
                                  .Where(x => x.Id == dTOPolicy.PolicyId).FirstOrDefault();
                                if (policyHeadersetp4 != null)
                                {
                                    context.PolicyHeader.Attach(policyHeadersetp4);
                                    context.Entry(policyHeadersetp4).State = EntityState.Modified;

                                    if (dTOPolicy.PaymentDetails.PaymentTypeId == (int)PaymentTypes.Cheque)
                                        policyHeadersetp4.PaymentStatus = PaymentStatus.Initiated;
                                    else
                                        policyHeadersetp4.PaymentStatus = PaymentStatus.Completed;

                                    policyHeadersetp4.TotalPaidPremimum = dTOPolicy.TotalPremium;
                                    policyHeadersetp4.UpdatedAt = DateTime.Now;
                                    policyHeadersetp4.UpdatedBy = _paymentRepository.UserId;

                                    context.SaveChanges();
                                }

                                var pay = dTOPolicy.PaymentDetails;
                                if (dTOPolicy.PaymentDetails.PaymentTypeId == (int)PaymentTypes.Cheque)
                                {
                                    PaymentModeCheque paymentModeCheque = new PaymentModeCheque();
                                    paymentModeCheque.BankName = pay.Offline.ChequeDetails.BankName;
                                    paymentModeCheque.Ifsccode = pay.Offline.ChequeDetails.Ifsccode;
                                    paymentModeCheque.Micrcode = pay.Offline.ChequeDetails.Micrcode;
                                    paymentModeCheque.ChequeNumber = pay.Offline.ChequeDetails.ChequeNumber;

                                    paymentModeCheque.InFavourOfAssociationId = pay.Offline.ChequeDetails.InFavourOfId;

                                    paymentModeCheque.Amount = pay.Offline.ChequeDetails.Amount;
                                    paymentModeCheque.Date = pay.Offline.ChequeDetails.Date;
                                    paymentModeCheque.PaymentDetailId = paymentId;
                                    paymentModeCheque.UserId = dTOPolicy.UserId;
                                    paymentModeCheque.PolicyId = dTOPolicy.PolicyId;
                                    paymentModeCheque.CampaignId = campaignId;
                                    paymentModeCheque.ChequeDepositLocation = pay.Offline.ChequeDetails.ChequeDepositLocation;
                                    context.PaymentModeCheque.Add(paymentModeCheque);
                                    context.SaveChanges();

                                    if (pay.Offline.ChequeDetails.ChequePhoto != null &&
                                    pay.Offline.ChequeDetails.ChequePhoto.File != null)
                                    {
                                        CommonFileModel commonFile = new CommonFileModel();
                                        if (pay.Offline.ChequeDetails.ChequePhoto.File.Length != 0)
                                        {
                                            commonFile = await new DataHelpers().UploadFile(pay.Offline.ChequeDetails.ChequePhoto.File
                                                , DataHelpers.CHEQUEPHOTO, paymentModeCheque.Id);
                                        }


                                        if (paymentModeCheque != null && paymentModeCheque.Id > 0
                                            && commonFile != null
                                            && !string.IsNullOrWhiteSpace(commonFile.Name)
                                            && !string.IsNullOrWhiteSpace(commonFile.Url))
                                        {
                                            paymentModeCheque.ChequePhotoDocumentName = commonFile.Name;
                                            paymentModeCheque.ChequePhotoDocumentUrl = commonFile.Url;
                                            context.PaymentModeCheque.Update(paymentModeCheque);
                                            await context.SaveChangesAsync();
                                        }
                                    }
                                }
                                else if (dTOPolicy.PaymentDetails.PaymentTypeId == (int)PaymentTypes.NEFT)
                                {

                                    PaymentModeNEFT paymentModeNEFT = new PaymentModeNEFT();
                                    paymentModeNEFT.BankName = pay.Offline.Neft.BankName;
                                    paymentModeNEFT.BranchName = pay.Offline.Neft.BranchName;
                                    paymentModeNEFT.AccountNumber = pay.Offline.Neft.AccountNumber;
                                    paymentModeNEFT.AccountName = pay.Offline.Neft.AccountName;
                                    paymentModeNEFT.IfscCode = pay.Offline.Neft.IfscCode;
                                    paymentModeNEFT.TransactionId = pay.Offline.Neft.TransactionId;
                                    paymentModeNEFT.Amount = pay.Offline.Neft.Amount;
                                    paymentModeNEFT.Date = pay.Offline.Neft.Date;
                                    paymentModeNEFT.PaymentDetailId = paymentId;
                                    paymentModeNEFT.PolicyId = dTOPolicy.PolicyId;
                                    paymentModeNEFT.UserId = dTOPolicy.UserId;
                                    paymentModeNEFT.CampaignId = campaignId;
                                    context.PaymentModeNEFT.Add(paymentModeNEFT);
                                    context.SaveChanges();

                                    if (pay.Offline.Neft.NeftPaymentReceipt != null &&
                                   pay.Offline.Neft.NeftPaymentReceipt.File != null)
                                    {
                                        CommonFileModel commonFile = new CommonFileModel();
                                        if (pay.Offline.Neft.NeftPaymentReceipt.File.Length != 0)
                                        {
                                            commonFile = await new DataHelpers().UploadFile(pay.Offline.Neft.NeftPaymentReceipt.File
                                                , DataHelpers.NEFTPAYMENTRECEIPT, paymentModeNEFT.Id);
                                        }


                                        if (paymentModeNEFT != null && paymentModeNEFT.Id > 0
                                            && commonFile != null
                                            && !string.IsNullOrWhiteSpace(commonFile.Name)
                                            && !string.IsNullOrWhiteSpace(commonFile.Url))
                                        {
                                            paymentModeNEFT.NEFTReceiptDocumentName = commonFile.Name;
                                            paymentModeNEFT.NEFTReceiptDocumentUrl = commonFile.Url;
                                            context.PaymentModeNEFT.Update(paymentModeNEFT);
                                            await context.SaveChangesAsync();
                                        }
                                    }


                                }
                                else if (dTOPolicy.PaymentDetails.PaymentTypeId == (int)PaymentTypes.UPI)
                                {

                                    PaymentModeUPI paymentModeUPI = new PaymentModeUPI();
                                   

                                    paymentModeUPI.TransactionNumber = pay.Offline.Upi.TransactionId;
                                    paymentModeUPI.Amount = pay.Offline.Upi.Amount;
                                    paymentModeUPI.Date = pay.Offline.Neft.Date;
                                    paymentModeUPI.PaymentDetailId = paymentId;
                                    paymentModeUPI.PolicyId = dTOPolicy.PolicyId;
                                    paymentModeUPI.UserId = dTOPolicy.UserId;
                                    paymentModeUPI.CampaignId = campaignId;
                                    context.PaymentModeUPI.Add(paymentModeUPI);
                                    context.SaveChanges();

                                    if (pay.Offline.Upi.UpiPaymentReceipt != null &&
                                   pay.Offline.Upi.UpiPaymentReceipt.File != null)
                                    {
                                        CommonFileModel commonFile = new CommonFileModel();
                                        if (pay.Offline.Upi.UpiPaymentReceipt.File.Length != 0)
                                        {
                                            commonFile = await new DataHelpers().UploadFile(pay.Offline.Upi.UpiPaymentReceipt.File
                                                , DataHelpers.UPIPAYMENTRECEIPT, paymentModeUPI.Id);
                                        }


                                        if (paymentModeUPI != null && paymentModeUPI.Id > 0
                                            && commonFile != null
                                            && !string.IsNullOrWhiteSpace(commonFile.Name)
                                            && !string.IsNullOrWhiteSpace(commonFile.Url))
                                        {
                                            paymentModeUPI.UPIReceiptDocumentName = commonFile.Name;
                                            paymentModeUPI.UPIReceiptDocumentUrl = commonFile.Url;
                                            context.PaymentModeUPI.Update(paymentModeUPI);
                                            await context.SaveChangesAsync();
                                        }
                                    }


                                }


                            }
                        }
                        if (iscommit)
                        {
                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Log.Error("Error" + ex.Message);
                        return await Task.FromResult(BadRequest(new { Message = "Policy Added failed", PolicyId = 0, BeneficiaryId = BeneficiaryId }));
                    }
                }

                DTOPolicy retdTOPolicy = new DTOPolicy();

                if ((step == 1 || step == 2) && dTOPolicy.IsUpdate == false)
                    retdTOPolicy = await _paymentRepository.GetBeneficieryDetails(dTOPolicy.UserId);

                return await Task.FromResult(Ok(new { Message = "Policy Added", PolicyId = policyHeaderId, BeneficiaryId = BeneficiaryId, dTOPolicy = retdTOPolicy }));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in AddProductPolicy {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }

        }


        [HttpGet("GetMyPolicies")]
        public async Task<IActionResult> GetMyPolicies([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
        {
            DataReturnPolicy dataReturn = new DataReturnPolicy();
            try
            {
                _paymentRepository.CurrentUser = HttpContext.User;
                if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
                {
                    Sorting = new DataSorting() { SortName = "id", SortDirection = "desc" };
                }
                DataFilter<DTOMyPolicies> filter = new DataFilter<DTOMyPolicies>();

                filter.PageNumber = searchDTO.Page;
                filter.Limit = searchDTO.pageSize;
                filter.Filter = null;
                filter.SortName = Sorting.SortName;
                filter.SortDirection = Sorting.SortDirection;
                if (searchDTO.userId > 0)
                    filter.userId = searchDTO.userId;
                if (searchDTO.CampaignId > 0)
                    filter.CampaignId = searchDTO.CampaignId;


                dataReturn = await _paymentRepository.GetMyPolicies(filter, _paymentRepository.UserId);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return Ok(dataReturn);
        }

        [HttpGet("GetPolicyDetails")]
        public async Task<IActionResult> GetPolicyDetails(int policyid, int campaignId)
        {
            DTOPolicy dTOPolicy = new DTOPolicy();
            try
            {
                dTOPolicy = await _paymentRepository.GetPolicyDetails(policyid, campaignId);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return await Task.FromResult(Ok(dTOPolicy));
        }

        #endregion ---Policy---

        #region ----Payment----

        [HttpGet("CheckPaymentByUser")]
        public async Task<IActionResult> CheckPaymentByUser(int userId)
        {

            if (userId == 0)
                return NotFound();
            try
            {
                var payment = context.PolicyHeader.AsNoTracking().
                    Where(x => x.UserId == userId
                    && x.PaymentStatus == PaymentStatus.Completed);
                if (payment == null || !payment.Any())
                {
                    return await Task.FromResult(Ok(new { IsDone = false }));
                }
                else
                {
                    return await Task.FromResult(Ok(new { IsDone = true }));
                }
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in CheckPaymentByUser {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }

        }

        [HttpGet("VerifyPolicyPaymentEntry")]
        public async Task<IActionResult> VerifyPolicyPaymentEntry(int userId)
        {

            if (userId == 0)
                return NotFound();
            try
            {
                var payment = context.PaymentDetails.AsNoTracking().
                    Where(x => x.UserId == userId);
                if (payment == null || !payment.Any())
                {
                    return await Task.FromResult(Ok(new { IsDone = false }));
                }
                else
                {
                    return await Task.FromResult(Ok(new { IsDone = true }));
                }
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in CheckPaymentByUser {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }

        }

        [HttpGet("GetPaymentModes")]
        public async Task<IActionResult> GetPaymentModes()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var paymentmodes = _commonHelperRepository.GetPaymentModes();
                return await Task.FromResult(Ok(paymentmodes));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetPaymentModes {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }

        [HttpGet("GetPaymentReceipt")]
        public async Task<IActionResult> GetPaymentReceipt(int policyId)
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var paymentmodes = await _paymentRepository.GetPaymentReceipt(policyId);
                return await Task.FromResult(Ok(paymentmodes));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetPaymentReceipt {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }

        [HttpGet("GetOfflinePaymentModes")]
        public async Task<IActionResult> GetOfflinePaymentModes()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var paymentmodes = _commonHelperRepository.GetOfflinePaymentModes();
                return await Task.FromResult(Ok(paymentmodes));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetOfflinePaymentModes {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }

        [HttpGet("GetOfflinePayments")]
        public async Task<IActionResult> GetOfflinePayments([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
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
            if (searchDTO.AssociationId > 0)
                filter.AssociationId = searchDTO.AssociationId;
            if (searchDTO.CampaignId > 0)
                filter.CampaignId = searchDTO.CampaignId;

            DataReturn<DTOOfflinePayments> dataReturn = new DataReturn<DTOOfflinePayments>();
            dataReturn = _paymentRepository.GetAllOfflinePayments(filter);
            return await Task.FromResult(Ok(dataReturn));
        }

        [HttpGet("Getpolicyorderbyid")]
        public async Task<IActionResult> Getpolicyorderbyid(int policyId)
        {
            var dataReturn = await _paymentRepository.GetPolicyOrderById(policyId);
            return Ok(dataReturn);

        }
        [HttpPost("Updatepolicyorder")]
        public async Task<IActionResult> UpdatePolicyOrder(DTOOnlinePolicyOrders payload1)
        {
            _paymentRepository.CurrentUser = HttpContext.User;

            if (payload1 == null || payload1.OrderId == 0)
            {
                return BadRequest();
            }
            NotifyPaymentStatusPayload payload = new NotifyPaymentStatusPayload();
            payload.premiumAmount = payload1.PaidAmount.Value;
            payload.PolicyId = payload1.OrderId;
            payload.transactionNumber = payload1.TransactionNumber;
            var policyheader = await context.PolicyHeader.AsNoTracking()
                                          .Where(x => x.Id == payload1.OrderId).FirstOrDefaultAsync();
            if (policyheader != null)
            {
                payload.UserId = policyheader.UserId;
            }
            else
                return NotFound("Request Id not found");
            AppLogs appLogs1 = new AppLogs();
            appLogs1.Auditdate = DateTime.Now;
            appLogs1.UserId = _paymentRepository.UserId;
            appLogs1.Recordtype = ApplogType.PaymentGateWay;
            appLogs1.Comment = $"Manual transaction number update started  {payload.transactionNumber} premiumAmount: {payload.premiumAmount}";
            context.AppLogs.Add(appLogs1);
            await context.SaveChangesAsync();

            bool b = await _paymentRepository.UpdatePolicyOrder(payload, _paymentRepository.UserId, payload1.Comment, isManual: true);
            AppLogs appLogs = new AppLogs();
            appLogs.Auditdate = DateTime.Now;
            appLogs.UserId = _paymentRepository.UserId;
            appLogs.Recordtype = ApplogType.PaymentGateWay;
            appLogs.Comment = $"Manual transaction number update successful  transaction number:{payload.transactionNumber} premiumAmount: {payload.premiumAmount}";
            context.AppLogs.Add(appLogs);
            await context.SaveChangesAsync();
            return Ok();

        }

        [HttpGet("GetPolicyOrders")]
        public async Task<IActionResult> GetPolicyOrders([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
        {
            if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
            {
                Sorting = new DataSorting() { SortName = "id", SortDirection = "desc" };
            }
            DataFilter<DTOPolicyOrders> filter = new DataFilter<DTOPolicyOrders>();

            filter.PageNumber = searchDTO.Page;
            filter.Limit = searchDTO.pageSize;
            filter.Filter = null;
            filter.SortName = Sorting.SortName;
            filter.SortDirection = Sorting.SortDirection;
            filter.Search = searchDTO.Search;
            filter.CampaignId = searchDTO.CampaignId;
            filter.userId = searchDTO.userId;
            filter.AssociationId = searchDTO.AssociationId;
            DataReturn<DTOOnlinePolicyOrders> dataReturn = new DataReturn<DTOOnlinePolicyOrders>();
            dataReturn = _paymentRepository.GetPolicyOrders(filter);
            return await Task.FromResult(Ok(dataReturn));
        }

        [HttpGet("GetPaymentHistory")]
        public async Task<IActionResult> GetPaymentHistory([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
        {
            _paymentRepository.CurrentUser = HttpContext.User;
            if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
            {
                Sorting = new DataSorting() { SortName = "id", SortDirection = "desc" };
            }
            DataFilter<DTOPaymentHistory> filter = new DataFilter<DTOPaymentHistory>()
            {
                PageNumber = searchDTO.Page,
                Limit = searchDTO.pageSize
                ,
                Filter = null,
                SortName = Sorting.SortName,
                SortDirection = Sorting.SortDirection,
                userId = searchDTO.userId
            };

            DataReturn<DTOPaymentHistory> dataReturn = new DataReturn<DTOPaymentHistory>();
            dataReturn = _paymentRepository.GetAllPaymentHistory(filter, _paymentRepository.UserId);
            return await Task.FromResult(Ok(dataReturn));
        }

        [HttpGet("GetOfflinePayment")]
        public async Task<IActionResult> GetOfflinePayment(int paymentId)
        {
            var offlinePayments = new DTOOfflinePayment();
            try
            {
                offlinePayments = await _paymentRepository.GetOfflinePayment(paymentId);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return await Task.FromResult(Ok(offlinePayments));

        }

        [HttpPost("Offlinepayment")]
        public async Task<IActionResult> GetOfflinePayment(DTOOfflinePaymentDetails dTOOfflinePayment)
        {

            try
            {
                _paymentRepository.CurrentUser = HttpContext.User;
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        decimal paidamount = 0;
                        var payment = await context.PaymentDetails.AsNoTracking()
                            .Where(x => x.Id == dTOOfflinePayment.PaymentId).FirstOrDefaultAsync();
                        if (payment != null)
                        {
                            if (payment.AmountPaid != null)
                                paidamount = payment.AmountPaid.Value;
                            payment.Id = dTOOfflinePayment.PaymentId;
                            payment.PaymentDate = DateTime.Now;
                            payment.Comment = dTOOfflinePayment.Comment;
                            payment.IsAccepted = dTOOfflinePayment.IsAccepted;
                            if (dTOOfflinePayment.IsAccepted == true)
                            {
                                payment.PaymentStatus = PaymentStatus.Completed;
                            }
                            else
                            {
                                payment.PaymentStatus = PaymentStatus.Rejected;
                                payment.AmountPaid = 0;
                            }
                            payment.UpdatedBy = _paymentRepository.UserId;
                            payment.UpdatedAt = DateTime.Now;
                            payment.PaymentAcceptedDate = DateTime.Now;
                            context.PaymentDetails.Update(payment);
                            context.SaveChanges();

                            var policy = await context.PolicyHeader.AsNoTracking()
                            .Where(x => x.Id == payment.PolicyId).FirstOrDefaultAsync();
                            if (policy != null)
                            {
                                policy.Id = payment.PolicyId.Value;
                                policy.Id = payment.PolicyId.Value;
                                if (dTOOfflinePayment.IsAccepted == true)
                                {
                                    policy.PaymentStatus = PaymentStatus.Completed;
                                }
                                else
                                {
                                    policy.PaymentStatus = PaymentStatus.Rejected;
                                    policy.TotalPaidPremimum = policy.TotalPaidPremimum - paidamount;
                                }
                                policy.UpdatedAt = DateTime.Now;
                                policy.UpdatedBy = _paymentRepository.UserId;

                                context.PolicyHeader.Update(policy);
                                context.SaveChanges();
                            }
                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Log.Error(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return await Task.FromResult(Ok(new { Message = "Offline Payment Details updated" }));
        }

        #endregion ---Payment---

        #region ---- Refund 

        [HttpGet("GetRefundRequests")]
        public async Task<IActionResult> GetRefundRequests([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
        {
            _paymentRepository.CurrentUser = HttpContext.User;
            if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
            {
                Sorting = new DataSorting() { SortName = "id", SortDirection = "desc" };
            }
            DataFilter<RefundRequstResult> filter = new DataFilter<RefundRequstResult>()
            {
                PageNumber = searchDTO.Page,
                Limit = searchDTO.pageSize
                ,
                Filter = null,
                SortName = Sorting.SortName,
                SortDirection = Sorting.SortDirection
            };
            filter.CampaignId = searchDTO.CampaignId;
            DataReturn<RefundRequstResult> dataReturn = new DataReturn<RefundRequstResult>();
            dataReturn = _refundRequestRepository.GetAll(filter);
            return await Task.FromResult(Ok(dataReturn));
        }

        [HttpGet("GetRefundRequest")]
        public async Task<IActionResult> GetRefundRequest(int requestId)
        {
            DTORefundRequest refund = new DTORefundRequest();
            try
            {
                refund = await _refundRequestRepository.GetByID(requestId);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return await Task.FromResult(Ok(refund));
        }

        [HttpPost("AddRefundRequest")]
        public async Task<IActionResult> AddRefundRequest(DTORefundRequest request)
        {

            try
            {
                _refundRequestRepository.CurrentUser = HttpContext.User;
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        RefundRequest refund = new RefundRequest();
                        refund.RefundAmount = Math.Abs(request.RefundAmount);
                        refund.RefundRequestDate = DateTime.Now;
                        refund.Status = PaymentStatus.Initiated;
                        refund.RetireeId = request.UserId;
                        refund.PolicyId = request.OrderNumber;
                        refund.RefundRequstHandledBy = _refundRequestRepository.UserId;
                        refund.CreatedBy = _refundRequestRepository.UserId;
                        refund.UpdatedBy = _refundRequestRepository.UserId;
                        refund.CreatedAt = DateTime.Now;
                        refund.UpdatedAt = DateTime.Now;

                        context.RefundRequest.Add(refund);
                        context.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Log.Error(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return await Task.FromResult(Ok(new { Message = "Offline Payment Details updated" }));
        }

        [HttpPatch("UpdateRefundRequest/{Id}")]
        public async Task<IActionResult> UpdateRefundRequest([FromForm] DTORefundRequestUpdate request, int refundid)
        {

            try
            {
                _refundRequestRepository.CurrentUser = HttpContext.User;
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var refund = await context.RefundRequest.AsNoTracking()
                            .Where(x => x.Id == request.RefundRequestNumber).FirstOrDefaultAsync();
                        if (refund != null)
                        {
                            if (request.IsAccepted == true)
                            {
                                refund.Status = PaymentStatus.Completed;
                            }
                            else
                            {
                                refund.Status = PaymentStatus.Rejected;
                            }
                            refund.Id = request.RefundRequestNumber;
                            refund.UpdatedBy = _refundRequestRepository.UserId;
                            refund.UpdatedAt = DateTime.Now;
                            if (request.IsAccepted == true)
                            {
                                if (request.ChequeDetails != null)
                                {
                                    refund.PaymentType = PaymentTypes.Cheque;
                                }
                                else if (request.Neft != null)
                                {
                                    refund.PaymentType = PaymentTypes.NEFT;
                                }
                                else if (request.Upi != null)
                                {
                                    refund.PaymentType = PaymentTypes.UPI;
                                }
                                else
                                    refund.PaymentType = null;
                            }
                            context.RefundRequest.Update(refund);
                            context.SaveChanges();

                            if (request.IsAccepted == true)
                            {

                                var payment = await context.PaymentHeader.AsNoTracking()
                              .Where(x => x.PolicyId == request.OrderNumber).FirstOrDefaultAsync();
                                if (payment != null)
                                {
                                    payment.Id = payment.Id;
                                    payment.PaidAmount = payment.PaidAmount - Math.Abs(request.RefundAmount);
                                    payment.RefundAmount = payment.RefundAmount;
                                    payment.UpdatedAt = DateTime.Now;
                                    payment.UpdatedBy = _refundRequestRepository.UserId;
                                    payment.RefundPaymentMode = (PaymentTypes)request.RefundPaymentMode;
                                    context.PaymentHeader.Update(payment);
                                    context.SaveChanges();

                                    //      var policyheader = await context.PolicyHeader.AsNoTracking()
                                    //.Where(x => x.Id == request.OrderNumber).FirstOrDefaultAsync();
                                    //      if (policyheader != null)
                                    //      {
                                    //          policyheader.Id = payment.Id;
                                    //          policyheader.TotalPaidPremimum = policyheader.TotalPaidPremimum;// - Math.Abs(request.RefundAmount);
                                    //          policyheader.UpdatedAt = DateTime.Now;
                                    //          policyheader.UpdatedBy = _refundRequestRepository.UserId;
                                    //          context.PolicyHeader.Update(policyheader);
                                    //          context.SaveChanges();
                                    //      }


                                    if ((PaymentTypes)request.RefundPaymentMode == PaymentTypes.Cheque)
                                    {
                                        if (request.ChequeDetails != null)
                                        {
                                            RefundPaymentModeCheque paymentModeCheque = new RefundPaymentModeCheque();
                                            paymentModeCheque.BankName = request.ChequeDetails.BankName;
                                            paymentModeCheque.ChequeNumber = request.ChequeDetails.ChequeNumber;
                                            paymentModeCheque.RetireeName = request.ChequeDetails.RetireeName;
                                            paymentModeCheque.Amount = request.ChequeDetails.Amount;
                                            paymentModeCheque.Date = request.ChequeDetails.Date;
                                            paymentModeCheque.RefundId = request.RefundRequestNumber;
                                            context.RefundPaymentModeCheque.Add(paymentModeCheque);
                                            context.SaveChanges();

                                            if (request.ChequeDetails.ChequePhoto != null &&
                                            request.ChequeDetails.ChequePhoto.File != null)
                                            {
                                                CommonFileModel commonFile = new CommonFileModel();
                                                if (request.ChequeDetails.ChequePhoto.File.Length != 0)
                                                {
                                                    commonFile = await new DataHelpers().UploadFile(request.ChequeDetails.ChequePhoto.File
                                                        , DataHelpers.REFUNDCHEQUEPHOTO, paymentModeCheque.Id);
                                                }


                                                if (paymentModeCheque != null && paymentModeCheque.Id > 0
                                                    && commonFile != null
                                                    && !string.IsNullOrWhiteSpace(commonFile.Name)
                                                    && !string.IsNullOrWhiteSpace(commonFile.Url))
                                                {
                                                    paymentModeCheque.ChequePhotoDocumentName = commonFile.Name;
                                                    paymentModeCheque.ChequePhotoDocumentUrl = commonFile.Url;
                                                    context.RefundPaymentModeCheque.Update(paymentModeCheque);
                                                    await context.SaveChangesAsync();
                                                }
                                            }
                                        }
                                    }
                                    else if ((PaymentTypes)request.RefundPaymentMode == PaymentTypes.NEFT)
                                    {
                                        if (request.Neft != null)
                                        {
                                            RefundPaymentModeNEFT paymentModeNEFT = new RefundPaymentModeNEFT();
                                            paymentModeNEFT.BankName = request.Neft.BankName;
                                            paymentModeNEFT.BranchName = request.Neft.BranchName;
                                            paymentModeNEFT.AccountNumber = request.Neft.AccountNumber;
                                            paymentModeNEFT.AccountName = request.Neft.AccountName;
                                            paymentModeNEFT.IfscCode = request.Neft.IfscCode;
                                            paymentModeNEFT.TransactionId = request.Neft.TransactionId;
                                            paymentModeNEFT.Amount = request.Neft.Amount;
                                            paymentModeNEFT.Date = request.Neft.Date;
                                            paymentModeNEFT.RefundId = request.RefundRequestNumber;
                                            context.RefundPaymentModeNEFT.Add(paymentModeNEFT);
                                            context.SaveChanges();

                                            if (request.Neft.NeftPaymentReceipt != null &&
                                          request.Neft.NeftPaymentReceipt.File != null)
                                            {
                                                CommonFileModel commonFile = new CommonFileModel();
                                                if (request.Neft.NeftPaymentReceipt.File.Length != 0)
                                                {
                                                    commonFile = await new DataHelpers().UploadFile(request.Neft.NeftPaymentReceipt.File
                                                        , DataHelpers.REFUNDNEFTPAYMENTRECEIPT, paymentModeNEFT.Id);
                                                }


                                                if (paymentModeNEFT != null && paymentModeNEFT.Id > 0
                                                    && commonFile != null
                                                    && !string.IsNullOrWhiteSpace(commonFile.Name)
                                                    && !string.IsNullOrWhiteSpace(commonFile.Url))
                                                {
                                                    paymentModeNEFT.NEFTReceiptDocumentName = commonFile.Name;
                                                    paymentModeNEFT.NEFTReceiptDocumentUrl = commonFile.Url;
                                                    context.RefundPaymentModeNEFT.Update(paymentModeNEFT);
                                                    await context.SaveChangesAsync();
                                                }
                                            }
                                        }
                                    }
                                    else if ((PaymentTypes)request.RefundPaymentMode == PaymentTypes.UPI)
                                    {
                                        if (request.Upi != null)
                                        {
                                            RefundPaymentModeUPI paymentModeUPI = new RefundPaymentModeUPI();
                                            paymentModeUPI.TransactionNumber = request.Upi.TransactionId;
                                            paymentModeUPI.Amount = request.Upi.Amount;
                                            paymentModeUPI.Date = request.Upi.Date;
                                            paymentModeUPI.RefundId = request.RefundRequestNumber;
                                            context.RefundPaymentModeUPI.Add(paymentModeUPI);
                                            context.SaveChanges();

                                            if (request.Upi.UpiPaymentReceipt != null &&
                                          request.Upi.UpiPaymentReceipt.File != null)
                                            {
                                                CommonFileModel commonFile = new CommonFileModel();
                                                if (request.Upi.UpiPaymentReceipt.File.Length != 0)
                                                {
                                                    commonFile = await new DataHelpers().UploadFile(request.Upi.UpiPaymentReceipt.File
                                                        , DataHelpers.REFUNDUPIPAYMENTRECEIPT, paymentModeUPI.Id);
                                                }


                                                if (paymentModeUPI != null && paymentModeUPI.Id > 0
                                                    && commonFile != null
                                                    && !string.IsNullOrWhiteSpace(commonFile.Name)
                                                    && !string.IsNullOrWhiteSpace(commonFile.Url))
                                                {
                                                    paymentModeUPI.UPIReceiptDocumentName = commonFile.Name;
                                                    paymentModeUPI.UPIReceiptDocumentUrl = commonFile.Url;
                                                    context.RefundPaymentModeUPI.Update(paymentModeUPI);
                                                    await context.SaveChangesAsync();
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Log.Error(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return await Task.FromResult(Ok(new { Message = "Offline Payment Details updated" }));
        }



        #endregion

        [HttpGet("GetPermissions")]
        public async Task<IActionResult> GetPermissions()
        {
            try
            {

                _userRepo.CurrentUser = HttpContext.User;
                var permissions = await _userRepo.GetUserPermissionsAsync(_userRepo.UserId);
                return await Task.FromResult(Ok(permissions));
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetPermissions" + ex.Message);
                return   this.InternalServerError(new { Message = "Something went wrong!" });
            }
        }

        [HttpGet("IsProfilefreez")]
        public async Task<IActionResult> IsProfilefreez(int userId)
        {
            _userRepo.CurrentUser = HttpContext.User;
            DTOUserPolicy obj = new DTOUserPolicy();
            var user = context.Applicationuser
                .Where(x => x.Id == userId)
                .FirstOrDefault(); ;
            var policy = context.PolicyHeader.AsNoTracking()
                .Where(x => x.Id == userId)
                .FirstOrDefault(); ;
            if (user != null)
            {
                obj.IsUserProfilePreez = user.IsProfilePreez;
                if (policy != null)
                {
                    obj.IsPolicyProfilePreez = policy.IsProfilePreez;
                }


            }
            return await Task.FromResult(Ok(obj));
        }

        [HttpGet("GetPaymentStatuss")]
        public async Task<IActionResult> GetPaymentStatuss()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var paymentmodes = _commonHelperRepository.GetPaymentStatuss();
                return await Task.FromResult(Ok(paymentmodes));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetPaymentStatus {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }

        [HttpGet("GetPaymentTypes")]
        public async Task<IActionResult> GetPaymentTypes()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var paymentmodes = _commonHelperRepository.GetPaymentTypes();
                return await Task.FromResult(Ok(paymentmodes));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetPaymentTypes {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }

        [HttpGet("GetAgeBand")]
        public async Task<IActionResult> GetAgeBand()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var ageband = _commonHelperRepository.GetAgeBand();
                return await Task.FromResult(Ok(ageband));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetAgeBand {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }
    }
}
