using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class Campaigns : BaseEntity
    {
        public string? CampaignName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? isCampaignOpen { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentURL { get; set; }
        public string? TemplateName { get; set; }
        public CampaignStatus? SentStatus { get; set; }

        // Navigation properties
        public ICollection<CampaignAssociations>? CampaignAssociations { get; set; } = new List<CampaignAssociations>();
        public ICollection<CampaignProducts>? CampaignProducts { get; set; } = new List<CampaignProducts>();
        public ICollection<ExecuteCampaign>? ExecuteCampaign { get; set; } = new List<ExecuteCampaign>();
        public ICollection<CampaignMembersDetails>? CampaignMembersDetails { get; set; } = new List<CampaignMembersDetails>();
    }

    public class CampaignAssociations : KeyEntity
    {
        // Foreign keys
        public int CampaignId { get; set; }
        public int AssociationId { get; set; }

        // Navigation properties
        public Campaigns Campaign { get; set; } = null!;
        public Association? Association { get; set; }
        public DateTime? SentDate { get; set; }
    }

    public class CampaignProducts : KeyEntity
    {
        // Foreign keys
        public int CampaignId { get; set; }
        public int? ProductId { get; set; }

        // Navigation properties
        public Campaigns Campaign { get; set; } = null!;
        public Product? Product { get; set; }
    }

    public class ExecuteCampaign : BaseEntity
    {
        // Foreign keys
        public int CampaignId { get; set; }
        public int? AssociationId { get; set; }
        public DateTime? SentDate { get; set; }

        // Navigation properties
        public Campaigns Campaign { get; set; } = null!;
        public Association? Association { get; set; }
        public ICollection<CampaignMembersDetails> CampaignMembersDetail { get; set; } = new List<CampaignMembersDetails>();
    }

    public class CampaignMembersDetails : KeyEntity
    {
        // Foreign keys
        public int ExecuteCampaignId { get; set; }
        public int UserId { get; set; }
        public bool? SentStatus { get; set; }
        public DateTime? SentDate { get; set; }

        // Navigation properties
        public ExecuteCampaign ExecuteCampaign { get; set; } = null!;

        // These are redundant since they can be accessed through ExecuteCampaign
        // but keeping them for direct access if needed
        public int? CampaignId { get; set; }
        public int? AssociationId { get; set; }
        public Campaigns? Campaign { get; set; }
        public Association? Association { get; set; }
    }
}
