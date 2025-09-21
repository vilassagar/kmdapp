using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;

namespace KMDRecociliationApp.Domain.Results
{
    public class RefundRequstResult : BaseResult<RefundRequest, RefundRequstResult>
    {
        public int RefundRequestNumber { get; set; }
        public int? OrderNumber { get; set; }
        public string? RetireeName { get; set; }
        public string? MobileNumber { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime RefundRequestDate { get; set; }
        public string? Status { get; set; }       
        public bool ? IsActive { get; set; }
        public int? AssociationId { get; set; }
        public string AssociationName { get; set; }
        public Delta<RolesResult> GetDelta()
        {
            Delta<RolesResult> deleta = new Delta<RolesResult>();
            if (RefundRequestNumber > 0) deleta.TrySetPropertyValue("Id", RefundRequestNumber);
         
            return deleta;

        }

        public override RefundRequstResult CopyPolicyData(RefundRequest x)
        {
            this.RefundRequestNumber = x.Id;
            this.RefundAmount = x.RefundAmount;
            this.RefundRequestDate = x.RefundRequestDate;
            this.IsActive = x.IsActive;
            this.OrderNumber = x.PolicyId;
            if (x.User != null)
                this.RetireeName = x.User.FullName;
            this.Status = x.Status.ToString();
       
            return this;
        }
        public  RefundRequstResult CopyRefundRequest(RefundRequest x)
        {
            this.RefundRequestNumber = x.Id;
            this.RefundAmount = x.RefundAmount;
            this.RefundRequestDate = x.RefundRequestDate;
            this.IsActive = x.IsActive;
            this.OrderNumber = x.PolicyId;
            if (x.User != null)
                this.RetireeName = x.User.FullName;
            this.Status = x.Status.ToString();

            return this;
        }
    }
}
