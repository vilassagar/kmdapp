using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
     
    public class CampaignMembersDetailsEntityTypeConfiguration
       : IEntityTypeConfiguration<CampaignMembersDetails>
    {

        public void Configure(EntityTypeBuilder<CampaignMembersDetails> builder)
        {


            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            //builder.Property(p => p.IsActive).HasDefaultValue(true);
            //builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.Now);
            //builder.Property(p => p.UpdatedAt).HasDefaultValue(DateTime.Now);
            ////default data
            //builder.HasData();

        }
    }
}
