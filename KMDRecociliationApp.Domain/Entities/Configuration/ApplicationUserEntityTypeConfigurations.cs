using KMDRecociliationApp.Domain.Enum;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
    public class ApplicationUserEntityTypeConfigurations
      : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            List<ApplicationUser> applicationUsers =
            [
                new ApplicationUser
                {
                    Id = 1,
                    FirstName = "Retiree",
                    LastName = "Retiree",
                    Email = "retiree@kmd.com",
                    MobileNumber = "1",
                    AssociationId = 1,
                    OrganisationId = 1,
                    CountryCode = "+91",
                    DOB = DateTime.Now,
                    UserType = UserType.Pensioner,
                    EMPIDPFNo = "123123123",
                    OTP = "1111",
                    OTPExpiration = DateTime.Now.AddYears(2),
                    IsProfileComplete = true,
                    CreatedBy = 0,
                    UpdatedBy = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new ApplicationUser
                {
                    Id = 2,
                    FirstName = "Admin",
                    LastName = "Admin",
                    Email = "admin@kmd.com",
                    MobileNumber = "2",
                    AssociationId = 1,
                    OrganisationId = null,
                    CountryCode = "+91",
                    DOB = DateTime.Now,
                    UserType = UserType.InternalUser,
                    EMPIDPFNo = "123123123",
                    OTP = "1111",
                    OTPExpiration = DateTime.Now.AddYears(2),
                    IsProfileComplete = true,
                    CreatedBy = 0,
                    UpdatedBy = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new ApplicationUser
                {
                    Id = 3,
                    FirstName = "Association",
                    LastName = "Association",
                    Email = "Association@kmd.com",
                    MobileNumber = "3",
                    AssociationId = 1,
                    OrganisationId = null,
                    CountryCode = "+91",
                    DOB = DateTime.Now,
                    UserType = UserType.Association,
                    EMPIDPFNo = "123123123",
                    OTP = "1111",
                    OTPExpiration = DateTime.Now.AddYears(2),
                    IsProfileComplete = true,
                    CreatedBy = 2,
                    UpdatedBy = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            ];

            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            builder.Property(p => p.MobileNumber).IsRequired()
                .HasColumnType("Varchar").HasMaxLength(200);
            builder.HasIndex(u => u.MobileNumber).IsUnique();
            builder.HasIndex(u => u.Email);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.Now);
            builder.Property(p => p.UpdatedAt).HasDefaultValue(DateTime.Now);

            //default data
            //builder.HasData(applicationUsers);

        }
    }


}
