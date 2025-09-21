using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTOPensioneer
    {
        public int PaymentId { get; set; }
        public string? RetireeName { get; set; }
        public string? MobileNumber { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        public string? AssociationName { get; set; }
        public string? status { get; set; }
        public string? PaymentMode { get; set; }
    }
}
