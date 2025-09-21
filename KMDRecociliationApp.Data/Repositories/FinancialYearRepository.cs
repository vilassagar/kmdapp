using KMDRecociliationApp.Data.Common;
using KMDRecociliationApp.Data.Helpers;
using KMDRecociliationApp.Domain.Common;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Repositories
{
    public class FinancialYearRepository : MainHeaderRepo<FinancialYear>
    {
        ApplicationDbContext context = null;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public FinancialYearRepository(ILoggerFactory logger, ApplicationDbContext appContext) : base(appContext)
        {
            context = appContext;
            _logger = logger.CreateLogger("FinancialYearRepository");
        }
        
        public async Task<FinancialYear> CheckFinancialYearLabel(DTOFinancialYear financialYear, bool update = false)
        {
            if (update)
            {
                return await context.FinancialYear.AsNoTracking()
                .Where(x => x.FinancialYearLabel == financialYear.FinancialYearLabel
                && x.Id != financialYear.Id
                ).FirstOrDefaultAsync();
            }
            else
            {
                return await context.FinancialYear.AsNoTracking()
                .Where(x => x.FinancialYearLabel == financialYear.FinancialYearLabel).FirstOrDefaultAsync();
            }
        }

        public async Task<FinancialYear?> CreateFinancialYearAsync(DTOFinancialYear dtoFinancialYear, int updatedBy)
        {
            

            FinancialYear tempFinancialYear = new FinancialYear();
            tempFinancialYear.IsActive = true;
            tempFinancialYear.FinancialYearLabel = dtoFinancialYear.FinancialYearLabel;
            tempFinancialYear.StartDate = dtoFinancialYear.StartDate;
            tempFinancialYear.EndDate = dtoFinancialYear.EndDate;
            tempFinancialYear.CreatedBy = updatedBy;
            tempFinancialYear.CreatedAt = DateTime.Now;
            tempFinancialYear.UpdatedBy = updatedBy;
            tempFinancialYear.UpdatedAt = DateTime.Now;

            context.FinancialYear.Add(tempFinancialYear);
            await context.SaveChangesAsync();

            // Ensure tempAssociation.Id is not 0 and was inserted successfully
            if (tempFinancialYear.Id == 0)
            {
                throw new Exception("Association add failed.");
            }
            else
            {
                return tempFinancialYear;
            }
            
        }

        public DataReturn<FinancialYearResult> GetFinancialYears(DataFilter<FinancialYearResult> filter)
         //public DataReturn<OrganisationResult> GetOrganisation(DataFilter<OrganisationResult> filter)
        {
            var ret = new DataReturn<FinancialYearResult>();
            var objList = new List<FinancialYear>();
            int numberOfRecords = 0;
            var financialYears = context.FinancialYear.AsQueryable().AsNoTracking();
            if (filter.Search != null)
            {
                objList = financialYears.Search(filter.Search, "FinancialYearLabel").ToList();
            }
            else
            {
                objList = new ObjectQuery<FinancialYear>().GetAllByFilter(filter.PageNumber, filter.Limit, filter.SortName
                   , filter.SortDirection, filter.Filter == null ? null : filter.Filter.GetDelta()
                   , financialYears, "FiniancialYear", out numberOfRecords).ToList();
            }
            ret.Contents = objList.Select(x => new FinancialYearResult().Copy(x : x)).ToList();
            ret.Source = "FiniancialYear";
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

        public async Task<FinancialYear?> GetFinancialYearByIdAsync(int Id)
        {
            return await context.FinancialYear
                .FirstOrDefaultAsync(fy => fy.Id == Id);
        }

        public async Task<FinancialYear?> UpdateFinancialYearAsync(int finiancialYearId, DTOFinancialYear dtoFinancialYear, int updatedBy)
        {
            FinancialYear tempFinancialYear = new FinancialYear();
            var financialYearobj = context.FinancialYear.AsNoTracking()
                .Where(x => x.Id == finiancialYearId).FirstOrDefault();
            if (financialYearobj != null)
            {
                tempFinancialYear = financialYearobj;
            }

            tempFinancialYear.IsCurrent = dtoFinancialYear.IsCurrent;
            tempFinancialYear.FinancialYearLabel = dtoFinancialYear.FinancialYearLabel;
            tempFinancialYear.StartDate = dtoFinancialYear.StartDate;
            tempFinancialYear.EndDate = dtoFinancialYear.EndDate;

            tempFinancialYear.UpdatedBy = updatedBy;
            tempFinancialYear.UpdatedAt = DateTime.Now;

            context.FinancialYear.Update(tempFinancialYear);
            await context.SaveChangesAsync();
            
            
            return tempFinancialYear;
        }

        public async Task<bool> DeleteFinancialYearAsync(int id)
         //public  Task<bool> DeleteOrganisationAsync(int id)
        {
            var financialYear = await context.FinancialYear.FindAsync(id);
            if (financialYear == null)
                return false;  // Return false if the organisation is not found

            context.FinancialYear.Remove(financialYear);
            await context.SaveChangesAsync();
            return true;  // Return true if deletion is successful
        }

    }
}
