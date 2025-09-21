using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTOPaymentReceipt
    {
        public string Name { get; set; }
        public int OrderNumber { get; set; }
        public decimal? TotalPremium { get; set; }
        public decimal? AmountPaid { get; set; }
        public List<DTOPaymentReceiptDetails>? PaymentReceiptDetails { get; set; }
    }
    public class DTOPaymentReceiptDetails
    {
        public DateTime? PaymentDate { get; set; }
        public decimal? AmountPaid { get; set; }
        public string? PaymentMode { get; set; }
        public DateTime? PaymentAcceptedDate { get; set; }
    }
}
