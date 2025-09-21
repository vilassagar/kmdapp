using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class AppLogs: KeyEntity
    {
        public string? Comment { get; set; }     
        public int UserId { get; set; }
        public DateTime? Auditdate { get; set; }=DateTime.Now;
        public ApplogType? Recordtype { get; set; }
    }
}
