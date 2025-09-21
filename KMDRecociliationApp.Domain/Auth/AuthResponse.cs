using FluentValidation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Auth
{
    public class AuthResponse
    {
        public string? MobilePhone { get; set; }
        public string? Token { get; set; }
        
    }
   
    public class LoginModel
    {
        public string? PhoneNumber { get; set; }      
        public string? OTP { get; set; }

    }
    public class LoginAuthModel
    {
        public bool IsPasswordLogin { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
    }
    public class ResetPasswordAuthModel
    {
        public string ConfirmPassword { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class LoginValidator : AbstractValidator<LoginAuthModel>
    {
        public LoginValidator()
        {
            RuleFor(user => user.PhoneNumber).NotEmpty().WithMessage("Mobile number cannot be empty");
        }
    }

    public class LoginAuthResponse
    {
        public LoginAuthResponse(string mobilePhone,string otp,string message)
        {
            MobilePhone=mobilePhone;
            OTP=otp;
            Message=message;
        }
        public string? MobilePhone { get; set; }
        public string? OTP { get; set; }
        public string? Message { get; set; }
    }
    public class LoginResponseDTO
    {
        public bool Flag { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
    }

    public class CaptchaRequest
    {
        public string? Token { get; set; }
    }
    public class CaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("challenge_ts")]
        public DateTime ChallengeTimestamp { get; set; }
        [JsonProperty("hostname")]
        public string? Hostname { get; set; }
    }
}
