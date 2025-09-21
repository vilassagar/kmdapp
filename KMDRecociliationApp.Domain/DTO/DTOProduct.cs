using FluentValidation;
using KMDRecociliationApp.Domain.Common;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using KMDRecociliationApp.Domain.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTOProduct
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Disclaimer { get; set; }
        public string? ProviderName { get; set; }
        public int? PolicyTypeId { get; set; } = 1;
        public CommonNameDTO? PolicyType { get; set; }
        public int? BasePolicyId { get; set; }
        public bool IsSpouseCoverage { get; set; }
        public bool? IsHandicappedChildrenCoverage { get; set; }
        public bool? IsParentsCoverage { get; set; }
        public bool IsInLawsCoverage { get; set; }
        public int NumberOfHandicappedChildren { get; set; }
        public int NumberOfParents { get; set; }
        public int NumberOfInLaws { get; set; }
        public string? ProductBroucher { get; set; }
        public string? ProductDocumentUrl { get; set; }
        public string? ProductDocumentName { get; set; }
        public CommonNameDTO? BasePolicy { get; set; }
        public List<DTOProductPremimumChart>? PremiumChart { get; set; }
        public DTOProduct CopyBasePolicy(Product x,List<AgeBandPremiumRate> ageBandPremiumRates)
        {
            var dto = new DTOProduct();
            if (x == null)
            {
                return dto;
            }
            else
            {
                dto.ProductName = x.ProductName; 
                dto.Disclaimer = (x.Disclaimer == null|| x.Disclaimer=="null" )? "" : x.Disclaimer;
                dto.ProviderName = x.ProviderName;
                dto.ProductId = x.Id;
                dto.PolicyTypeId = x.PolicyTypeId;
                dto.IsSpouseCoverage = x.IsSpouseCoverage.Value;
                dto.IsParentsCoverage = x.IsParentsCoverage;
                dto.IsHandicappedChildrenCoverage = x.IsHandicappedChildrenCoverage;
                dto.IsInLawsCoverage = x.IsInLawsCoverage.Value;
                dto.NumberOfInLaws = x.NumberOfInLaws.Value;
                dto.NumberOfHandicappedChildren = x.NumberOfHandicappedChildren.Value;
                dto.NumberOfParents = x.NumberOfParents.Value;
                dto.ProductDocumentName = x.ProductDocumentName;
                dto.ProductDocumentUrl = x.ProductDocumentUrl;               
                dto.BasePolicy = x.BasePolicy != null ? new CommonNameDTO() { Id = x.BasePolicy.Id, Name = x.BasePolicy.ProductName } : new CommonNameDTO();
                dto.PolicyType = x.PolicyTypeId != null ?
                    new CommonNameDTO()
                    {
                        Id = x.PolicyTypeId.Value,
                        Name =((ProductPolicyType) x.PolicyTypeId.Value).ToString()
                    }
                    : new CommonNameDTO();

                dto.ProductDocument = new CommonFileModel()
                {
                    Id = x.Id,
                    Name = x.ProductDocumentName,
                    Url = x.ProductDocumentUrl
                };

                if (x.ProductPremiums != null && x.ProductPremiums.Count > 0)
                {
                    dto.PremiumChart = new List<DTOProductPremimumChart>();

                    foreach (var item in x.ProductPremiums)
                    {
                        DTOProductPremimumChart premiumChartResult = new DTOProductPremimumChart();

                        premiumChartResult = premiumChartResult.Copy(item, ageBandPremiumRates);


                        
                        dto.PremiumChart.Add(premiumChartResult);
                    }
                }
                return dto;
            }
        }
        public DTOProduct CopyTopupPolicy(Product x, Product baseProduct)
        {
            var dto = new DTOProduct();
            if (x == null || baseProduct == null)
            {
                return dto;
            }
            else
            {
                dto.ProductName = x.ProductName;
                dto.Disclaimer = (x.Disclaimer == null || x.Disclaimer == "null") ? "" : x.Disclaimer;
                dto.ProviderName = x.ProviderName;
                dto.ProductId = x.Id;
                dto.PolicyTypeId = x.PolicyTypeId;
                dto.IsSpouseCoverage = x.IsSpouseCoverage.Value;
                dto.IsParentsCoverage = x.IsParentsCoverage;
                dto.IsHandicappedChildrenCoverage = x.IsHandicappedChildrenCoverage;
                dto.IsInLawsCoverage = x.IsInLawsCoverage.Value;
                dto.NumberOfInLaws = x.NumberOfInLaws.Value;
                dto.NumberOfHandicappedChildren = x.NumberOfHandicappedChildren.Value;
                dto.NumberOfParents = x.NumberOfParents.Value;
                dto.ProductDocumentName = x.ProductDocumentName;
                dto.ProductDocumentUrl = x.ProductDocumentUrl;
                dto.BasePolicy = x.BasePolicy != null ? new CommonNameDTO() { Id = x.BasePolicy.Id, Name = x.BasePolicy.ProductName } : new CommonNameDTO();
                dto.PolicyType = x.PolicyTypeId != null ?
                    new CommonNameDTO()
                    {
                        Id = x.PolicyTypeId.Value,
                        Name = ((ProductPolicyType)x.PolicyTypeId.Value).ToString()
                    }
                    : new CommonNameDTO();

                dto.ProductDocument = new CommonFileModel()
                {
                    Id = x.Id,
                    Name = x.ProductDocumentName,
                    Url = x.ProductDocumentUrl
                };
                if (baseProduct.ProductPremiums != null && baseProduct.ProductPremiums.Count > 0)
                {
                    dto.PremiumChart = new List<DTOProductPremimumChart>();

                    foreach (var item in baseProduct.ProductPremiums)
                    {
                        DTOProductPremimumChart premiumChartResult = new DTOProductPremimumChart();

                        premiumChartResult = premiumChartResult.Copy(item,null);

                        var topUpOptions = x.ProductPremiums.Where(x1 => x1.ParentProductPremimumId == item.Id); //item.Id;
                        if (topUpOptions != null && topUpOptions.Any())
                        {
                            premiumChartResult.TopUpOptions = new List<DTOProductPremimumChart>();
                            foreach (var topupitem in topUpOptions)
                            {
                                DTOProductPremimumChart topUpOptionsResult = new DTOProductPremimumChart();
                                premiumChartResult.TopUpOptions.Add(topUpOptionsResult.Copy(topupitem,null));
                            }
                        }

                        dto.PremiumChart.Add(premiumChartResult);
                    }
                }
                return dto;
            }
        }
        // public IFormFile? ProductDocument { get; set; }
        public bool? IsProductDocumentUpdated { get; set; }
        public CommonFileModel ProductDocument { get; set; } = new CommonFileModel();

    }

    public class DTOProductPremimumChart
    {
        public int ProductPremiumId { get; set; } = 0;
        public int ProductId { get; set; }
        public decimal SumInsured { get; set; }
        public decimal? SelfOnlyPremium { get; set; }
        public decimal? SelfSpousePremium { get; set; } = 0;
        public decimal? SelfSpouse2ChildrenPremium { get; set; } = 0;
        public decimal? SelfSpouse1ChildrenPremium { get; set; } = 0;
        public decimal? Self2ChildrenPremium { get; set; } = 0;
        public decimal? Self1ChildrenPremium { get; set; } = 0;
        public int? AgeBandPremiumRateId { get; set; } = 1;
        public CommonNameDTO? AgeBandPremiumRateValue { get; set; }
        public decimal? SpousePremium { get; set; } = 0;
        public decimal? Child1Premium { get; set; } = 0;
        public decimal? Child2Premium { get; set; } = 0;
        public decimal? Parent1Premium { get; set; } = 0;
        public decimal? Parent2Premium { get; set; } = 0;
        public decimal? InLaw1Premium { get; set; } = 0;
        public decimal? InLaw2Premium { get; set; } = 0;
        public int? ParentProductPremiumId { get; set; } = 0;
        public List<DTOProductPremimumChart>? TopUpOptions { get; set; }

        public DTOProductPremimumChart Copy(ProductPremimum item,List <AgeBandPremiumRate> ageBandPremiumRates)
        {
            DTOProductPremimumChart tempproductPremimum = new DTOProductPremimumChart();
            tempproductPremimum.Child1Premium = item.Child1Premium;
            tempproductPremimum.Child2Premium = item.Child2Premium;
            tempproductPremimum.Parent1Premium = item.Parent1Premium;
            tempproductPremimum.Parent2Premium = item.Parent2Premium;
            tempproductPremimum.InLaw1Premium = item.InLaw1Premium;
            tempproductPremimum.InLaw2Premium = item.InLaw2Premium;
            tempproductPremimum.SelfOnlyPremium = item.SelfOnlyPremium;
            tempproductPremimum.SpousePremium = item.SpousePremium;
            tempproductPremimum.SelfSpousePremium = item.SelfSpousePremium;
            tempproductPremimum.Self1ChildrenPremium = item.Self1ChildrenPremium;
            tempproductPremimum.Self2ChildrenPremium = item.Self2ChildrenPremium;
            tempproductPremimum.SelfSpouse1ChildrenPremium = item.SelfSpouse1ChildrenPremium;
            tempproductPremimum.SelfSpouse2ChildrenPremium = item.SelfSpouse2ChildrenPremium;
            tempproductPremimum.SumInsured = item.SumInsured.Value;
            tempproductPremimum.ProductPremiumId = item.Id;
            tempproductPremimum.ProductId = item.ProductId;
            tempproductPremimum.ParentProductPremiumId = item.ParentProductPremimumId == null ? 0 : item.ParentProductPremimumId;
            tempproductPremimum.AgeBandPremiumRateId = item.AgeBandPremiumRateId;
            AgeBandPremiumRate agebandvalue = null;
            if (ageBandPremiumRates != null)
            {
                agebandvalue = ageBandPremiumRates.FirstOrDefault(x => x.Id == item.AgeBandPremiumRateId);
            }
            if (agebandvalue != null)
            {
                tempproductPremimum.AgeBandPremiumRateValue =
                        new CommonNameDTO()
                        {
                            Id = agebandvalue.Id,
                            Name = agebandvalue.AgeBandValue
                        };
                       
            }
            else
            {
                tempproductPremimum.AgeBandPremiumRateValue = item.AgeBandPremiumRate != null ?
                        new CommonNameDTO()
                        {
                            Id = item.AgeBandPremiumRate.Id,
                            Name = item.AgeBandPremiumRate.AgeBandValue
                        }
                        : new CommonNameDTO();
            }
                
            return tempproductPremimum;
        }

    }
    public class ProductValidator : AbstractValidator<DTOProduct>
    {
        public ProductValidator()
        {
            RuleFor(applicationUser => applicationUser.ProductName).NotEmpty().WithMessage("First Name is required.");
            RuleFor(applicationUser => applicationUser.PolicyTypeId).NotNull().WithMessage("Policy Type is required.");
        }
    }


}
