using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTOPaymentHistory
    {
        public int PaymentId { get; set; }
        public string? PaymentMode { get; set; }
        public decimal? payableAmount { get; set; }
        public DateTime? Date { get; set; }
        public decimal? amountPaid { get; set; }
        public string? transactionId { get; set; }
        public string? OrderNumber { get; set; }
        public string? paymentStatus { get; set; }
        public string? acknowledgement { get; set; }


    }
}
