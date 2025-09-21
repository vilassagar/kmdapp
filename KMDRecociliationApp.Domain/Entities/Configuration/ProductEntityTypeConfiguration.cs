using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMDRecociliationApp.Domain.Enum;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{

    public class ProductEntityTypeConfiguration
       : IEntityTypeConfiguration<Product>
    {

        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.Now);
            builder.Property(p => p.UpdatedAt).HasDefaultValue(DateTime.Now);
            //default data
            //builder.HasData();

            List<Product> products =
           [
               new Product
                {
                    Id = 1,
                    ProductName= "Group Health Insurance Policy",
                    ProviderName= "New India Assurance Co. Ltd.",
                    PolicyType= ProductPolicyType.BasePolicy,
                    PolicyTypeId=(int)ProductPolicyType.BasePolicy,
                     BasePolicyId= null,
                    IsSpouseCoverage= false,
                    IsHandicappedChildrenCoverage= true,
                    IsParentsCoverage= false,
                    IsInLawsCoverage= false,
                    NumberOfHandicappedChildren= 2,
                    NumberOfParents= 0,
                    NumberOfInLaws= 0,
                    IsActive = true,
                    CreatedBy = 2,
                    UpdatedBy = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new Product
                {
                    Id = 2,
                    ProductName= "Super Top Up Policy",
                    ProviderName= "New India Assurance Co. Ltd.",
                    PolicyType= ProductPolicyType.TopUpPolicy,
                      PolicyTypeId=(int)ProductPolicyType.TopUpPolicy,
                    BasePolicyId=1,
                    IsSpouseCoverage= false,
                    IsHandicappedChildrenCoverage= true,
                    IsParentsCoverage= false,
                    IsInLawsCoverage= false,
                    NumberOfHandicappedChildren= 2,
                    NumberOfParents= 0,
                    NumberOfInLaws= 0,
                    IsActive = true,
                    CreatedBy = 2,
                    UpdatedBy = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },

           ];

           // builder.HasData(products);


        }
    }
}
