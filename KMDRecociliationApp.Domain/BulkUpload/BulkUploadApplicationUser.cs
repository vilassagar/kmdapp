using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.BulkUpload
{
    public class BulkUploadApplicationUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EMPIDPFNo { get; set; }
        public int? OrganisationId { get; set; }
        public int? AssociationId { get; set; }
        public DateTime? DOB { get; set; }
        public int? Gender { get; set; }
        public string? Email { get; set; }
        public string? CountryCode { get; set; }
        public string? MobileNumber { get; set; }
        public int? StateId { get; set; }
        public string? Pincode { get; set; }
        public string? Address { get; set; }

    }
}
