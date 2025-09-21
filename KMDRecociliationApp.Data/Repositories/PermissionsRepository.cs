
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
    public class PermissionsRepository : MainHeaderRepo<ApplicationPermission>
    //, ISearchRepoBase<ApplicationPermission>
    {
        ApplicationDbContext _context;
        public PermissionsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<DataReturn<PermissionResult>> GetAll(DataFilter<PermissionResult> filter)
        {
            var ret = new DataReturn<PermissionResult>();
            var objList = new List<ApplicationPermission>();
           int numberOfRecords = 0;
            var permissions = _context.ApplicationPermission.AsQueryable().AsNoTracking();
            if (filter.Search != null)
            {
                objList = objList = permissions.Search(filter.Search, "PermissionType", "Description").ToList();
            }
            else
            {

                objList = new ObjectQuery<ApplicationPermission>().GetAllByFilter(filter.PageNumber, filter.Limit
                    , filter.SortName, filter.SortDirection,
                   filter.Filter == null ? null
                   : filter.Filter.GetDelta()
                   , permissions, "ApplicationPermission", out numberOfRecords).ToList();
            }
            ret.Contents = objList.Select(x => new PermissionResult().CopyPolicyData(x: x)).ToList();
            //ret.Contents = context.Permissions.Select(x => new PermissionResult().Copy(x)).Skip((filter.PageNumber - 1) * filter.Limit).Take(filter.Limit).ToList();
            ret.Source = "ApplicationPermission";
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

        public async Task<ApplicationPermissionDTO> GetByID(int? id)
        {
            var permission = await _context.ApplicationPermission.AsNoTracking()
                .Where(x => x.Id == id).FirstOrDefaultAsync()!;
            if (permission != null)
                return new ApplicationPermissionDTO().Copy(permission);
            else
                return await Task.FromResult(new ApplicationPermissionDTO());

        }
        public async Task<ApplicationPermission> GetPermissionByID(int? id)
        {
          var permission= await _context.ApplicationPermission.AsNoTracking()
                .Where(x => x.Id == id).FirstOrDefaultAsync()!;
            if (permission != null)
                return permission;
            else
                return await Task.FromResult(new ApplicationPermission());
        }
        public async Task<bool> CheckPermission(ApplicationPermissionDTO permission, bool update = false)
        {
            if (update == false)
            {
                return await _context.ApplicationPermission.AsNoTracking()
                     .AnyAsync(x => x.PermissionType == permission.type
                     //&& x.AccessType == permission.AccessType
                     && x.Description == permission.name
                     && x.IsActive == true);
            }
            else
            {
                return await _context.ApplicationPermission.AsNoTracking()
                   .AnyAsync(x => x.PermissionType == permission.type
                   // && x.AccessType == permission.AccessType
                    && x.Description == permission.name
                     && x.Id != permission.id
                    && x.IsActive == true);
            }

        }
    }
}
