using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTOExecuteCampaign
    {
        public int? CampaignId { get; set; }
        public int? AssociationId { get; set; }
        public string? MessageTemplateName { get; set; }
    }
}
