using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTOFinancialYear
    {
        public int? Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? FinancialYearLabel { get; set; }//(e.g., "2023-2024")
        public bool? IsCurrent { get; set; }
    }
}
