using KMDRecociliationApp.Data.Repositories.BaseRepositories;
using KMDRecociliationApp.Data.Repositories.Interfaces;
using KMDRecociliationApp.Data.Repositories.Interfaces.KMD.EnrolmentPortal.Repositories.Interfaces;
using KMDRecociliationApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Repositories.DataCollectionrepo
{
    public class ApplicantRepository : Repository<ApplicantInsurancePolicy>, IApplicantRepository
    {
        public ApplicantRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ApplicantInsurancePolicy> GetApplicantWithDetailsAsync(int id)
        {
            return await _context.ApplicantInsurancePolicies
                .Include(a => a.BankDetails)
                .Include(a => a.Dependents)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<ApplicantBankDetails> GetBankDetailsByApplicantIdAsync(int id)
        {
            return await _context.ApplicantBankDetails.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<ApplicantDependent> GetDependentDetailsByApplicantIdAsync(int id)
        {
            return await _context.ApplicantDependents.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<ApplicantInsurancePolicy> CheckUnique(string uniqueCode)
        {
            return await _context.ApplicantInsurancePolicies.AsNoTracking()
                   .FirstOrDefaultAsync(a => a.UniqueIdentifier == uniqueCode);
           
        
        }
        public async Task<ApplicantInsurancePolicy> GetApplicantByUniqueIdentifierAsync(string uniqueIdentifier)
        {
            return await _context.ApplicantInsurancePolicies
                .Include(a => a.BankDetails)
                .Include(a => a.Dependents)
                .FirstOrDefaultAsync(a => a.UniqueIdentifier == uniqueIdentifier);
        }

        public async Task<IEnumerable<ApplicantInsurancePolicy>> GetApplicantsByOrganizationAsync(string organization)
        {
            return await _context.ApplicantInsurancePolicies
                .Where(a => a.AssociatedOrganization == organization)
                .ToListAsync();
        }
        public async Task<(IEnumerable<ApplicantInsurancePolicy> applicants, int totalCount)> GetFilteredApplicantsAsync(
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
           int pageSize)
        {
            // Start with a base query
            var query = _context.ApplicantInsurancePolicies              
                .AsQueryable();

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(a =>
                    a.FirstName.ToLower().Contains(searchTerm) ||
                      a.LastName.ToLower().Contains(searchTerm) ||
                    a.IdCardNumber.ToLower().Contains(searchTerm) ||
                    a.UniqueIdentifier.ToLower().Contains(searchTerm) ||
                    a.AssociatedOrganization.ToLower().Contains(searchTerm) ||
                    a.Address.ToLower().Contains(searchTerm));
            }

            // Apply filters if provided
            if (!string.IsNullOrEmpty(organization))
            {
                query = query.Where(a => a.AssociatedOrganization == organization);
            }

            if (!string.IsNullOrEmpty(idCardType))
            {
                int value = Convert.ToInt32((IdCardType)Enum.Parse(typeof(IdCardType), idCardType));
                          
                query = query.Where(a => a.IdCardType ==(IdCardType) value);
            }

            if (minSalary.HasValue)
            {
                query = query.Where(a => a.Salary >= minSalary.Value);
            }

            if (maxSalary.HasValue)
            {
                query = query.Where(a => a.Salary <= maxSalary.Value);
            }

            //if (!string.IsNullOrEmpty(gender))
            //{
            //    query = query.Where(a => a.Gender == gender);
            //}

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.DateOfBirth >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.DateOfBirth <= toDate.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, sortBy, sortDescending);

            // Apply pagination
            var applicants = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (applicants, totalCount);
        }

        private IQueryable<ApplicantInsurancePolicy> ApplySorting(
            IQueryable<ApplicantInsurancePolicy> query,
            string sortBy,
            bool sortDescending)
        {
            // Default sort
            if (string.IsNullOrEmpty(sortBy))
            {
                return sortDescending
                    ? query.OrderByDescending(a => a.Id)
                    : query.OrderBy(a => a.Id);
            }

            // Apply sorting based on property name
            switch (sortBy.ToLower())
            {
                case "fullname":
                    return sortDescending
                        ? query.OrderByDescending(a => a.FullName)
                        : query.OrderBy(a => a.FullName);
                case "dob":
                case "dateofbirth":
                    return sortDescending
                        ? query.OrderByDescending(a => a.DateOfBirth)
                        : query.OrderBy(a => a.DateOfBirth);
                case "organization":
                case "associatedorganization":
                    return sortDescending
                        ? query.OrderByDescending(a => a.AssociatedOrganization)
                        : query.OrderBy(a => a.AssociatedOrganization);
                case "salary":
                    return sortDescending
                        ? query.OrderByDescending(a => a.Salary)
                        : query.OrderBy(a => a.Salary);
                case "idcardnumber":
                    return sortDescending
                        ? query.OrderByDescending(a => a.IdCardNumber)
                        : query.OrderBy(a => a.IdCardNumber);
                case "createdat":
                    return sortDescending
                        ? query.OrderByDescending(a => a.CreatedAt)
                        : query.OrderBy(a => a.CreatedAt);
                default:
                    return sortDescending
                        ? query.OrderByDescending(a => a.Id)
                        : query.OrderBy(a => a.Id);
            }
        }

    }

    public class BankDetailsRepository : Repository<ApplicantBankDetails>, IBankDetailsRepository
    {
        public BankDetailsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ApplicantBankDetails> GetBankDetailsByApplicantIdAsync(int applicantId)
        {
            return await _context.ApplicantBankDetails
                .FirstOrDefaultAsync(b => b.ApplicantId == applicantId);
        }
    }
    public class DependentRepository : Repository<ApplicantDependent>, IDependentRepository
    {
        public DependentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ApplicantDependent>> GetDependentsByApplicantIdAsync(int applicantId)
        {
            return await _context.ApplicantDependents
                .Where(d => d.ApplicantId == applicantId)
                .ToListAsync();
        }
    }
}
