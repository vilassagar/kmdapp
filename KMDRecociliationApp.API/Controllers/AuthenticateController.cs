using Microsoft.AspNetCore.Mvc;
using KMDRecociliationApp.Services;
using Microsoft.AspNetCore.Authorization;
using KMDRecociliationApp.Domain.Auth;
using KMDRecociliationApp.Data;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Domain.DTO;

using FluentValidation;
using KMDRecociliationApp.API.Common;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Options;
using KMDRecociliationApp.Domain.Enum;
using Serilog;
using KMDRecociliationApp.API.Common.Filters;
using System.Linq.Dynamic.Core;
using KMDRecociliationApp.API.Services;
using Microsoft.EntityFrameworkCore;
using SqlHelpers;
using DocumentFormat.OpenXml.Spreadsheet;
namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticateController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthenticateController> _logger;
        private readonly ApplicationUserRepository _userRepository;
        private readonly ApplicationDbContext _context;
        private readonly IValidator<LoginAuthModel> _validator;
        private readonly OTPService _otpService;
        private readonly KMDAPISecretKey _kMDAPISecretKey;
        private readonly CommonHelperRepository _commonHelperRepository;
        private readonly IEmailService _emailService;
        private readonly SMSRequestObject _sMSRequestObject;
        private readonly OTPConfiguration _oTPConfiguration;
        private readonly IVerificationService _verificationService;
        //  private readonly DistributedCacheEntryOptions _cacheOptions;
        public AuthenticateController(ApplicationUserRepository userRepository,
           ApplicationDbContext context, ITokenService tokenService
            , ILogger<AuthenticateController> logger, ISmsSender smsSender
            , IValidator<LoginAuthModel> validator, OTPService otpService
             , CommonHelperRepository commonHelperRepository
              , IOptions<KMDAPISecretKey> kmdapikey, IEmailService emailService
             , IOptions<SMSRequestObject> sMSRequestObject
            , IOptions<OTPConfiguration> oTPConfiguration,
           IVerificationService verificationService
            )
        {
            _tokenService = tokenService;
            _logger = logger;
            _userRepository = userRepository;
            _context = context;
            _validator = validator;
            _otpService = otpService;
            _kMDAPISecretKey = kmdapikey.Value;
            _commonHelperRepository = commonHelperRepository;
            _emailService = emailService;
            _sMSRequestObject = sMSRequestObject.Value;
            _oTPConfiguration = oTPConfiguration.Value;
            _verificationService = verificationService;

        }

        [HttpPost]
        [Route("Login")]
        //[ValidateCustomAntiforgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginAuthModel request)
        {
            if (Request.Headers.TryGetValue(CommonHelper.KMDSECRETAPIKEY, out StringValues value))
            {
                if (_kMDAPISecretKey.SecretKey != value.FirstOrDefault())
                {
                    return Unauthorized(new { Message = "API Key Invalid" });
                }
                var validationResult = _validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }


                var managedUser = await _userRepository.FindByMobilePhoneAsync(request.PhoneNumber!);

                if (managedUser == null)
                {
                    return NotFound(new { Message = "Mobile Number not found" });
                }
                else
                {
                   
                    if (request.IsPasswordLogin == true)
                    {
                        var user = await _userRepository.ValidatePassword(request.PhoneNumber, request.Password);

                        if (user == null)
                            return Unauthorized(new { Message = "Invalid Password" });
                        DateTime? expiresAt = null, createdAt = null;
                        var accessToken = _tokenService.GenerateToken(request.PhoneNumber, out expiresAt, out createdAt);
                        var isProfileComplete = false;
                        if (user != null && user.IsProfileComplete != null)
                            isProfileComplete = user.IsProfileComplete.Value;
                        bool IsPolicyPurchased = true;
                        if (user != null && user.UserType == UserType.Pensioner)
                        {
                            var policy = _context.PolicyHeader
                            .Where(x => x.UserId == user.Id).FirstOrDefault();                       
                            if (policy == null)
                            {
                                IsPolicyPurchased = false;
                            }
                        }
                        var roles = (from r in _context.ApplicationRole.AsNoTracking()
                                     join ur in _context.ApplicationUserRole.AsNoTracking()
                                     on r.Id equals ur.RoleId
                                     where ur.UserId == user.Id
                                     select r.RoleName
                                  ).ToList();
                        return Ok(new
                        {
                            IsProfileComplete = isProfileComplete,
                            UserId = user?.Id,
                            Token = accessToken,
                            createdAt = createdAt,
                            expiresAt = expiresAt,
                            Is_SystemAdmin = user.Is_SystemAdmin,
                            UserName = user.FirstName,
                            IsPolicy= IsPolicyPurchased,
                            userType = new { id = user.UserType.Value, name = user.UserType.Value.ToString() },
                            roles=roles,
                            Message = "Logged in successfully"
                        });
                    }
                    else
                    {

                        var otp = _otpService.GenerateOTP();
                        //var otp = await _smsSender.SendMessageAsync(managedUser.PhoneNumber, "");
                        if (otp == null)
                        {
                            var responseobj = new LoginAuthResponse(request.PhoneNumber, otp, "OTP Gengration failed");

                            return BadRequest(responseobj);
                        }
                        if (_oTPConfiguration.Manual.Equals("yes"))
                        {
                            managedUser.OTP = "1111";
                            _otpService.SendEmail(managedUser.Email, otp);

                            //var (response, content) = await _otpService.SendOTP(request.PhoneNumber, otp, _sMSRequestObject);
                            //if (response != null)
                            //{
                            //    AppLogs appLogs = new AppLogs();
                            //    appLogs.Auditdate = DateTime.Now;
                            //    appLogs.UserId = managedUser.Id;
                            //    appLogs.Recordtype = ApplogType.SMSGateWay;
                            //    appLogs.Comment = $"SMS gateway response StatusCode:" +
                            //        $" {response.StatusCode} Content: {response.Content}" +
                            //        $"Version: {response.Version} Payload {content.ToString()} ";
                            //    _context.AppLogs.Add(appLogs);
                            //    await _context.SaveChangesAsync();
                            //}
                        }
                        else
                        {
                            managedUser.OTP = otp;
                            //StringContent content = null;
                            var (response, content) = await _otpService.SendOTP(request.PhoneNumber, otp, _sMSRequestObject);
                            _otpService.SendEmail(managedUser.Email, otp);
                            if (response != null)
                            {
                                AppLogs appLogs = new AppLogs();
                                appLogs.Auditdate = DateTime.Now;
                                appLogs.UserId = managedUser.Id;
                                appLogs.Recordtype = ApplogType.SMSGateWay;
                                appLogs.Comment = $"SMS gateway response StatusCode:" +
                                    $" {response.StatusCode} Content: {response.Content}" +
                                    $"Version: {response.Version} Payload {content.ToString()} ";
                                _context.AppLogs.Add(appLogs);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                AppLogs appLogs = new AppLogs();
                                appLogs.Auditdate = DateTime.Now;
                                appLogs.UserId = managedUser.Id;
                                appLogs.Recordtype = ApplogType.SMSGateWay;
                                appLogs.Comment = $"SMS Response error payload {content}";
                                _context.AppLogs.Add(appLogs);
                                await _context.SaveChangesAsync();
                            }


                        }
                        managedUser.OTPExpiration = DateTime.Now.AddMonths(1);
                        _userRepository.Update(managedUser);
                        return Ok(new { Message = "OTP sent successfully" });
                    }
                }
            }
            else
                return Unauthorized();
        }

        [HttpPost]
        [Route("SendVerificationCodeSignUp")]
        [AllowAnonymous]
        public async Task<ActionResult> SendVerificationCodeSignUp([FromBody] LoginAuthModel request)
        {
            if (Request.Headers.TryGetValue(CommonHelper.KMDSECRETAPIKEY, out StringValues value))
            {
                if (_kMDAPISecretKey.SecretKey != value.FirstOrDefault())
                {
                    return Unauthorized(new { Message = "API Key Invalid" });
                }
                var validationResult = _validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var managedUser = await _userRepository.FindByMobilePhoneAsync(request.PhoneNumber!);

                if (managedUser == null)
                {
                    var otp = _otpService.GenerateOTP();
                   
                    if (otp == null)
                    {
                        var responseobj = new LoginAuthResponse(request.PhoneNumber, otp, "OTP Gengration failed");

                        return BadRequest(responseobj);
                    }
                    if (!_oTPConfiguration.Manual.Equals("yes") && _otpService != null)
                    {
                        var (response, content) = await _otpService.SendOTP(request.PhoneNumber, otp, _sMSRequestObject);
                        // Save OTP in a cache or database with expiration
                        try
                        {
                            await _verificationService.SendVerificationCodeAsync(
                    request.PhoneNumber, otp);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("error on " + ex.Message);
                        }


                    }
                    else
                    {
                        await _verificationService.SendVerificationCodeAsync(
                        request.PhoneNumber, "1111");
                    }

                    return Ok(new { Message = "OTP sent successfully" });
                }
                else
                {
                    return Conflict(new { Message = "Mobile Number already exists" });
                   
                }
            }
            else
                return Unauthorized();
        }

        [HttpPost]
        [Route("ResetPassword")]
        //[ValidateCustomAntiforgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword([FromBody] LoginAuthModel request)
        {
            if (Request.Headers.TryGetValue(CommonHelper.KMDSECRETAPIKEY, out StringValues value))
            {
                if (_kMDAPISecretKey.SecretKey != value.FirstOrDefault())
                {
                    return Unauthorized(new { Message = "API Key Invalid" });
                }
                var validationResult = _validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }


                var managedUser = _context.Applicationuser.Where(x=>x.MobileNumber==request.PhoneNumber).FirstOrDefault();

                if (managedUser == null)
                {
                    return NotFound(new { Message = "Mobile Number not found" });
                }
                else
                {


                    var otp = _otpService.GenerateOTP();
                    //var otp = await _smsSender.SendMessageAsync(managedUser.PhoneNumber, "");
                    if (otp == null)
                    {
                        var responseobj = new LoginAuthResponse(request.PhoneNumber, otp, "OTP Gengration failed");

                        return BadRequest(responseobj);
                    }
                   
                        managedUser.OTP = otp;
                        //StringContent content = null;
                        var (response, content) = await _otpService.SendOTP(request.PhoneNumber, otp, _sMSRequestObject);
                        _otpService.SendEmail(managedUser.Email, otp);
                  


                    
                    managedUser.OTPExpiration = DateTime.Now.AddMonths(1);
                    managedUser.ResetPasswordOTP = otp;
                    _context.Applicationuser.Update(managedUser);
                    _context.SaveChanges();
                    return Ok(new { Message = "OTP sent successfully" });

                }
            }
            else
                return Unauthorized();
        }
       
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult> RefreshToken([FromBody] LoginAuthModel request)
        {


            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }


            var managedUser = await _userRepository.FindByMobilePhoneAsync(request.PhoneNumber!);

            if (managedUser == null)
            {
                return NotFound(new { Message = "Mobile Number not found" });
            }
            else
            {
                var userId= managedUser?.Id;
                DateTime? expiresAt = null, createdAt = null;
                var accessToken = _tokenService.GenerateToken(request.PhoneNumber, out expiresAt, out createdAt);
                var isProfileComplete = false;
                var roles = (from r in _context.ApplicationRole.AsNoTracking()
                             join ur in _context.ApplicationUserRole.AsNoTracking()
                on r.Id equals ur.RoleId
                             where ur.UserId == userId
                             select r.RoleName
                                ).ToList();
                return Ok(new
                {
                    IsProfileComplete = isProfileComplete,
                    UserId = managedUser?.Id,
                    Token = accessToken,
                    createdAt = createdAt,
                    expiresAt = expiresAt,
                    UserName = managedUser.FirstName,
                    Is_SystemAdmin = managedUser.Is_SystemAdmin,
                    Email = managedUser.Email,
                    MobileNumber = managedUser.MobileNumber,
                    userType = new { id = managedUser.UserType.Value, name = managedUser.UserType.Value.ToString() },
                    roles=roles,
                    Message = "Mobile validated successfully"
                });
            }

        }


        [HttpPost]
        [Route("ProfileLogin")]
        //  [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ProfileLogin([FromBody] LoginModel request)
        {
            if (Request.Headers.TryGetValue(CommonHelper.KMDSECRETAPIKEY, out StringValues value))
            {
                if (_kMDAPISecretKey.SecretKey != value.FirstOrDefault())
                {
                    return Unauthorized(new { Message = "RegisterationKey Invalid" });
                }


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var managedUser = await _userRepository.FindByMobilePhoneAsync(request.PhoneNumber!);

                if (managedUser == null)
                {
                    return NotFound(new { Message = "Mobile Number not found" });
                }
                else
                {
                    var UserId = managedUser?.Id;
                    DateTime? expiresAt = null, createdAt = null;
                    var accessToken = _tokenService.GenerateToken(request.PhoneNumber, out expiresAt, out createdAt);
                    var isProfileComplete = false;
                    var roles = (from r in _context.ApplicationRole.AsNoTracking()
                                 join ur in _context.ApplicationUserRole.AsNoTracking()
                    on r.Id equals ur.RoleId
                                 where ur.UserId == UserId
                                 select r.RoleName
                               ).ToList();
                    return Ok(new
                    {
                        IsProfileComplete = isProfileComplete,
                        UserId = managedUser?.Id,
                        Token = accessToken,
                        createdAt = createdAt,
                        expiresAt = expiresAt,
                        UserName = managedUser.FirstName,
                        Is_SystemAdmin = managedUser.Is_SystemAdmin,
                        Email = managedUser.Email,
                        MobileNumber = managedUser.MobileNumber,
                        userType = new { id = managedUser.UserType.Value, name = managedUser.UserType.Value.ToString() },
                        Message = "Mobile validated successfully"
                    });
                }
            }
            else
            {
                return Unauthorized();
            }


        }

        [HttpPost]
        [Route("VerifyOTP")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyOTP([FromBody] LoginModel request)
        {
            if (Request.Headers.TryGetValue(CommonHelper.KMDSECRETAPIKEY, out StringValues value))
            {
                if (_kMDAPISecretKey.SecretKey != value.FirstOrDefault())
                {
                    return Unauthorized(new { Message = "RegisterationKey Invalid" });
                }


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userRepository.ValidateOTP(request.PhoneNumber, request.OTP);

                if (user == null)
                    return Unauthorized(new { Message = "Invalid or expired OTP" });

                DateTime? expiresAt = null, createdAt = null;
                var accessToken = _tokenService.GenerateToken(request.PhoneNumber, out expiresAt, out createdAt);
                var isProfileComplete = false; int associationId = 0;
                if (user != null && user.IsProfileComplete != null)
                {
                    isProfileComplete = user.IsProfileComplete.Value;
                    associationId = user.AssociationId.HasValue ? user.AssociationId.Value : 0;
                }
                bool IsPolicyPurchased = true;
                if (user!=null&& (user.UserType == UserType.Pensioner || user.UserType == UserType.Community))
                {

                    var policy = _context.PolicyHeader
                               .Where(x => x.UserId == user.Id).FirstOrDefault();

                    if (policy == null)
                    {
                        IsPolicyPurchased = false;
                    }
                }
                var userId = user?.Id;
                var roles = (from r in _context.ApplicationRole.AsNoTracking()
                             join ur in _context.ApplicationUserRole.AsNoTracking()
                on r.Id equals ur.RoleId
                             where ur.UserId == userId
                             select r.RoleName
                             ).ToList();
                return Ok(new
                {
                    IsProfileComplete = isProfileComplete,
                    UserId = user?.Id,
                    Token = accessToken,
                    expiresAt = expiresAt,
                    createdAt = createdAt,
                    AssociationId = associationId,
                    Is_SystemAdmin = user.Is_SystemAdmin,
                    UserName = user.FirstName,
                    IsPolicy = IsPolicyPurchased,
                    userType = new { id = user.UserType.Value, name = user.UserType.Value.ToString() },
                    Message = "OTP validated successfully"
                });

            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost]

        [HttpPost]
        [Route("VerifySignUpOTP")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifySignUpOTP([FromBody] LoginModel request)
        {
            if (Request.Headers.TryGetValue(CommonHelper.KMDSECRETAPIKEY, out StringValues value))
            {
                try
                {
                    if (_kMDAPISecretKey.SecretKey != value.FirstOrDefault())
                    {
                        return Unauthorized(new { Message = "RegisterationKey Invalid" });
                    }
                    var cacheKey = $"otp_{request.PhoneNumber}";

                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    var isValid = await _verificationService.VerifyOtpAsync(
                     request.PhoneNumber, request.OTP);

                    if (!isValid)
                        return BadRequest(new { message = "Invalid or expired verification code" });

                    return Ok(true);
                }
                catch (Exception ex)
                {
                    
                }
                // OTP verified successfully
                return Ok(new { Message = "Mobile number verified successfully." });



            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpPost]
        [Route("ResetPasswordVerifyOTP")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPasswordVerifyOTP([FromBody] LoginModel request)
        {
            if (Request.Headers.TryGetValue(CommonHelper.KMDSECRETAPIKEY, out StringValues value))
            {
                if (_kMDAPISecretKey.SecretKey != value.FirstOrDefault())
                {
                    return Unauthorized(new { Message = "RegisterationKey Invalid" });
                }


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userRepository.ResetPasswordValidateOTP(request.PhoneNumber, request.OTP);

                if (user == null)
                    return Unauthorized(new { Message = "Invalid OTP" });


                return Ok(new
                {
                    UserId = user?.Id,
                    UserName = user.FirstName,
                    Is_SystemAdmin = user.Is_SystemAdmin,
                    userType = new { id = user.UserType.Value, name = user.UserType.Value.ToString() },
                    Message = "OTP validated successfully"
                });

            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpPost]
        [Route("UpdateResetPassword")]
        [AllowAnonymous]
        public async Task<ActionResult> UpdateResetPassword([FromBody] ResetPasswordAuthModel request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _context.Applicationuser.Where(x => x.MobileNumber == request.PhoneNumber).FirstOrDefault();
            if (user == null)
                return BadRequest(new { Message = "User Not Found" });

            user.Password = request.Password;
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = user.Id;
            _context.Applicationuser.Update(user);
            _context.SaveChanges();


            return Ok(new
            {
                Message = "Password updated successfully"
            });


        }


        [HttpPost]
        [Route("gettoken")]
        [AllowAnonymous]
        public async Task<ActionResult> gettoken([FromBody] LoginModel request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.ValidateOTP(request.PhoneNumber, request.OTP);

            if (user == null)
                return Unauthorized(new { Message = "Invalid or expired OTP" });

            DateTime? expiresAt = null, createdAt = null;
            var accessToken = _tokenService.GenerateToken(request.PhoneNumber, out expiresAt, out createdAt);
            var isProfileComplete = false;
            if (user != null && user.IsProfileComplete != null)
                isProfileComplete = user.IsProfileComplete.Value;

            return Ok(new
            {
                IsProfileComplete = isProfileComplete,
                UserId = user?.Id,
                Token = accessToken,
                expiresAt = expiresAt,
                createdAt = createdAt,
                UserName = user.FirstName,
                userType = new { id = user.UserType.Value, name = user.UserType.Value.ToString() },
                Message = "OTP validated successfully"
            });


        }

        [HttpGet]
        [Route("Helthcheck")]
        [AllowAnonymous]
        public async Task<ActionResult> Helthcheck()
        {
            return Ok();
        }
        [HttpGet]
        [Route("emailCheck")]
        [AllowAnonymous]
        [IpAuthorizationFilter]
        public async Task<ActionResult> EmailCheck()
        {
            // await _emailService.SendEmailAsync("vilas.s@syborgtech.com", "test", "test");
            return Ok();
        }
        [HttpGet("GetStates")]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> GetStates()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                var user = _commonHelperRepository.GetStates();
                return await Task.FromResult(Ok(user));
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
        [AllowAnonymous]
        public IActionResult GetDetailedIpInfo()
        {
            var connection = HttpContext.Connection;
            return Ok(new
            {
                RemoteIpAddress = connection.RemoteIpAddress?.ToString(),
                LocalIpAddress = connection.LocalIpAddress?.ToString(),
                RemotePort = connection.RemotePort,
                LocalPort = connection.LocalPort,
                ForwardedHeader = Request.Headers["X-Forwarded-For"].ToString(),
                Host = Request.Host.ToString()
            });
        }
    }
}
