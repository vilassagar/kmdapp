using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ClosedXML.Excel.XLPredefinedFormat;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DateTime = System.DateTime;

namespace KMDRecociliationApp.Domain.Entities
{
    public class FinancialYear: BaseEntity
    {
        //public int FinancialYearId {  get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? FinancialYearLabel { get; set; }//(e.g., "2023-2024")
        public bool? IsCurrent {  get; set; }
         
    }
}
