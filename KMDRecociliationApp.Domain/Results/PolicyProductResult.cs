using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;

namespace KMDRecociliationApp.Domain.Results
{
    public class PolicyProductResult : BaseResult<Product, PolicyProductResult>
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; } = string.Empty;
        public string? ProviderName { get; set; }
        public string? PolicyType { get; set; }
        public string? BasePolicy { get; set; }
        public int? PolicyTypeId { get; set; }
        public int BasePolicyId { get; set; }
        public bool? IsSpouseCoverage { get; set; }
        public bool? IsParentsCoverage { get; set; }
        public bool? IsHandicappedChildrenCoverage { get; set; }
        public bool? IsInlawsCoverage { get; set; }
        public int? NumberOfHandicappedChildren { get; set; }
        public int? NumberOfParents { get; set; }
        public int? NumberOfInLaws { get; set; }
        public int? NumberOfChildren { get; set; }
        public List<PremiumChartResult>? PremiumChart { get; set; } = new List<PremiumChartResult>();


        public Delta<PolicyProductResult> GetDelta()
        {
            Delta<PolicyProductResult> deleta = new Delta<PolicyProductResult>();
            if (ProductId > 0) deleta.TrySetPropertyValue("Id", ProductId);
            if (!string.IsNullOrEmpty(ProductName)) deleta.TrySetPropertyValue("ProductName", ProductName);
            if (PolicyTypeId > 0) deleta.TrySetPropertyValue("PolicyTypeId", PolicyTypeId);
            return deleta;

        }

        public override PolicyProductResult CopyPolicyData(Product x)
        {
            this.ProductName = x.ProductName;
            this.ProviderName = x.ProviderName;
            this.ProductId = x.Id;
            this.PolicyTypeId = x.PolicyTypeId;
            this.IsSpouseCoverage = x.IsSpouseCoverage;
            this.IsParentsCoverage = x.IsParentsCoverage;
            this.IsHandicappedChildrenCoverage = x.IsHandicappedChildrenCoverage;
            this.IsInlawsCoverage = x.IsInLawsCoverage;
            this.NumberOfInLaws = x.NumberOfInLaws;
            this.NumberOfChildren = x.NumberOfHandicappedChildren;
            this.NumberOfParents = x.NumberOfParents;
            this.BasePolicy = x.BasePolicy != null ? x.BasePolicy.ProductName : "";
            this.PolicyType = ((ProductPolicyType)x.PolicyTypeId).ToString();
            if (x.ProductPremiums != null && x.ProductPremiums.Count > 0)
            {
                this.PremiumChart = new List<PremiumChartResult>();
                foreach (var item in x.ProductPremiums)
                {
                    PremiumChartResult premiumChartResult = new PremiumChartResult();
                    premiumChartResult.Child1Premium = item.Child1Premium;
                    premiumChartResult.Child2Premium = item.Child2Premium;
                    premiumChartResult.Parent1Premium = item.Parent1Premium;
                    premiumChartResult.Parent2Premium = item.Parent2Premium;
                    premiumChartResult.InLaw1Premium = item.InLaw1Premium;
                    premiumChartResult.InLaw2Premium = item.InLaw2Premium;
                    premiumChartResult.SelfOnlyPremium = item.SelfOnlyPremium;
                    premiumChartResult.SelfSpouse2ChildrenPremium = item.SelfSpouse2ChildrenPremium;
                    premiumChartResult.SelfSpouse1ChildrenPremium = item.SelfSpouse2ChildrenPremium;
                    premiumChartResult.Self1ChildrenPremium = item.Self1ChildrenPremium;
                    premiumChartResult.Self2ChildrenPremium = item.Self2ChildrenPremium;
                    premiumChartResult.AgeBandPremiumRateValue = item.AgeBandPremiumRate != null ?
                        item.AgeBandPremiumRate.AgeBandValue : "";
                    premiumChartResult.SpousePremium = item.SpousePremium;
                    premiumChartResult.SelfSpousePremium = item.SelfSpousePremium;
                    premiumChartResult.SumInsured = item.SumInsured;
                    premiumChartResult.ProductPremiumId = item.Id;

                    if (item.TopUpOptions != null && item.TopUpOptions.Any())
                    {
                        premiumChartResult.TopUpOptions = new List<PremiumChartResult>();
                        foreach (var topupitem in item.TopUpOptions)
                        {
                            PremiumChartResult topUpOptionsResult = new PremiumChartResult();
                            topUpOptionsResult.Child1Premium = topupitem.Child1Premium;
                            topUpOptionsResult.Child2Premium = topupitem.Child2Premium;
                            topUpOptionsResult.Parent1Premium = topupitem.Parent1Premium;
                            topUpOptionsResult.Parent2Premium = topupitem.Parent2Premium;
                            topUpOptionsResult.InLaw1Premium = topupitem.InLaw1Premium;
                            topUpOptionsResult.InLaw2Premium = topupitem.InLaw2Premium;
                            topUpOptionsResult.SelfOnlyPremium = topupitem.SelfOnlyPremium;
                            topUpOptionsResult.SelfSpouse2ChildrenPremium = topupitem.SelfSpouse2ChildrenPremium;
                            topUpOptionsResult.SelfSpouse1ChildrenPremium = topupitem.SelfSpouse2ChildrenPremium;
                            topUpOptionsResult.Self1ChildrenPremium = topupitem.Self1ChildrenPremium;
                            topUpOptionsResult.Self2ChildrenPremium = topupitem.Self2ChildrenPremium;
                            topUpOptionsResult.SpousePremium = topupitem.SpousePremium;
                            topUpOptionsResult.SumInsured = topupitem.SumInsured;
                            topUpOptionsResult.ProductPremiumId = topupitem.Id;
                            topUpOptionsResult.ParentProductPremiumId = item.Id;
                            topUpOptionsResult.AgeBandPremiumRateValue = topupitem.AgeBandPremiumRate != null ?
                       topupitem.AgeBandPremiumRate.AgeBandValue : "";
                            premiumChartResult.TopUpOptions.Add(topUpOptionsResult);
                        }
                    }
                    this.PremiumChart.Add(premiumChartResult);
                }



            }
            return this;
        }
        public PolicyProductResult CopyTopupPolicyData(Product x, Product baseProduct)
        {
            this.ProductName = x.ProductName;
            this.ProviderName = x.ProviderName;
            this.ProductId = x.Id;
            this.PolicyTypeId = x.PolicyTypeId;
            this.IsSpouseCoverage = x.IsSpouseCoverage;
            this.IsParentsCoverage = x.IsParentsCoverage;
            this.IsHandicappedChildrenCoverage = x.IsHandicappedChildrenCoverage;
            this.IsInlawsCoverage = x.IsInLawsCoverage;
            this.NumberOfInLaws = x.NumberOfInLaws;
            this.NumberOfChildren = x.NumberOfHandicappedChildren;
            this.NumberOfParents = x.NumberOfParents;
            this.BasePolicy = x.BasePolicy != null ? x.BasePolicy.ProductName : "";
            this.PolicyType = ((ProductPolicyType)x.PolicyTypeId).ToString();
            //this.PolicyType = x.PolicyTypeId == 1 ? ProductPolicyType.BasePolicy.ToString() : ProductPolicyType.TopUpPolicy.ToString();
            if (x.ProductPremiums != null && x.ProductPremiums.Count > 0)
            {
                this.PremiumChart = new List<PremiumChartResult>();
                foreach (var item in baseProduct.ProductPremiums)
                {
                    PremiumChartResult premiumChartResult = new PremiumChartResult();
                    premiumChartResult.Child1Premium = item.Child1Premium;
                    premiumChartResult.Child2Premium = item.Child2Premium;
                    premiumChartResult.Parent1Premium = item.Parent1Premium;
                    premiumChartResult.Parent2Premium = item.Parent2Premium;
                    premiumChartResult.InLaw1Premium = item.InLaw1Premium;
                    premiumChartResult.InLaw2Premium = item.InLaw2Premium;
                    premiumChartResult.SelfOnlyPremium = item.SelfOnlyPremium;
                    premiumChartResult.SelfSpousePremium = item.SelfSpousePremium;
                    premiumChartResult.SpousePremium = item.SpousePremium;
                    premiumChartResult.SelfSpouse2ChildrenPremium = item.SelfSpouse2ChildrenPremium;
                    premiumChartResult.SelfSpouse1ChildrenPremium = item.SelfSpouse2ChildrenPremium;
                    premiumChartResult.Self1ChildrenPremium = item.Self1ChildrenPremium;
                    premiumChartResult.Self2ChildrenPremium = item.Self2ChildrenPremium;
                    premiumChartResult.SelfSpousePremium = item.SelfSpousePremium;
                    premiumChartResult.SumInsured = item.SumInsured;
                    premiumChartResult.ProductPremiumId = item.Id;

                    var topUpOptions = x.ProductPremiums
                        .Where(x1 => x1.ParentProductPremimumId == item.Id); //item.Id;

                    if (topUpOptions != null && topUpOptions.Any())
                    {
                        premiumChartResult.TopUpOptions = new List<PremiumChartResult>();
                        foreach (var topupitem in topUpOptions)
                        {
                            PremiumChartResult topUpOptionsResult = new PremiumChartResult();
                            topUpOptionsResult.Child1Premium = topupitem.Child1Premium;
                            topUpOptionsResult.Child2Premium = topupitem.Child2Premium;
                            topUpOptionsResult.Parent1Premium = topupitem.Parent1Premium;
                            topUpOptionsResult.Parent2Premium = topupitem.Parent2Premium;
                            topUpOptionsResult.InLaw1Premium = topupitem.InLaw1Premium;
                            topUpOptionsResult.InLaw2Premium = topupitem.InLaw2Premium;
                            topUpOptionsResult.SelfOnlyPremium = topupitem.SelfOnlyPremium;
                            topUpOptionsResult.SelfSpousePremium = topupitem.SelfSpousePremium;
                            topUpOptionsResult.SelfSpouse2ChildrenPremium = topupitem.SelfSpouse2ChildrenPremium;
                            topUpOptionsResult.SelfSpouse1ChildrenPremium = topupitem.SelfSpouse2ChildrenPremium;
                            topUpOptionsResult.Self1ChildrenPremium = topupitem.Self1ChildrenPremium;
                            topUpOptionsResult.Self2ChildrenPremium = topupitem.Self2ChildrenPremium;
                            topUpOptionsResult.SpousePremium = topupitem.SpousePremium;
                            topUpOptionsResult.SumInsured = topupitem.SumInsured;
                            topUpOptionsResult.ProductPremiumId = topupitem.Id;
                            topUpOptionsResult.ParentProductPremiumId = item.Id;
                            premiumChartResult.TopUpOptions.Add(topUpOptionsResult);
                        }
                    }
                    this.PremiumChart.Add(premiumChartResult);
                }



            }
            return this;
        }
    }

    public class PremiumChartResult
    {
        public int? ProductPremiumId { get; set; }
        public decimal? SumInsured { get; set; }
        public decimal? SelfOnlyPremium { get; set; }
        public decimal? SelfSpousePremium { get; set; }
        public decimal? SelfSpouse2ChildrenPremium { get; set; } = 0;
        public decimal? SelfSpouse1ChildrenPremium { get; set; } = 0;
        public decimal? Self2ChildrenPremium { get; set; } = 0;
        public decimal? Self1ChildrenPremium { get; set; } = 0;
        public decimal? SpousePremium { get; set; }
        public decimal? Child1Premium { get; set; }
        public decimal? Child2Premium { get; set; }
        public decimal? Parent1Premium { get; set; }
        public decimal? Parent2Premium { get; set; }
        public decimal? InLaw1Premium { get; set; }
        public decimal? InLaw2Premium { get; set; }
        public int? ParentProductPremiumId { get; set; }
        public string? AgeBandPremiumRateValue { get; set; }
        public List<PremiumChartResult>? TopUpOptions { get; set; }
    }
}
