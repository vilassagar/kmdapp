using KMDRecociliationApp.Domain.Common;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTOBeneficiaryDetails
    {
        public int PolicyId { get; set; }
        public int Id { get; set; }
        public DTOBeneficiaryPerson? Spouse { get; set; } = new DTOBeneficiaryPerson();
        public DTOBeneficiaryPerson? Child1 { get; set; } = new DTOBeneficiaryPerson();
        public DTOBeneficiaryPerson? Child2 { get; set; } = new DTOBeneficiaryPerson();
        public DTOBeneficiaryPerson? Parent1 { get; set; } = new DTOBeneficiaryPerson();
        public DTOBeneficiaryPerson? Parent2 { get; set; } = new DTOBeneficiaryPerson();
        public DTOBeneficiaryPerson? InLaw1 { get; set; } = new DTOBeneficiaryPerson();
        public DTOBeneficiaryPerson? InLaw2 { get; set; } = new DTOBeneficiaryPerson();

    }
    public class DTOBeneficiaryPerson
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public CommonNameDTO? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsCertificateUpdated { get; set; }
        public CommonFileModel? DisabilityCertificate { get; set; }
        public CommonNameDTO? NomineeRelation { get; set; }
    }

}

