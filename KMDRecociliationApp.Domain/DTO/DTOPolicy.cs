using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using KMDRecociliationApp.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTOPolicy
    {
        public int UserId { get; set; }
        public int? CampaignId { get; set; } = 0;
        public int? PolicyId { get; set; }
        public int? BeneficiaryId { get; set; }
        public decimal? TotalPremium { get; set; } = 0;
        public decimal? AmountPaid { get; set; } = 0;
        public string? PaymentStatus { get; set; }
        public decimal? TotalPaidPremium { get; set; } = 0;
        public decimal? ChildPremium { get; set; } = 0;
        public List<DTOPolicyProduct>? Products { get; set; } = new List<DTOPolicyProduct>();
        public DTOBeneficiaryDetails? beneficiaries { get; set; }
        public DTOBeneficiaryPerson? Nominee { get; set; }
        public DTOPaymentDetails? PaymentDetails { get; set; }
        public bool IsUpdate { get; set; } = false;
    }

    public class SelectedSumInsured
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public decimal Name { get; set; }
    }
    public class DTOPolicyProduct
    {
        public int Id { get; set; } = 0;

        public int ProductId { get; set; }
        public bool? IsDisclaimerAccepted { get; set; } = false;
        public string? Disclaimer { get; set; }
        public int? CampaignId { get; set; }
        public bool? IsCampaignExpired { get; set; }
        public string? ProductName { get; set; }
        public SelectedSumInsured? SelectedSumInsured { get; set; }
        public SelectedSumInsured? SelectedTopUpOption { get; set; }
        public string? ProviderName { get; set; }
        public int? PolicyTypeId { get; set; } = 1;
        public CommonNameDTO? PolicyType { get; set; }
        public int? BasePolicyId { get; set; }
        public bool IsSpouseCoverage { get; set; }
        public bool IsTopUpSelected { get; set; }
        public bool? IsHandicappedChildrenCoverage { get; set; }
        public bool? IsParentsCoverage { get; set; }
        public bool IsInLawsCoverage { get; set; }
        public int NumberOfHandicappedChildren { get; set; }
        public int NumberOfParents { get; set; }
        public int NumberOfInLaws { get; set; }
        public string? ProductBroucher { get; set; }
        public CommonNameDTO? BasePolicy { get; set; }
        public bool? IsProductSelected { get; set; } = false;
        public decimal? TotalProductPremium { get; set; }
        public string? AgeBandPremiumRateValue { get; set; }
        public List<DTOPolicyProductPremimumChart>? PremiumChart { get; set; } = new List<DTOPolicyProductPremimumChart>();


        public DTOPolicyProduct CopyBasePolicy(Product x, Product topuppolicyProduct
            , List<AgeBandPremiumRate> ageBandPremiumRates)
        {
            var dto = new DTOPolicyProduct();
            try
            {
                if (x == null)
                {
                    return dto;
                }
                else
                {
                    dto.ProductName = x.ProductName;
                    dto.Disclaimer = (x.Disclaimer == null || x.Disclaimer == "null") ? "" : x.Disclaimer;
                    dto.ProviderName = x.ProviderName;
                    dto.ProductBroucher = x.ProductDocumentUrl;
                    dto.ProductId = x.Id;
                    dto.PolicyTypeId = x.PolicyTypeId;
                    dto.IsSpouseCoverage = x.IsSpouseCoverage.Value;

                    dto.IsParentsCoverage = x.IsParentsCoverage;
                    dto.IsHandicappedChildrenCoverage = x.IsHandicappedChildrenCoverage;
                    dto.IsInLawsCoverage = x.IsInLawsCoverage.Value;
                    dto.NumberOfInLaws = x.NumberOfInLaws.Value;
                    dto.NumberOfHandicappedChildren = x.NumberOfHandicappedChildren.Value;
                    dto.NumberOfParents = x.NumberOfParents.Value;
                    dto.BasePolicy = x.BasePolicy != null ? new CommonNameDTO() { Id = x.BasePolicy.Id, Name = x.BasePolicy.ProductName } : new CommonNameDTO();
                    dto.BasePolicyId = x.BasePolicyId == null ? 0 : x.BasePolicyId;
                    dto.PolicyType = x.PolicyTypeId != null ?
                        new CommonNameDTO()
                        {
                            Id = x.PolicyTypeId.Value,
                            Name = ((ProductPolicyType)x.PolicyTypeId.Value).ToString()
                        }
                        : new CommonNameDTO();

                    if (x.ProductPremiums != null && x.ProductPremiums.Count > 0)
                    {
                        dto.PremiumChart = new List<DTOPolicyProductPremimumChart>();

                        var lstProductPremiums = x.ProductPremiums;
                        if (x.PolicyTypeId == (int)ProductPolicyType.AgeBandPremium
                            && ageBandPremiumRates != null)
                        {
                            // Get all AgeBandPremiumRateId values from ageBandPremiumRates list
                            var ageBandPremiumRateIds = ageBandPremiumRates.Select(x => x.Id).ToList();

                            // Filter lstProductPremiums to only include items with matching AgeBandPremiumRateId values
                            lstProductPremiums = lstProductPremiums
                                .Where(x => ageBandPremiumRateIds.Contains(x.AgeBandPremiumRateId.Value))
                                .ToList();

                        }
                        foreach (var item in lstProductPremiums)
                        {
                            var premiumChartResult = new DTOPolicyProductPremimumChart();
                            var ageBandValue = ""; int ageBandValueId = 0;
                            if (ageBandPremiumRates != null && ageBandPremiumRates.Any())
                            {
                                var ageBandValueObj = ageBandPremiumRates.Where(x => x.Id == item.AgeBandPremiumRateId)
                                      .FirstOrDefault();
                                if (ageBandValueObj != null)
                                {
                                    ageBandValue = ageBandValueObj.AgeBandValue;
                                    ageBandValueId = ageBandValueObj.Id;
                                }
                            }
                            premiumChartResult = premiumChartResult.CopyProductPremimumData(item, ageBandValueId, ageBandValue);
                            premiumChartResult.Id = item.Id;
                            if (topuppolicyProduct != null
                                && topuppolicyProduct.ProductPremiums != null
                                && topuppolicyProduct.ProductPremiums.Any())
                            {
                                var topUpOptions = topuppolicyProduct.ProductPremiums.Where(x1 => x1.ParentProductPremimumId == item.Id); //item.Id;
                                if (topUpOptions != null && topUpOptions.Any())
                                {
                                    premiumChartResult.TopUpOptions = new List<DTOPolicyProductPremimumChart>();
                                    foreach (var topupitem in topUpOptions)
                                    {
                                        var topUpOptionsResult = new DTOPolicyProductPremimumChart();
                                        var t = topUpOptionsResult.CopyProductPremimumData(topupitem);
                                        t.Id = topupitem.Id;
                                        premiumChartResult.TopUpOptions.Add(t);
                                    }
                                }
                            }
                            dto.PremiumChart.Add(premiumChartResult);

                        }
                        if (!lstProductPremiums.Any())
                        {
                            DTOPolicyProductPremimumChart premiumChartResult1 = new DTOPolicyProductPremimumChart();
                            dto.PremiumChart.Add(premiumChartResult1);
                        }
                    }
                    return dto;
                }
            }
            catch (Exception Ex)
            {
            }
            return dto;
        }
        public DTOPolicyProduct CopyTopupPolicy(Product x, Product topuppolicyProduct)
        {
            var dto = new DTOPolicyProduct();
            if (x == null || topuppolicyProduct == null)
            {
                return dto;
            }
            else
            {
                dto.Id = x.Id;
                dto.ProductName = x.ProductName;
                dto.Disclaimer = (x.Disclaimer == null || x.Disclaimer == "null") ? "" : x.Disclaimer;
                dto.ProviderName = x.ProviderName;
                dto.ProductBroucher = x.ProductDocumentUrl;
                dto.ProductId = x.Id;
                dto.PolicyTypeId = x.PolicyTypeId;
                dto.IsSpouseCoverage = x.IsSpouseCoverage.Value;
                dto.IsParentsCoverage = x.IsParentsCoverage;
                dto.IsHandicappedChildrenCoverage = x.IsHandicappedChildrenCoverage;
                dto.IsInLawsCoverage = x.IsInLawsCoverage.Value;
                dto.NumberOfInLaws = x.NumberOfInLaws.Value;
                dto.NumberOfHandicappedChildren = x.NumberOfHandicappedChildren.Value;
                dto.NumberOfParents = x.NumberOfParents.Value;
                dto.BasePolicyId = x.BasePolicyId == null ? 0 : x.BasePolicyId;
                dto.BasePolicy = x.BasePolicy != null ? new CommonNameDTO() { Id = x.BasePolicy.Id, Name = x.BasePolicy.ProductName } : new CommonNameDTO();
                dto.PolicyType = x.PolicyTypeId != null ?
                    new CommonNameDTO()
                    {
                        Id = x.PolicyTypeId.Value,
                        Name = x.PolicyTypeId.Value == 1 ? ProductPolicyType.BasePolicy.ToString() :
                    ProductPolicyType.TopUpPolicy.ToString()
                    }
                    : new CommonNameDTO();

                if (topuppolicyProduct.ProductPremiums != null && topuppolicyProduct.ProductPremiums.Count > 0)
                {
                    dto.PremiumChart = new List<DTOPolicyProductPremimumChart>();

                    foreach (var item in topuppolicyProduct.ProductPremiums)
                    {
                        var premiumChartResult = new DTOPolicyProductPremimumChart();

                        premiumChartResult = premiumChartResult.CopyProductPremimumData(item);
                        premiumChartResult.Id = item.Id;
                        var topUpOptions = x.ProductPremiums.Where(x1 => x1.ParentProductPremimumId == item.Id); //item.Id;
                        if (topUpOptions != null && topUpOptions.Any())
                        {
                            premiumChartResult.TopUpOptions = new List<DTOPolicyProductPremimumChart>();
                            foreach (var topupitem in topUpOptions)
                            {
                                var topUpOptionsResult = new DTOPolicyProductPremimumChart();
                                var t = topUpOptionsResult.CopyProductPremimumData(topupitem);
                                t.Id = topupitem.Id;
                                premiumChartResult.TopUpOptions.Add(t);
                            }
                        }

                        dto.PremiumChart.Add(premiumChartResult);
                    }
                }
                return dto;
            }
        }
    }
    public class DTOPolicyProductPremimumChart
    {
        public int Id { get; set; } = 0;
        public int? ProductPremiumId { get; set; } = 0;
        public int ProductId { get; set; } = 0;
        public decimal SumInsured { get; set; } = 0;
        public decimal? SelfOnlyPremium { get; set; } = 0;
        public decimal? SpousePremium { get; set; } = 0;
        public decimal? SelfSpousePremium { get; set; } = 0;
        public decimal? SelfSpouse2ChildrenPremium { get; set; } = 0;
        public decimal? SelfSpouse1ChildrenPremium { get; set; } = 0;
        public decimal? Self2ChildrenPremium { get; set; } = 0;
        public decimal? Self1ChildrenPremium { get; set; } = 0;

        public decimal? Child1Premium { get; set; } = 0;
        public decimal? Child2Premium { get; set; } = 0;
        public decimal? Parent1Premium { get; set; } = 0;
        public decimal? Parent2Premium { get; set; } = 0;
        public decimal? InLaw1Premium { get; set; } = 0;
        public decimal? InLaw2Premium { get; set; } = 0;
        public int? ParentProductPremiumId { get; set; }
        public bool IsPremiumSelected { get; set; } = false;
        public bool IsChild1PremiumSelected { get; set; } = false;
        public bool IsChild2PremiumSelected { get; set; } = false;
        public bool IsInLaw1PremiumSelected { get; set; } = false;
        public bool IsInLaw2PremiumSelected { get; set; } = false;
        public bool IsParent1PremiumSelected { get; set; } = false;
        public bool IsParent2PremiumSelected { get; set; } = false;
        public bool IsSpousePremiumSelected { get; set; } = false;
        public bool IsSelfPremiumSelected { get; set; } = false;
        public bool IsSelfSpousePremiumSelected { get; set; } = false;
        public bool IsTopUpSpousePremiumSelected { get; set; } = false;
        public bool IsTopUpSelfSpousePremiumSelected { get; set; } = false;
        public bool IsTopUpSelfPremiumSelected { get; set; } = false;
        public bool? IsSelfSpouse2ChildrenPremiumSelected { get; set; } = false;
        public bool? IsSelfSpouse1ChildrenPremiumSelected { get; set; } = false;
        public bool? IsSelf2ChildrenPremiumSelected { get; set; } = false;
        public bool? IsSelf1ChildrenPremiumSelected { get; set; } = false;
        public bool IsTopUpSelected { get; set; } = false;
        public string? AgeBandPremiumRateValue { get; set; } = "";
        public int? AgeBandPremiumRateId { get; set; } = 0;
        public List<DTOPolicyProductPremimumChart>? TopUpOptions { get; set; }

        public DTOPolicyProductPremimumChart CopyProductPremimumData(ProductPremimum item
            ,int ageBandPremiumRateId=0, string ageBandValue = "")
        {
            DTOPolicyProductPremimumChart tempproductPremimum = new DTOPolicyProductPremimumChart();
            tempproductPremimum.Child1Premium = item.Child1Premium;
            tempproductPremimum.Child2Premium = item.Child2Premium;
            tempproductPremimum.Parent1Premium = item.Parent1Premium;
            tempproductPremimum.Parent2Premium = item.Parent2Premium;
            tempproductPremimum.InLaw1Premium = item.InLaw1Premium;
            tempproductPremimum.InLaw2Premium = item.InLaw2Premium;
            tempproductPremimum.SelfOnlyPremium = item.SelfOnlyPremium;
            tempproductPremimum.SpousePremium = item.SpousePremium;
            tempproductPremimum.SelfSpousePremium = item.SelfSpousePremium;
            tempproductPremimum.SelfSpouse2ChildrenPremium = item.SelfSpouse2ChildrenPremium != null ? item.SelfSpouse2ChildrenPremium : 0;
            tempproductPremimum.SelfSpouse1ChildrenPremium = item.SelfSpouse1ChildrenPremium != null ? item.SelfSpouse1ChildrenPremium : 0;
            tempproductPremimum.Self1ChildrenPremium = item.Self1ChildrenPremium != null ? item.Self1ChildrenPremium : 0;
            tempproductPremimum.Self2ChildrenPremium = item.Self2ChildrenPremium != null ? item.Self2ChildrenPremium : 0;
            tempproductPremimum.AgeBandPremiumRateValue = ageBandValue;
            tempproductPremimum.AgeBandPremiumRateId = ageBandPremiumRateId;
            tempproductPremimum.SumInsured = item.SumInsured.Value;
            tempproductPremimum.ProductPremiumId = item.Id;
            tempproductPremimum.ProductId = item.ProductId;
            tempproductPremimum.ParentProductPremiumId = item.ParentProductPremimumId == null ? 0 : item.ParentProductPremimumId;

            return tempproductPremimum;
        }
    }

    public class DTOUserPolicy
    {
        public bool? IsPolicyProfilePreez { get; set; }
        public bool? IsUserProfilePreez { get; set; }

    }
}
