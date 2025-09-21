using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class VMRolePermission
    {
        public int Roleid { get; set; }
        public string Rolename { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string[] Permissions { get; set; }

    }

}
