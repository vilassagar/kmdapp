using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class ApplicationUserTemplate
    {
        public IFormFile template { get; set; }
    }
    public class ImportDataTemplateResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }

    public class AssociationTemplate
    {
        public IFormFile template { get; set; }
    }
    public class ApplicantsTemplate
    {
        public IFormFile template { get; set; }
    }
    public class ProductTemplate
    {
        public IFormFile template { get; set; }
    }
    public class ReconcilationChequeTemplate
    {
        public int CampaignId { get; set; }
        public IFormFile template { get; set; }
    }
    public class ReconcilationNEFTTemplate
    {
        public int CampaignId { get; set; }
        public IFormFile template { get; set; }

    }
}