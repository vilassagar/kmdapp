using KMDRecociliationApp.Data.Interface;
using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Repositories
{
    public class UserRoleRepository : MainHeaderRepo<ApplicationUserRole>
    {
        ApplicationDbContext _context;
        public UserRoleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
