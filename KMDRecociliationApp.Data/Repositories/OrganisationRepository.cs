using DocumentFormat.OpenXml.InkML;
using KMDRecociliationApp.Data.Common;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace KMDRecociliationApp.Data.Repositories
{
    public class OrganisationRepository : MainHeaderRepo<Organisation>
    {
        ApplicationDbContext _context = null;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public OrganisationRepository(ILogger<OrganisationRepository> logger, ApplicationDbContext appContext)
            : base(appContext)
        {
            _context = appContext;
            _logger = logger;
        }

        public DataReturn<OrganisationResult> GetOrganisation(DataFilter<OrganisationResult> filter)
        {
            var ret = new DataReturn<OrganisationResult>();
            var objList = new List<Organisation>();
            int numberOfRecords = 0;
            var organisation = _context.Organisations.AsQueryable().AsNoTracking();
                //.Include(x => x.ApplicationUser)
                //.Include(x => x.Organisation).Include(x => x.AssociationContactDetails)
                //.Include(x => x.AssociationBankDetails);
            if (filter.Search != null)
            {
                objList = organisation.Search(filter.Search, "Name").ToList();
            }
            else
            {
                objList = new ObjectQuery<Organisation>().GetAllByFilter(filter.PageNumber, filter.Limit, filter.SortName
                   , filter.SortDirection, filter.Filter == null ? null : filter.Filter.GetDelta()
                   , organisation, "Organisation", out numberOfRecords).ToList();
            }
            ret.Contents = objList.Select(x => new OrganisationResult().CopyPolicyData(x: x)).ToList();
            ret.Source = "Organisation";
            ret.ResultCount = numberOfRecords;
            ret.StatusCode = 200;
            //Paging information
            int numberOfPages = (numberOfRecords / filter.Limit) + ((numberOfRecords % filter.Limit > 0) ? 1 : 0);
            DataPaging paging = new DataPaging();
            paging.RecordsPerPage = filter.Limit;
            paging.PageNumber = filter.PageNumber;
            paging.NumberOfPages = numberOfPages;
            if (filter.PageNumber > 1)
                paging.PreviousPageNumber = filter.PageNumber - 1;
            if (numberOfPages > filter.PageNumber)
                paging.NextPageNumber = filter.PageNumber + 1;
            ret.Paging = paging;
            DataSorting sorting = new DataSorting();
            sorting.SortName = filter.SortName;
            sorting.SortDirection = filter.SortDirection;
            ret.Sorting = sorting;

            return ret;
        }

        public async Task<Organisation?> GetOrganisationByIdAsync(int id)
        {
            return await _context.Organisations
                .FirstOrDefaultAsync(org => org.Id == id);
        }


        //public async Task AddOrganisationAsync(string name, Organisation organisation)
        //{
        //    if (await IsOrganisationDuplicateAsync(organisation))
        //    {
        //        _logger.LogWarning("Attempt to add duplicate organisation: {OrganisationName}", organisation.Name);
        //        throw new InvalidOperationException("An organisation with the same name already exists.");
        //    }
        //    await _context.Organisations.AddAsync(organisation);
        //    await _context.SaveChangesAsync();
        //}

        private async Task<bool> IsOrganisationDuplicateAsync(Organisation organisation, int? excludeId = null)
        {
            return await _context.Organisations
                .AnyAsync(o => o.Name == organisation.Name && (!excludeId.HasValue || o.Id != excludeId.Value));
        }

        public async Task UpdateOrganisationAsync(Organisation organisation)
        {
            _context.Organisations.Update(organisation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteOrganisationAsync(int id)
        {
            var organisation = await _context.Organisations.FindAsync(id);
            if (organisation == null)
                return false;  // Return false if the organisation is not found

            _context.Organisations.Remove(organisation);
            await _context.SaveChangesAsync();
            return true;  // Return true if deletion is successful
        }

        public async Task<bool> CheckOrganisationNameAsync(string organisationName)
        {
            return await _context.Organisations
                .AnyAsync(o => o.Name == organisationName);
        }
        public async Task<Organisation> CheckOrganisationName(OrganisationDTO organisation, bool update = false)
        {
            if (update)
            {
                return await _context.Organisations.AsNoTracking()
                .Where(x => x.Name == organisation.Name
                && x.Id != organisation.Id
                ).FirstOrDefaultAsync();
            }
            else
            {
                return await _context.Organisations.AsNoTracking()
                .Where(x => x.Name == organisation.Name).FirstOrDefaultAsync();
            }
        }

        public async Task<Organisation?> AddOrganisationAsync(OrganisationDTO organisation)
        {
            var _organisation = new Organisation
            {
                Name = organisation.Name,
                Description = organisation.Description
            };

            await _context.Organisations.AddAsync(_organisation);
            await _context.SaveChangesAsync();

            // Return the organisation after saving
            return _organisation;
        }

        public object GetOrganisationById(int organisationid)
        {
            throw new NotImplementedException();
        }
    }
}
