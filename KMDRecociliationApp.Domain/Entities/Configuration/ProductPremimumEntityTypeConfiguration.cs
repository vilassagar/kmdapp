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
   
     public class ProductPremimumEntityTypeConfiguration
       : IEntityTypeConfiguration<ProductPremimum>
    {

        public void Configure(EntityTypeBuilder<ProductPremimum> builder)
        {
            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.Now);
            builder.Property(p => p.UpdatedAt).HasDefaultValue(DateTime.Now);

            builder.HasOne(pp => pp.AgeBandPremiumRate)
            .WithMany(abpr => abpr.ProductPremiums)
            .HasForeignKey(pp => pp.AgeBandPremiumRateId);
            
            //default data
            //builder.HasData();

            List<ProductPremimum> ProductPremiums =
          [
              new ProductPremimum
                {
                    Id = 1,
                    ProductId = 1,
                    SumInsured = 200000,
                    SelfOnlyPremium = 30006,
                    SelfSpousePremium =37509,
                    SpousePremium = 0,
                    Child1Premium  = 2360,
                    Child2Premium = 2360,
                    Parent1Premium = 0,
                    Parent2Premium = 0,
                    InLaw1Premium = 0,
                    InLaw2Premium = 0,
                    ParentProductPremimumId = null,
                    IsActive = true,
                    CreatedBy = 2,
                    UpdatedBy = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
              new ProductPremimum
                {
                    Id = 2,
                    ProductId = 1,
                    SumInsured = 300000,
                    SelfOnlyPremium = 34220,
                    SelfSpousePremium = 44891,
                    SpousePremium = 0,
                    Child1Premium  = 4720,
                    Child2Premium = 4720,
                    Parent1Premium = 0,
                    Parent2Premium = 0,
                    InLaw1Premium = 0,
                    InLaw2Premium = 0,
                    ParentProductPremimumId = null,
                    IsActive = true,
                    CreatedBy = 2,
                    UpdatedBy = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
              new ProductPremimum
                {
                    Id = 3,
                    ProductId = 1,
                    SumInsured = 400000,
                    SelfOnlyPremium = 48675,
                    SelfSpousePremium = 57717,
                    SpousePremium = 0,
                    Child1Premium  = 7079,
                    Child2Premium = 7080,
                    Parent1Premium = 0,
                    Parent2Premium = 0,
                    InLaw1Premium = 0,
                    InLaw2Premium = 0,
                    ParentProductPremimumId = null,
                    IsActive = true,
                    CreatedBy = 2,
                    UpdatedBy = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
              new ProductPremimum
                {
                    Id = 4,
                    ProductId = 2,
                    SumInsured = 200000,
                    SelfOnlyPremium = 5256,
                    SelfSpousePremium = 7108,
                    SpousePremium = 0,
                    Child1Premium  = 0,
                    Child2Premium = 0,
                    Parent1Premium = 0,
                    Parent2Premium = 0,
                    InLaw1Premium = 0,
                    InLaw2Premium = 0,
                    ParentProductPremimumId = 1 ,
                    IsActive = true,
                    CreatedBy = 0,
                    UpdatedBy = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
              new ProductPremimum
                {
                    Id = 5,
                    ProductId = 2,
                    SumInsured = 300000,
                    SelfOnlyPremium = 6187,
                    SelfSpousePremium = 9209,
                    SpousePremium = 0,
                    Child1Premium  = 0,
                    Child2Premium = 0,
                    Parent1Premium = 0,
                    Parent2Premium = 0,
                    InLaw1Premium = 0,
                    InLaw2Premium = 0,
                    ParentProductPremimumId = 1,
                    IsActive = true,
                    CreatedBy = 0,
                    UpdatedBy = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
              new ProductPremimum
                {
                    Id = 6,
                    ProductId = 2,
                    SumInsured = 300000,
                    SelfOnlyPremium = 6187,
                    SelfSpousePremium = 9209,
                    SpousePremium = 0,
                    Child1Premium  = 0,
                    Child2Premium = 0,
                    Parent1Premium = 0,
                    Parent2Premium = 0,
                    InLaw1Premium = 0,
                    InLaw2Premium = 0,
                    ParentProductPremimumId = 2 ,
                    IsActive = true,
                    CreatedBy = 0,
                    UpdatedBy = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
              new ProductPremimum
                {
                    Id = 7,
                    ProductId = 2,
                    SumInsured = 400000,
                    SelfOnlyPremium = 7425,
                    SelfSpousePremium = 11050,
                    SpousePremium = 0,
                    Child1Premium  = 0,
                    Child2Premium = 0,
                    Parent1Premium = 0,
                    Parent2Premium = 0,
                    InLaw1Premium = 0,
                    InLaw2Premium = 0,
                    ParentProductPremimumId = 2,
                    IsActive = true,
                    CreatedBy = 0,
                    UpdatedBy = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
              new ProductPremimum
                {
                    Id = 8,
                    ProductId = 2,
                    SumInsured = 400000,
                    SelfOnlyPremium = 7425,
                    SelfSpousePremium = 11051,
                    SpousePremium = 0,
                    Child1Premium  = 0,
                    Child2Premium = 0,
                    Parent1Premium = 0,
                    Parent2Premium = 0,
                    InLaw1Premium = 0,
                    InLaw2Premium = 0,
                    ParentProductPremimumId = 3 ,
                    IsActive = true,
                    CreatedBy = 0,
                    UpdatedBy = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
              new ProductPremimum
                {
                    Id = 9,
                    ProductId = 2,
                    SumInsured = 500000,
                    SelfOnlyPremium = 10313,
                    SelfSpousePremium = 15348,
                    SpousePremium = 0,
                    Child1Premium  = 0,
                    Child2Premium = 0,
                    Parent1Premium = 0,
                    Parent2Premium = 0,
                    InLaw1Premium = 0,
                    InLaw2Premium = 0,
                    ParentProductPremimumId = 3,
                    IsActive = true,
                    CreatedBy = 0,
                    UpdatedBy = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
          ];

            //builder.HasData(ProductPremiums);
        }
    }
}
