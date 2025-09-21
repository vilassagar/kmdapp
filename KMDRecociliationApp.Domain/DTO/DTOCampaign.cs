using FluentValidation;
using KMDRecociliationApp.Domain.Common;
using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTOCampaign
    {
        public int? CampaignId { get; set; }
        public string? CampaignName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? isCampaignOpen { get; set; } = true;
        public List<int>? AssociationIds { get; set; } = new List<int>();
        public List<int>? ProductIds { get; set; }
        public List<DtoAssociation>? Associations { get; set; }
        public List<DtoProducts>? Products { get; set; }
        public string? TemplateDocumentUrl { get; set; }
        public string? TemplateDocumentName { get; set; }
        public bool istemplateDocumentUpdated { get; set; }
        public string? TemplateName { get; set; }
        public CommonFileModel? TemplateDocument { get; set; } = null;
    }
    public class DtoAssociation
    {
        public int associationId { get; set; }
        public string? associationName { get; set; }
        public string? organisationName { get; set; }
        public int members { get; set; }

    }
    public class DtoProducts
    {
        public int productId { get; set; }
        public string? productName { get; set; }
        public string? providerName { get; set; }
        public string? policyType { get; set; }
        public string? basePolicy { get; set; }
    }

    public class DTOCampaignData
    {
        public int? CampaignId { get; set; }
        public string? CampaignName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? AssociationIds { get; set; }
        public string? ProductIds { get; set; }
    }
}
