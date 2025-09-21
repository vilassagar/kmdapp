using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public partial class PermissionPivotView
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public bool? Read { get; set; }
        public bool? Create { get; set; }
        public bool? Update { get; set; }
        public bool? Delete { get; set; }
        //public bool ReadChecked { get; set; }
        //public bool CreateChecked { get; set; }
        //public bool UpdateChecked { get; set; }
        //public bool DeleteChecked { get; set; }
    }

}
