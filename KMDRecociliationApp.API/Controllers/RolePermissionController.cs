using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.InkML;
using KMDRecociliationApp.Data;
namespace KMDRecociliationApp.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
  //  [Authorize]
    public class RolePermissionController : ApiBaseController
    {
        private readonly RolePermissionRepository _rolepermissionRepo;
        private readonly ApplicationDbContext _context;
        public RolePermissionController(RolePermissionRepository rolepermissionRepo,ApplicationDbContext context) : base(context)
        {
            _rolepermissionRepo = rolepermissionRepo;
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get(int Rolepermissionid)
        {
            if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
                List<ApplicationRolePermission> rolepermissiondetails = new List<ApplicationRolePermission>();
                rolepermissiondetails = _rolepermissionRepo.GetAll();
                return await Task.FromResult(Ok(rolepermissiondetails));
            }
            return Unauthorized();
        }

        [HttpPut]
        [Route("assignpermissiontorole")]
        public async Task<IActionResult> AssignPermissiontoRole(int Rolepermissionid, int Roleid, int Permissionid)
        {
            if (Request.HttpContext.User.Identity.IsAuthenticated)
            {
                var success_msg = await _rolepermissionRepo.AssignPermission(Rolepermissionid, Roleid, Permissionid);
                if (success_msg == "Success")
                {
                    return await Task.FromResult(Ok("Permission added to Role successfully"));
                }
            }
            return Unauthorized();

        }

    }
}
