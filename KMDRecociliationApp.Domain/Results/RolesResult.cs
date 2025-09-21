using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;

namespace KMDRecociliationApp.Domain.Results
{
    public class RolesResult : BaseResult<ApplicationRole, RolesResult>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
      

        public Delta<RolesResult> GetDelta()
        {
            Delta<RolesResult> deleta = new Delta<RolesResult>();
            if (Id > 0) deleta.TrySetPropertyValue("Id", Id);
            if (!string.IsNullOrEmpty(Description)) deleta.TrySetPropertyValue("Description", Description) ;
            if (!string.IsNullOrEmpty(Name)) deleta.TrySetPropertyValue("RoleName", Name);
            return deleta;

        }

        public override RolesResult CopyPolicyData(ApplicationRole x)
        {
            this.Name = x.RoleName;
            this.Id = x.Id;
            this.Description = x.Description;
            this.IsActive = x.IsActive;
            return this;
        }

    }
}
