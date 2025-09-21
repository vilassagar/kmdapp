using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Repositories
{
    public class ProductPolicyRepository : MainHeaderRepo<PolicyHeader>
    {
        ApplicationDbContext _context;
        public ProductPolicyRepository(
            ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
