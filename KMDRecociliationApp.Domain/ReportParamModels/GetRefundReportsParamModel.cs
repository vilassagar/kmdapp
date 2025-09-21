using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.ReportParamModels
{
    public class GetRefundReportsParamModel
    {
        public int AssociationId { get; set; }
        public int OrganisationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
