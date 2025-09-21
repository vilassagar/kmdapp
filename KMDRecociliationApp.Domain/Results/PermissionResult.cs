using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;

namespace KMDRecociliationApp.Domain.Results
{
    public class PermissionResult : BaseResult<ApplicationPermission, PermissionResult>
    {
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Actions Actions { get; set; } = new Actions();
        public int Id { get; set; }
        public bool IsActive { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public DateTime UpdatedAt { get; set; }
        //public int CreatedBy { get; set; }
        //public int UpdatedBy { get; set; }

        public Delta<PermissionResult> GetDelta()
        {
            Delta<PermissionResult> deleta = new Delta<PermissionResult>();
            if (Id > 0) deleta.TrySetPropertyValue("Id", Id);
            if (!string.IsNullOrEmpty(Type)) deleta.TrySetPropertyValue("PermissionType", Type);
            // if (!string.IsNullOrEmpty(AccessType)) deleta.TrySetPropertyValue("AccessType", AccessType);
            if (!string.IsNullOrEmpty(Name)) deleta.TrySetPropertyValue("Description", Name);
            return deleta;

        }
        //public PermissionResult Copy(ApplicationPermission permission)
        //{
        //    this.PermissionType = permission.PermissionType;
        //    this.AccessType = permission.AccessType;
        //    this.Description = permission.Description;
        //    return this;
        //}
        public override PermissionResult CopyPolicyData(ApplicationPermission x)
        {
            this.Type = x.PermissionType;
            this.Id = x.Id;
            this.Name = x.Description;
            this.Actions.create = x.Create;
            this.Actions.update = x.Update;
            this.Actions.read = x.Read;
            this.Actions.delete = x.Delete;

            return this;
        }
    }
}
