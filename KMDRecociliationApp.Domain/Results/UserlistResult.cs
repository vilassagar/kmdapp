using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;

namespace KMDRecociliationApp.Domain.Results
{
    public class UserlistResult : BaseResult<ApplicationUser, UserlistResult>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? CountryCode { get; set; }
        public string? MobileNumber { get; set; }
        public string? AssociationName { get; set; }
        public int ? AssociationId { get; set; }
        public string? UserType { get; set; }
        public int UserId { get; set; }
        public bool? IsProfilePreez { get; set; }
        public string Roles { get; set; }
        public Delta<UserlistResult> GetDelta()
        {
            Delta<UserlistResult> deleta = new Delta<UserlistResult>();
            if (UserId > 0) deleta.TrySetPropertyValue("Id", UserId);
            if (!string.IsNullOrEmpty(FirstName)) deleta.TrySetPropertyValue("FirstName", FirstName);
            if (!string.IsNullOrEmpty(LastName)) deleta.TrySetPropertyValue("LastName", LastName);
            return deleta;
        }

        public override UserlistResult CopyPolicyData(ApplicationUser x)
        {
            this.FirstName = x.FirstName;
            this.LastName = x.LastName;
            this.Email = x.Email;
            this.CountryCode = x.CountryCode;
            this.MobileNumber = x.MobileNumber;
            this.UserId = x.Id;
            this.IsProfilePreez = x.IsProfilePreez;
            this.UserType = x.UserType != null ? x.UserType.Value.ToString() : "";
            this.AssociationName = x.Association != null ? x.Association.AssociationName : "";
            return this;
        }

    }
}
