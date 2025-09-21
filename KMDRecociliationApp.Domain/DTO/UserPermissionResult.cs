using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class UserPermissionResult
    {
        public string? PageName { get; set; }
        public bool? Read { get; set; } = false;
        public bool? Create { get; set; } = false;
        public bool? Delete { get; set; } = false;
        public bool? Update { get; set; } = false;
    }
}
