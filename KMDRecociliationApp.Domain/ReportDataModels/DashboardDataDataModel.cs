using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.ReportDataModels
{
    public class DashboardDataDataModel
    {
        public int totalPensioner { get; set; }
        public int totalPensionerStarted { get; set; }
        public int totalPensionerNotStarted { get; set; }
        public int totalPendingRejected { get; set; }
        public int totalPending { get; set; }
        public int totalRejected { get; set; }
        public int totalInitiated { get; set; }
        public int totalInitiatedCheque { get; set; }
        public int totalInitiatedNEFT { get; set; }
        public int totalCompleted { get; set; }
        public int totalCompletedOnline { get; set; }
        public int totalCompletedCheque { get; set; }
        public int totalCompletedNEFT { get; set; }
    }
    
}
