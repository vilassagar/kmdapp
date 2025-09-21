using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class Association:BaseEntity
    {
        public string? AssociationName { get; set; }
        public string? AssociationCode { get; set; }
        public int? OraganisationId { get; set; }
        public int? ParentAssociationId { get; set; } 
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }       
        public int? StateId { get; set; }
        public string? PINCode { get; set; }
        public int? CountryId { get; set; }
        public bool AcceptOnePayPayment { get; set; }
        public ICollection<ApplicationUser> ApplicationUser { get; set; } = new List<ApplicationUser>();
        public ICollection<AssociationContactDetails> AssociationContactDetails { get; set; } = new List<AssociationContactDetails>();
        public ICollection<AssociationMessageDetails> AssociationMessageDetails { get; set; } = new List<AssociationMessageDetails>();
        public Organisation? Organisation { get; set; }
        public AssociationBankDetails? AssociationBankDetails { get; set; }
    }

    public class AssociationOnePayDetails:BaseEntity
    {
        public int AssociationId { get; set; }
        public string? OnepayUrl { get; set; }
        public string? OnePayId { get; set; }
        public Association? Association { get; set; }

    }
    public class AssociationContactDetails:BaseEntity
    {
        public int AssociationId { get; set; }
        public string? FirstName { get; set; }   
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Association? Association { get; set; }
    }
    public class AssociationBankDetails:BaseEntity
    {      
        public int AssociationId { get; set; }
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? MICRCode { get; set; }
        public string? MendatePath { get; set; }
        public string? AccountName { get; set; }
        public int? MendateId { get; set; }
        public string? MendateName { get; set; }
        public string? MendateUrl { get; set; }
        public int? QRCodeId { get; set; }
        public string? QRCodeName { get; set; }
        public string? QRCodeUrl { get; set; }
        public Association? Association { get; set; }
    }

    public class AssociationMessageDetails : BaseEntity
    {
        public int AssociationId { get; set; }
        public string? Name { get; set; }
        public string? Template { get; set; }
        public bool IsApproved { get; set; }
        public Association? Association { get; set; }

    }


}
