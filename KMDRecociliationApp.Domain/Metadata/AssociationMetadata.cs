using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Metadata
{
        public class AssociationMetadata
        {
            public List<CommonMetadataModel> YesNo { get; set; }
            public List<CommonMetadataModel> States { get; set; }
            public List<CommonMetadataModel> Countries { get; set; }
            public List<CommonMetadataModel> Organisations { get; set; }
            public List<CommonMetadataModel> Association { get; set; }
        }
}
