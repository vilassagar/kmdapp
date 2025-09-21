using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class ExceptionLog: KeyEntity
    { 

        public int? ErrorLine { get; set; }
        public string? ErrorMessage {  get; set; }
        public int? ErrorNumber {  get; set; }
        public string? ErrorProcedure { get; set; }
        public int? ErrorSeverity { get; set; }
        public int? ErrorState { get; set; }
        public DateTime? DateErrorRaised { get; set; }
       
    }
}
