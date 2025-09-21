using AutoMapper;
using KMDRecociliationApp.Data.Repositories.Interfaces;
using KMDRecociliationApp.Data.Services.Interfaces;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Services
{
    public class DependentService : IDependentService
    {
        private readonly IDependentRepository _dependentRepository;
        private readonly IMapper _mapper;

        public DependentService(IDependentRepository dependentRepository, IMapper mapper)
        {
            _dependentRepository = dependentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DependentDto>> GetAllDependentsAsync()
        {
            var dependents = await _dependentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DependentDto>>(dependents);
        }

        public async Task<DependentDto> GetDependentByIdAsync(int id)
        {
            var dependent = await _dependentRepository.GetByIdAsync(id);
            if (dependent == null)
                return null;

            return _mapper.Map<DependentDto>(dependent);
        }

        public async Task<IEnumerable<DependentDto>> GetDependentsByApplicantIdAsync(int applicantId)
        {
            var dependents = await _dependentRepository.GetDependentsByApplicantIdAsync(applicantId);
            return _mapper.Map<IEnumerable<DependentDto>>(dependents);
        }

        public async Task<DependentDto> CreateDependentAsync(DependentDto dependentDto)
        {
            var dependent = _mapper.Map<ApplicantDependent>(dependentDto);
            dependent.CreatedAt = DateTime.UtcNow;

            await _dependentRepository.AddAsync(dependent);
            await _dependentRepository.SaveChangesAsync();

            return _mapper.Map<DependentDto>(dependent);
        }

        public async Task<DependentDto> UpdateDependentAsync(int id, DependentDto dependentDto)
        {
            var existingDependent = await _dependentRepository.GetByIdAsync(id);
            if (existingDependent == null)
                return null;

            _mapper.Map(dependentDto, existingDependent);
            existingDependent.UpdatedAt = DateTime.UtcNow;

            await _dependentRepository.UpdateAsync(existingDependent);
            await _dependentRepository.SaveChangesAsync();

            return _mapper.Map<DependentDto>(existingDependent);
        }

        public async Task DeleteDependentAsync(int id)
        {
            var dependent = await _dependentRepository.GetByIdAsync(id);
            if (dependent == null)
                return;

            await _dependentRepository.DeleteAsync(dependent);
            await _dependentRepository.SaveChangesAsync();
        }
    }
}
