using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.ReportDataModels
{
    public class IncompleteTransactionDataModels
    {
        public int? id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? CountryCode { get; set; }
        public string? MobileNumber { get; set; }
        public string? OrganisationName { get; set; }
        public string? AssociationName { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal? PremimumAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public int? PaymentType { get; set; }
        public int? PaymentMode { get; set; }
        public int? PaymentStatus { get; set; }
    }
}
