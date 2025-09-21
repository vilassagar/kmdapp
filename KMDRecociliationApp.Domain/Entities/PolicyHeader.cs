using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class PolicyHeader : BaseEntity
    {
        public int UserId { get; set; }
        public decimal? TotalPremimum { get; set; }
        public decimal? TotalPaidPremimum { get; set; } = 0;
        public decimal? ChildPremium { get; set; } = 0;
        public string? DigitPaymentId { get; set; }
        public string? Comment { get; set; }
        public bool? IsManual { get; set; }
        public bool? IsProfilePreez { get; set; }
        public PaymentStatus? PaymentStatus { get; set; } = 0;
        public int? CampaignId { get; set; }
        public ICollection<PolicyProductDetails> PolicyProductDetailsList { get; set; } = new List<PolicyProductDetails>();
    
    }
    public class PolicyProductDetails : KeyEntity
    {
        public int PolicyHeaderId { get; set; }
        public int ?UserId { get; set; }
        public bool? IsDisclaimerAccepted { get; set; } = false;
        public int? CampaignId { get; set; }
        public decimal? TotalProductPremimum { get; set; }
        public decimal ?SumInsured { get; set; }
        public decimal? TopupSumInsured { get; set; } = 0;
        public bool ?IsTopUpSelected { get; set; }
        public int ?SumInsuredPremimumId { get; set; }
        public int? TopupSumInsuredPremimumId { get; set; } = 0;
        public int? ProductId { get; set; }  
        public PolicyHeader? PolicyHeader { get; set; }
        public Product? Product { get; set; }
        public ICollection<PolicyProductPremimum> ProductPremiums { get; set; } = new List<PolicyProductPremimum>();
    }
    public class PolicyProductPremimum : KeyEntity
    {
       
        public int PolicyProductDetailsId { get; set; }
        public int PolicyHeaderId { get; set; }
        public int? ProductPremimumId { get; set; }
        public int? ProductId { get; set; }
        public int ?AgeBandPremiumRateId { get; set; }
        public string? AgeBandValue { get; set; }
        public int? UserId { get; set; }
        public int? CampaignId { get; set; }
        public decimal? SumInsured { get; set; }
        public decimal? SelfOnlyPremium { get; set; }
        public decimal? SpousePremium { get; set; }
        public decimal? SelfSpousePremium { get; set; }
        public decimal? SelfSpouse2ChildrenPremium { get; set; }
        public decimal? SelfSpouse1ChildrenPremium { get; set; }
        public decimal? Self1ChildrenPremium { get; set; }
        public decimal? Self2ChildrenPremium { get; set; }
        public decimal? Child1Premium { get; set; }
        public decimal? Child2Premium { get; set; }
        public decimal? Parent1Premium { get; set; }
        public decimal? Parent2Premium { get; set; }
        public decimal? InLaw1Premium { get; set; }
        public decimal? InLaw2Premium { get; set; }      
        public int? ParentProductPremimumId { get; set; } 
        public bool ?IsChild1PremiumSelected { get; set; }
        public bool ?IsChild2PremiumSelected { get; set; }
        public bool? IsInLaw1PremiumSelected { get; set; }
        public bool ?IsInLaw2PremiumSelected { get; set; }
        public bool? IsParent1PremiumSelected { get; set; }
        public bool ?IsParent2PremiumSelected { get; set; }
        public bool? IsSpousePremiumSelected { get; set; }
        public bool? IsSelfSpousePremiumSelected { get; set; }
        public bool? IsSelfSpouse2ChildrenPremiumSelected { get; set; }
        public bool? IsSelfSpouse1ChildrenPremiumSelected { get; set; }
        public bool? IsSelf2ChildrenPremiumSelected { get; set; }
        public bool? IsSelf1ChildrenPremiumSelected { get; set; }
        public bool? IsSelfPremiumSelected { get; set; }
        public AgeBandPremiumRate AgeBandPremiumRate { get; set; }
    }
 public class AgeBandPremiumRate:ParentEntity
    {
        [Required]        
        public int AgeBandStart { get; set; }
        [Required]   
        public int AgeBandEnd { get; set; }        
        [Required]
        public string? AgeBandValue { get; set; }
        // Navigation property (optional - for reverse navigation)
        public ICollection<ProductPremimum> ProductPremiums { get; set; }
        public ICollection<PolicyProductPremimum>? PolicyProductPremimums { get; set; }
        }
}
