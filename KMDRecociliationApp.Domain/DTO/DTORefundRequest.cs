using KMDRecociliationApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTORefundRequest
    {
        public int RefundRequestNumber { get; set; }     
        public int OrderNumber { get; set; }
        public string? RetireeName { get; set; }
        public int UserId { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime RefundRequestDate { get; set; }
        public int Status { get; set; }
        public string? RefundRequestStatus { get; set; }

    }

    public class DTORefundRequestUpdate
    {
        public int RefundRequestNumber { get; set; }
        public int OrderNumber { get; set; }
        public decimal RefundAmount { get; set; }
        public bool IsAccepted { get; set; }
        public int? RefundRequestId { get; set; }
        public int RefundPaymentMode { get; set; }
        public DTOChequeDetails? ChequeDetails { get; set; }
        public DTOPaymentModeNEFT? Neft { get; set; }
        public DTOPaymentModeUPI? Upi { get; set; }

    }

    public class DTORefundPaymentModeUPI
    {
        public string? TransactionNumber { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; } = DateTime.Now;
        public CommonFileModel? UpiPaymentReceipt { get; set; }
    }

    public class DTORefundPaymentModeNEFT
    {
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IfscCode { get; set; }
        public string? TransactionId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; } = DateTime.MinValue;
        public CommonFileModel? NeftPaymentReceipt { get; set; }

    }
    public class DTORefundChequeDetails
    {
        public string? ChequeNumber { get; set; }
        public decimal? Amount { get; set; }
        public string? BankName { get; set; }
        public DateTime? Date { get; set; } = DateTime.Now;
        public int? InFavourOfId { get; set; }
        public CommonNameDTO? InFavourOf { get; set; }
        public bool IsChequePhotoUpdated { get; set; } = false;
        public CommonFileModel? ChequePhoto { get; set; }
    }
}
