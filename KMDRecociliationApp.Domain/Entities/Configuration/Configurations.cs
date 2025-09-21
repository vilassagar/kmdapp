using KMDRecociliationApp.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{

    
   
   
    
     public class AddressCountryEntityTypeConfigurations
       : IEntityTypeConfiguration<AddressCountry>
    {

        public void Configure(EntityTypeBuilder<AddressCountry> builder)
        {
            List<AddressCountry> countries =
            [
                new AddressCountry
                {
                    Id = 1,
                    Name = "India",
                    IsActive = true,
                },

            ];

            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            builder.Property(p => p.Name).IsRequired()
                .HasColumnType("Varchar").HasMaxLength(200);
            builder.Property(p => p.IsActive).HasDefaultValue(true);

            //default data
            builder.HasData(countries);

        }
    }

}
