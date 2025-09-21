using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTOMyPolicies
    {
        public int OrderId { get; set; }
        public decimal TotalPremium { get; set; }
        public DateTime OrderDate { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentType { get; set; }
        public string? PaymentMode { get; set; }
        public decimal AmountPaid { get; set; }
        public bool IsOrderfreez { get; set; }=false;
        public List<DTOPolicies> policies { get; set; }
        public int AssociationId { get; set; } 
    }
    public class DTOPolicies
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal SumInsured { get; set; }
        public decimal TopupSumInsured { get; set; }
        public decimal Premium { get; set; }
        public string? PremiumReceipt { get; set; }
    }
    public class DTOPolicyOrders
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal SumInsured { get; set; }
        public decimal TopupSumInsured { get; set; }
        public decimal Premium { get; set; }
        public string? PremiumReceipt { get; set; }
    }
    public class DTOOnlinePolicyOrders
    {
        public int OrderId { get; set; }
        public decimal? Amount { get; set; }   
        public decimal? PaidAmount { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Status { get; set; }
        public string OrganisationName { get; set; }
        public string AssociationName { get; set; }
        public string Comment { get; set; }
        public string TransactionNumber { get; set; } 
        public DateTime? Date { get; set; }
        public int? AssociationId { get; set; }
    }



}
