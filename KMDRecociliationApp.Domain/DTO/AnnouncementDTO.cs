using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class AnnouncementDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AnnouncementText { get; set; }
        public string AnnouncementLocation { get; set; }
        //public bool IsActive { get; set; }
        public bool IsCurrent { get; set; }
    }
}
