using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.ReportDataModels
{
    public class FormatedDashboardDataDataModel
    {
        public List<NameCountCommonModel> Pensioner{get; set; }
        public List<NameCountCommonModel> PendingRejected { get; set; }
        public List<NameCountCommonModel> Initiated { get; set; }
        public List<NameCountCommonModel> Completed { get; set; }

    }
    public class NameCountCommonModel
    { 
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
