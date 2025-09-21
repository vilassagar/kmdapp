using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.IO.Util;
using KMDRecociliationApp.Data.Common;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using KMDRecociliationApp.Domain.PaymentDTO;
using KMDRecociliationApp.Domain.Results;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KMDRecociliationApp.Data.Repositories
{
    public class PaymentRepository : MainHeaderRepo<PaymentHeader>
    {
        ApplicationDbContext _context;
        public PaymentRepository(
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
        public DataReturn<DTOOfflinePayments> GetAllOfflinePayments(DataFilter<DTOOfflinePayments> filter)
        {
            var ret = new DataReturn<DTOOfflinePayments>();

            int numberOfRecords = 0;
            var paymnets = new List<DTOOfflinePayments>();
            if (filter.AssociationId > 0)
            {
                var id = getAssociationID(filter.AssociationId);
                paymnets = (from pay in _context.PaymentDetails.AsNoTracking()
                            join user in _context.Applicationuser.AsNoTracking()
                              on pay.UserId equals user.Id
                            join association in _context.Association.AsNoTracking()
                            on user.AssociationId equals association.Id
                            where user.AssociationId == id && pay.CampaignId == filter.CampaignId
                            select new DTOOfflinePayments
                            {
                                PaymentId = pay.Id,
                                RetireeName = user.FirstName + "" + user.LastName,
                                Amount = pay.AmountPaid,
                                Date = pay.PaymentDate,
                                AssociationName = association.AssociationName,
                                status = pay.PaymentStatus.ToString(),
                                PaymentMode = pay.PaymentType.ToString()

                            }).ToList();

            }
            else
            {
                if (filter.Search != null)
                {
                    paymnets = (from pay in _context.PaymentDetails.AsNoTracking()
                                join user in _context.Applicationuser.AsNoTracking()
                                  on pay.UserId equals user.Id
                                join association in _context.Association.AsNoTracking()
                                on user.AssociationId equals association.Id
                                where pay.CampaignId == filter.CampaignId && (
                                //pay.PaymentMode==PaymentMode.Offline &&  
                                association.AssociationName.Contains(filter.Search) ||
                                user.FirstName.Contains(filter.Search) ||
                                user.LastName.Contains(filter.Search) ||
                                user.MobileNumber.Contains(filter.Search))
                                select new DTOOfflinePayments
                                {
                                    PaymentId = pay.Id,
                                    RetireeName = user.FirstName + "" + user.LastName,
                                    MobileNumber = user.MobileNumber,
                                    Amount = pay.AmountPaid,
                                    Date = pay.PaymentDate,
                                    AssociationName = association.AssociationName,
                                    status = pay.PaymentStatus.ToString(),
                                    PaymentMode = pay.PaymentType.ToString()

                                }).ToList();
                    numberOfRecords = paymnets.Count();
                    paymnets = filter.Limit == 0 ? paymnets.ToList() : paymnets.Skip((filter.PageNumber - 1) * filter.Limit).Take(filter.Limit).ToList();

                }
                else
                {
                    paymnets = (from pay in _context.PaymentDetails.AsNoTracking()
                                join user in _context.Applicationuser.AsNoTracking()
                                  on pay.UserId equals user.Id
                                join association in _context.Association.AsNoTracking()
                                on user.AssociationId equals association.Id
                                where pay.CampaignId == filter.CampaignId
                                select new DTOOfflinePayments
                                {
                                    PaymentId = pay.Id,
                                    RetireeName = user.FirstName + "" + user.LastName,
                                    MobileNumber = user.MobileNumber,
                                    Amount = pay.AmountPaid,
                                    Date = pay.PaymentDate,
                                    AssociationName = association.AssociationName,
                                    status = pay.PaymentStatus.ToString(),
                                    PaymentMode = pay.PaymentType.ToString()

                                }).ToList();
                    numberOfRecords = paymnets.Count();
                    paymnets = filter.Limit == 0 ? paymnets.ToList() : paymnets.Skip((filter.PageNumber - 1) * filter.Limit).Take(filter.Limit).ToList();

                }

            }

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
        private (List<DTOOnlinePolicyOrders> Policies, int TotalCount) GetPolicyOrderData(DataFilter<DTOPolicyOrders> filter)
        {
            // Base query
            var query = from policy in _context.PolicyHeader.AsNoTracking()
                        join user in _context.Applicationuser.AsNoTracking()
                          on policy.UserId equals user.Id
                        join association in _context.Association.AsNoTracking()
                          on user.AssociationId equals association.Id
                        join org in _context.Organisations.AsNoTracking()
                          on association.OraganisationId equals org.Id
                        join p in _context.PaymentDetails.AsNoTracking()
                          on policy.Id equals p.PolicyId
                          into PaymentDetails
                        from pc in PaymentDetails.DefaultIfEmpty()
                        where policy.CampaignId == filter.CampaignId
                        select new DTOOnlinePolicyOrders
                        {
                            OrderId = policy.Id,
                            AssociationId = association.Id,
                            Name = string.Concat(user.FirstName, " ", user.LastName),
                            Amount = policy.TotalPremimum,
                            Date = policy.CreatedAt,
                            AssociationName = association.AssociationName,
                            Status = policy.PaymentStatus.ToString(),
                            OrganisationName = org.Name,
                            MobileNumber = user.MobileNumber,
                            TransactionNumber = pc.TransactionId,
                            PaidAmount = policy.TotalPaidPremimum
                        };

            if (filter.AssociationId > 0)
            {
                query = query.Where(x => x.AssociationId == filter.AssociationId);
                query = query.Where(x => x.AssociationId == filter.AssociationId);
            }
            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.AssociationName.ToLower().Contains(searchTerm) ||
                    x.Name.ToLower().Contains(searchTerm) ||
                    x.MobileNumber.Contains(searchTerm));
            }

            // Get total count before pagination
            var totalCount = query.Count();

            // Apply pagination
            var paginatedResults = filter.Limit > 0
                ? query.Skip((filter.PageNumber - 1) * filter.Limit)
                      .Take(filter.Limit)
                      .ToList()
                : query.ToList();

            return (paginatedResults, totalCount);
        }


        public DataReturn<DTOOnlinePolicyOrders> GetPolicyOrders(DataFilter<DTOPolicyOrders> filter)
        {
            var ret = new DataReturn<DTOOnlinePolicyOrders>();
            //int numberOfRecords = 0;
           // var policyData = new List<DTOOnlinePolicyOrders>();
            var (policyData, numberOfRecords) = GetPolicyOrderData(filter);
            

            ret.Contents = policyData;
            ret.Source = "Policy Orders";
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

        public async Task<DTOOfflinePayment> GetOfflinePayment(int paymentId)
        {
            var ret = new DTOOfflinePayment();

            try
            {
                var paymnets = await (from pay in _context.PaymentDetails.AsNoTracking()
                                      join u in _context.Applicationuser.AsNoTracking()
                                      on pay.UserId equals u.Id
                                      where pay.Id == paymentId
                                      // && pay.PaymentMode == PaymentMode.Offline
                                      select new
                                      {
                                          pay.PaymentType,
                                          pay.PolicyId,
                                          PaymentId = paymentId,
                                          RetireeName = u.FullName,
                                          Comment = pay.Comment
                                      }).FirstOrDefaultAsync();
                if (paymnets != null)
                {
                    ret.RetireeName = paymnets.RetireeName;
                    var paymentTypes = paymnets.PaymentType;
                    ret.PaymentMode = paymentTypes.ToString();

                    if (paymentTypes == PaymentTypes.Cheque)
                    {
                        ret.ChequeDetails = new DTOChequeDetails();
                        var cheque = await (from pay in _context.PaymentDetails.AsNoTracking()
                                            join c in _context.PaymentModeCheque.AsNoTracking()
                                            on pay.Id equals c.PaymentDetailId
                                            join a in _context.Association.AsNoTracking()
                                            on c.InFavourOfAssociationId equals a.Id
                                            join b in _context.AssociationBankDetails.AsNoTracking()
                                            on a.Id equals b.AssociationId

                                            where pay.Id == paymentId
                                            select new
                                            {
                                                PaymentModeCheque = c,
                                                Association = a,
                                                AssociationBankDetails = b
                                            }).FirstOrDefaultAsync();
                        if (cheque != null)
                        {
                            ret.ChequeDetails.Date = cheque.PaymentModeCheque.Date;
                            ret.ChequeDetails.Amount = cheque.PaymentModeCheque.Amount;
                            ret.ChequeDetails.BankName = cheque.PaymentModeCheque.BankName;
                            ret.ChequeDetails.ChequeNumber = cheque.PaymentModeCheque.ChequeNumber;
                            ret.ChequeDetails.Micrcode = cheque.PaymentModeCheque.Micrcode;
                            ret.ChequeDetails.Ifsccode = cheque.PaymentModeCheque.Ifsccode;

                            ret.ChequeDetails.InFavourOf = new CommonAssociationDTO()
                            {
                                Id = cheque.Association.Id,
                                Name = cheque.Association.AssociationName,
                                AccountName = cheque.AssociationBankDetails.AccountName
                            ,
                                Code = cheque.Association.AssociationCode
                            };
                            ret.ChequeDetails.ChequePhoto = new Domain.Common.CommonFileModel();
                            ret.ChequeDetails.ChequePhoto.Id = cheque.PaymentModeCheque.Id;
                            ret.ChequeDetails.ChequePhoto.Name = cheque.PaymentModeCheque.ChequePhotoDocumentName;
                            ret.ChequeDetails.ChequePhoto.Url = cheque.PaymentModeCheque.ChequePhotoDocumentUrl;
                            if (ret.ChequeDetails.ChequePhoto.Url != null)
                            {
                                var imagePath = Path.Combine("wwwroot", "images", ret.ChequeDetails.ChequePhoto.Url);

                                if (File.Exists(imagePath))
                                {
                                    // Determine MIME type
                                    var provider = new FileExtensionContentTypeProvider();
                                    if (!provider.TryGetContentType(imagePath, out string contentType))
                                    {
                                        contentType = "application/octet-stream"; // Fallback if unknown
                                    }
                                    // Read image data as byte array
                                    ret.ChequeDetails.ChequePhoto.FileData = File.ReadAllBytes(imagePath);

                                }
                            }

                        }

                    }
                    if (paymentTypes == PaymentTypes.NEFT)
                    {
                        ret.Neft = new DTOPaymentModeNEFT();
                        var objNeft = await (from pay in _context.PaymentDetails.AsNoTracking()
                                             join c in _context.PaymentModeNEFT.AsNoTracking()
                                             on pay.Id equals c.PaymentDetailId

                                             where pay.Id == paymentId
                                             select new
                                             {
                                                 neft = c

                                             }).FirstOrDefaultAsync();
                        if (objNeft != null)
                        {
                            ret.Neft.Date = objNeft.neft.Date;
                            ret.Neft.Amount = objNeft.neft.Amount;
                            ret.Neft.BankName = objNeft.neft.BankName;
                            ret.Neft.BranchName = objNeft.neft.BranchName;
                            ret.Neft.AccountName = objNeft.neft.AccountNumber;
                            ret.Neft.TransactionId = objNeft.neft.TransactionId;
                            ret.Neft.AccountNumber = objNeft.neft.AccountNumber;
                            ret.Neft.IfscCode = objNeft.neft.IfscCode;
                            ret.Neft.NeftPaymentReceipt = new Domain.Common.CommonFileModel();
                            ret.Neft.NeftPaymentReceipt.Name = objNeft.neft.NEFTReceiptDocumentName;
                            ret.Neft.NeftPaymentReceipt.Url = objNeft.neft.NEFTReceiptDocumentUrl;
                            ret.Neft.NeftPaymentReceipt.Id = objNeft.neft.Id;
                            if (ret.Neft.NeftPaymentReceipt.Url != null)
                            {
                                var imagePath = Path.Combine("wwwroot", "images", ret.Neft.NeftPaymentReceipt.Url);

                                if (File.Exists(imagePath))
                                {
                                    // Determine MIME type
                                    var provider = new FileExtensionContentTypeProvider();
                                    if (!provider.TryGetContentType(imagePath, out string contentType))
                                    {
                                        contentType = "application/octet-stream"; // Fallback if unknown
                                    }
                                    // Read image data as byte array
                                    ret.Neft.NeftPaymentReceipt.FileData = File.ReadAllBytes(imagePath);

                                }
                            }
                        }

                    }
                    if (paymentTypes == PaymentTypes.UPI)
                    {
                        ret.Upi = new DTOPaymentModeUPI();
                        var objUpi = await (from pay in _context.PaymentDetails.AsNoTracking()
                                            join c in _context.PaymentModeUPI.AsNoTracking()
                                            on pay.Id equals c.PaymentDetailId
                                            where pay.Id == paymentId
                                            select new
                                            {
                                                upi = c

                                            }).FirstOrDefaultAsync();
                        if (objUpi != null)
                        {
                            ret.Upi.Date = objUpi.upi.Date;
                            ret.Upi.Amount = objUpi.upi.Amount;
                            ret.Upi.TransactionId = objUpi.upi.TransactionNumber;
                            ret.Upi.UpiPaymentReceipt = new Domain.Common.CommonFileModel();
                            ret.Upi.UpiPaymentReceipt.Name = objUpi.upi.UPIReceiptDocumentName;
                            ret.Upi.UpiPaymentReceipt.Url = objUpi.upi.UPIReceiptDocumentUrl;
                            ret.Upi.UpiPaymentReceipt.Id = objUpi.upi.Id;
                            if (ret.Upi.UpiPaymentReceipt.Url != null)
                            {
                                var imagePath = Path.Combine("wwwroot", "images", ret.Upi.UpiPaymentReceipt.Url);

                                if (File.Exists(imagePath))
                                {
                                    // Determine MIME type
                                    var provider = new FileExtensionContentTypeProvider();
                                    if (!provider.TryGetContentType(imagePath, out string contentType))
                                    {
                                        contentType = "application/octet-stream"; // Fallback if unknown
                                    }
                                    // Read image data as byte array
                                    ret.Upi.UpiPaymentReceipt.FileData = File.ReadAllBytes(imagePath);

                                }
                            }
                        }

                    }
                    if (paymentTypes == PaymentTypes.Gateway)
                    {
                        ret.Gateway = new DTOPaymentModeGateway();
                        var objGateway = await (from pay in _context.PaymentDetails.AsNoTracking()
                                                join c in _context.PaymentModeGateway.AsNoTracking()
                                                on pay.Id equals c.PaymentDetailId
                                                where pay.Id == paymentId
                                                select new
                                                {
                                                    gateway = c

                                                }).FirstOrDefaultAsync();
                        if (objGateway != null)
                        {
                            ret.Gateway.Date = objGateway.gateway.Date.Value;
                            ret.Gateway.Amount = objGateway.gateway.Amount;
                            ret.Gateway.TransactionId = objGateway.gateway.TransactionId;
                            ret.Gateway.transactionNumber = objGateway.gateway.TransactionId;


                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error("Error", ex.Message);
            }
            return ret;
        }

        public async Task<DTOOnlinePolicyOrders> GetPolicyOrderById(int policyId)
        {
            var ret = new DTOOnlinePolicyOrders();

            ret = await (from policy in _context.PolicyHeader.AsNoTracking()
                         join user in _context.Applicationuser.AsNoTracking()
                           on policy.UserId equals user.Id
                         join association in _context.Association.AsNoTracking()
                         on user.AssociationId equals association.Id
                         join org in _context.Organisations.AsNoTracking()
                         on association.OraganisationId equals org.Id
                         join p in _context.PaymentDetails.AsNoTracking()
                                 on policy.Id equals p.PolicyId
                                 into PaymentDetails
                         from pc in PaymentDetails.DefaultIfEmpty()
                         where policy.Id == policyId
                         select new DTOOnlinePolicyOrders
                         {
                             OrderId = policy.Id,
                             Name = user.FirstName + "" + user.LastName,
                             Amount = policy.TotalPremimum,
                             PaidAmount = policy.TotalPaidPremimum,
                             Date = policy.CreatedAt,
                             AssociationName = association.AssociationName,
                             Status = policy.PaymentStatus.ToString(),
                             OrganisationName = org.Name,
                             MobileNumber = user.MobileNumber,
                             TransactionNumber = pc.TransactionId,
                             Comment = policy.Comment
                         }).FirstOrDefaultAsync();
            return ret;
        }

        public async Task<bool> UpdatePolicyOrder(NotifyPaymentStatusPayload payload, int updatedBy, string Comment = "", bool isManual = false)
        {
            try
            {
                if (payload.PolicyId != null && payload.PolicyId.Value > 0)
                {


                    var policyheader = await _context.PolicyHeader.AsNoTracking()
                                .Where(x => x.Id == payload.PolicyId.Value).FirstOrDefaultAsync();
                    if (policyheader != null)
                    {


                        if (!string.IsNullOrWhiteSpace(payload.transactionNumber))
                        {
                            var payment = await _context.PaymentHeader.AsNoTracking()
                                .Where(x => x.PolicyId == policyheader.Id).FirstOrDefaultAsync();
                            int paymentHeaderId = 0;
                            if (payment != null)
                            {
                                paymentHeaderId = payment.Id;
                                PaymentHeader paymentHeader = new PaymentHeader();
                                paymentHeader.PolicyId = policyheader.Id;

                                paymentHeader.PaidAmount = payload.premiumAmount;
                                paymentHeader.TotalPremimumAmount = policyheader.TotalPremimum;
                                paymentHeader.UpdatedAt = DateTime.Now;
                                paymentHeader.UpdatedBy = updatedBy;
                                _context.PaymentHeader.Update(paymentHeader);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                PaymentHeader paymentHeader = new PaymentHeader();
                                paymentHeader.PolicyId = policyheader.Id;
                                paymentHeader.UserId = policyheader.UserId;
                                paymentHeader.CampaignId = policyheader.CampaignId;
                                paymentHeader.PaidAmount = payload.premiumAmount;
                                paymentHeader.TotalPremimumAmount = policyheader.TotalPremimum;
                                paymentHeader.CreatedBy = updatedBy;
                                paymentHeader.CreatedAt = DateTime.Now;
                                paymentHeader.UpdatedAt = DateTime.Now;
                                paymentHeader.UpdatedBy = updatedBy;
                                _context.PaymentHeader.Add(paymentHeader);
                                await _context.SaveChangesAsync();
                                paymentHeaderId = paymentHeader.Id;

                            }


                            PaymentDetails paymentDetails = new PaymentDetails();
                            var objpaymentDetails = await _context.PaymentDetails.AsNoTracking()
                                .Where(x => x.PolicyId == policyheader.Id)
                                .FirstOrDefaultAsync();
                            bool isupdate = false;
                            if (objpaymentDetails != null)
                            {
                                paymentDetails = objpaymentDetails;
                                paymentDetails.UpdatedAt = DateTime.Now;
                                paymentDetails.UpdatedBy = updatedBy;
                                isupdate = true;
                            }
                            else
                            {
                                paymentDetails.UserId = policyheader.UserId;
                                paymentDetails.CreatedBy = updatedBy;
                                paymentDetails.CreatedAt = DateTime.Now;
                                paymentDetails.UpdatedAt = DateTime.Now;
                                paymentDetails.UpdatedBy = updatedBy;
                            }
                            paymentDetails.PaymentHeaderId = paymentHeaderId;
                            paymentDetails.UserId = policyheader.UserId;
                            paymentDetails.IsActive = true;
                            paymentDetails.PaymentDate = DateTime.Now;
                            paymentDetails.PaymentMode = PaymentMode.Online;
                            paymentDetails.PaymentType = PaymentTypes.Gateway;
                            paymentDetails.PaymentStatus = PaymentStatus.Completed;
                            paymentDetails.TransactionId = payload.transactionNumber;
                            paymentDetails.TotalPremimumAmount = policyheader.TotalPremimum;
                            paymentDetails.PayableAmount = payload.premiumAmount;
                            paymentDetails.AmountPaid = payload.premiumAmount;
                            paymentDetails.PolicyId = policyheader.Id;
                            paymentDetails.CampaignId = policyheader.CampaignId;
                            if (isupdate)
                                _context.PaymentDetails.Update(paymentDetails);
                            else
                                _context.PaymentDetails.Add(paymentDetails);

                            _context.SaveChanges();


                            if (paymentDetails.Id > 0)
                            {
                                PaymentModeGateway paymentModeGateway = new PaymentModeGateway();
                                var paymentModeGatewayObj = await _context.PaymentModeGateway.AsNoTracking()
                                    .Where(x => x.PaymentDetailId == paymentDetails.Id).FirstOrDefaultAsync();
                                if (paymentModeGatewayObj != null)
                                {
                                    paymentModeGateway = paymentModeGatewayObj;
                                    paymentModeGateway.TransactionId = payload.transactionNumber;
                                    paymentModeGateway.PaymentDetailId = paymentDetails.Id;
                                    paymentModeGateway.PolicyId = policyheader.Id;
                                    paymentModeGateway.UserId = policyheader.UserId;
                                    paymentModeGateway.CampaignId = policyheader.CampaignId;
                                    paymentModeGateway.DigitPaymentId = policyheader.DigitPaymentId;
                                    paymentModeGateway.Amount = payload.premiumAmount;
                                    paymentModeGateway.Date = DateTime.Now;
                                    _context.PaymentModeGateway.Update(paymentModeGateway);
                                    await _context.SaveChangesAsync();
                                }
                                else
                                {
                                    paymentModeGateway.TransactionId = payload.transactionNumber;
                                    paymentModeGateway.PaymentDetailId = paymentDetails.Id;
                                    paymentModeGateway.PolicyId = policyheader.Id;
                                    paymentModeGateway.UserId = policyheader.UserId;
                                    paymentModeGateway.CampaignId = policyheader.CampaignId;
                                    paymentModeGateway.DigitPaymentId = policyheader.DigitPaymentId;
                                    paymentModeGateway.Amount = payload.premiumAmount;
                                    paymentModeGateway.Date = DateTime.Now;
                                    _context.PaymentModeGateway.Add(paymentModeGateway);
                                    await _context.SaveChangesAsync();
                                }


                            }
                        }

                        policyheader.Id = policyheader.Id;
                        policyheader.PaymentStatus = PaymentStatus.Completed;
                        policyheader.TotalPaidPremimum = payload.premiumAmount;
                        policyheader.UpdatedAt = DateTime.Now;
                        policyheader.TotalPremimum = payload.premiumAmount;
                        policyheader.IsManual = isManual;
                        policyheader.Comment = Comment;
                        _context.PolicyHeader.Update(policyheader);
                        _context.SaveChanges();
                    }

                    return true;
                }
                else
                    return false;


            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in PaymentStatus {ex.Message}");
                return false;
            }
        }
        public DataReturn<DTOPaymentHistory> GetAllPaymentHistory(DataFilter<DTOPaymentHistory> filter, int userId)
        {
            var ret = new DataReturn<DTOPaymentHistory>();

            int numberOfRecords = 0;
            var paymnets = (from pay in _context.PaymentDetails.AsNoTracking()

                            where pay.IsActive == true && pay.UserId == filter.userId
                            select new DTOPaymentHistory
                            {
                                PaymentId = pay.Id,
                                PaymentMode = pay.PaymentType.ToString(),
                                payableAmount = pay.PayableAmount,
                                Date = pay.PaymentDate,
                                amountPaid = pay.AmountPaid,
                                transactionId = pay.TransactionId,
                                OrderNumber = pay.PolicyId.ToString(),
                                paymentStatus = pay.PaymentStatus.ToString(),
                                acknowledgement = pay.PaymentMode.ToString()
                            }).ToList();



            ret.Contents = paymnets;
            ret.Source = "PaymentHistory";
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

        private List<DTOPolicyProductPremimumChart> GetProductPremimumChart
          (int headerId, int productdetsilsId,int ageBandPremiumRateId,string ageBandPremiumRateValue)
        {
            List<DTOPolicyProductPremimumChart> items = new List<DTOPolicyProductPremimumChart>();
            var charts = _context.PolicyProductPremimum
                .Where(p => p.PolicyHeaderId == headerId
                //&& p.ProductId ==productId
                && p.PolicyProductDetailsId == productdetsilsId
                ).ToList();

            foreach (var chart in charts.Where(x => x.ParentProductPremimumId == 0 || x.ParentProductPremimumId == null))
            {

                DTOPolicyProductPremimumChart premimumChart = new DTOPolicyProductPremimumChart();

                premimumChart.Id = chart.Id;
                premimumChart.ProductId = chart.ProductId.Value;
                premimumChart.ProductPremiumId = chart.ProductPremimumId;
                premimumChart.SumInsured = chart.SumInsured.Value;
                premimumChart.SelfOnlyPremium = chart.SelfOnlyPremium;
                premimumChart.IsSelfPremiumSelected = chart.IsSelfPremiumSelected.Value;
                premimumChart.SelfSpousePremium = chart.SelfSpousePremium;
                premimumChart.SelfSpouse2ChildrenPremium = chart.SelfSpouse2ChildrenPremium!=null?
                    chart.SelfSpouse2ChildrenPremium:0;
                premimumChart.SelfSpouse1ChildrenPremium = chart.SelfSpouse1ChildrenPremium!=null?
                    chart.SelfSpouse1ChildrenPremium:0;
                premimumChart.Self1ChildrenPremium = chart.Self1ChildrenPremium!=null? chart.Self1ChildrenPremium:0;
                premimumChart.Self2ChildrenPremium = chart.Self2ChildrenPremium!=null ? chart.Self2ChildrenPremium:0;
                premimumChart.IsSelfSpousePremiumSelected =chart.IsSelfSpousePremiumSelected.Value;
                premimumChart.SpousePremium = chart.SpousePremium!=null? chart.SpousePremium:0;
                premimumChart.IsSpousePremiumSelected = chart.IsSpousePremiumSelected.Value;

                premimumChart.IsSelfSpouse1ChildrenPremiumSelected = chart.IsSelfSpouse1ChildrenPremiumSelected!=null?
                    chart.IsSelfSpouse1ChildrenPremiumSelected:false;
                premimumChart.IsSelfSpouse2ChildrenPremiumSelected = chart.IsSelfSpouse2ChildrenPremiumSelected!=null?
                    chart.IsSelfSpouse2ChildrenPremiumSelected:false;
                premimumChart.IsSelf2ChildrenPremiumSelected = chart.IsSelf2ChildrenPremiumSelected!=null?
                    chart.IsSelf2ChildrenPremiumSelected:false;
                premimumChart.IsSelf1ChildrenPremiumSelected = chart.IsSelf1ChildrenPremiumSelected!=null?
                     chart.IsSelf1ChildrenPremiumSelected:false;


                premimumChart.Child1Premium = chart.Child1Premium;
                premimumChart.Child2Premium = chart.Child2Premium;
                premimumChart.IsChild1PremiumSelected = chart.IsChild1PremiumSelected.Value;
                premimumChart.IsChild2PremiumSelected = chart.IsChild2PremiumSelected.Value;

                premimumChart.Parent1Premium = chart.Parent1Premium;
                premimumChart.Parent2Premium = chart.Parent2Premium;
                premimumChart.IsParent1PremiumSelected = chart.IsParent1PremiumSelected.Value;
                premimumChart.IsParent2PremiumSelected = chart.IsParent2PremiumSelected.Value;

                premimumChart.InLaw1Premium = chart.InLaw1Premium;
                premimumChart.InLaw2Premium = chart.InLaw2Premium;

                premimumChart.IsInLaw1PremiumSelected = chart.IsInLaw1PremiumSelected.Value;
                premimumChart.IsInLaw2PremiumSelected = chart.IsInLaw2PremiumSelected.Value;

                premimumChart.ParentProductPremiumId = 0;
                premimumChart.AgeBandPremiumRateId = ageBandPremiumRateId;
                premimumChart.AgeBandPremiumRateValue = ageBandPremiumRateValue;
              

                items.Add(premimumChart);
            }


            //var parentProductPremimums = charts.Where(x => x.ParentProductPremimumId != 0 &&
            //x.ParentProductPremimumId != null);
            foreach (var chartItem in items)
            {

                var tops = charts.Where(x => x.ParentProductPremimumId == chartItem.ProductPremiumId);

                if (tops != null && tops.Count() > 0)
                {

                    chartItem.TopUpOptions = new List<DTOPolicyProductPremimumChart>();
                    foreach (var chart in tops)
                    {
                        DTOPolicyProductPremimumChart premimumChart = new DTOPolicyProductPremimumChart();

                        premimumChart.Id = chart.Id;
                        premimumChart.ProductId = chart.ProductId.Value;
                        premimumChart.ProductPremiumId = chart.ProductPremimumId;
                        premimumChart.SumInsured = chart.SumInsured.Value;
                        premimumChart.SelfOnlyPremium = chart.SelfOnlyPremium;
                        premimumChart.IsTopUpSelfPremiumSelected = chart.IsSelfPremiumSelected.Value;

                        premimumChart.SelfSpousePremium = chart.SelfSpousePremium;
                        premimumChart.SpousePremium = chart.SpousePremium;
                        premimumChart.IsTopUpSpousePremiumSelected = chart.IsSpousePremiumSelected.Value;
                        premimumChart.IsTopUpSelfSpousePremiumSelected = chart.IsSelfSpousePremiumSelected.Value;
                        premimumChart.Child1Premium = chart.Child1Premium;
                        premimumChart.Child2Premium = chart.Child2Premium;
                        premimumChart.IsChild1PremiumSelected = chart.IsChild1PremiumSelected.Value;
                        premimumChart.IsChild2PremiumSelected = chart.IsChild2PremiumSelected.Value;

                        premimumChart.Parent1Premium = chart.Parent1Premium;
                        premimumChart.Parent2Premium = chart.Parent2Premium;
                        premimumChart.IsParent1PremiumSelected = chart.IsParent1PremiumSelected.Value;
                        premimumChart.IsParent2PremiumSelected = chart.IsParent2PremiumSelected.Value;

                        premimumChart.InLaw1Premium = chart.InLaw1Premium;
                        premimumChart.InLaw2Premium = chart.InLaw2Premium;

                        premimumChart.IsInLaw1PremiumSelected = chart.IsInLaw1PremiumSelected.Value;
                        premimumChart.IsInLaw2PremiumSelected = chart.IsInLaw2PremiumSelected.Value;

                        premimumChart.ParentProductPremiumId = chart.ParentProductPremimumId;
                        chartItem.TopUpOptions.Add(premimumChart);
                        //premimumChart.ParentProductPremiumId = chart.ParentProductPremimumId;
                    }

                }
                // chart.Add(premimumChart);
            }

            return items;
        }
        public static bool IsDateInRange(DateTime currentDate, DateTime startDate, DateTime endDate)
        {
            // Normalize all dates to start of day to ignore time component
            currentDate = currentDate.Date;
            startDate = startDate.Date;
            endDate = endDate.Date;

            // Check if current date is between start and end dates (inclusive)
            return !(currentDate >= startDate && currentDate <= endDate);
        }
        public bool IsCampaignExpired(int productId, int userId)
        {
            var compaignProducts = (from c in _context.Campaigns.AsNoTracking()
                                    join cp in _context.CampaignProducts.AsNoTracking()
                                    on c.Id equals cp.CampaignId
                                    join p in _context.Product
                                    on cp.ProductId equals p.Id
                                    join ca in _context.CampaignAssociations.AsNoTracking()
                                    on c.Id equals ca.CampaignId
                                    join u in _context.Applicationuser.AsNoTracking()
                                    on ca.AssociationId equals u.AssociationId
                                    where u.Id == userId
                                    && (p.PolicyTypeId == (int)ProductPolicyType.BasePolicy ||
                                    p.PolicyTypeId == (int)ProductPolicyType.OPD ||
                                    p.PolicyTypeId == (int)ProductPolicyType.Other ||
                                     p.PolicyTypeId == (int)ProductPolicyType.AgeBandPremium ||
                                    p.PolicyTypeId == (int)ProductPolicyType.PaymentProtectionPolicy

                                    )
                                    && (DateTime.Now.Date >= c.StartDate.Value.Date
                                    && DateTime.Now.Date <= c.EndDate.Value.Date)
                                    select c).ToList();
            if (compaignProducts.Any())
                return false;
            else
                return true;
        }
        public List<AgeBandPremiumRate> GetAgeBandPremiumRateId(int age)
        {
            var ageBandPremium = _context.AgeBandPremiumRate.AsNoTracking()
                    .Where(x => x.AgeBandStart <= age && x.AgeBandEnd >= age).ToList();
           
            return ageBandPremium;
        }

        public async Task<DTOPolicy> GetPolicyDetails(int policyId,int campaignId)
        {
            DTOPolicy policy = new DTOPolicy();

            try
            {
                policy.beneficiaries = new DTOBeneficiaryDetails();
                policy.Nominee = new DTOBeneficiaryPerson();
                policy.PaymentDetails = new DTOPaymentDetails();

                var policydetsils = await (from ph in _context.PolicyHeader.AsNoTracking()
                                         .Where(x => x.Id == policyId)
                                           select new DTOPolicy
                                           {
                                               UserId = ph.UserId,
                                               CampaignId = ph.CampaignId,
                                               PolicyId = ph.Id,
                                               TotalPremium = ph.TotalPremimum,
                                               TotalPaidPremium = ph.TotalPaidPremimum,
                                               PaymentStatus = ph.PaymentStatus.ToString(),

                                           }).FirstOrDefaultAsync();

                if (policydetsils != null)
                    policy = policydetsils;
                else
                    return policy;

                if (campaignId == 0&& policy!=null&& policy.CampaignId!=null)
                    campaignId = policy.CampaignId.Value;

                var productquery = await _context.PolicyProductDetails.AsNoTracking().Distinct()
                    .Include(x => x.Product)
                    .Where(x => x.PolicyHeaderId == policyId)
                    .Distinct().ToListAsync();
                var dob = _context.Applicationuser.Where(x => x.Id == policydetsils.UserId)
                                   .FirstOrDefault().DOB;
               
                int ageBandPremiumRateId = 0;string ageBandPremiumValue = null;
               
                    int age = CommonHelper.CalculateAge(dob.Value);
                    var ageBandPremiumRate = GetAgeBandPremiumRateId(age);
                    //if (ageBandPremiumRate != null)
                    //{
                    //    ageBandPremiumRateId = ageBandPremiumRate.Id;
                    //    ageBandPremiumValue = ageBandPremiumRate.AgeBandValue;


                    //}
                
                policy.Products = new List<DTOPolicyProduct>();
                foreach (var item in productquery)
                {

                    if (item.Product != null)
                    {
                        var productdata = await _context.Product.AsNoTracking()
                                .Include(x => x.ProductPremiums)
                                //.ThenInclude(x=>x.AgeBandPremiumRate)
                                .Where(x => x.Id == item.ProductId).FirstOrDefaultAsync();
                        DTOPolicy dTOPolicyProduct = new DTOPolicy();
                        if (productdata != null)
                        {
                           
                            if (CommonHelper.CheckPolicyExists(item.Product.PolicyTypeId.Value))
                            {
                               
                                var topupProduct = _context.Product.AsNoTracking()
                                              .Include(x => x.ProductPremiums)
                                              //.ThenInclude(x=>x.AgeBandPremiumRate)
                                              .Where(x => x.Id == productdata.BasePolicyId)
                                              .FirstOrDefault();
                               
                                
                                var productitem = new DTOPolicyProduct().CopyBasePolicy(productdata, topupProduct
                                    , ageBandPremiumRate);
                                productitem.TotalProductPremium = item.TotalProductPremimum;
                                productitem.Id = item.Id;
                                productitem.IsProductSelected = true;
                                productitem.CampaignId = item.CampaignId;
                                productitem.IsCampaignExpired = IsCampaignExpired(item.ProductId.Value, policydetsils.UserId);
                                var exists = policy.Products.Any(x => x.ProductId == item.ProductId
                                && x.SelectedSumInsured.Id == item.SumInsuredPremimumId);
                                productitem.IsDisclaimerAccepted = item.IsDisclaimerAccepted;
                                productitem.AgeBandPremiumRateValue = ageBandPremiumValue;
                                if (!exists)
                                    policy.Products.Add(productitem);

                                productitem.SelectedSumInsured = new SelectedSumInsured();
                                productitem.SelectedSumInsured.Id = item.SumInsuredPremimumId.Value;
                                productitem.SelectedSumInsured.Name = item.SumInsured.Value;
                                productitem.SelectedSumInsured.Index = 0;

                                productitem.SelectedTopUpOption = new SelectedSumInsured();
                                productitem.SelectedTopUpOption.Id = item.TopupSumInsuredPremimumId.Value;
                                productitem.SelectedTopUpOption.Name = item.TopupSumInsured.Value;
                                productitem.SelectedTopUpOption.Index = 0;
                                productitem.IsTopUpSelected = item.IsTopUpSelected.Value;
                            }
                            else
                            {


                                var baseProduct = _context.Product.AsNoTracking()
                                              .Include(x => x.ProductPremiums)
                                              .Where(x => x.Id == productdata.BasePolicyId)
                                              .FirstOrDefault();

                                if (baseProduct != null)
                                {
                                    var productitem = new DTOPolicyProduct().CopyTopupPolicy(productdata, baseProduct
                                        );
                                    productitem.TotalProductPremium = item.TotalProductPremimum;
                                    productitem.Id = item.Id;
                                    productitem.IsProductSelected = true;
                                    productitem.CampaignId = item.CampaignId;
                                    productitem.IsCampaignExpired = IsCampaignExpired(item.ProductId.Value, policydetsils.UserId);
                                    productitem.SelectedSumInsured = new SelectedSumInsured();
                                    productitem.SelectedSumInsured.Id = item.SumInsuredPremimumId.Value;
                                    productitem.SelectedSumInsured.Name = item.SumInsured.Value;
                                    productitem.SelectedSumInsured.Index = 0;

                                    productitem.SelectedTopUpOption = new SelectedSumInsured();
                                    productitem.SelectedTopUpOption.Id = item.TopupSumInsuredPremimumId.Value;
                                    productitem.SelectedTopUpOption.Name = item.TopupSumInsured.Value;
                                    productitem.SelectedTopUpOption.Index = 0;
                                    productitem.IsDisclaimerAccepted = item.IsDisclaimerAccepted;
                                    productitem.AgeBandPremiumRateValue = ageBandPremiumValue;
                                    var exists = policy.Products.Any(x => x.ProductId == item.ProductId && x.SelectedSumInsured.Id == item.SumInsuredPremimumId);
                                    if (!exists)
                                        policy.Products.Add(productitem);
                                }
                            }
                        }
                    }
                }

                foreach (var item in policy.Products)
                {
                    item.PremiumChart = GetProductPremimumChart(policyId, item.Id,ageBandPremiumRateId,ageBandPremiumValue);

                    foreach (var charts in item.PremiumChart)
                    {
                        if (item.SelectedTopUpOption != null && charts.TopUpOptions != null && charts.TopUpOptions.Any())
                        {
                            var b = item.IsTopUpSelected == true && charts.TopUpOptions.Any(x => x.SumInsured == item.SelectedTopUpOption.Name);
                            charts.IsTopUpSelected = b;
                        }
                    }
                }
                

                //Other Existing product 
                var currentDate = DateTime.Now.Date;
                var compaignProducts = (from c in _context.Campaigns.AsNoTracking()
                                        join cp in _context.CampaignProducts.AsNoTracking()
                                        on c.Id equals cp.CampaignId
                                        join p in _context.Product
                                        on cp.ProductId equals p.Id
                                        join ca in _context.CampaignAssociations.AsNoTracking()
                                        on c.Id equals ca.CampaignId
                                        join u in _context.Applicationuser.AsNoTracking()
                                        on ca.AssociationId equals u.AssociationId
                                        where u.Id == policy.UserId
                                        && c.Id==campaignId
                                        && (p.PolicyTypeId == (int)ProductPolicyType.BasePolicy ||
                                        p.PolicyTypeId == (int)ProductPolicyType.OPD ||
                                        p.PolicyTypeId == (int)ProductPolicyType.Other ||
                                        p.PolicyTypeId == (int)ProductPolicyType.PaymentProtectionPolicy||
                                        p.PolicyTypeId == (int)ProductPolicyType.AgeBandPremium

                                        )

                                        && ((DateTime.Now.Date >= c.StartDate.Value.Date ||
                                         DateTime.Now.Date <= c.StartDate.Value.Date)
                                         && DateTime.Now.Date <= c.EndDate.Value.Date)
                                        select new { Campaign = c, CampaignProduct = cp })
                                        .Distinct().ToList();


                foreach (var p in compaignProducts)
                {

                    if (!policy.Products.Any(x => x.ProductId == p.CampaignProduct.ProductId))
                    {
                        var policyproduct = (from p1 in _context.PolicyProductDetails.AsNoTracking()
                                             join cp in _context.PolicyHeader.AsNoTracking()
                                              on p1.PolicyHeaderId equals cp.Id
                                             where cp.UserId == policy.UserId &&
                                             p1.ProductId == p.CampaignProduct.ProductId
                                             && p.CampaignProduct.CampaignId == cp.CampaignId
                                             select p1).ToList();

                        if (policyproduct != null && policyproduct.Count() == 0)
                        {
                            var item = (_context.Product.AsNoTracking()
                                .Include(x => x.BasePolicy)
                                 .Include(p => p.ProductPremiums)
                                .Where(x => x.Id == p.CampaignProduct.ProductId)).FirstOrDefault();

                            if (item != null)
                            {
                                DTOPolicy dTOPolicyProduct = new DTOPolicy();
                                if (item.PolicyTypeId == (int)ProductPolicyType.BasePolicy ||
                                    item.PolicyTypeId == (int)ProductPolicyType.OPD ||
                                    item.PolicyTypeId == (int)ProductPolicyType.Other ||
                                    item.PolicyTypeId == (int)ProductPolicyType.PaymentProtectionPolicy||
                                    item.PolicyTypeId == (int)ProductPolicyType.AgeBandPremium

                                    )
                                {
                                    var topupProduct = _context.Product.AsNoTracking()
                                           .Include(x => x.ProductPremiums)
                                           .Where(x => x.BasePolicyId == item.Id)
                                           .FirstOrDefault();

                                    var policyProductdata = new DTOPolicyProduct().CopyBasePolicy(item, topupProduct, ageBandPremiumRate);
                                        //,ageBandPremiumRateId,ageBandPremiumValue);
                                    policyProductdata.CampaignId = p.CampaignProduct.CampaignId;
                                    policyProductdata.IsCampaignExpired = IsDateInRange(DateTime.Now, p.Campaign.StartDate.Value, p.Campaign.EndDate.Value);
                                    policyProductdata.TotalProductPremium = 0;
                                    policyProductdata.Id = item.Id;
                                    policyProductdata.IsProductSelected = false;
                                    policyProductdata.CampaignId = policy.CampaignId;
                                    policyProductdata.IsCampaignExpired = false;
                                    policyProductdata.SelectedSumInsured = new SelectedSumInsured();
                                    var chart = policyProductdata.PremiumChart.FirstOrDefault();
                                    if (chart != null)
                                    {
                                        policyProductdata.SelectedSumInsured.Id = chart.ProductPremiumId.Value;
                                        policyProductdata.SelectedSumInsured.Name = chart.SumInsured;
                                        policyProductdata.SelectedSumInsured.Index = 0;

                                        if (topupProduct != null)
                                        {
                                            var topUppremimum = chart.TopUpOptions.FirstOrDefault();
                                            if (topUppremimum != null)
                                            {
                                                policyProductdata.SelectedTopUpOption = new SelectedSumInsured();
                                                policyProductdata.SelectedTopUpOption.Id = topUppremimum.ProductPremiumId.Value;
                                                policyProductdata.SelectedTopUpOption.Name = topUppremimum.SumInsured;
                                                policyProductdata.SelectedTopUpOption.Index = 0;
                                            }
                                        }
                                    }




                                    policy.Products.Add(policyProductdata);

                                }
                                else
                                {
                                    var baseProduct = _context.Product.AsNoTracking()
                                            .Include(x => x.ProductPremiums)
                                            .Where(x => x.Id == item.BasePolicyId)
                                            .FirstOrDefault();
                                    if (baseProduct != null)
                                    {
                                        var policyProductdatabase = new DTOPolicyProduct().CopyTopupPolicy(item, baseProduct);
                                        policyProductdatabase.CampaignId = p.CampaignProduct.CampaignId;
                                        policyProductdatabase.IsCampaignExpired = IsDateInRange(DateTime.Now, p.Campaign.StartDate.Value, p.Campaign.EndDate.Value);
                                        policyProductdatabase.CampaignId = p.CampaignProduct.CampaignId;
                                        policyProductdatabase.IsCampaignExpired = IsDateInRange(DateTime.Now, p.Campaign.StartDate.Value, p.Campaign.EndDate.Value);
                                        policyProductdatabase.TotalProductPremium = 0;
                                        policyProductdatabase.Id = item.Id;
                                        policyProductdatabase.IsProductSelected = false;
                                        policyProductdatabase.CampaignId = policy.CampaignId;
                                        policyProductdatabase.IsCampaignExpired = false;
                                        policyProductdatabase.SelectedSumInsured = new SelectedSumInsured();
                                        var chart = policyProductdatabase.PremiumChart.FirstOrDefault();
                                        if (chart != null)
                                        {
                                            policyProductdatabase.SelectedSumInsured.Id = chart.ProductPremiumId.Value;
                                            policyProductdatabase.SelectedSumInsured.Name = chart.SumInsured;
                                            policyProductdatabase.SelectedSumInsured.Index = 0;


                                        }
                                        policy.Products.Add(policyProductdatabase);
                                    }
                                }


                            }

                        }
                    }
                }

                var beneficiaryDetails = await _context.BeneficiaryDetails.AsNoTracking()
                    .Where(x => x.PolicyId == policyId).FirstOrDefaultAsync();

                if (beneficiaryDetails != null)
                    policy.BeneficiaryId = beneficiaryDetails.Id;

                //Beneficiary details
                policy.beneficiaries = new DTOBeneficiaryDetails();
                var spouseObj = (from b in _context.BeneficiaryDetails.AsNoTracking()
                                 join spouse in _context.BeneficiaryPerson
                                 on b.Spouse equals spouse.Id
                                 where b.PolicyId == policyId
                                 select spouse).FirstOrDefault();
                if (spouseObj != null)
                {
                    var x = new DTOBeneficiaryPerson();
                    x.DateOfBirth = spouseObj.DateOfBirth;
                    x.Gender = new CommonNameDTO((int)spouseObj.Gender, spouseObj.Gender.ToString());
                    x.Name = spouseObj.Name;
                    x.Id = spouseObj.Id;
                    policy.beneficiaries.Spouse = x;
                }
                else
                {
                    var spouseUserObj = (from b in _context.UserSpouseDetail.AsNoTracking()
                                         where b.UserId == policy.UserId
                                         select b).FirstOrDefault();
                    if (spouseUserObj != null)
                    {
                        var x = new DTOBeneficiaryPerson();
                        x.DateOfBirth = spouseUserObj.DateOfBirth;
                        x.Gender = new CommonNameDTO((int)spouseUserObj.Gender, spouseUserObj.Gender.ToString());
                        x.Name = spouseUserObj.Name;
                        x.Id = 0;
                        policy.beneficiaries.Spouse = x;
                    }
                }

                var Child1Obj = (from b in _context.BeneficiaryDetails.AsNoTracking()
                                 join child1 in _context.BeneficiaryPerson.AsNoTracking()
                                 on b.Child1 equals child1.Id
                                 where b.PolicyId == policyId
                                 select child1).FirstOrDefault();
                if (Child1Obj != null)
                {
                    var x = new DTOBeneficiaryPerson();
                    x.DateOfBirth = Child1Obj.DateOfBirth;
                    x.Gender = new CommonNameDTO((int)Child1Obj.Gender, Child1Obj.Gender.ToString());
                    x.Name = Child1Obj.Name;
                    x.Id = Child1Obj.Id;
                    x.DisabilityCertificate = new Domain.Common.CommonFileModel();
                    if (x.DisabilityCertificate != null)
                    {
                        x.DisabilityCertificate.Id = Child1Obj.Id;
                        x.DisabilityCertificate.Name = Child1Obj.DisabilityDocumentName;
                        x.DisabilityCertificate.Url = Child1Obj.DisabilityDocumentUrl;

                    }
                    policy.beneficiaries.Child1 = x;
                }

                var Child2Obj = (from b in _context.BeneficiaryDetails.AsNoTracking()
                                 join child2 in _context.BeneficiaryPerson.AsNoTracking()
                                 on b.Child2 equals child2.Id
                                 where b.PolicyId == policyId
                                 select child2).FirstOrDefault();

                if (Child2Obj != null)
                {
                    var x = new DTOBeneficiaryPerson();
                    x.DateOfBirth = Child2Obj.DateOfBirth;
                    x.Gender = new CommonNameDTO((int)Child2Obj.Gender, Child2Obj.Gender.ToString());
                    x.Name = Child2Obj.Name;
                    x.Id = Child2Obj.Id;
                    x.DisabilityCertificate = new Domain.Common.CommonFileModel();
                    if (x.DisabilityCertificate != null)
                    {
                        x.DisabilityCertificate.Id = Child2Obj.Id;
                        x.DisabilityCertificate.Name = Child2Obj.DisabilityDocumentName;
                        x.DisabilityCertificate.Url = Child2Obj.DisabilityDocumentUrl;

                    }
                    policy.beneficiaries.Child2 = x;
                }

                var Parent1Obj = (from b in _context.BeneficiaryDetails.AsNoTracking()
                                  join parent1 in _context.BeneficiaryPerson.AsNoTracking()
                                  on b.Parent1 equals parent1.Id
                                  where b.PolicyId == policyId
                                  select parent1).FirstOrDefault();

                if (Parent1Obj != null)
                {
                    var x = new DTOBeneficiaryPerson();
                    x.DateOfBirth = Parent1Obj.DateOfBirth;
                    x.Gender = new CommonNameDTO((int)Parent1Obj.Gender, Parent1Obj.Gender.ToString());
                    x.Name = Parent1Obj.Name;
                    x.Id = Parent1Obj.Id;
                    policy.beneficiaries.Parent1 = x;
                }

                var Parent2Obj = (from b in _context.BeneficiaryDetails.AsNoTracking()
                                  join child2 in _context.BeneficiaryPerson.AsNoTracking()
                                  on b.Child2 equals child2.Id
                                  where b.PolicyId == policyId
                                  select child2).FirstOrDefault();

                if (Parent2Obj != null)
                {
                    var x = new DTOBeneficiaryPerson();
                    x.DateOfBirth = Parent2Obj.DateOfBirth;
                    x.Gender = new CommonNameDTO((int)Parent2Obj.Gender, Parent2Obj.Gender.ToString());
                    x.Name = Parent2Obj.Name;
                    x.Id = Parent2Obj.Id;
                    policy.beneficiaries.Parent2 = x;
                }
                var InLaw1Obj = (from b in _context.BeneficiaryDetails.AsNoTracking()
                                 join inlaw1 in _context.BeneficiaryPerson.AsNoTracking()
                                 on b.InLaw1 equals inlaw1.Id
                                 where b.PolicyId == policyId
                                 select inlaw1).FirstOrDefault();

                if (InLaw1Obj != null)
                {
                    var x = new DTOBeneficiaryPerson();
                    x.DateOfBirth = InLaw1Obj.DateOfBirth;
                    x.Gender = new CommonNameDTO((int)InLaw1Obj.Gender, InLaw1Obj.Gender.ToString());
                    x.Name = InLaw1Obj.Name;
                    x.Id = InLaw1Obj.Id;
                    policy.beneficiaries.InLaw1 = x;
                }

                var InLaw2Obj = (from b in _context.BeneficiaryDetails.AsNoTracking()
                                 join inlaw2 in _context.BeneficiaryPerson.AsNoTracking()
                                 on b.InLaw2 equals inlaw2.Id
                                 where b.PolicyId == policyId
                                 select inlaw2).FirstOrDefault();

                if (InLaw2Obj != null)
                {
                    var x = new DTOBeneficiaryPerson();
                    x.DateOfBirth = InLaw2Obj.DateOfBirth;
                    x.Gender = new CommonNameDTO((int)InLaw2Obj.Gender, InLaw2Obj.Gender.ToString());
                    x.Name = InLaw2Obj.Name;
                    x.Id = InLaw2Obj.Id;
                    policy.beneficiaries.InLaw2 = x;
                }




                var NomineeObj = (from b in _context.BeneficiaryDetails.AsNoTracking()
                                  join n in _context.BeneficiaryPerson.AsNoTracking()
                                  on b.Nominee equals n.Id
                                  where b.PolicyId == policyId
                                  select n
                                   ).FirstOrDefault();

                if (NomineeObj != null)
                {


                    var x = new DTOBeneficiaryPerson();
                    x.DateOfBirth = NomineeObj.DateOfBirth;
                    x.Gender = new CommonNameDTO((int)NomineeObj.Gender, NomineeObj.Gender.ToString());
                    x.NomineeRelation = new CommonNameDTO((int)NomineeObj.NomineeRelation, NomineeObj.NomineeRelation.ToString());

                    x.Name = NomineeObj.Name;
                    x.Id = NomineeObj.Id;
                    policy.Nominee = x;
                }
                else
                {
                    var nomineeUserObj = (from b in _context.UserNomineeDetail.AsNoTracking()
                                          where b.UserId == policy.UserId
                                          select b
                                 ).FirstOrDefault();
                    if (nomineeUserObj != null)
                    {
                        var x = new DTOBeneficiaryPerson();
                        x.DateOfBirth = nomineeUserObj.DateOfBirth;
                        x.Gender = new CommonNameDTO((int)nomineeUserObj.Gender, nomineeUserObj.Gender.ToString());
                        x.NomineeRelation = new CommonNameDTO((int)nomineeUserObj.NomineeRelation, nomineeUserObj.NomineeRelation.ToString());
                        x.Name = nomineeUserObj.Name;
                        x.Id = 0;
                        policy.Nominee = x;
                    }
                }

                
                var payment = _context.PaymentDetails
                                      .AsNoTracking()
                                      .Where(x => x.PolicyId == policyId)
                                      .OrderByDescending(pd => pd.Id)
                                      .FirstOrDefault();
                if (payment != null)
                {
                    policy.PaymentDetails = new DTOPaymentDetails();
                    policy.PaymentDetails.PaymentModeId = (int)payment.PaymentMode;
                    policy.PaymentDetails.PaymentTypeId = (int)payment.PaymentType;
                    policy.PaymentDetails.IsPaymentConfirmed = payment.IsPaymentConfirmed;
                    policy.AmountPaid = 0;


                    if (payment.PaymentMode == PaymentMode.Online)
                    {
                        policy.PaymentDetails.Online = new PaymentModeOnline();
                    }
                    else
                    {
                        policy.PaymentDetails.Offline = new PaymentModeOffline();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return policy;
        }

        public async Task<DTOPolicy> GetBeneficieryDetails(int userId)
        {
            DTOPolicy policy = new DTOPolicy();

            try
            {
                policy.beneficiaries = new DTOBeneficiaryDetails();
                policy.Nominee = new DTOBeneficiaryPerson();
             

                
             

                //Beneficiary details
                policy.beneficiaries = new DTOBeneficiaryDetails();
                var spouseObj =await _context.UserSpouseDetail
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (spouseObj != null)
                {
                    var x = new DTOBeneficiaryPerson();
                    x.DateOfBirth = spouseObj.DateOfBirth;
                    x.Gender = new CommonNameDTO((int)spouseObj.Gender, spouseObj.Gender.ToString());
                    x.Name = spouseObj.Name;
                    x.Id = 0;
                    policy.beneficiaries.Spouse = x;
                }

                var NomineeObj =await _context.UserNomineeDetail.FirstOrDefaultAsync(x => x.UserId == userId);


                if (NomineeObj != null)
                {


                    var x = new DTOBeneficiaryPerson();
                    x.DateOfBirth = NomineeObj.DateOfBirth;
                    x.Gender = new CommonNameDTO((int)NomineeObj.Gender, NomineeObj.Gender.ToString());
                    x.NomineeRelation = new CommonNameDTO((int)NomineeObj.NomineeRelation, NomineeObj.NomineeRelation.ToString());

                    x.Name = NomineeObj.Name;
                    x.Id = 0;
                    policy.Nominee = x;
                }
               
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return policy;
        }

        private List<DTOPolicies> CreatePolicies(int headerId)
        {
            var policies = new List<DTOPolicies>();

            policies = (from pd in _context.PolicyProductDetails.AsNoTracking()
                        join p in _context.Product.AsNoTracking()
                       on pd.ProductId equals p.Id
                        where pd.PolicyHeaderId == headerId
                        select new DTOPolicies
                        {
                            OrderId = headerId,
                            ProductId = pd.ProductId.Value,
                            ProductName = p.ProductName,
                            SumInsured = pd.SumInsured.Value,
                            TopupSumInsured = pd.TopupSumInsured.Value,
                            Premium = pd.TotalProductPremimum.Value,
                            PremiumReceipt = ""
                        }).Distinct().ToList();


            return policies;
        }
        public async Task<DataReturnPolicy> GetMyPolicies(DataFilter<DTOMyPolicies> filter, int userId)
        {
            var ret = new DataReturnPolicy();
            try
            {
                int numberOfRecords = 0;
                List<DTOMyPolicies> myPolicies = new List<DTOMyPolicies>();
                var paymentData = await (from ph in _context.PolicyHeader.AsNoTracking()
                                         join p in _context.PaymentDetails.AsNoTracking()
                                         on ph.Id equals p.PolicyId
                                         into PaymentDetails
                                         from pc in PaymentDetails.DefaultIfEmpty() // This ensures a left join
                                         where ph.UserId == filter.userId && ph.CampaignId == filter.CampaignId
                                         select new
                                         {
                                             ph,
                                             pc
                                         }
                                  ).Distinct().ToListAsync();

                foreach (var policy in paymentData)
                {
                    DTOMyPolicies dTOMyPolicies = new DTOMyPolicies();
                    if (policy.ph != null)
                    {
                        dTOMyPolicies.OrderId = policy.ph.Id;
                        dTOMyPolicies.IsOrderfreez = policy.ph.IsProfilePreez.HasValue ? policy.ph.IsProfilePreez.Value : false;
                        dTOMyPolicies.OrderDate = policy.ph.CreatedAt.HasValue ? policy.ph.CreatedAt.Value : DateTime.Now;
                        dTOMyPolicies.PaymentStatus = Convert.ToString(policy.ph.PaymentStatus);
                        dTOMyPolicies.AmountPaid = policy.ph.TotalPaidPremimum.HasValue ?
                            policy.ph.TotalPaidPremimum.Value : 0;
                        dTOMyPolicies.TotalPremium = policy.ph.TotalPremimum.HasValue ?
                            policy.ph.TotalPremimum.Value : 0;
                        if (policy.pc != null)
                        {
                            dTOMyPolicies.PaymentType = Convert.ToString(policy.pc.PaymentType);
                            dTOMyPolicies.PaymentMode = Convert.ToString(policy.pc.PaymentMode);
                        }
                        else
                        {
                            dTOMyPolicies.PaymentType = "-";
                            dTOMyPolicies.PaymentMode = "-";
                        }
                        dTOMyPolicies.policies = new List<DTOPolicies>();

                        dTOMyPolicies.policies = CreatePolicies(policy.ph.Id);

                        var isexists = myPolicies.Any(x => x.OrderId == dTOMyPolicies.OrderId);
                        if (!isexists)
                            myPolicies.Add(dTOMyPolicies);
                    }
                }
                ret.Contents = myPolicies;
                ret.Source = "policies";
                numberOfRecords = myPolicies.Count();
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
                ret.ErrorMessage = ex.Message;
                Log.Error(ex.Message);
            }
            return ret;
        }

        public async Task<DTOPaymentReceipt> GetPaymentReceipt(int policyId)
        {
            DTOPaymentReceipt receipt = new DTOPaymentReceipt();

            var payment = await _context.PolicyHeader.AsNoTracking()
                .Where(x => x.Id == policyId).FirstOrDefaultAsync();
            if (payment != null)
            {
                receipt.OrderNumber = policyId;
                receipt.TotalPremium = payment.TotalPremimum;
                receipt.AmountPaid = payment.TotalPaidPremimum;
                var user = await _context.Applicationuser.AsNoTracking()
                    .Where(x => x.Id == payment.UserId).FirstOrDefaultAsync();
                if (user != null)
                {
                    receipt.Name = user.FullName;
                }
                receipt.PaymentReceiptDetails = new List<DTOPaymentReceiptDetails>();
                var paymentdetails = await _context.PaymentDetails.AsNoTracking()
                 .Where(x => x.PolicyId == policyId).ToListAsync();
                foreach (var item in paymentdetails)
                {
                    DTOPaymentReceiptDetails dTOPaymentReceipt = new DTOPaymentReceiptDetails();

                    dTOPaymentReceipt.PaymentDate = item.PaymentDate;
                    if (item.PaymentAcceptedDate != null)
                        dTOPaymentReceipt.PaymentAcceptedDate = item.PaymentAcceptedDate;
                    else
                        dTOPaymentReceipt.PaymentAcceptedDate = item.PaymentDate;
                    dTOPaymentReceipt.PaymentMode = item.PaymentType.ToString();
                    dTOPaymentReceipt.AmountPaid = item.AmountPaid;
                    receipt.PaymentReceiptDetails.Add(dTOPaymentReceipt);
                }

            }

            return receipt;
        }

    }
}
