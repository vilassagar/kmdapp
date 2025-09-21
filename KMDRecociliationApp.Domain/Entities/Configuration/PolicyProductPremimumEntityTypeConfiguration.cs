using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
    
   public class PolicyProductPremimumEntityTypeConfiguration
       : IEntityTypeConfiguration<PolicyProductPremimum>
    {

        public void Configure(EntityTypeBuilder<PolicyProductPremimum> builder)
        {


            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            builder.HasOne(pp => pp.AgeBandPremiumRate)
           .WithMany(abpr => abpr.PolicyProductPremimums)
           .HasForeignKey(pp => pp.AgeBandPremiumRateId);
            //builder.Property(p => p.IsActive).HasDefaultValue(true);
            //builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.Now);
            //builder.Property(p => p.UpdatedAt).HasDefaultValue(DateTime.Now);
            //default data
            //builder.HasData();

        }
    }
}
