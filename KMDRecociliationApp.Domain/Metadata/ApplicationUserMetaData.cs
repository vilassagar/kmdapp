using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Metadata
{
    public class ApplicationUserMetaData
    {
        public List<CommonMetadataModel> UserType { get; set; }
        public List<CommonMetadataModel> Gender { get; set; }
        public List<CommonMetadataModel> CountryCode { get; set; }
        public List<CommonMetadataModel> State { get; set; }
        public List<CommonMetadataModel> Association { get; set; }
        public List<CommonMetadataModel> Organisations { get; set; }
    }
}
