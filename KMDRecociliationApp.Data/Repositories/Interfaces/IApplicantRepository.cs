using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;


namespace KMDRecociliationApp.Data.Repositories.Interfaces
{
    public interface IApplicantRepository : IRepository<ApplicantInsurancePolicy>
    {
        Task<ApplicantInsurancePolicy> GetApplicantWithDetailsAsync(int id);
        Task<ApplicantInsurancePolicy> GetApplicantByUniqueIdentifierAsync(string uniqueIdentifier);
        Task<IEnumerable<ApplicantInsurancePolicy>> GetApplicantsByOrganizationAsync(string organization);
        Task<ApplicantInsurancePolicy> CheckUnique(string uniqueCode);
        Task<(IEnumerable<ApplicantInsurancePolicy> applicants, int totalCount)> GetFilteredApplicantsAsync(
             string searchTerm,
             string organization,
             string idCardType,
             decimal? minSalary,
             decimal? maxSalary,
             string gender,
             DateTime? fromDate,
             DateTime? toDate,
             string sortBy,
             bool sortDescending,
             int pageNumber,
             int pageSize);
    }
    namespace KMD.EnrolmentPortal.Repositories.Interfaces
    {
        public interface IBankDetailsRepository : IRepository<ApplicantBankDetails>
        {
            Task<ApplicantBankDetails> GetBankDetailsByApplicantIdAsync(int applicantId);
        }
    }
    public interface IDependentRepository : IRepository<ApplicantDependent>
    {
        Task<IEnumerable<ApplicantDependent>> GetDependentsByApplicantIdAsync(int applicantId);
    }
}
