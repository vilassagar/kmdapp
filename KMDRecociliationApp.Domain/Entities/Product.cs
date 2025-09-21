using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string? ProductName { get; set; }
        public string? ProviderName { get; set; }
        public string? Disclaimer { get; set; }
        public int? PolicyTypeId { get; set; }
        public int? BasePolicyId { get; set; }
        public bool? IsSpouseCoverage { get; set; }
        public bool? IsHandicappedChildrenCoverage { get; set; }
        public bool? IsParentsCoverage { get; set; }
        public bool ?IsInLawsCoverage { get; set; }
        public int ?NumberOfHandicappedChildren { get; set; }
        public int ?NumberOfParents { get; set; }
        public int ?NumberOfInLaws { get; set; }
        public string? ProductBroucher { get; set; }
        public ProductPolicyType? PolicyType { get; set; }
        public string? ProductDocumentName { get; set; }
        public string? ProductDocumentUrl { get; set; }
        public Product? BasePolicy { get; set; }
        // Navigation property to the child ProductPremiums
        public ICollection<ProductPremimum> ProductPremiums { get; set; } = new List<ProductPremimum>();
        // Navigation property to the child products
        public ICollection<Product>? ChildProducts { get; set; } = new List<Product>();
    }

    public class ProductType
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
    public class ProductDocument : BaseEntity
    {
        public int ProductId { get; set; }
        public string ProductBroucher { get; set; }
        public Product Product { get; set; }

    }
    public class ProductPremimum : BaseEntity
    {
        public int ProductId { get; set; }
       
        public decimal? SumInsured { get; set; }
        public decimal? SelfOnlyPremium { get; set; }
        public decimal? SpousePremium { get; set; }
        public decimal? SelfSpousePremium { get; set; }
        public decimal? SelfSpouse2ChildrenPremium { get; set; } = 0;
        public decimal? SelfSpouse1ChildrenPremium { get; set; } = 0;
        public decimal? Self2ChildrenPremium { get; set; } = 0;
        public decimal? Self1ChildrenPremium { get; set; } = 0;
        public decimal? Child1Premium { get; set; }
        public decimal? Child2Premium { get; set; }
        public decimal? Parent1Premium { get; set; }
        public decimal? Parent2Premium { get; set; }
        public decimal? InLaw1Premium { get; set; }
        public decimal? InLaw2Premium { get; set; }
        // Foreign key property
        public int ?AgeBandPremiumRateId { get; set; }
        public string? AgeBandPremiumRateValue { get; set; }

        public Product? Product { get; set; }
        public AgeBandPremiumRate? AgeBandPremiumRate { get; set; }
        public int? ParentProductPremimumId { get; set; } // Foreign key to the parent ProductPremimum

        // Navigation property to the parent ProductPremimum
        public ProductPremimum? ParentProductPremimum { get; set; }
        public ICollection<ProductPremimum> ? TopUpOptions { get; set; } = new List<ProductPremimum>();
    }


}
