using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class Announcement:BaseEntity
    {
       // public int Id { get; set; }
        public string Name { get; set; }
        public string AnnouncementText { get; set; }
        public string? AnnouncementLocation { get; set; } // Add this line
        public bool IsCurrent { get; set; }

        
        //public bool IsActive { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public string? UpdatedBy { get; set; }
        //public DateTime? UpdatedAt { get; set; }
    }
}
