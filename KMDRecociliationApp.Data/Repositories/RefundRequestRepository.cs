using KMDRecociliationApp.Data.Common;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Spreadsheet;
namespace KMDRecociliationApp.Data.Repositories
{
    public class RefundRequestRepository : MainHeaderRepo<RefundRequest>
    {
        ApplicationDbContext _context;
        public RefundRequestRepository(
            ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }
        private int getAssociationID(int id)
        {
            var userdetails = _context.Applicationuser.AsNoTracking()
                        .Where(x => x.Id == id).FirstOrDefault();
            if (userdetails != null
                && userdetails.AssociationId != null)
                return userdetails.AssociationId.Value;
            else
                return 0;
        }
        public DataReturn<RefundRequstResult> GetAll(DataFilter<RefundRequstResult> filter)
        {
            var objList = new List<RefundRequstResult>();
            var ret = new DataReturn<RefundRequstResult>();
            int numberOfRecords = 0;

           

            var query = from r in _context.RefundRequest.AsNoTracking()
                        join u in _context.Applicationuser.AsNoTracking()
                        on r.RetireeId equals u.Id
                        join association in _context.Association.AsNoTracking()
                         on u.AssociationId equals association.Id
                        join p in _context.PolicyHeader on r.PolicyId equals p.Id
                        where  p.CampaignId == filter.CampaignId
                        select new RefundRequstResult
                        {
                            OrderNumber = r.PolicyId,
                            AssociationId= u.AssociationId,
                            AssociationName=association.AssociationName,
                            //RetireeId=r.RetireeId,
                            RefundAmount = r.RefundAmount,
                            RefundRequestNumber = r.Id,
                            RefundRequestDate = r.RefundRequestDate,
                            Status = r.Status.ToString(),
                            RetireeName = string.Concat(u.FirstName, " ", u.LastName),
                            MobileNumber=u.MobileNumber,
                        };

            if (filter.AssociationId > 0)
            {
                var id = getAssociationID(filter.AssociationId);
                query=query.Where(a => a.AssociationId == id);
            }
            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.AssociationName.ToLower().Contains(searchTerm) ||
                    x.RetireeName.ToLower().Contains(searchTerm) ||
                    x.MobileNumber.Contains(searchTerm));
            }

            // Get total count before pagination
             numberOfRecords = query.Count();
            //).ToList();

            
            var paginatedResults = filter.Limit > 0
         ? query.Skip((filter.PageNumber - 1) * filter.Limit)
               .Take(filter.Limit)
               .ToList()
         : query.ToList();

            ret.Contents = paginatedResults; 
            ret.Source = "RefundRequst";
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

        public async Task<DTORefundRequest> GetByID(int? id)
        {
            DTORefundRequest refundRequest = new DTORefundRequest();
            var refund = await _context.RefundRequest
                .AsNoTracking()
                .Include(x=>x.User)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (refund != null)
            {
                refundRequest.OrderNumber = refund.PolicyId;
                if (refund.User != null)
                    refundRequest.RetireeName = refund.User.FullName;
                refundRequest.RefundRequestDate = refund.RefundRequestDate;
                refundRequest.RefundAmount = refund.RefundAmount;
                refundRequest.RefundRequestNumber = refund.Id;
                refundRequest.RefundRequestStatus = refund.Status.ToString();

            }

            return refundRequest;
        }
    }
}
