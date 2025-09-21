using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.DTO.Common;
using KMDRecociliationApp.Domain.DTO.InsurerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Services.Interfaces
{
    public interface IApplicantService
    {
        Task<IEnumerable<ApplicantDto>> GetAllApplicantsAsync();
        Task<ApplicantDto> GetApplicantByIdAsync(int id);
        Task<ApplicantDto> GetApplicantByUniqueIdentifierAsync(string uniqueIdentifier);
        Task<IEnumerable<ApplicantDto>> GetApplicantsByOrganizationAsync(string organization);
        Task<(ApplicantDto, List<string> messages)> CreateApplicantAsync(ApplicantDto applicantDto);
        Task<ApplicantDto> UpdateApplicantAsync(int id, ApplicantDto applicantDto);
        Task DeleteApplicantAsync(int id);
        Task<DataReturn<ApplicantDto>> GetFilteredApplicantsAsync(ApplicantFilterDto filterDto);

    }
}
