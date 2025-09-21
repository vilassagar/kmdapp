using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.ReportParamModels
{
    public class AssociationWisePaymentDetailsReportParamModel
    {
        public int AssociationId { get; set; }
        public int PaymentStatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
    }
}
