
using KMDRecociliationApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using KMDRecociliationApp.API.Common;
using Microsoft.Extensions.Options;
using Serilog;
using KMDRecociliationApp.Domain.Enum;
using Azure.Core;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http.HttpResults;
using Org.BouncyCastle.Asn1.Ocsp;
using KMDRecociliationApp.Services;
using Microsoft.EntityFrameworkCore;

namespace KMDRecociliationApp.API.Controllers
{       
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]

    public class UserController : ControllerBase
    {
        //private readonly TokenService _tokenService;
        private readonly ILogger<UserController> _logger;
        private readonly UserRepository _userRepository;
        private readonly KMDAPISecretKey _kMDAPISecretKey;
        private readonly ApplicationDbContext _context;
        private readonly CommonHelperRepository _commonHelperRepository;
        private readonly IValidator<UserDTO> _validator;
        private readonly RoleRepository _roleRepository;
        private readonly ITokenService _tokenService;
        public UserController(UserRepository userRepository,
           ApplicationDbContext context, ILogger<UserController> logger
            , IValidator<UserDTO> validator
           , IOptions<KMDAPISecretKey> kmdapikey
            , RoleRepository roleRepository
            , CommonHelperRepository commonHelperRepository,
            ITokenService tokenService)
        {
            // _tokenService = tokenService;
            _logger = logger;
            _userRepository = userRepository;
            _context = context;
            _validator = validator;
            _kMDAPISecretKey = kmdapikey.Value;
            _roleRepository = roleRepository;
            _commonHelperRepository = commonHelperRepository;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register(UserDTO userDTO)
        {
            try
            {
                ValidationResult validationResult = _validator.Validate(userDTO);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                ApplicationUser retApplicationUser = null;
                var result = await _userRepository.IsUserExists(userDTO);

                if (result.Id == 0 && userDTO != null)
                {

                    var role = new List<ApplicationRole>();
                    if (userDTO.UserTypeId != null)
                    {
                        if (userDTO.UserTypeId.Value == (int)UserType.Pensioner 
                            || userDTO.UserTypeId.Value == (int)UserType.Community)
                        {
                            role = await _roleRepository.GetByName(CommonHelper.RetireeRoleName);
                        }
                        else
                        {
                            role = await _roleRepository.GetByName(CommonHelper.AssociationRoleName);
                        }
                        if (role != null && role.Any())
                        {
                            if (userDTO.RoleIds == null)
                                userDTO.RoleIds = new List<int>();
                            userDTO.RoleIds.Add(role.FirstOrDefault().Id);
                        }
                        userDTO.pensioneridtypeId =(int) UserIdType.PANNumber;
                    }
                   
                    var orgObj = _context.Association.AsNoTracking().FirstOrDefault(x => x.Id == userDTO.AssociationId);
                    if (orgObj != null)
                    {
                        userDTO.OrganisationId = orgObj.OraganisationId;
                    }
                    //_userRepository.CurrentUser = HttpContext.User;
                    retApplicationUser = await _userRepository.RegisterUserAsync(userDTO, 0);
                    DateTime? expiresAt = null, createdAt = null;
                    var accessToken = _tokenService.GenerateToken(retApplicationUser.MobileNumber, out expiresAt, out createdAt);
                    var isProfileComplete = false; int associationId = 0;
                    if (retApplicationUser != null && retApplicationUser.IsProfileComplete != null)
                    {
                        isProfileComplete = retApplicationUser.IsProfileComplete.Value;
                        associationId = retApplicationUser.AssociationId.HasValue ? retApplicationUser.AssociationId.Value : 0;
                    }
                    bool IsPolicyPurchased = false;
                    IsPolicyPurchased = false;
                    if (retApplicationUser != null && retApplicationUser.Id > 0)
                    {
                        return Ok(new
                        {
                            IsProfileComplete = isProfileComplete,
                            UserId = retApplicationUser?.Id,
                            Token = accessToken,
                            expiresAt = expiresAt,
                            createdAt = createdAt,
                            AssociationId = associationId,
                            UserName = retApplicationUser.FirstName,
                            IsPolicy = IsPolicyPurchased,
                            userType = new { id = retApplicationUser.UserType.Value, name = retApplicationUser.UserType.Value.ToString() },
                            Message = "User registration successfully"
                        });
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
                //}
                //else
                //{
                //    return Unauthorized(new { Message = "Unauthorized" });
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetOrganisations")]
        [AllowAnonymous]
        public async Task<ActionResult> GetOrganisations()
        {
            try
            {
                var result = await _userRepository.GetOrganisations();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem("Something went wrong");
            }
        }

        [HttpGet("GetUserTypes")]
        public async Task<IActionResult> GetUserTypes()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var user = _commonHelperRepository.GetUserTypesuserscreen();
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

        [HttpGet]
        [Route("GetAssociations/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAssociations(int id)
        {
            try
            {
                var result = await _userRepository.GetAssociationsAsync(id);
                return Ok(result);
            }
            catch (Exception)
            {
                return Problem("Something went wrong");
            }
        }
       
        [HttpGet]
        [Route("getCampignAssociations/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> getCampignAssociations(int id)
        {
            if (id <= 0)
                return BadRequest();
            try
            {
                var result = await _userRepository.getCampignAssociationAsync(id);
                return Ok(result);
            }
            catch (Exception)
            {
                return Problem("Something went wrong");
            }
        }

        [HttpGet]
        [Route("GetCountries")]
        [AllowAnonymous]
        public async Task<ActionResult> GetCountries()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var countries = _commonHelperRepository.GetCountries();
                return await Task.FromResult(Ok(countries));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetStates {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }

        [HttpGet]
        [Route("GetStates")]
        [AllowAnonymous]
        public async Task<ActionResult> GetStates()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var states = _commonHelperRepository.GetStates();
                return await Task.FromResult(Ok(states));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetStates {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }
    }
}
