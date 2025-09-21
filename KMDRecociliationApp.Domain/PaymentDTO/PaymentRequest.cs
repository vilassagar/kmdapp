using KMDRecociliationApp.Domain.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.PaymentDTO
{

    public class DTOPaymentPolicy
    {
        public int UserId { get; set; }
        public int? PolicyId { get; set; }
        public decimal? TotalPremium { get; set; } = 0;
        public decimal? AmountPaid { get; set; } = 0;
        public decimal? TotalPaidPremium { get; set; } = 0;
        public PaymentModeOnline? Online { get; set; }
        public bool IsUpdate { get; set; } = false;
        public PaymentRequestBody1 PaymentRequestBody { get; set; }
    }

    public class PaymentAuthenticationRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class PaymentTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expiresIn")]
        public int ExpiresIn { get; set; }
    }
    public class PaymentFloatRequest
    {
        public decimal premiumAmount { get; set; }  // Mandatory
        public string customerCode { get; set; }   // Mandatory
    }

    public class PaymentRequestBody
    {
        public string MobileNumber { get; set; }
        public string requestReference { get; set; }   // Mandatory
        public string successReturnUrl { get; set; }   // Mandatory
        public string cancelReturnUrl { get; set; }    // Mandatory
        public string Email { get; set; }
        public List<PaymentFloatRequest> floatRequests { get; set; }
       
    }
    public class PaymentRequestBody1
    {
        public string MobileNumber { get; set; }
        public string requestReference { get; set; }   // Mandatory
        public string successReturnUrl { get; set; }   // Mandatory
        public string cancelReturnUrl { get; set; }    // Mandatory
        public string Email { get; set; }
        public List<PaymentFloatRequest> floatRequests { get; set; }
        public NotificationPayLoad? notification { get; set; } = null;
    }
    public class NotificationPayLoad
    {
        public string method { get; set; }
        public string requestId { get; set; }
        public string uri { get; set; }

    }
    public class PaymentstatusPayload
    {
        public string requestId { get; set; }
        public string premiumAmount { get; set; }
        public int statusCode { get; set; }
        public string transactionDate { get; set; }
        public string transactionNumber { get; set; }
        public string status { get; set; }
        public bool absPaymentUpdate { get; set; }
        public bool absPolicyIssuance { get; set; }
    }

    public class NotifyPaymentStatusPayload
    {
        public int UserId { get; set; }
        public int? PolicyId { get; set; }
        public string transactionNumber { get; set; }=string.Empty;
        public decimal premiumAmount { get; set; }
    }
    public class ManualNotifyPaymentStatusPayload
    {
        public string MobileNumber { get; set; }
        public string transactionNumber { get; set; } = string.Empty;
        public decimal premiumAmount { get; set; }
    }


    public class FloatRequest
    {
        public decimal premiumAmount { get; set; }
       // public string customerCode { get; set; }
    }

    public class Notification
    {
        public string method { get; set; }
        public string requestId { get; set; }
        public string uri { get; set; }
    }

    public class RootObject
    {
        public string mobileNumber { get; set; }
        public string requestReference { get; set; }
        public string successReturnUrl { get; set; }
        public string cancelReturnUrl { get; set; }
        public string email { get; set; }
        public List<FloatRequest> floatRequests { get; set; }
        public Notification notification { get; set; }
    }

}
