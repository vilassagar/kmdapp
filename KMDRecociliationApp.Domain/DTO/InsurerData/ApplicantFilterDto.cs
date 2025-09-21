using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO.InsurerData
{
    public class ApplicantFilterDto
    {
        // Pagination parameters
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Searching parameters
        public string? Search { get; set; } = string.Empty;

        // Filtering parameters
        public string? Organization { get; set; }
        public string ?IdCardType { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? Gender { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Sorting parameters
        public string SortBy { get; set; } = "Id";
        public bool SortDescending { get; set; } = false;

        // Helper method to add additional filters 
        public List<KeyValuePair<string, string>> GetFilters()
        {
            var filters = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrEmpty(Organization))
                filters.Add(new KeyValuePair<string, string>("Organization", Organization));

            if (!string.IsNullOrEmpty(IdCardType))
                filters.Add(new KeyValuePair<string, string>("IdCardType", IdCardType));

            if (MinSalary.HasValue)
                filters.Add(new KeyValuePair<string, string>("MinSalary", MinSalary.Value.ToString()));

            if (MaxSalary.HasValue)
                filters.Add(new KeyValuePair<string, string>("MaxSalary", MaxSalary.Value.ToString()));

            if (!string.IsNullOrEmpty(Gender))
                filters.Add(new KeyValuePair<string, string>("Gender", Gender));

            if (FromDate.HasValue)
                filters.Add(new KeyValuePair<string, string>("FromDate", FromDate.Value.ToString("yyyy-MM-dd")));

            if (ToDate.HasValue)
                filters.Add(new KeyValuePair<string, string>("ToDate", ToDate.Value.ToString("yyyy-MM-dd")));

            return filters;
        }
    }
}
