
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Results;
using KMDRecociliationApp.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security;
namespace KMDRecociliationApp.Data.Repositories
{
    public class AuditRepository : MainHeaderRepo<Audit>
    {
        ApplicationDbContext _context=null;

        public AuditRepository(ApplicationDbContext appContext) : base(appContext)
        {
            _context = appContext;

        }

        public IEnumerable<Audit> GetAll()
        {
            return _context.AuditTrail.ToList();
        }
        public async Task<DataReturn<AuditLogResult>> GetAll(DataFilter<AuditLogResult> filter)
        {
            var ret = new DataReturn<AuditLogResult>();
            int numberOfRecords = 0;
            var objList = new ObjectQuery<Audit>().GetAllByFilter(filter.PageNumber, filter.Limit, filter.SortName, filter.SortDirection
                , filter: filter.Filter == null ? null : filter.Filter.GetDelta()
                , _context.AuditTrail.AsQueryable().AsNoTracking(), "Audit"
                , out numberOfRecords).ToList();
            ret.Contents = objList.Select(x => new AuditLogResult().Copy(x: x)).ToList();
            ret.Source = "Audit";
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
        public async Task<AuditLogResult> GetByID(int id)
        {
            try
            {
                var obj = await _context.AuditTrail
                    .Where(x => x.Auditid == id).AsNoTracking().FirstOrDefaultAsync();
                if (obj != null)
                    return new AuditLogResult().Copy(obj);
                else
                    return new AuditLogResult();
            }
            catch (Exception ex)
            {
                return new AuditLogResult();
            }
        }

        public async Task<List<AuditLogResult>> GetAuditLogsByEmail(string email)
        {
            try
            {
                var objList = await _context.AuditTrail.AsNoTracking()
                    .Where(x => x.Emailid.ToLower() == email.ToLower()).ToListAsync();


                if (objList != null && objList.Any())
                {
                    return objList.Select(x => new AuditLogResult().Copy(x: x)).ToList();
                }
                else
                    return new List<AuditLogResult>();
            }
            catch (Exception ex)
            {
                return new List<AuditLogResult>();
            }
        }


       
    }
}
