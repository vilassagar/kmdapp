using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
    public class AgeBandPremiumRateEntityConfiguration : IEntityTypeConfiguration<AgeBandPremiumRate>
    {
        public void Configure(EntityTypeBuilder<AgeBandPremiumRate> builder)
        {

            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            
        }
    }
}
