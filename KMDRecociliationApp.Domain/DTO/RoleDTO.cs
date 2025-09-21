using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class RoleDTO
    {
        public string? name { get; set; }
        public string? description { get; set; }
        public int? id { get; set; }
        public bool? isactive { get; set; }
       public List<ApplicationPermissionDTO>? permissions { get; set; }
        public RoleDTO Copy(ApplicationRole role)
        {
            RoleDTO roleDTO = new RoleDTO();
            roleDTO.name = role.RoleName;
            roleDTO.id = role.Id;
            roleDTO.description = role.Description;
            roleDTO.isactive = role.IsActive;
            return roleDTO;
        }
        
    }

  
}
