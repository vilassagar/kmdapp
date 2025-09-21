using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
    public class ApplicationRoleEntityTypeConfigurations
        : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            List<ApplicationRole> applicationRoles =
            [
                new ApplicationRole
                {
                    Id = 1,
                    RoleName = "Retiree",
                    Description = "Retiree Role",
                    IsActive = true,
                    CreatedBy = 0,
                    UpdatedBy = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new ApplicationRole
                {
                    Id = 2,
                    RoleName = "Admin",
                    Description = "Admin Role",
                    IsActive = true,
                    CreatedBy = 0,
                    UpdatedBy = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationRole
                {
                    Id = 3,
                    RoleName = "Association",
                    Description = "Association Role",
                    IsActive = true,
                    CreatedBy = 2,
                    UpdatedBy = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            ];

            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            builder.Property(p => p.RoleName).IsRequired()
                .HasColumnType("Varchar").HasMaxLength(200);
            builder.HasIndex(u => u.RoleName).IsUnique();
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.Now);
            builder.Property(p => p.UpdatedAt).HasDefaultValue(DateTime.Now);

            //default data
            builder.HasData(applicationRoles);

        }
    }

}
