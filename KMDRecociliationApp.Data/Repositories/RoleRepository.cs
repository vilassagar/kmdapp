using KMDRecociliationApp.Data.Common;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;

using Microsoft.EntityFrameworkCore;
using KMDRecociliationApp.Domain.Results;
namespace KMDRecociliationApp.Data.Repositories
{
    public class RoleRepository : MainHeaderRepo<ApplicationRole>
    {
        ApplicationDbContext context = null;
        private readonly ILogger _logger;

        public RoleRepository(ILoggerFactory logger, ApplicationDbContext appContext) : base(appContext)
        {
            context = appContext;
            _logger = logger.CreateLogger("RoleRepository");
        }

        public DataReturn<RolesResult> GetAll(DataFilter<RolesResult> filter)
        {
            var objList = new List<ApplicationRole>();
            var ret = new DataReturn<RolesResult>();
            int numberOfRecords = 0;
            var roles = context.ApplicationRole.AsNoTracking()
                .Where(x=>x.IsActive==true)
                .AsQueryable();
            if (filter.Search != null)
            {
                objList = objList = roles.Search(filter.Search, "RoleName", "Description").ToList();
            }
            else
            {

                objList = new ObjectQuery<ApplicationRole>().GetAllByFilter(filter.PageNumber, filter.Limit, filter.SortName
                    , filter.SortDirection, filter.Filter == null ? null : filter.Filter.GetDelta()
                    , roles, "Roles", out numberOfRecords).ToList();
            }
            ret.Contents = objList.Select(x => new RolesResult().CopyPolicyData(x: x)).ToList();
            ret.Source = "Roles";
            ret.ResultCount = numberOfRecords;
            ret.StatusCode = 200;
            //Paging information
            int numberOfPages = (numberOfRecords / filter.Limit) + ((numberOfRecords % filter.Limit > 0) ? 1 : 0);
            DataPaging paging = new DataPaging();
            paging.RecordsPerPage = filter.Limit;
            paging.PageNumber = filter.PageNumber;
            paging.NumberOfPages = numberOfPages;
            if (filter.PageNumber > 1)
                paging.PreviousPageNumber = filter.PageNumber - 1;
            if (numberOfPages > filter.PageNumber)
                paging.NextPageNumber = filter.PageNumber + 1;
            ret.Paging = paging;
            DataSorting sorting = new DataSorting();
            sorting.SortName = filter.SortName;
            sorting.SortDirection = filter.SortDirection;
            ret.Sorting = sorting;

            return ret;
        }

        public async Task<RoleDTO> GetByID(int? roleid)
        {
            RoleDTO roleDTO = new RoleDTO();
            var role = await context.ApplicationRole
                .AsNoTracking().Where(x => x.Id == roleid)
                .FirstOrDefaultAsync();
            if (role == null)
                return roleDTO;
            var permission = GetPermissionView(roleid);
            roleDTO = roleDTO.Copy(role);
            roleDTO.permissions = permission;
            return roleDTO;
        }
        public async Task<ApplicationRole> GetApplicationRoleID(int? roleid)
        {
            var role = await context.ApplicationRole
                .AsNoTracking().Where(x => x.Id == roleid)
                .FirstOrDefaultAsync();
            if (role != null)
                return role;
            else
                return new ApplicationRole();
        }
        public async Task<List<ApplicationRole>> GetByName(string rolenames)
        {
            return await context.ApplicationRole.AsNoTracking()
                .Where(record => rolenames.ToLower().Trim()
                .Contains(record.RoleName.ToLower().Trim()))
                .ToListAsync();
            //.Where(x =>  x.Rolename.Trim().ToLower() == Rolename.Trim().ToLower()).ToListAsync();
        }
        public async Task<List<CommonNameDTO>> GetAllRoles()
        {
            var list= await context.ApplicationRole.AsNoTracking()                
                .ToListAsync();

            var roles = new List<CommonNameDTO>();
            foreach (var role in list)
            {
                roles.Add(new CommonNameDTO()
                {
                    Id = role.Id,
                    Name = role.RoleName.ToLower()
                });
            }
            return roles;
        }
        public async Task<bool> CheckRoleName(RoleDTO roleObj, bool update = false)
        {
            if (update)
            {
                return await context.ApplicationRole.AsNoTracking()
                .AnyAsync(x => x.RoleName == roleObj.name
                && x.Id != roleObj.id
                );
            }
            else
            {
                return await context.ApplicationRole.AsNoTracking()
                .AnyAsync(x => x.RoleName == roleObj.name);
            }
        }
        public string UpdateCurrentRole(int Roleid, string Rolename, string Description)
        {
            var currentRole = context.ApplicationRole.Where(x => x.Id == Roleid).ToList();
            if (currentRole.Count() > 0)
            {
                currentRole[0].RoleName = Rolename;
                currentRole[0].Description = Description;

                context.ApplicationRole.UpdateRange(currentRole);
            }
            else
            {
                ApplicationRole roledetails = new ApplicationRole();
                roledetails.Id = Roleid;
                roledetails.RoleName = Rolename;
                roledetails.Description = Description;

                context.ApplicationRole.Add(roledetails);

            }
            if (context.ApplicationRole.Local.Count > 0)
            {
                context.SaveChanges();
                return "Success";
            }
            else
            {
                return "Failure";
            }

        }
        public string DeleteCurrentRole(int Roleid)
        {
            var currentRole = context.ApplicationRole.Where(x => x.Id == Roleid).ToList();
            if (currentRole.Count() > 0)
            {
                currentRole[0].IsActive = false;
                context.ApplicationRole.UpdateRange(currentRole);
                context.SaveChanges();
                return "Success";
            }
            else
            {
                return "Failure";
            }
        }
        public  List<ApplicationPermissionDTO> GetPermissionView(int? roleId)
        {
            var PermissionPivotView = new List<ApplicationPermissionDTO>();
            var permissions = context?.ApplicationPermission
                .AsNoTracking()
                .ToList();

            if (permissions != null)
            {
                foreach (var item in permissions)
                {
                    var permissionPivot = new ApplicationPermissionDTO();
                    permissionPivot.type = item.PermissionType;
                    permissionPivot.name = item.Description;
                    permissionPivot.id = item.Id;
                    if (permissionPivot.actions == null)
                        permissionPivot.actions = new Actions();

                    if (roleId != null || roleId > 0)
                    {
                        var rolepermission = context?.ApplicationRolePermission
                        .AsNoTracking()
                         .Where(x => x.RoleId == roleId &&
                         x.PermissionId == item.Id)
                          .FirstOrDefault();
                        if (rolepermission != null)
                        {
                            permissionPivot.actions.create = rolepermission.Create;
                            permissionPivot.actions.update = rolepermission.Update;
                            permissionPivot.actions.read = rolepermission.Read;
                            permissionPivot.actions.delete = rolepermission.Delete;
                        }
                        else
                        {
                            permissionPivot.actions.create = false;
                            permissionPivot.actions.update = false;
                            permissionPivot.actions.read = false;
                            permissionPivot.actions.delete = false;
                        }
                    }
                    else
                    {
                        permissionPivot.actions.create = false;
                        permissionPivot.actions.update = false;
                        permissionPivot.actions.read = false;
                        permissionPivot.actions.delete = false;

                    }
                    PermissionPivotView.Add(permissionPivot);

                }
            }
            return PermissionPivotView;
        }


    }

}
