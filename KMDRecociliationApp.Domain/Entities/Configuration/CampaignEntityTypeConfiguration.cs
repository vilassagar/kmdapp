using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
    //internal class CampaignEntityTypeConfiguration
    public class CampaignEntityTypeConfiguration
       : IEntityTypeConfiguration<Campaigns>
    {

        public void Configure(EntityTypeBuilder<Campaigns> builder)
        {
            List<Campaigns> campaigns =
            [
                new Campaigns
                {
                    Id = 1,
                    CampaignName = "Campaign 1",
                    isCampaignOpen = true,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(30),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,


                }
            ];


            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.Now);
            builder.Property(p => p.UpdatedAt).HasDefaultValue(DateTime.Now);
            //default data
           // builder.HasData(campaigns);

        }
    }

}
