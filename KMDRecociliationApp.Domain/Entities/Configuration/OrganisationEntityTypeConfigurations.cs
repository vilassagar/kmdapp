using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
    public class OrganisationEntityTypeConfigurations
      : IEntityTypeConfiguration<Organisation>
    {
        public void Configure(EntityTypeBuilder<Organisation> builder)
        {
            List<Organisation> organizations =
            [
                new Organisation
                {
                    Id = 1,
                    Name = "PUNJAB NATIONAL BANK",
                    Description = "PUNJAB NATIONAL BANK",
                    IsActive = true,

                },
                new Organisation
                {
                    Id = 2,
                    Name = "UCO BANK",
                    Description = "UCO BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 3,
                    Name = "UNION BANK",
                    Description = "UNION BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 4,
                    Name = "UNITED BANK OF INDIA",
                    Description = "UNITED BANK OF INDIA",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 5,
                    Name = "SYNDICATE BANK",
                    Description = "SYNDICATE BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 6,
                    Name = "ALLAHABAD BANK",
                    Description = "ALLAHABAD BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 7,
                    Name = "PUNJAB & SIND BANK",
                    Description = "PUNJAB & SIND BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 8,
                    Name = "BANK OF INDIA",
                    Description = "BANK OF INDIA",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 9,
                    Name = "CANARA BANK",
                    Description = "CANARA BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 10,
                    Name = "FEDERAL BANK",
                    Description = "FEDERAL BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 11,
                    Name = "SOUTH INDIAN BANK",
                    Description = "SOUTH INDIAN BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 12,
                    Name = "VIJAYA BANK",
                    Description = "VIJAYA BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 13,
                    Name = "ANDHRA BANK",
                    Description = "ANDHRA BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 14,
                    Name = "CORPORATION BANK",
                    Description = "CORPORATION BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 15,
                    Name = "BANK OF MAHARASHTRA",
                    Description = "BANK OF MAHARASHTRA",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 16,
                    Name = "A B",
                    Description = "A B",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 17,
                    Name = "KERALA BANK",
                    Description = "KERALA BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 18,
                    Name = "CENTRAL BANK",
                    Description = "CENTRAL BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 19,
                    Name = "DENA BANK",
                    Description = "DENA BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 20,
                    Name = "BANK OF BARODA",
                    Description = "BANK OF BARODA",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 21,
                    Name = "Other",
                    Description = "Other",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 22,
                    Name = "INDIAN BANK",
                    Description = "INDIAN BANK",
                    IsActive = true,
                },
                new Organisation
                {
                    Id = 23,
                    Name = "CATHOLIC SYRIAN BANK",
                    Description = "CATHOLIC SYRIAN BANK",
                    IsActive = true,
                }
            ];

            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            builder.Property(p => p.Name).IsRequired()
                .HasColumnType("Varchar").HasMaxLength(200);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            


            //default data
           // builder.HasData(organizations);

        }
    }
}
