using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.PaymentDTO
{
    public class GoDigitPaymentGateway
    {
        public AuthEndPoint AuthEndPoint { get; set; }
        public ExecutorEndPoint ExecutorEndPoint { get; set; }
        public NotificationURL NotificationURL { get; set; }
    }
    public class AuthEndPoint
    {
        public string url { get; set; }
        public string Cookie { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class ExecutorEndPoint
    {
        public string url { get; set; }
        public string IntegrationId { get; set; }
        public string Cookie { get; set; }

    }
    public class NotificationURL
    {
        public string url { get; set; }        
    }
}
