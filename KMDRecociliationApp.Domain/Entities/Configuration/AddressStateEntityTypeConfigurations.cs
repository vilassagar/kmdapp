using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
    public class AddressStateEntityTypeConfigurations
       : IEntityTypeConfiguration<AddressState>
    {


        public void Configure(EntityTypeBuilder<AddressState> builder)
        {
            List<AddressState> applicationRoles =
            [
                new AddressState
                {
                    Id = 1,
                    Name = "ANDAMAN AND NICOBAR ISLANDS",
                    IsActive = true,
                    CountryId = 1

                },
                new AddressState
                {
                    Id = 2,
                    Name = "ANDHRA PRADESH",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 3,
                    Name = "ARUNACHAL PRADESH",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 4,
                    Name = "ASSAM",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 5,
                    Name = "BIHAR",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 6,
                    Name = "CHATTISGARH",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 7,
                    Name = "CHANDIGARH",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 8,
                    Name = "DAMAN AND DIU",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 9,
                    Name = "DELHI",
                    IsActive = true,
                    CountryId = 1
                },

                new AddressState
                {
                    Id = 10,
                    Name = "DADRA AND NAGAR HAVELI",
                    IsActive = true,
                    CountryId = 1

                },
                new AddressState
                {
                    Id = 11,
                    Name = "GOA",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 12,
                    Name = "GUJARAT",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 13,
                    Name = "HIMACHAL PRADESH",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 14,
                    Name = "HARYANA",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 15,
                    Name = "JAMMU AND KASHMIR",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 16,
                    Name = "JHARKHAND",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 17,
                    Name = "KERALA",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 18,
                    Name = "KARNATAKA",
                    IsActive = true,
                    CountryId = 1
                },

                new AddressState
                {
                    Id = 19,
                    Name = "LAKSHADWEEP",
                    IsActive = true,
                    CountryId = 1

                },
                new AddressState
                {
                    Id = 20,
                    Name = "MEGHALAYAH",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 21,
                    Name = "MAHARASHTRA",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 22,
                    Name = "MANIPUR",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 23,
                    Name = "MADHYA PRADESH",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 24,
                    Name = "MIZORAM",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 25,
                    Name = "NAGALAND",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 26,
                    Name = "ORISSA",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 27,
                    Name = "PUNJAB",
                    IsActive = true,
                    CountryId = 1
                },

                new AddressState
                {
                    Id = 28,
                    Name = "PONDICHERRY",
                    IsActive = true,
                    CountryId = 1

                },
                new AddressState
                {
                    Id = 29,
                    Name = "RAJASTHAN",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 30,
                    Name = "SIKKIM",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 31,
                    Name = "TAMIL NADU",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 32,
                    Name = "TRIPURA",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 33,
                    Name = "UTTARAKHAND",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 34,
                    Name = "UTTAR PRADESH",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 35,
                    Name = "WEST BENGAL",
                    IsActive = true,
                    CountryId = 1
                },
                new AddressState
                {
                    Id = 36,
                    Name = "TELANGANA",
                    IsActive = true,
                    CountryId = 1
                },


            ];

            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            builder.Property(p => p.Name).IsRequired()
                .HasColumnType("Varchar").HasMaxLength(200);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            

            //default data
           // builder.HasData(applicationRoles);

        }
    }

}
