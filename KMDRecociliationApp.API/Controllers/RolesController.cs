using KMDRecociliationApp.API.Common;
using KMDRecociliationApp.Data;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Serilog;


namespace KMDRecociliationApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ApiBaseController
    {
        private readonly RoleRepository _rolesRepo;
        private readonly RolePermissionRepository _rolePermissionRepo;
        private readonly ApplicationDbContext context;
        private readonly CommonHelperRepository _commonHelperRepository;

        public RolesController(ILoggerFactory logger, ApplicationDbContext _context
            , RoleRepository rolesRepo, RolePermissionRepository rolePermissionRepo
            , CommonHelperRepository commonHelperRepository) : base(_context)
        {
            _rolesRepo = rolesRepo;
            context = _context;
            _rolePermissionRepo = rolePermissionRepo;
            _commonHelperRepository = commonHelperRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Getroles(
            [FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting
            //  [FromQuery] int PageNumber, [FromQuery] int Limit, [FromQuery] RolesResult roleResult, [FromQuery] DataSorting Sorting
            )
        {
            //if (HttpContext.User.Identity.IsAuthenticated)
            //{
            if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
            {
                Sorting = new DataSorting() { SortName = "id", SortDirection = "desc" };
            }
            DataFilter<RolesResult> filter = new DataFilter<RolesResult>()
            {
                PageNumber = searchDTO.Page,
                Limit = searchDTO.pageSize,
                Filter = null,
                SortName = Sorting.SortName,
                Search = searchDTO.Search,
                SortDirection = Sorting.SortDirection
            };
            DataReturn<RolesResult> nVReturn = new DataReturn<RolesResult>();
            nVReturn = _rolesRepo.GetAll(filter);
            return await Task.FromResult(Ok(nVReturn));
            //}
            //else
            //{
            //    DataReturn<PermissionResult> permissionErrorResult = new DataReturn<PermissionResult>();
            //    permissionErrorResult.ErrorMessage = "You dont have access to permissions";
            //    permissionErrorResult.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    Log.Warning($"Error Permission controller -Get All Permission method, Status Code : {permissionErrorResult.StatusCode}, Message: {permissionErrorResult.ErrorMessage}");
            //    return Unauthorized(permissionErrorResult);
            //}
        }

        [HttpPost("")]
        public async Task<IActionResult> PostCreateRole(RoleDTO roleObj)
        {

            _rolesRepo.CurrentUser = HttpContext.User;

            if (roleObj.id == 0)
            {
                var temp = await _rolesRepo.CheckRoleName(roleObj);
                if (temp == false)
                {
                    var tempRole = new ApplicationRole();
                    tempRole.IsActive = true;
                    tempRole.RoleName = roleObj.name;
                    tempRole.Description = roleObj.description;
                    tempRole.CreatedAt = DateTime.Now;
                    tempRole.UpdatedAt = DateTime.Now;
                    tempRole.CreatedBy = _rolesRepo.UserId;
                    tempRole.UpdatedBy = _rolesRepo.UserId;
                    var retRole = _rolesRepo.Add(tempRole, _rolesRepo.UserEmail, _rolesRepo.UserFullName, "Role Added");

                    if (retRole != null && retRole.Id > 0)
                    {
                        foreach (var item in roleObj.permissions)
                        {
                            ApplicationRolePermission rolepermission = new ApplicationRolePermission();
                            rolepermission.Id = 0;
                            rolepermission.RoleId = retRole.Id;
                            rolepermission.IsActive = true;
                            rolepermission.PermissionId
                                = Convert.ToInt32(item.id);
                            rolepermission.CreatedAt = DateTime.Now;
                            rolepermission.UpdatedAt = DateTime.Now;
                            rolepermission.CreatedBy = _rolesRepo.UserId;
                            rolepermission.UpdatedBy = _rolesRepo.UserId;

                            if (item.actions != null)
                            {
                                rolepermission.Create = item.actions.create;
                                rolepermission.Update = item.actions.update;
                                rolepermission.Delete = item.actions.delete;
                                rolepermission.Read = item.actions.read;
                            }

                            _rolePermissionRepo.Add(rolepermission, _rolePermissionRepo.UserEmail, _rolePermissionRepo.UserFullName, "Role Permission Added");

                        }
                    }

                }
                else
                {
                    string error = $"Role Name: {roleObj.name} already exists";
                    return Conflict(new { Message = error });
                }
            }


            return await Task.FromResult(Ok(new { Message = "Role Addedd" }));

        }

        [HttpPatch("{roleid}")]
        public async Task<IActionResult> PatchUpdateRole(int roleid,  RoleDTO roleObj)
        {
            try
            {
                if (roleid <= 0)
                    return await Task.FromResult(BadRequest(new { Message = "role Id missing" }));

                _rolesRepo.CurrentUser = HttpContext.User;

                var existingData = await _rolesRepo.CheckRoleName(roleObj, update: true);
                if (existingData == false)
                {
                    var roleDTO = await _rolesRepo.GetByID(roleObj.id.Value);
                    if (roleDTO != null)
                    {
                        ApplicationRole uprole = new ApplicationRole();
                        uprole.RoleName = roleObj.name;
                        uprole.Description = roleObj.description;
                        uprole.Id = roleObj.id.Value;
                        uprole.IsActive = roleDTO.isactive.Value;

                        uprole.UpdatedAt = DateTime.Now;

                        uprole.UpdatedBy = _rolesRepo.UserId;
                        var retRole = _rolesRepo.Update(uprole, _rolesRepo.UserEmail, _rolesRepo.UserFullName, "Role Updated");

                        if (uprole != null && uprole.Id > 0 && roleObj.permissions != null)
                        {
                            foreach (var item in roleObj.permissions)
                            {

                                var uprolepermission = _rolePermissionRepo.GetByID(roleObj.id.Value, Convert.ToInt32(item.id));
                                ApplicationRolePermission applicationRolePermission
                                      = new ApplicationRolePermission();

                                if (uprolepermission != null && uprolepermission.Id > 0)
                                {
                                    applicationRolePermission.Id = uprolepermission.Id;
                                    applicationRolePermission.RoleId = uprole.Id;
                                    applicationRolePermission.PermissionId = item.id.Value;
                                    applicationRolePermission.UpdatedAt = DateTime.Now;
                                    applicationRolePermission.UpdatedBy = _rolesRepo.UserId;


                                    if (item.actions != null)
                                    {
                                        applicationRolePermission.Update = item.actions.update;
                                        applicationRolePermission.Read = item.actions.read;
                                        applicationRolePermission.Delete = item.actions.delete;
                                        applicationRolePermission.Create = item.actions.create;
                                    }
                                    _rolePermissionRepo.Update(applicationRolePermission, _rolePermissionRepo.UserEmail, _rolePermissionRepo.UserFullName, "Role permission updated");

                                }
                                else
                                {
                                    applicationRolePermission.Id = 0;
                                    applicationRolePermission.RoleId = uprole.Id;
                                    applicationRolePermission.PermissionId = item.id.Value;
                                    applicationRolePermission.UpdatedAt = DateTime.Now;
                                    applicationRolePermission.UpdatedBy = _rolesRepo.UserId;
                                    applicationRolePermission.CreatedAt = DateTime.Now;
                                    applicationRolePermission.CreatedBy = _rolesRepo.UserId;

                                    if (item.actions != null)
                                    {
                                        applicationRolePermission.Update = false;
                                        applicationRolePermission.Read = false;
                                        applicationRolePermission.Delete = false;
                                        applicationRolePermission.Create = false;
                                    }
                                    _rolePermissionRepo.Add(applicationRolePermission, _rolePermissionRepo.UserEmail, _rolePermissionRepo.UserFullName, "Role permission updated");

                                }



                            }

                        }
                    }
                    else
                    {
                        return await Task.FromResult(NotFound(new { Message = "Role Id not found" }));
                    }
                }
                else
                {
                    string error = $"Role Name: {roleObj.name} already exists";
                    return Conflict(new { Message = error });
                }


            }
            catch (Exception ex)
            {
                return await Task.FromResult(this.InternalServerError(new { Message = $"Something went wrong!" }));
            }
            return await Task.FromResult(Ok(new { Message = "Role updated" }));
        }

        [HttpGet("{Roleid:int}")]
        public async Task<IActionResult> Getbyid(int Roleid)
        {

            try
            {
                var roledetails = _rolesRepo.GetByID(Roleid).Result;
                return await Task.FromResult(Ok(roledetails));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in Getbyid {ex.Message}");
                return await Task.FromResult(this.InternalServerError(new { Message = $"Something went wrong!" }));
            }

        }

        [HttpGet("{Rolename:alpha}")]
        public async Task<IActionResult> Getbyname(string Rolename)
        {
            var roledetails = _rolesRepo.GetByName(Rolename).Result;
            return await Task.FromResult(Ok(roledetails.FirstOrDefault()));
        }
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roledetails = _rolesRepo.GetAllRoles().Result;
            return await Task.FromResult(Ok(roledetails));
        }

        [HttpGet("GetPermissionView/{roleID?}")]
        public async Task<IActionResult> GetPermissionView(int? roleID)
        {

            var permissions = _rolesRepo.GetPermissionView(roleID);
            return await Task.FromResult(Ok(permissions));
        }

        [HttpDelete]
        [Route("{Roleid:int}")]
        public async Task<IActionResult> DeleteRole(int Roleid)
        {
            try
            {

                var ApplicationRole = await _rolesRepo.GetApplicationRoleID(Roleid);
                if (ApplicationRole == null)
                {

                    return await Task.FromResult(NotFound(new { Message = $"ApplicationRole Not Found " }));
                }
                var obj = _rolesRepo.Delete(ApplicationRole, _rolesRepo.UserEmail, _rolesRepo.UserFullName, "ApplicationRole Deleted");

                if (obj != null)
                {
                    return await Task.FromResult(Ok(ApplicationRole));
                }
                else
                {
                    return await Task.FromResult(Problem(detail: $"Unable to delete!"));
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(this.InternalServerError(new { Message = $"Something went wrong!", Details = ex.Message }));
            }
        }


    }

}
