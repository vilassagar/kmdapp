using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class RefundRequest : BaseEntity
    {
        public int PolicyId { get; set; }  
        public int RetireeId { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime RefundRequestDate { get; set; }      
        public PaymentStatus  Status { get; set; }
        public string? Comment { get; set; }
        public int? RefundRequstHandledBy { get; set; }
        public string? RefundDocumentName { get; set; }
        public string? RefundDocumentUrl { get; set; }
        public ApplicationUser ?User { get; set; }
        public PaymentTypes? PaymentType { get; set; }
        public int? UserId { get; set; }
     
    }

    public class RefundPaymentModeUPI : KeyEntity
    {
        public int? RefundId { get; set; }
        public string? TransactionNumber { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        public string? UPIReceiptDocumentName { get; set; }
        public string? UPIReceiptDocumentUrl { get; set; }
        public int? UserId { get; set; }
       
    }

    public class RefundPaymentModeNEFT : KeyEntity
    {
        public int? RefundId { get; set; }
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public string? IfscCode { get; set; }
        public string? TransactionId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        public string? NEFTReceiptDocumentName { get; set; }
        public string? NEFTReceiptDocumentUrl { get; set; }
        public int? UserId { get; set; }

    }
    public class RefundPaymentModeCheque : KeyEntity
    {
        public int? RefundId { get; set; }
        public string? ChequeNumber { get; set; }
        public decimal? Amount { get; set; }
        public string? BankName { get; set; }
        public DateTime? Date { get; set; }
        public string? RetireeName { get; set; }
        //public int? InFavourOfAssociationId { get; set; }
        public string? ChequePhotoDocumentName { get; set; }
        public string? ChequePhotoDocumentUrl { get; set; }
        public int? UserId { get; set; }
    }
}
