using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTOOfflinePayment
    {
        public string PaymentMode { get; set; }
        public string Comment { get; set; }
        public string? RetireeName { get; set; }
        public DTOChequeDetails? ChequeDetails { get; set; }
        public DTOPaymentModeNEFT? Neft { get; set; }
        public DTOPaymentModeUPI? Upi { get; set; }
        public DTOPaymentModeGateway? Gateway { get; set; }
    }
    public class DTOOfflinePaymentDetails
    {
        public int PaymentId { get; set; }
        public bool? IsAccepted { get; set; }
        public string? Comment { get; set; }
    }
}
