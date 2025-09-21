using KMDRecociliationApp.Data.Repositories.Interfaces.KMD.EnrolmentPortal.Repositories.Interfaces;
using KMDRecociliationApp.Data.Repositories.Interfaces;
using KMDRecociliationApp.Data.Services.Interfaces;
using KMDRecociliationApp.Domain.Entities;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMDRecociliationApp.Domain.DTO.InsurerData;
using AutoMapper;
using KMDRecociliationApp.Domain.DTO.Common;
using KMDRecociliationApp.Data.Exceptions;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Results;

namespace KMDRecociliationApp.Data.Services
{
    public class ApplicantService : IApplicantService
    {
        private readonly IApplicantRepository _applicantRepository;
        private readonly IBankDetailsRepository _bankDetailsRepository;
        private readonly IDependentRepository _dependentRepository;
        private readonly IMapper _mapper;

        public ApplicantService(
            IApplicantRepository applicantRepository,
            IBankDetailsRepository bankDetailsRepository,
            IDependentRepository dependentRepository,
            IMapper mapper)
        {
            _applicantRepository = applicantRepository;
            _bankDetailsRepository = bankDetailsRepository;
            _dependentRepository = dependentRepository;
            _mapper = mapper;
        }
        public async Task<DataReturn<ApplicantDto>> GetFilteredApplicantsAsync(ApplicantFilterDto filterDto)
        {
            var ret = new DataReturn<ApplicantDto>();
            // Default values if not provided
            int pageNumber = filterDto.Page < 1 ? 1 : filterDto.Page;
            int pageSize = filterDto.PageSize < 1 ? 10 : filterDto.PageSize;

            // Call the repository method
            var (applicants, totalCount) = await _applicantRepository.GetFilteredApplicantsAsync(
                filterDto.Search,
                filterDto.Organization,
                filterDto.IdCardType,
                filterDto.MinSalary,
                filterDto.MaxSalary,
                filterDto.Gender,
                filterDto.FromDate,
                filterDto.ToDate,
                filterDto.SortBy,
                filterDto.SortDescending,
                pageNumber,
                pageSize);

            // Map the entities to DTOs
            var applicantDtos = _mapper.Map<List<ApplicantDto>>(applicants);

            // Calculate total pages
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Create the paginated response
            var response = new PaginatedResponseDto<ApplicantDto>
            {
                Items = applicantDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            // Add metadata for UI display
            //response.Metadata.Add("AppliedFilters", filterDto.GetFilters().Count.ToString());
            //response.Metadata.Add("SortedBy", $"{filterDto.SortBy} {(filterDto.SortDescending ? "Descending" : "Ascending")}");
            ret.Contents = applicantDtos;// objList.Select(x => new AssociationResult().Copy(x: x)).ToList();
            ret.Source = "Association";
            ret.ResultCount = totalCount;
            ret.StatusCode = 200;
            //Paging information
            int numberOfPages = (totalCount / filterDto.PageSize) + ((totalCount % filterDto.PageSize > 0) ? 1 : 0);
            DataPaging paging = new DataPaging();
            paging.RecordsPerPage = filterDto.PageSize;
            paging.PageNumber = filterDto.Page;
            paging.NumberOfPages = numberOfPages;
            if (filterDto.Page > 1)
                paging.PreviousPageNumber = filterDto.Page - 1;
            if (numberOfPages > filterDto.Page)
                paging.NextPageNumber = filterDto.Page + 1;
            ret.Paging = paging;
            DataSorting sorting = new DataSorting();
            sorting.SortName = "";
            sorting.SortDirection = "";
            ret.Sorting = sorting;

            return ret;
        }
        public async Task<IEnumerable<ApplicantDto>> GetAllApplicantsAsync()
        {
            var applicants = await _applicantRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ApplicantDto>>(applicants);
        }

        public async Task<ApplicantDto> GetApplicantByIdAsync(int id)
        {
            var applicant = await _applicantRepository.GetApplicantWithDetailsAsync(id);
            if (applicant == null)
                return null;

            // Map the entities to DTOs
            var applicantDtos = _mapper.Map<ApplicantDto>(applicant);
            if(applicant.BankDetails!=null)
            applicantDtos.BankDetails = _mapper.Map<BankDetailsDto>(applicant.BankDetails);
            if(applicant.Dependents != null && applicant.Dependents.Count > 0)
                applicantDtos.Dependents = _mapper.Map<List<DependentDto>>(applicant.Dependents);

            return applicantDtos;
        }

        public async Task<ApplicantDto> GetApplicantByUniqueIdentifierAsync(string uniqueIdentifier)
        {
            var applicant = await _applicantRepository.GetApplicantByUniqueIdentifierAsync(uniqueIdentifier);
            if (applicant == null)
                return null;

            return _mapper.Map<ApplicantDto>(applicant);
        }

        public async Task<IEnumerable<ApplicantDto>> GetApplicantsByOrganizationAsync(string organization)
        {
            var applicants = await _applicantRepository.GetApplicantsByOrganizationAsync(organization);
            return _mapper.Map<IEnumerable<ApplicantDto>>(applicants);
        }

        public async Task<(ApplicantDto, List<string> messages)> CreateApplicantAsync(ApplicantDto applicantDto)
        {
            var applicant = _mapper.Map<ApplicantInsurancePolicy>(applicantDto);
           var errrs = new List<string>();

            // Generate unique identifier
            applicant.GenerateUniqueIdentifier();

            var appobj= await _applicantRepository.CheckUnique(applicant.UniqueIdentifier);
            if (appobj != null)
            {
                errrs.Add("First Name & Id Card already exists");
              
                return (new ApplicantDto(),errrs);
            }
            // Set timestamps
            applicant.CreatedAt = DateTime.UtcNow;

            applicant.BankDetails = null;
            applicant.Dependents = null;

            // Add the applicant to the database
            await _applicantRepository.AddAsync(applicant);
            await _applicantRepository.SaveChangesAsync();

            // Add bank details if provided
            if (applicantDto.BankDetails != null)
            {
                if (!string.IsNullOrWhiteSpace(applicantDto.BankDetails.BankAccountNumber) )
                {
                    var bankDetails = _mapper.Map<ApplicantBankDetails>(applicantDto.BankDetails);
                    bankDetails.ApplicantId = applicant.Id;
                    bankDetails.CreatedAt = DateTime.UtcNow;
                    await _bankDetailsRepository.AddAsync(bankDetails);
                    await _bankDetailsRepository.SaveChangesAsync();
                }
            }

            // Add dependents if provided
            if (applicantDto.Dependents != null && applicantDto.Dependents.Count > 0)
            {
                foreach (var dependentDto in applicantDto.Dependents)
                {
                    if (string.IsNullOrWhiteSpace(dependentDto.FirstName))
                        continue;
                    var dependent = _mapper.Map<ApplicantDependent>(dependentDto);
                    dependent.ApplicantId = applicant.Id;
                    dependent.CreatedAt = DateTime.UtcNow;
                    await _dependentRepository.AddAsync(dependent);
                }
                await _dependentRepository.SaveChangesAsync();
            }

            // Return the newly created applicant with all details
           
            return (await GetApplicantByIdAsync(applicant.Id), errrs);
        }
        public async Task<ApplicantDto> UpdateApplicantAsync(int id, ApplicantDto applicantDto)
        {
            // Find the existing applicant
            var existingApplicant = await _applicantRepository.GetByIdAsync(id);
            if (existingApplicant == null)
            {
                throw new NotFoundException($"Applicant with ID {id} not found");
            }

            // Update the applicant properties
            _mapper.Map(applicantDto, existingApplicant);
            // Preserve the original identifier
            // Do not modify: existingApplicant.UniqueIdentifier
            // Update modification timestamp
            existingApplicant.UpdatedAt = DateTime.UtcNow;

            // Update the applicant in the database
      
            await _applicantRepository.UpdateAsync(existingApplicant);
            await _applicantRepository.SaveChangesAsync();
            // Handle bank details
            if (applicantDto.BankDetails != null)
            {
                // Check if bank details already exist
                var existingBankDetails = await _bankDetailsRepository.GetBankDetailsByApplicantIdAsync(id);

                if (existingBankDetails != null)
                {
                    // Update existing bank details
                    _mapper.Map(applicantDto.BankDetails, existingBankDetails);
                    existingBankDetails.ApplicantId = id;
                    existingBankDetails.UpdatedAt = DateTime.UtcNow;
                    _bankDetailsRepository.UpdateAsync(existingBankDetails);
                }
                else
                {
                    // Create new bank details
                    var bankDetails = _mapper.Map<ApplicantBankDetails>(applicantDto.BankDetails);
                    bankDetails.ApplicantId = id;
                    bankDetails.CreatedAt = DateTime.UtcNow;
                    await _bankDetailsRepository.AddAsync(bankDetails);
                }

                await _bankDetailsRepository.SaveChangesAsync();
            }

            // Handle dependents
            if (applicantDto.Dependents != null)
            {
                // Get existing dependents
                var existingDependents = await _dependentRepository.GetDependentsByApplicantIdAsync(id);

                // Process each dependent in the DTO
                foreach (var dependentDto in applicantDto.Dependents)
                {
                    if (dependentDto.Id > 0 && existingDependents.Any(d => d.Id == dependentDto.Id))
                    {
                        // Update existing dependent
                        var existingDependent = existingDependents.First(d => d.Id == dependentDto.Id);
                        _mapper.Map(dependentDto, existingDependent);
                        existingDependent.ApplicantId = id; // Ensure the applicant ID is set
                        existingDependent.UpdatedAt = DateTime.UtcNow;
                        _dependentRepository.UpdateAsync(existingDependent);
                    }
                    else
                    {
                        // Add new dependent
                        var newDependent = _mapper.Map<ApplicantDependent>(dependentDto);
                        newDependent.ApplicantId = id;
                        newDependent.CreatedAt = DateTime.UtcNow;
                        await _dependentRepository.AddAsync(newDependent);
                    }
                }

                // Remove dependents that are no longer in the list
                var dependentIdsToKeep = applicantDto.Dependents
                    .Where(d => d.Id > 0)
                    .Select(d => d.Id)
                    .ToList();

                var dependentsToRemove = existingDependents
                    .Where(d => !dependentIdsToKeep.Contains(d.Id))
                    .ToList();

                foreach (var dependent in dependentsToRemove)
                {
                    _dependentRepository.DeleteAsync(dependent);
                }

                await _dependentRepository.SaveChangesAsync();
            }

            // Return the updated applicant with all details
            return await GetApplicantByIdAsync(id);
        }
   
        public async Task DeleteApplicantAsync(int id)
        {
            var applicant = await _applicantRepository.GetByIdAsync(id);
            if (applicant == null)
                return;
            var dependentsToRemove = await _dependentRepository.GetDependentsByApplicantIdAsync(id);
            if (dependentsToRemove != null)
            {
                foreach (var dependent in dependentsToRemove)
                {
                    _dependentRepository.DeleteAsync(dependent);
                    
                }
            }
            await _applicantRepository.SaveChangesAsync();
            var bankDetailsToRemove = await _bankDetailsRepository.GetBankDetailsByApplicantIdAsync(id);
            if (bankDetailsToRemove != null)
            {
                _bankDetailsRepository.DeleteAsync(bankDetailsToRemove);
                await _applicantRepository.SaveChangesAsync();
            }
            await _applicantRepository.DeleteAsync(applicant);
            await _applicantRepository.SaveChangesAsync();
        }
    }
}
