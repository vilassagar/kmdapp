using KMDRecociliationApp.Domain.Common;
using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;

namespace KMDRecociliationApp.Domain.Results
{
    public class CampaignResult
    {
        
        public int campaignId { get; set; }
        public int Id { get; set; }
        public string? CampaignName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int remainingDays { get; set; }
        public bool isCampaignOpen {  get; set; }
        public ICollection<CampaignProductsResult> CampaignProducts { get; set; } = new List<CampaignProductsResult>();
        public ICollection<CampaignAssociationsResult> CampaignAssociations { get; set; } = new List<CampaignAssociationsResult>();

        public static int GetDaysDifferenceFromToday(DateTime endDate)
        {
            DateTime today = DateTime.Now.Date; // Use DateTime.Now.Date to ignore the time part
            if (endDate < today)
            {
                return 0;
            }

            TimeSpan difference = endDate - today;
            return (int)difference.TotalDays;
        }
    }
    public class CampaignProductsResult
    {
        public int? CampaignId { get; set; }
        public int? ProductId { get; set; }     
        public string? ProductName { get; set; }
    }
    public class CampaignAssociationsResult 
    {
        public int? CampaignId { get; set; }
        public int? AssociationId { get; set; }     
        public string? AssociationName { get; set; }
        public string TemplateName { get; set; }
        public CommonFileModel TemplateDocument { get; set; } = new CommonFileModel();
        public int members { get; set; }
    }
}
