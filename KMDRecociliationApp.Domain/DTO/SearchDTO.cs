using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class SearchDTO
    {
        public  int Page { get; set; }
        public int pageSize { get; set; }
        public int userTypeId { get; set; }

        public  string? Search { get; set; }=string.Empty;
        public string ? BaseFilter { get; set; } = string.Empty;
        public int userId { get; set; } = 0;
        public int AssociationId { get; set; } = 0;
        public int CampaignId { get; set; } = 0;
    }
}
