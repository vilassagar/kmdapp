using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Repositories
{
    public class BaseRepo
    {
        ApplicationDbContext _context;
        public BaseRepo() { }
        public BaseRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public ClaimsPrincipal CurrentUser;
        public string UserName { get; set; }

        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public int UserId
        {
            get
            {
                if (CurrentUser != null)
                {
                    var phone = CurrentUser.Claims.FirstOrDefault().Value;// CurrentUser.Claims.Where(x => x.Type == "mobilephone").Select(x => x.Value).FirstOrDefault();
                    if (phone == null)
                        return 0;
                    else
                    {
                        var user = _context.Applicationuser.AsNoTracking()
                            .Where(x=>x.MobileNumber==phone).FirstOrDefault();
                        if (user == null) return 0;
                        else
                            return user.Id;
                    }
                }
                else { return 0; }
            }
        }

    }
}
