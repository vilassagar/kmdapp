using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class SMSRequestObject
    {
        public long feedid { get; set; }
        public long username { get; set; }
        public string password { get; set; }
        public string jobname { get; set; }
        public long mobile { get; set; }
        public string messages { get; set; } = "";
        //public string templateid { get; set; }
        //public string entityid { get; set; }
    }
}
