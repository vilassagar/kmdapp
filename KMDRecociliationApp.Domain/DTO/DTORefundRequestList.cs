using Azure.Core;
using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTORefundRequestList
    {
        public int RefundRequestNumber { get; set; }
        public string? ProductName { get; set; }
        public string? RetireeName { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime RefundRequestDate { get; set; }
        public string Status { get; set; }
        public string? Acknowledgement { get; set; }  
    }
}
