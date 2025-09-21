using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.ReportDataModels
{
    public class GetRefundReportsDataModels
    {
        [Key]
        public int RefundRequestId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? CountryCode { get; set; }
        public string? MobileNumber { get; set; }
        public DateTime? DOB { get; set; }
        public int? UserType { get; set; }
        public int? Gender { get; set; }
        public string? OrganisationName { get; set; }
        public string? AssociationName { get; set; }
        public string? EMPIDPFNo { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Pincode { get; set; }
        public int? PolicyId { get; set; }
        public int? RetireeId { get; set; }
        public decimal? RefundAmount {get; set; }
        public DateTime? RefundRequestDate { get; set; }
        public int? Status { get; set; }
        public string? Comment { get; set; }
        public int? RefundRequstHandledBy { get; set; }
        public int? PaymentType { get; set; }

        //Cheque = 1, NEFT = 2,UPI=3, Gateway=4
        // Conditional Payment Details for UPI
        public string? TransactionNumber{ get; set; }
        public decimal? UPIAmount{ get; set; }
        public DateTime? UPIDate { get; set; }

        // Conditional Payment Details for NEFT
        public string? NEFTTransactionId{ get; set; }
        public decimal? NEFTAmount { get; set; }
        public DateTime? NEFTDate { get; set; }
        public string? NEFTBankName { get; set; }
        public string? NEFTBranchName { get; set; }
        public string? NEFTAccountNumber { get; set; }
        public string? NEFTIfscCode { get; set; }

        // Conditional Payment Details for Cheque
        public string? ChequeNumber { get; set; }
        public decimal? ChequeAmount { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string? ChequeBankName { get; set; }


    }
}
