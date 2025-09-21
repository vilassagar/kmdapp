using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class PaymentHeader:BaseEntity
    {
        public decimal? TotalPremimumAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public int?PolicyId { get; set; }
        public int?UserId { get; set; }
        public int? CampaignId { get; set; }
        public int? CurrentPaymentId { get; set; }
        public PaymentTypes? RefundPaymentMode { get; set; }
        public decimal? RefundAmount { get; set; }
    }
    public class PaymentDetails : BaseEntity
    {
        public int PaymentHeaderId { get; set; }
        public decimal? TotalPremimumAmount { get; set; }
        public int? PolicyId { get; set; }
        public int? UserId { get; set; }       
        public int? CampaignId { get; set; }
        public PaymentTypes PaymentType { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public bool? IsPaymentConfirmed { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public string? TransactionId { get; set; }
        public decimal? PayableAmount { get; set; }
        public decimal? AmountPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? PaymentAcceptedDate { get; set; }
        public bool? IsAccepted { get; set; }
        public string? Comment { get; set; }
    }

}
