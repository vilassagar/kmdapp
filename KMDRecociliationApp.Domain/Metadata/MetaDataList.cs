using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Metadata
{
    public class MetaDataList
    {
        public string Worksheet { get; set; }

        public string ColumnName { get; set; }

        public string ColumnKey { get; set; }

        public int ColumnNumber { get; set; }

        public string TemplateName { get; set; }
    }
}
