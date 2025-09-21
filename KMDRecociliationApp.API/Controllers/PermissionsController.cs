using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Data;
using KMDRecociliationApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KMDRecociliationApp.Domain.Results;
using KMDRecociliationApp.Domain.DTO;

using KMDRecociliationApp.Domain.Entities;

using KMDRecociliationApp.API.Common;

namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class PermissionsController : ApiBaseController
    {
        //private readonly TokenService _tokenService;
        private readonly ILogger<PermissionsController> _logger;
        private readonly PermissionsRepository _permissionRepository;
        private readonly ApplicationDbContext _context;

        public PermissionsController(PermissionsRepository permissionRepository,
           ApplicationDbContext context
            //, TokenService tokenService
            , ILogger<PermissionsController> logger) : base(context)
        {
            // _tokenService = tokenService;
            _logger = logger;
            _permissionRepository = permissionRepository;
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
        //[FromQuery] int PageNumber, [FromQuery] int PageSize, [FromQuery] int Limit, [FromQuery] PermissionResult permission, [FromQuery] DataSorting Sorting)
        {
            //if (HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
                {
                    Sorting = new DataSorting() { SortName = "Id", SortDirection = "desc" };
                }

                DataFilter<PermissionResult> filter = new DataFilter<PermissionResult>()
                {
                    PageNumber = searchDTO.Page,
                    Limit = searchDTO.pageSize,
                    Filter = null,
                    Search = searchDTO.Search,
                    SortName = Sorting.SortName,
                    SortDirection = Sorting.SortDirection
                };
                // NVReturn<PermissionResult> result = new NVReturn<PermissionResult>();
                var result = await _permissionRepository.GetAll(filter);

                return Ok(result);
            }
            catch (Exception)
            {
                return Problem("Something went wrong");
            }
            //}
            //else
            //{

            //    DataReturn<PermissionResult> permissionresult = new DataReturn<PermissionResult>();
            //    permissionresult.ErrorMessage = "you don't have access to this resource ";
            //    permissionresult.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    Log.Warning($"Error in Permission Controller --GetAll method ,status code:{permissionresult.StatusCode}, Message:{permissionresult.ErrorMessage}");
            //    return Unauthorized(permissionresult);
            //}
        }

        [HttpGet("{permissionid:int}")]
        public async Task<IActionResult> GetById(int permissionid)
        {

            try
            {
                var permission = await _permissionRepository.GetByID(permissionid);
                if (permission == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(permission);
                }
            }
            catch (Exception ex)
            {
                return this.InternalServerError(new { Message = "Something went wrong", Detail = ex.Message });
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreatePermission(ApplicationPermissionDTO permissionDTO)
        {
            string message = "";
            try
            {
                PermissionResult permissionResult = new PermissionResult();

                var existingData = await _permissionRepository.CheckPermission(permissionDTO);
                if (existingData == false)
                {
                    var permission = new ApplicationPermission();
                    permission.IsActive = true;
                    permission.Description = permissionDTO.name;
                    permission.PermissionType = permissionDTO.type;
                    if (permissionDTO.actions != null)
                    {
                        permission.Create = permissionDTO.actions.create;
                        permission.Delete = permissionDTO.actions.delete;
                        permission.Read = permissionDTO.actions.read;
                        permission.Update = permissionDTO.actions.update;
                    }
                    var retPermission = _permissionRepository.Add(permission, _permissionRepository.UserEmail, _permissionRepository.UserFullName, "Permission added");
                    if (retPermission.Id > 0)
                        message = "Permission added";
                    //  await Task.FromResult(Ok(new { ID = retPermission.Id, Message = "Permission added" }));
                    else
                        return StatusCode(500, new { Message = "Permission added failed" });
                }
                else
                {
                    return await Task.FromResult(Conflict(new { Message = "Permission Name: " + permissionDTO.name + " already exists" }));
                }

            }
            catch (Exception ex)
            {
                return this.InternalServerError(new { Message = "Something went wrong", Detail = ex.Message });
            }
            return await Task.FromResult(Ok(new { Message = message }));
        }


        [HttpPatch("{permissionid:int}")]
        public async Task<IActionResult> UpdatePermission(int permissionid, ApplicationPermissionDTO permissionDTO)
        {
            PermissionResult permissionResult = new PermissionResult();
            try
            {

                var existingData = await _permissionRepository.CheckPermission(permissionDTO, update: true);
                if (existingData == false)
                {


                    var permissiondto = await _permissionRepository.GetByID(permissionDTO.id);
                    if (permissiondto != null)
                    {
                        var permission = new ApplicationPermission();
                        permission.Id = permissionDTO.id.Value;
                        permission.IsActive = true;
                        permission.Description = permissionDTO.name;
                        permission.PermissionType = permissionDTO.type;

                        if (permissionDTO.actions != null)
                        {
                            permission.Create = permissionDTO.actions.create;
                            permission.Delete = permissionDTO.actions.delete;
                            permission.Read = permissionDTO.actions.read;
                            permission.Update = permissionDTO.actions.update;
                        }
                        var upPermission = _permissionRepository.Update(permission, _permissionRepository.UserEmail, _permissionRepository.UserFullName, "Permission updated");
                        permissionResult = new PermissionResult().CopyPolicyData(upPermission);
                    }
                }
                else
                {
                    return await Task.FromResult(Conflict(new { Message = $"Permission {permissionDTO.name}  already exists" }));
                }
                return await Task.FromResult(Ok(permissionResult));
            }
            catch (Exception ex)
            {
                return this.InternalServerError(new { Message = "Something went wrong", Detail = ex.Message });
            }

        }
       
        [HttpDelete("{Permissionid:int}")]
        public async Task<IActionResult> DeletePermission(int permissionid)
        {


            var upPermission = await _permissionRepository.GetPermissionByID(permissionid);
            if (upPermission == null)
            {
                return await Task.FromResult(NotFound(new { messages = $"Permission Not Found " }));
            }
            else
            {

                var obj = _permissionRepository.Delete(upPermission, _permissionRepository.UserEmail, _permissionRepository.UserFullName, "Permission Deleted");
                if (obj != null)
                {
                    return await Task.FromResult(Ok(upPermission));
                }
                else
                {
                    return await Task.FromResult(this.InternalServerError(new { Message = $"Unable to delete!" }));
                }
            }

        }
    }
}
