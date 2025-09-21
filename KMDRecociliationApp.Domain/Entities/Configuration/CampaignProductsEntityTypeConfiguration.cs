using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
    public class CampaignProductsEntityTypeConfiguration
       : IEntityTypeConfiguration<CampaignProducts>
    {

        public void Configure(EntityTypeBuilder<CampaignProducts> builder)
        {
            List<CampaignProducts> campaignProducts =
            [
                new CampaignProducts
                {
                    Id = 1,
                    CampaignId = 1,
                    ProductId = 1,
                }
                //,
                // new CampaignProducts
                //{
                //    Id = 2,
                //    CampaignId = 1,
                //    ProductId = 2,
                //}
            ];


            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            //builder.Property(p => p.IsActive).HasDefaultValue(true);
            //builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.Now);
            //builder.Property(p => p.UpdatedAt).HasDefaultValue(DateTime.Now);
            //default data
            //builder.HasData(campaignProducts);

        }
    }
}
