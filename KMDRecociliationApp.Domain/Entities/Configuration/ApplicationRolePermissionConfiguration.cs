using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities.Configuration
{
    internal class ApplicationRolePermissionConfiguration : IEntityTypeConfiguration<ApplicationRolePermission>
    {
        public void Configure(EntityTypeBuilder<ApplicationRolePermission> builder)
        {
            List<ApplicationRolePermission> applicationRoles =
            [
                new ApplicationRolePermission
                {
                    Id = 1,
                    RoleId = 2,PermissionId=1,                    
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new ApplicationRolePermission
                {
                    Id = 2,
                     RoleId = 2,PermissionId=2,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationRolePermission
                {
                    Id = 3,
                     RoleId = 2,PermissionId=3,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationRolePermission
                {
                    Id = 4,
                  RoleId = 2,PermissionId=4,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationRolePermission
                {
                    Id = 5,
                   RoleId = 2,PermissionId=5,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationRolePermission
                {
                    Id = 6,
                     RoleId = 2,PermissionId=6,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationRolePermission
                {
                    Id = 7,
                     RoleId = 2,PermissionId=7,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationRolePermission
                {
                    Id = 8,
                     RoleId = 2,PermissionId=8,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationRolePermission
                {
                    Id = 9,
                      RoleId = 2,PermissionId=9,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationRolePermission
                {
                    Id = 10,
                      RoleId = 2,PermissionId=10,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationRolePermission
                {
                    Id = 11,
                    RoleId = 1,PermissionId=11,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                  new ApplicationRolePermission
                {
                    Id = 12,
                     RoleId = 1,PermissionId=12,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                   new ApplicationRolePermission
                {
                     Id = 13,
                     RoleId = 1,PermissionId=13,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationRolePermission
                {
                     Id = 14,
                     RoleId = 2,PermissionId=14,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },

                // Association Role 
                //new ApplicationRolePermission
                //{
                //       Id = 15,
                //      RoleId = 3,PermissionId=4,
                //    Create = true,Read=true,Delete=true,Update=true,
                //    IsActive = true,
                //    CreatedBy =0,UpdatedBy=0,
                //    CreatedAt = DateTime.Now,
                //    UpdatedAt = DateTime.Now
                //},

                 new ApplicationRolePermission
                {
                    Id = 16,
                    RoleId = 3,PermissionId=5,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                  new ApplicationRolePermission
                {
                    Id = 17,
                     RoleId = 3,PermissionId=6,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                   new ApplicationRolePermission
                {
                     Id = 18,
                     RoleId = 3,PermissionId=9,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationRolePermission
                {
                     Id = 19,
                     RoleId = 3,PermissionId=10,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 new ApplicationRolePermission
                {
                     Id = 20,
                     RoleId = 3,PermissionId=13,
                    Create = true,Read=true,Delete=true,Update=true,
                    IsActive = true,
                    CreatedBy =0,UpdatedBy=0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                 

            ];

            builder.Property(p => p.Id).IsRequired().UseIdentityColumn();            
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.Now);
            builder.Property(p => p.UpdatedAt).HasDefaultValue(DateTime.Now);

            //default data
           // builder.HasData(applicationRoles);

        }
    }
}
