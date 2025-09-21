using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public  class ApplicationUserRole:BaseEntity
    {
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }   
    }
}
