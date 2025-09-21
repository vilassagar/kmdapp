using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;

namespace KMDRecociliationApp.Domain.Results
{
    public class AuditLogResult
    {
        public int Auditid { get; set; }

        public string? Audittype { get; set; }

        public string? Recordid { get; set; }

        public string? Recordtype { get; set; }

        public string? Keyword { get; set; }

        public string? Oldvalue { get; set; }

        public string? Newvalue { get; set; }

        public string? Username { get; set; }
        public string? Emailid { get; set; }

        public DateTime? Auditdate { get; set; }
        public string Auditdatestring { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public Delta<AuditLogResult> GetDelta()
        {
            Delta<AuditLogResult> delta = new Delta<AuditLogResult>();
            if (Auditid > 0) delta.TrySetPropertyValue("Auditid", Auditid);
            if (!string.IsNullOrEmpty(Audittype)) delta.TrySetPropertyValue("Audittype", Audittype);
            if (!string.IsNullOrEmpty(Recordid)) delta.TrySetPropertyValue("Recordid", Recordid);
            if (!string.IsNullOrEmpty(Recordtype)) delta.TrySetPropertyValue("Applicatiodescription", Recordtype);
            if (!string.IsNullOrEmpty(Keyword)) delta.TrySetPropertyValue("Keyword", Keyword);
            if (!string.IsNullOrEmpty(Newvalue)) delta.TrySetPropertyValue("Newvalue", Newvalue);
            if (!string.IsNullOrEmpty(Oldvalue)) delta.TrySetPropertyValue("Oldvalue", Oldvalue);
            if (!string.IsNullOrEmpty(Comment)) delta.TrySetPropertyValue("Comment", Comment);
            if (!string.IsNullOrEmpty(Username)) delta.TrySetPropertyValue("Username", Username);
            if (!string.IsNullOrEmpty(Emailid)) delta.TrySetPropertyValue("Emailid", Emailid);
            return delta;
        }



        public AuditLogResult Copy(Audit x)
        {
            if (x != null)
            {
                this.Audittype = x.Audittype;
                this.Recordid = x.Recordid;
                this.Recordtype = x.Recordtype;
                this.Keyword = x.Keyword;
                this.Newvalue = x.Newvalue;
                this.Oldvalue = x.Oldvalue;
                this.Username = x.Userfullname;
                this.Emailid = x.Emailid;
                this.Comment = x.Comment;
                this.Auditdate = x.Auditdate;

                this.Auditdatestring = x.Auditdate == null ? "" : x.Auditdate.Value.ToString("MMMM dd, yyyy hh:mm:ss tt");

                this.Auditid = x.Auditid;
            }
            return this;
        }
    }
}
