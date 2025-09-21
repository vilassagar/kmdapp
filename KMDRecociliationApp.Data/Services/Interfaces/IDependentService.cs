using KMDRecociliationApp.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Services.Interfaces
{
    public interface IDependentService
    {
        Task<IEnumerable<DependentDto>> GetAllDependentsAsync();
        Task<DependentDto> GetDependentByIdAsync(int id);
        Task<IEnumerable<DependentDto>> GetDependentsByApplicantIdAsync(int applicantId);
        Task<DependentDto> CreateDependentAsync(DependentDto dependentDto);
        Task<DependentDto> UpdateDependentAsync(int id, DependentDto dependentDto);
        Task DeleteDependentAsync(int id);
    }
}
