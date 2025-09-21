using KMDRecociliationApp.Domain.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTOPaymentDetails
    {
        public int Id { get; set; } = 0;
        public int PaymentModeId { get; set; }
        public string OfflinePaymentMode { get; set; }=string.Empty;
        public int PaymentTypeId { get; set; }
        public PaymentModeOnline? Online { get; set; }
        public PaymentModeOffline? Offline { get; set; }
        public bool? IsPaymentConfirmed { get; set; }

    }
    public class DTOPaymentModeGateway
    {
        public string? transactionNumber { get; set; }
        public string? TransactionId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; } = DateTime.Now;
    }

    public class PaymentModeOnline
    {
        public decimal? AmountPayable { get; set; }
        public decimal? AmountPaid { get; set; }
        public string? TransactionId { get; set; }
        public string? PaymentStatus { get; set; }
    }
    public class PaymentModeOffline
    {
        public int? OfflinePaymentModeId { get; set; }
        public DTOChequeDetails? ChequeDetails { get; set; }
        public DTOPaymentModeNEFT? Neft { get; set; }
        public DTOPaymentModeUPI? Upi { get; set; }
    }
    public class DTOPaymentModeUPI
    {
        public string? TransactionId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }= DateTime.Now;
        public CommonFileModel? UpiPaymentReceipt { get; set; }
    }

    public class DTOPaymentModeNEFT
    {
        public string? BankName { get; set; }
        public string? BranchName { get; set; }    
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IfscCode { get; set; }
        public string? TransactionId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }= DateTime.MinValue;
        public CommonFileModel? NeftPaymentReceipt { get; set; }

    }
    public class DTOChequeDetails
    {
        public string? ChequeNumber { get; set; }
        public decimal? Amount { get; set; }
        public string? BankName { get; set; }
        public string? Ifsccode { get; set; }
        public string? Micrcode { get; set; }
        public string? ChequeDepositLocation { get; set; }
        public DateTime? Date { get; set; } = DateTime.Now;
        public int? InFavourOfId { get; set; }
        public string? RetireeName { get; set; }
        public CommonAssociationDTO? InFavourOf { get; set; }
        public bool IsChequePhotoUpdated { get; set; }=false;
        public CommonFileModel? ChequePhoto { get; set; }
    }
  

}
