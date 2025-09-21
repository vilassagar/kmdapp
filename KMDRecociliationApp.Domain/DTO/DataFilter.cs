using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DataFilter<T>
    {
        public int Limit { get; set; } = 50;
        public int PageNumber { get; set; } = 1;
        public string SortName { get; set; } = "UpdatedAt";
        public string SortDirection { get; set; } = "desc";
        public string? Search { get; set; }=string.Empty;
        public string? BaseFilter { get; set; } = string.Empty;
        public T? Filter { get; set; } 
        public int userId { get; set; }
        public int userType { get; set; }

        public int AssociationId { get; set; }
        public int CampaignId { get; set; }

    }
}
