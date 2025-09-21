using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
    public class ApplicationUserRoleEntityTypeConfigurations
       : IEntityTypeConfiguration<ApplicationUserRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
        {
            List<ApplicationUserRole> applicationUserRole =
            [
                new ApplicationUserRole
                {
                    Id = 1,
                    RoleId = 1,
                    UserId = 1,
                    IsActive = true,
                    CreatedBy = 0,
                    UpdatedBy = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new ApplicationUserRole
                {
                    Id = 2,
                    RoleId = 2,
                    UserId = 2,
                    IsActive = true,
                    CreatedBy = 0,
                    UpdatedBy = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new ApplicationUserRole
                {
                    Id = 3,
                    RoleId = 3,
                    UserId = 3,
                    IsActive = true,
                    CreatedBy = 2,
                    UpdatedBy = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            ];

            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            builder.Property(p => p.RoleId).IsRequired();
            builder.Property(u => u.UserId).IsRequired();
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.Now);
            builder.Property(p => p.UpdatedAt).HasDefaultValue(DateTime.Now);

            //default data
            //builder.HasData(applicationUserRole);

        }
    }

}
