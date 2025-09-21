using KMDRecociliationApp.Domain.Common;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class BeneficiaryDetails:KeyEntity
    {
        public int ?PolicyId { get; set; }
        public int? UserId { get; set; }
        public int? Spouse { get; set; }
        public int? Child1 { get; set; }
        public int? Child2 { get; set; }
        public int? Parent1 { get; set; }
        public int? Parent2 { get; set; }
        public int? InLaw1 { get; set; }
        public int? InLaw2 { get; set; }
        public int? Nominee { get; set; }

    }

    public class BeneficiaryPerson : KeyEntity
    {
        public int? PolicyId { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Gender ?Gender { get; set; } 
        public DateTime? DateOfBirth { get; set; }
        public string? DisabilityDocumentName { get; set; }
        public string? DisabilityDocumentUrl { get; set; }
        public NomineeRelation? NomineeRelation { get; set; }
    }

    public class UserSpouseDetail : KeyEntity
    {
    
        public int? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
    public class UserNomineeDetail : KeyEntity
    {
        public int? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? DisabilityDocumentName { get; set; }
        public string? DisabilityDocumentUrl { get; set; }
        public NomineeRelation? NomineeRelation { get; set; }
    }
    public class UserChild1 : KeyEntity
    {   
        public int? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? DisabilityDocumentName { get; set; }
        public string? DisabilityDocumentUrl { get; set; }
     
    }
    public class UserChild2 : KeyEntity
    {
        public int? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? DisabilityDocumentName { get; set; }
        public string? DisabilityDocumentUrl { get; set; }

    }
}
