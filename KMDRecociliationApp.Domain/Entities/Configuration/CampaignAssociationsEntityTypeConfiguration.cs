using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
    
    public class CampaignAssociationsEntityTypeConfiguration
       : IEntityTypeConfiguration<CampaignAssociations>
    {

        public void Configure(EntityTypeBuilder<CampaignAssociations> builder)
        {
            List<CampaignAssociations> campaignAssociations = new List<CampaignAssociations>();

            // for (int i = 1; i <= 54; i++)
            // {
            //     campaignAssociations.Add(new CampaignAssociations
            //     {
            //         Id = 1,
            //         CampaignId = 1,
            //         AssociationId = i,
            //     });
            // }

            campaignAssociations.Add(new CampaignAssociations
                {
                    Id = 1,
                    CampaignId = 1,
                    AssociationId = 1,
                });


            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            //builder.Property(p => p.IsActive).HasDefaultValue(true);
            //builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.Now);
            //builder.Property(p => p.UpdatedAt).HasDefaultValue(DateTime.Now);
            //default data
            //builder.HasData(campaignAssociations);

        }
    }
}
