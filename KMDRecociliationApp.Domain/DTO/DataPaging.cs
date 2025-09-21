using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DataPaging
    {
        public int NumberOfPages { get; set; }
        public int PageNumber { get; set; } 
        public int RecordsPerPage{get;set;}
        public int NextPageNumber { get; set; } 
        public int PreviousPageNumber { get; set; }

    }

}
