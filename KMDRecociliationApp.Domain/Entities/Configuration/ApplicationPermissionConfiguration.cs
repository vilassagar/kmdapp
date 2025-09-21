using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
    public class ApplicationPermissionTypeConfigurations : IEntityTypeConfiguration<ApplicationPermission>
    {
        public void Configure(EntityTypeBuilder<ApplicationPermission> builder)
        {
            List<ApplicationPermission> applicationRoles =
            [
                new ApplicationPermission
                {
                    Id = 1,
                    Description = "createproduct",
                    PermissionType="api",
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new ApplicationPermission
                {
                    Id = 2,
                    Description = "campaigns",
                    PermissionType="api",
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationPermission
                {
                    Id = 3,
                    Description = "users",PermissionType="api",
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationPermission
                {
                    Id = 4,
                    Description = "reports",PermissionType="api",
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationPermission
                {
                    Id = 5,
                    Description = "importdata",PermissionType="api",
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationPermission
                {
                    Id = 6,
                    Description = "offlinepayments",PermissionType="api",
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationPermission
                {
                    Id = 7,
                    Description = "roles",PermissionType="api",
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationPermission
                {
                    Id = 8,
                    Description = "permissions",PermissionType="api",
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationPermission
                {
                    Id = 9,
                    Description = "refundrequests",PermissionType="api",
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationPermission
                {
                    Id = 10,
                    Description = "dashboard",PermissionType="api",
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationPermission
                {
                    Id = 11,
                    Description = "profile",PermissionType="api",
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                  new ApplicationPermission
                {
                    Id = 12,
                    Description = "products",PermissionType="api",
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                   new ApplicationPermission
                {
                    Id = 13,
                    Description = "payments",PermissionType="api",
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationPermission
                {
                    Id = 14,
                    Description = "associations",PermissionType="api",
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },

            ];

            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();
            builder.Property(p => p.PermissionType).IsRequired().HasColumnType("Varchar").HasMaxLength(50);
            builder.Property(p => p.Description).IsRequired()
                .HasColumnType("Varchar").HasMaxLength(200);
            builder.HasIndex(u => u.Description).IsUnique();
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.Now);
            builder.Property(p => p.UpdatedAt).HasDefaultValue(DateTime.Now);

            //default data
            builder.HasData(applicationRoles);

        }
    }
}
