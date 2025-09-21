using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class ApplicationPermissionDTO
    {
        public Actions? actions { get; set; }
        public string? name { get; set; }
        public int? id { get; set; }
        public string? type { get; set; }
        public ApplicationPermissionDTO Copy(ApplicationPermission applicationPermission)
        { 
        var copy = new ApplicationPermissionDTO();
            
            copy.name=applicationPermission.Description;
            copy.id=applicationPermission.Id;
            copy.type = applicationPermission.PermissionType;
            if (copy.actions == null)
                copy.actions=new Actions();

                copy.actions.create = applicationPermission.Create;
                copy.actions.update = applicationPermission.Update;
                copy.actions.read = applicationPermission.Read;
                copy.actions.delete = applicationPermission.Delete;
            
            return copy;
        }

    }
    public class Actions
    {
        public bool? create { get; set; }=false;
        public bool? read { get; set; } = false;
        public bool? update { get; set; } = false;
        public bool? delete { get; set; } = false;
    }
}
