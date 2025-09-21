using KMDRecociliationApp.Data.Helpers;
using KMDRecociliationApp.Data.Mapper;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.DTO.Dashboard;
using KMDRecociliationApp.Domain.Enum;
using KMDRecociliationApp.Domain.ReportDataModels;
using KMDRecociliationApp.Domain.ReportParamModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SqlHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Repositories
{
    public class DashboardRepository
    {
        private readonly IConfiguration _configuration;
        private IMsSqlHelper msSqlHelper;
        ApplicationDbContext context = null;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;


        public DashboardRepository(ILoggerFactory logger, ApplicationDbContext appContext, IConfiguration configuration)
        {
            context = appContext;
            _logger = logger.CreateLogger("Dashboard Repository");
            _configuration = configuration;
            msSqlHelper = new MsSqlHelper(_configuration.GetConnectionString("constr"));
        }
        private int getAssociationID(int id)
        {
            var userdetails = context.Applicationuser.AsNoTracking()
                        .Where(x => x.Id == id).FirstOrDefault();
            if (userdetails != null
                && userdetails.AssociationId != null)
                return userdetails.AssociationId.Value;
            else
                return 0;
        }
        public async Task<List<DTOOfflinePaymentsStatus>> GetOfflinePayments(int associationId)
        {
            if (associationId == 0)
            {
                return await context.PaymentDetails.AsNoTracking()
                            .GroupBy(p => p.PaymentType)
                             .Select(g => new DTOOfflinePaymentsStatus
                             {
                                 Status = g.Key.ToString(),
                                 Count = g.Count()
                             })
                            .ToListAsync();
            }
            else
            {
                var id = getAssociationID(associationId);
                return await (from p in context.PaymentDetails.AsNoTracking()
                              join u in context.Applicationuser.AsNoTracking()
                              on p.UserId equals u.Id
                              where u.AssociationId == id
                              group p by new { p.PaymentType } into g
                              select new DTOOfflinePaymentsStatus
                              {
                                  Status = Convert.ToString(g.Key.PaymentType),
                                  Count = g.Count()
                              }).ToListAsync();

            }
        }

        public async Task<List<DTOOfflinePaymentsStatus>> getCompletedpayments(int associationId)
        {
            if (associationId == 0)
            {
                return await context.PaymentDetails.AsNoTracking()
                            .Where(x => x.PaymentStatus == PaymentStatus.Completed)
                            .GroupBy(p => p.PaymentType)
                             .Select(g => new DTOOfflinePaymentsStatus
                             {
                                 Status = g.Key.ToString(),
                                 Count = g.Count()
                             })
                            .ToListAsync();
            }
            else
            {
                var id = getAssociationID(associationId);
                return await (from p in context.PaymentDetails.AsNoTracking()
                              join u in context.Applicationuser.AsNoTracking()
                              on p.UserId equals u.Id
                              where u.AssociationId == id && p.PaymentStatus == PaymentStatus.Completed
                              group p by new { p.PaymentType } into g
                              select new DTOOfflinePaymentsStatus
                              {
                                  Status = Convert.ToString(g.Key.PaymentType),
                                  Count = g.Count()
                              }).ToListAsync();

            }
        }

        public async Task<List<DTOPaymentsModes>> getPaymentModes(int associationId)
        {
            List<DTOPaymentsModes> paymentsModes = new List<DTOPaymentsModes>();
            if (associationId == 0)
            {
                paymentsModes = await context.PaymentDetails.AsNoTracking()
                            .GroupBy(p => p.PaymentMode)
                             .Select(g => new DTOPaymentsModes
                             {
                                 Mode = g.Key.ToString(),
                                 Count = g.Count()
                             })
                            .ToListAsync();
            }
            else
            {
                var id = getAssociationID(associationId);
                paymentsModes = await (from p in context.PaymentDetails.AsNoTracking()
                                       join u in context.Applicationuser.AsNoTracking()
                                       on p.UserId equals u.Id
                                       where u.AssociationId == id
                                       && u.UserType == UserType.Pensioner
                                       group p by new { p.PaymentMode } into g
                                       select new DTOPaymentsModes
                                       {
                                           Mode = Convert.ToString(g.Key.PaymentMode),
                                           Count = g.Count()
                                       }).ToListAsync();

            }
            var onine = paymentsModes.Where(x => x.Mode == PaymentMode.Online.ToString());
            if (!onine.Any())
            {
                paymentsModes.Add(new DTOPaymentsModes
                { Mode = PaymentMode.Online.ToString(), Count = 0 });
            }
            var offline = paymentsModes.Where(x => x.Mode == PaymentMode.Offline.ToString());
            if (!onine.Any())
            {
                paymentsModes.Add(new DTOPaymentsModes
                { Mode = PaymentMode.Offline.ToString(), Count = 0 });
            }
            return paymentsModes;

        }

        public async Task<List<DTOUserCount>> getUserCount(int associationId)
        {
            List<DTOUserCount> payments = new List<DTOUserCount>();
            var usercount = 0;
            if (associationId == 0)
            {
                payments = await context.PolicyHeader.AsNoTracking()
                            .GroupBy(p => p.PaymentStatus)
                             .Select(g => new DTOUserCount
                             {
                                 Name = Convert.ToString(g.Key),
                                 Count = g.Count()
                             })
                            .ToListAsync();
                usercount = await context.Applicationuser.AsNoTracking()
               .Where(u => u.UserType == UserType.Pensioner)
               .CountAsync();
            }
            else
            {
                var id = getAssociationID(associationId);
                payments = await (from p in context.PolicyHeader.AsNoTracking()
                                  join u in context.Applicationuser.AsNoTracking()
                                  on p.UserId equals u.Id
                                  where u.AssociationId == id
                                  && u.UserType == UserType.Pensioner
                                  group p by new { p.PaymentStatus } into g
                                  select new DTOUserCount
                                  {
                                      Name = (g.Key.PaymentStatus.HasValue
                                                ? (PaymentStatus)g.Key.PaymentStatus
                                                : PaymentStatus.Unknown).ToString(),
                                      Count = g.Count()
                                  }).ToListAsync();

                usercount = await context.Applicationuser.AsNoTracking()
                  .Where(u => u.UserType == UserType.Pensioner &&
                  u.AssociationId == id)
                  .CountAsync();
            }

            // Update names where Count is 0
            //payments
            //    .ForEach(user =>
            //    {
            //        if (Convert.ToInt32(user.Name) == (int)PaymentStatus.Pending)
            //            user.Name = PaymentStatus.Pending.ToString();
            //        else if (Convert.ToInt32(user.Name) == (int)PaymentStatus.Rejected)
            //            user.Name = PaymentStatus.Rejected.ToString();
            //        else if (Convert.ToInt32(user.Name) == (int)PaymentStatus.Completed)
            //            user.Name = PaymentStatus.Completed.ToString();
            //        else if (Convert.ToInt32(user.Name) == (int)PaymentStatus.Initiated)
            //            user.Name = PaymentStatus.Initiated.ToString();
            //        else if (Convert.ToInt32(user.Name) == (int)PaymentStatus.Failed)
            //            user.Name = PaymentStatus.Failed.ToString();
            //    });

            payments.Add(new DTOUserCount() { Name = "TotalUser", Count = usercount });

            return payments;

        }


        public async Task<List<DTOCampaignsCount>> getCampaignsCount(int associationId)
        {
            return new List<DTOCampaignsCount>();
        }

        public async Task<DataReturn<DTOAssociationPaymentStatus>>
            GetAssociationWisePaymentStatus(DataFilter<DTOAssociationPaymentStatus> filter)
        {
            var ret = new DataReturn<DTOAssociationPaymentStatus>();
            int numberOfRecords = 0;
            try
            {
                var paymnetsObj = new List<DTOAssociationPaymentStatus>();
                var paymnetsObj1 = new List<DTOAssociationPaymentStatus>();
                if (filter.AssociationId > 0)
                {
                    var id = getAssociationID(filter.AssociationId);
                    paymnetsObj = await (from p in context.PaymentDetails.AsNoTracking()
                                         join u in context.Applicationuser.AsNoTracking()
                                         on p.UserId equals u.Id
                                         join a in context.Association.AsNoTracking()
                                         on u.AssociationId equals a.Id
                                         where u.AssociationId == id
                                         && u.UserType == UserType.Pensioner
                                         group p by new { a.AssociationName, p.PaymentStatus } into g
                                         select new DTOAssociationPaymentStatus
                                         {
                                             AssociationName = g.Key.AssociationName,
                                             PaymentStatus = (PaymentStatus)g.Key.PaymentStatus,
                                             TotalAmountPaid = g.Sum(x => x.AmountPaid)

                                         }).ToListAsync();
                }
                else
                {
                    paymnetsObj = await (from p in context.PaymentDetails.AsNoTracking()
                                         join u in context.Applicationuser.AsNoTracking()
                                         on p.UserId equals u.Id
                                         join a in context.Association.AsNoTracking()
                                         on u.AssociationId equals a.Id
                                         where u.UserType == UserType.Pensioner
                                         group p by new { a.AssociationName, p.PaymentStatus } into g
                                         select new DTOAssociationPaymentStatus
                                         {
                                             AssociationName = g.Key.AssociationName,
                                             PaymentStatus = g.Key.PaymentStatus.HasValue
                                                     ? (PaymentStatus)g.Key.PaymentStatus
                                                     : PaymentStatus.Unknown, // You
                                             //(PaymentStatus)g.Key.PaymentStatus,
                                             TotalAmountPaid = g.Sum(x => x.AmountPaid)

                                         }).ToListAsync();

                }
                foreach (var o in paymnetsObj)
                {

                    if (paymnetsObj1.Any(x => x.AssociationName == o.AssociationName))
                    {
                        var obj = paymnetsObj1.
                             Where(x => x.AssociationName == o.AssociationName).FirstOrDefault();
                        if (obj != null)
                        {

                            if (o.PaymentStatus == PaymentStatus.Completed)
                                obj.CompletedPayment = o.TotalAmountPaid;
                            else if (o.PaymentStatus == PaymentStatus.Initiated)
                                obj.InitiatedPayment = o.TotalAmountPaid;
                            else if (o.PaymentStatus == PaymentStatus.Failed)
                                obj.FailedPayment = o.TotalAmountPaid;
                            else if (o.PaymentStatus == PaymentStatus.Rejected)
                                obj.RejectedPayment = o.TotalAmountPaid;
                        }
                    }
                    else
                    {
                        if (o.PaymentStatus == PaymentStatus.Completed)
                            o.CompletedPayment = o.TotalAmountPaid;
                        else if (o.PaymentStatus == PaymentStatus.Initiated)
                            o.InitiatedPayment = o.TotalAmountPaid;
                        else if (o.PaymentStatus == PaymentStatus.Failed)
                            o.FailedPayment = o.TotalAmountPaid;
                        else if (o.PaymentStatus == PaymentStatus.Rejected)
                            o.RejectedPayment = o.TotalAmountPaid;
                        paymnetsObj1.Add(o);
                    }



                }

                ret.Contents = paymnetsObj1;
                ret.Source = "payments";
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
            }
            catch (Exception ex)
            {

            }
            return ret;
        }

        public async Task<DashboardDataDataModel> getDashboardDataAsync(DashboardDataParamModel dashboardDataParamModel)
        {
            if (dashboardDataParamModel?.campaignId == null || dashboardDataParamModel?.associationId == null)
            {
                throw new ArgumentException("CampaignId and AssociationId cannot be null.");
            }
            if (dashboardDataParamModel.associationId == -1)
            {
                dashboardDataParamModel.associationId = 0;
            }
            IList<SqlParameter> parameters = new List<SqlParameter>
            {
                dashboardDataParamModel.campaignId.CreateSqlParameter("@CampaignId"),
                dashboardDataParamModel.associationId.CreateSqlParameter("@AssociationId")
            };

            DashboardDataDataModel ret = new DashboardDataDataModel();

            try
            {
                var result = await Task.Run(() =>
                    msSqlHelper.ExecuteDataSet<SqlParameter>(CommandType.StoredProcedure,
                        DBConstant.USPGETDASHBOARDDATA, parameters));

                if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    var row = result.Tables[0].Rows[0];
                    ret.totalPensioner = row["TotalPensioner"] != DBNull.Value ? Convert.ToInt32(row["TotalPensioner"]) : 0;
                    ret.totalPensionerStarted = row["totalPensionerStarted"] != DBNull.Value ? Convert.ToInt32(row["totalPensionerStarted"]) : 0;
                    ret.totalPensionerNotStarted = row["totalPensionerNotStarted"] != DBNull.Value ? Convert.ToInt32(row["totalPensionerNotStarted"]) : 0;
                    ret.totalPendingRejected = row["TotalPendingRejectedCount"] != DBNull.Value ? Convert.ToInt32(row["TotalPendingRejectedCount"]) : 0;
                    ret.totalPending = row["TotalPendingCount"] != DBNull.Value ? Convert.ToInt32(row["TotalPendingCount"]) : 0;
                    ret.totalRejected = row["TotalRejectedCount"] != DBNull.Value ? Convert.ToInt32(row["TotalRejectedCount"]) : 0;
                    ret.totalInitiated = row["totalInitiatedCount"] != DBNull.Value ? Convert.ToInt32(row["totalInitiatedCount"]) : 0;
                    ret.totalInitiatedCheque = row["totalInitiatedCheque"] != DBNull.Value ? Convert.ToInt32(row["totalInitiatedCheque"]) : 0;
                    ret.totalInitiatedNEFT = row["totalInitiatedNEFT"] != DBNull.Value ? Convert.ToInt32(row["totalInitiatedNEFT"]) : 0;
                    ret.totalCompleted = row["totalCompletedCount"] != DBNull.Value ? Convert.ToInt32(row["totalCompletedCount"]) : 0;
                    ret.totalCompletedOnline = row["totalCompletedOnline"] != DBNull.Value ? Convert.ToInt32(row["totalCompletedOnline"]) : 0;
                    ret.totalCompletedCheque = row["totalCompletedCheque"] != DBNull.Value ? Convert.ToInt32(row["totalCompletedCheque"]) : 0;
                    ret.totalCompletedNEFT = row["totalCompletedNEFT"] != DBNull.Value ? Convert.ToInt32(row["totalCompletedNEFT"]) : 0;
                }
            }
            catch (Exception ex)
            {
                // Use a logging framework
                Console.WriteLine(ex.Message); // Replace with proper logging
                throw; // Optionally rethrow or handle
            }

            return ret;
        }

        public FormatedDashboardDataDataModel ConvertToFormattedModel(DashboardDataDataModel dashboardData)
        {
            var formattedModel = new FormatedDashboardDataDataModel
            {
                Pensioner = new List<NameCountCommonModel>
                {
                    //new NameCountCommonModel { Name = "Total Pensioner", Count = dashboardData.totalPensioner },
                    new NameCountCommonModel { Name = "Pensioner Started", Count = dashboardData.totalPensionerStarted },
                    new NameCountCommonModel { Name = "Pensioner Not Started", Count = dashboardData.totalPensionerNotStarted }
                },
                PendingRejected = new List<NameCountCommonModel>
                {
                    //new NameCountCommonModel { Name = "Total Pending Rejected", Count = dashboardData.totalPendingRejected },
                    new NameCountCommonModel { Name = "Total Pending", Count = dashboardData.totalPending },
                    new NameCountCommonModel { Name = "Total Rejected", Count = dashboardData.totalRejected }
                },
                Initiated = new List<NameCountCommonModel>
                {
                    //new NameCountCommonModel { Name = "Total Initiated", Count = dashboardData.totalInitiated },
                    new NameCountCommonModel { Name = "Total Initiated Cheque", Count = dashboardData.totalInitiatedCheque },
                    new NameCountCommonModel { Name = "Total Initiated NEFT", Count = dashboardData.totalInitiatedNEFT }
                },
                Completed = new List<NameCountCommonModel>
                {
                    //new NameCountCommonModel { Name = "Total Completed", Count = dashboardData.totalCompleted },
                    new NameCountCommonModel { Name = "Total Completed Online", Count = dashboardData.totalCompletedOnline },
                    new NameCountCommonModel { Name = "Total Completed Cheque", Count = dashboardData.totalCompletedCheque },
                    new NameCountCommonModel { Name = "Total Completed NEFT", Count = dashboardData.totalCompletedNEFT }
                }
            };

            return formattedModel;
        }


        public DataReturn<DTOOfflinePayments> getDashboardDataDetailsAsync(DataFilter<DTOOfflinePayments> filter
           )
        {
            var ret = new DataReturn<DTOOfflinePayments>();

            int numberOfRecords = 0;
            var paymnets = new List<DTOOfflinePayments>();
            int? paymentMode = null;
            int? paymentStatus = null;
            int? paymentType = null;
            filter.BaseFilter = filter.BaseFilter.Replace(" ", "").ToLower();

            if (filter.BaseFilter != null && filter.BaseFilter.Contains("initiated") && filter.BaseFilter.ToLower().Contains("Cheque"))
            {
                paymentMode = 2;
                paymentStatus = 3;
                paymentType = 1;
            }
            else if (filter.BaseFilter != null && filter.BaseFilter.Contains("initiated") && filter.BaseFilter.ToLower().Contains("neft"))
            {
                paymentMode = 2;
                paymentStatus = 3;
                paymentType = 2;
            }
            if (filter.BaseFilter != null && filter.BaseFilter.Contains("initiated") && filter.BaseFilter.ToLower().Contains("upi"))
            {
                paymentMode = 2;
                paymentStatus = 3;
                paymentType = 2;
            }

            //completed
            else if (filter.BaseFilter != null && filter.BaseFilter.Contains("completed") && filter.BaseFilter.ToLower().Contains("Cheque"))
            {
                paymentMode = 2;
                paymentStatus = 1;
                paymentType = 1;
            }
            else if (filter.BaseFilter != null && filter.BaseFilter.Contains("completed") && filter.BaseFilter.ToLower().Contains("neft"))
            {
                paymentMode = 2;
                paymentStatus = 1;
                paymentType = 2;
            }
            else if (filter.BaseFilter != null && filter.BaseFilter.Contains("completed") && filter.BaseFilter.ToLower().Contains("upi"))
            {
                paymentMode = 2;
                paymentStatus = 1;
                paymentType = 3;
            }
            else if (filter.BaseFilter != null && filter.BaseFilter.Contains("completed") && filter.BaseFilter.ToLower().Contains("online"))
            {
                paymentMode = 1;
                paymentStatus = 1;
                paymentType = 4;
            }
            //pending 
            else if (filter.BaseFilter != null && filter.BaseFilter.Contains("pending"))
            {
                paymentMode = null;
                paymentStatus = 0;
                paymentType = null;
            }
            //rejected 
            else if (filter.BaseFilter != null && filter.BaseFilter.Contains("rejected"))
            {
                paymentMode = null;
                paymentStatus = 2;
                paymentType = null;
            }

            
            IList<SqlParameter> parameters =
            [
                filter.PageNumber.CreateSqlParameter("@Page"),
                filter.Limit.CreateSqlParameter("@PageSize"),
                filter.CampaignId.CreateSqlParameter("@CampaignId"),
               // filter.AssociationId.CreateSqlParameter("@AssociationId"),
            ];

            if (filter.AssociationId>0)
                parameters.Add(filter.AssociationId.CreateSqlParameter("@AssociationId"));

            if (!string.IsNullOrWhiteSpace(filter.Search))
                parameters.Add(filter.Search.CreateSqlParameter("@SearchValue"));
            if (!string.IsNullOrWhiteSpace(filter.BaseFilter))
                parameters.Add(filter.BaseFilter.CreateSqlParameter("@Filter"));

            if (paymentMode != null)
                parameters.Add(paymentMode.CreateSqlParameter("@PaymentMode"));
            if (paymentStatus != null)
                parameters.Add(paymentStatus.CreateSqlParameter("@PaymentStatus"));
            if (paymentType != null)
                parameters.Add(paymentType.CreateSqlParameter("@PaymentType"));


            //var ret = new List<DTOPensioneer>();

            try
            {
                // Execute stored procedure and retrieve dataset
                var result = msSqlHelper.ExecuteDataSet<SqlParameter>(
                CommandType.StoredProcedure,
                DBConstant.USPGETDASHBOARDDATADETAILS,
                parameters
                );

                // Assuming result is a DataSet, process each DataRow in the first DataTable
                 if (result != null && result.Tables.Count > 0)
                {
                    var dataTable = result.Tables[0];
                    foreach (DataRow row in dataTable.Rows)
                    {
                        var dto = new DTOOfflinePayments
                        {
                         
                            PaymentId = row["PaymentId"] != DBNull.Value ? Convert.ToInt32(row["PaymentId"]) : 0,
                            RetireeName = row["RetireeName"]?.ToString(),
                            MobileNumber = row["MobileNumber"]?.ToString(),
                            Amount = row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"]) : (decimal?)null,
                            Date = row["Date"] != DBNull.Value ? Convert.ToDateTime(row["Date"]) : (DateTime?)null,
                            AssociationName = row["AssociationName"]?.ToString(),
                            status =((PaymentStatus)Convert.ToInt32(row["status"]?.ToString())).ToString(),
                            PaymentMode =((PaymentTypes) Convert.ToInt32(row["paymenttype"]?.ToString())).ToString()
                             
                        };
                        numberOfRecords = row["TotalCount"] != DBNull.Value ? Convert.ToInt32(row["TotalCount"]) : 0;
                        paymnets.Add(dto);

                    }

                }
            }
            catch (Exception ex)
            {
                // Use a logging framework
                Console.WriteLine(ex.Message); // Replace with proper logging
                throw; // Optionally rethrow or handle
            }
            //numberOfRecords = paymnets.Count();
            //paymnets = filter.Limit == 0 ? paymnets.ToList() : paymnets.Skip((filter.PageNumber - 1) * filter.Limit).Take(filter.Limit).ToList();

            ret.Contents = paymnets;// objList.Select(x => new UserlistResult().Copy(x: x)).ToList();
            ret.Source = "Offline payments";
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
    }
}
