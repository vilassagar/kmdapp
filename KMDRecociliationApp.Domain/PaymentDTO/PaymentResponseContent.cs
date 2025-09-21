using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.PaymentDTO
{
    
    public class RootResponse
    {
        public string requestReference { get; set; }
        public string paymentLink { get; set; }
        public string digitPaymentId { get; set; }
    }


}
