using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{


    public class PaymentModeGateway : KeyEntity
    {
        public int? PaymentDetailId { get; set; }
        public int? PolicyId { get; set; }
        public string? TransactionId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        public string ?DigitPaymentId { get; set; }
        public int? UserId { get; set; }
        public int? CampaignId { get; set; }
    }
      
    public class PaymentModeUPI : KeyEntity
    {
        public int? PaymentDetailId { get; set; }
        public int? PolicyId { get; set; }
        public string? TransactionNumber { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        public string? UPIReceiptDocumentName { get; set; }
        public string? UPIReceiptDocumentUrl { get; set; }
        public int? UserId { get; set; }
        public int? CampaignId { get; set; }

    }

    public class PaymentModeNEFT : KeyEntity
    {
        public int? PaymentDetailId { get; set; }
        public int? PolicyId { get; set; }
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
        public int? CampaignId { get; set; }

    }
    public class PaymentModeCheque : KeyEntity
    {
        public int? PaymentDetailId { get; set; }
        public int? PolicyId { get; set; }
        public string? ChequeNumber { get; set; }
        public decimal? Amount { get; set; }
        public string? BankName { get; set; }
        public string? Ifsccode { get; set; }
        public string? Micrcode { get; set; }
        public DateTime? Date { get; set; }
        public int? InFavourOfAssociationId { get; set; }
        public string? ChequePhotoDocumentName { get; set; }
        public string? ChequePhotoDocumentUrl { get; set; }
        public string? ChequeDepositLocation { get; set; }
        public int? UserId { get; set; }
        public int? CampaignId { get; set; }
    }

}
