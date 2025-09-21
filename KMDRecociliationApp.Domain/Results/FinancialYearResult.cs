using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;

namespace KMDRecociliationApp.Domain.Results
{
    public class FinancialYearResult :BaseResult<FinancialYear, FinancialYearResult>
    {
        public int? Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string?  FinancialYearLabel { get; set; }//(e.g., "2023-2024")
        public bool? IsCurrent { get; set; }

        public FinancialYearResult Copy(FinancialYear? x)
        {
            this.Id = x.Id;
            this.StartDate= x.StartDate;
            this.EndDate= x.EndDate;
            this.FinancialYearLabel = x.FinancialYearLabel;
            this.IsCurrent= x.IsCurrent;
            return this;
        }

        public Delta GetDelta()
        {
            Delta<FinancialYearResult> deleta = new Delta<FinancialYearResult>();
            if (Id > 0) deleta.TrySetPropertyValue("Id", Id);
            if (!string.IsNullOrEmpty(FinancialYearLabel)) deleta.TrySetPropertyValue("FinancialYearLabel", FinancialYearLabel);
            return deleta;
        }
    }
}
