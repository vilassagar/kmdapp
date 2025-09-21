using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KMDRecociliationApp.API.Common;
using System.Security.Claims;
using KMDRecociliationApp.Data;
using Microsoft.EntityFrameworkCore;
namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[AllowAnonymous]
    public class ApiBaseController : ControllerBase
    {
        ApplicationDbContext _context;
        // Non-nullable initialization
        public ClaimsPrincipal CurrentUser { get; protected set; } = null!;
        public ApiBaseController(ApplicationDbContext context) => _context = context;
 
        public int LoggedInUserId
        {
            get
            {
                if (CurrentUser != null&& CurrentUser.Claims!=null)
                {
                    var phone = CurrentUser.Claims.FirstOrDefault().Value;
                    if (phone == null)
                        return 0;
                    else
                    {
                        var user = _context.Applicationuser.AsNoTracking()
                            .Where(x => x.MobileNumber == phone).FirstOrDefault();
                        if (user == null) return 0;
                        else
                            return user.Id;
                    }
                }else
                    return 0;
            }
        }

    }
}
