using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
namespace KMDRecociliationApp.Data.Repositories
{
    public class RolePermissionRepository : MainHeaderRepo<ApplicationRolePermission>
    {
        ApplicationDbContext context = null;
        private readonly ILogger _logger;

        public RolePermissionRepository(ILoggerFactory logger, ApplicationDbContext appContext) : base(appContext)
        {
            context = appContext;
            _logger = logger.CreateLogger("RolePermissionRepository");
        }
        public async Task<List<ApplicationRolePermission>> GetByID(int Roleid)
        {
            return await context.ApplicationRolePermission.AsNoTracking()
                .Where(x => x.RoleId == Roleid)
                .Where(x => x.IsActive == true)
                .ToListAsync();
        }
        public ApplicationRolePermission GetByID(int roleid, int permissionid)
        {
            var role_permission_details
                = context.ApplicationRolePermission.AsNoTracking()
                .Where(x => x.RoleId == roleid)
                .Where(x => x.PermissionId == permissionid)
                .FirstOrDefault();
            if (role_permission_details != null)
                return role_permission_details;
            else
                return new ApplicationRolePermission();
        }

        public List<ApplicationRolePermission> GetAll()
        {
            return context.ApplicationRolePermission.ToList();
        }
        public async Task<string> AssignPermission(int Rolepermissionid, int Roleid, int Permissionid)
        {
            var permission_details = await context.ApplicationPermission
                .AsNoTracking()
                .Where(x => x.Id == Permissionid).ToListAsync();

            var role_permission_details = await context.ApplicationRolePermission
                .AsNoTracking()
                .Where(x => x.Id == Rolepermissionid).ToListAsync();

            var role_details = await context.ApplicationRole
                .AsNoTracking().Where(x => x.Id == Roleid).ToListAsync();

            if (permission_details.Count() == 1 & role_details.Count() == 1)
            {
                if (role_permission_details.Count() == 0)
                {
                    ApplicationRolePermission rolepermissiondetails = new ApplicationRolePermission();
                    rolepermissiondetails.Id = Rolepermissionid;
                    rolepermissiondetails.PermissionId = Permissionid;
                    rolepermissiondetails.RoleId = Roleid;

                    context.ApplicationRolePermission.Add(rolepermissiondetails);

                }
                else
                {
                    role_permission_details[0].PermissionId = Permissionid;
                    role_permission_details[0].RoleId = Roleid;
                }
            }


            if (context.ApplicationRolePermission.Local.Count > 0)
            {
                context.SaveChanges();
                return "Success";
            }
            else
            {
                return "Failure";
            }

        }

    }
}
