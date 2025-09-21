using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class Audit
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
        public string? AffectedColumns { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public string?  Userfullname { get; set; }
        public int UserId { get; set; }
        public string? Type { get; set; }
        public string? TableName { get; set; }
        public DateTime DateTime { get; set; }
        public string? PrimaryKey{ get; set; }


    }
    
}
