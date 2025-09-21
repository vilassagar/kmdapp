using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DataReturn<T>
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Source { get; set; }
        public string SearchTerm { get; set; }=string.Empty;   
        public int ResultCount { get; set; }
        public List<T> Contents { get; set; }
        public DataPaging Paging { get; set; }
        public DataSorting Sorting { get; set; }

    }


    public class DataReturnPolicy
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Source { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int ResultCount { get; set; }
        public List<DTOMyPolicies> Contents { get; set; }
        public DataPaging Paging { get; set; }
        public DataSorting Sorting { get; set; }

    }

}
