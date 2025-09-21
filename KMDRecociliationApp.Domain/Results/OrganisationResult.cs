using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;


namespace KMDRecociliationApp.Domain.Results
{
    public class OrganisationResult : BaseResult<Organisation, OrganisationResult>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Discription { get; set; }
        public Delta<OrganisationResult> GetDelta()
        {
            Delta<OrganisationResult> deleta = new Delta<OrganisationResult>();
            if (Id > 0) deleta.TrySetPropertyValue("Id", Id);
            if (!string.IsNullOrEmpty(Name)) deleta.TrySetPropertyValue("OrganisationName", Name);
            return deleta;

        }

        public override OrganisationResult CopyPolicyData(Organisation x)
        {
            this.Name = x.Name;
            this.Id = x.Id;
            this.Discription = x.Description;
            return this;
        }


    }
}
