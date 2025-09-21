using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class TemplateMetadataLookup
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string Worksheet { get; set; }
        public int ColumnNumber { get; set; }
        public string ColumnKey {  get; set; }
        public string Description { get; set; }
        
    }
}
