using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;


namespace KMDRecociliationApp.Domain.Results
{
    public class AssociationResult : BaseResult<Association, AssociationResult>
    {
        public int Id { get; set; }
        public string? AssociationName { get; set; }
        public int AssociationId { get; set; }
        public int OraganisationId { get; set; }
        public string OrganisationName { get; set; }
        public int ParentAssociationId { get; set; }
        public string? Address { get; set; }
        public int? StateId { get; set; }
        public string? PINCode { get; set; }
        public int? Members {  get; set; }
        public Delta<AssociationResult> GetDelta()
        {
            Delta<AssociationResult> deleta = new Delta<AssociationResult>();
            if (Id > 0) deleta.TrySetPropertyValue("Id", Id);
            if (!string.IsNullOrEmpty(AssociationName)) deleta.TrySetPropertyValue("AssociationName", AssociationName);
            return deleta;

        }

        public override AssociationResult CopyPolicyData(Association x)
        {
            this.AssociationName = x.AssociationName;
            this.Id = x.Id;
            this.AssociationId = x.Id;
            this.OraganisationId = (int)x.OraganisationId;
            this.Members = x.ApplicationUser!=null?x.ApplicationUser.Count:0;
            this.OrganisationName = x.Organisation != null ? x.Organisation.Name : "";
            return this;
        }

      
    }
}
