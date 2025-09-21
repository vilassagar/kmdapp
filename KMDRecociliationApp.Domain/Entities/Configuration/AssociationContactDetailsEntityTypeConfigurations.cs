using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
    public class AssociationContactDetailsEntityTypeConfigurations
   : IEntityTypeConfiguration<AssociationContactDetails>
    {
        public void Configure(EntityTypeBuilder<AssociationContactDetails> builder)
        {
            List<AssociationContactDetails> associationContactDetails =
            [
                new AssociationContactDetails
                {
                    Id = 1,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,


                },
                new AssociationContactDetails
                {
                    Id = 2,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 3,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 3,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 4,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 4,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 5,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 5,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 6,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 6,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 7,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 7,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 8,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 8,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 9,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 9,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 10,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 10,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 11,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 11,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 12,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 12,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 13,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 13,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 14,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 14,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 15,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 15,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 16,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 16,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 17,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 17,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 18,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 18,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 19,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 19,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 20,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 20,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = 0,
                },
                new AssociationContactDetails
                {
                    Id = 21,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Phone = "123456789",
                    Email = "abc2aabc.com",
                    AssociationId = 21,
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
          //  builder.HasData(associationContactDetails);

        }
    }

}
