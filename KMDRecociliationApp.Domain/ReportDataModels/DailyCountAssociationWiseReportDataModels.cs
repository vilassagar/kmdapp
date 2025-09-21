using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.ReportDataModels
{
    public class DailyCountAssociationWiseReportDataModels
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }       
        public string? CountryCode { get; set; }
        public string? MobileNumber { get; set; }
        public DateTime DOB { get; set; }
        public int? UserType { get; set; }
        public int? Gender { get; set; }
        public int? OrganisationId { get; set; }        
        public string? Organisation { get; set; }
        public int? AssociationId { get; set; }
        public string? AssociationName { get; set; }
        public string? EMPIDPFNo { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int? StateId { get; set; }
        public string? State { get; set; }
        public int? CountryId { get; set; }
        public string? Country { get; set; }
        public string? Pincode { get; set; }
        public bool IsProfileComplete { get; set; }
        public  DateTime CreatedAt { get; set; }

    }
}
