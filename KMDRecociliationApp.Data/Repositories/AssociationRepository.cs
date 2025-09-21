using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using KMDRecociliationApp.Data.Common;
using KMDRecociliationApp.Data.Helpers;
using KMDRecociliationApp.Domain.Common;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Data;

namespace KMDRecociliationApp.Data.Repositories
{
    public class AssociationRepository : MainHeaderRepo<Association>
    {
        ApplicationDbContext context = null;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public AssociationRepository(ILoggerFactory logger, ApplicationDbContext appContext) : base(appContext)
        {
            context = appContext;
            _logger = logger.CreateLogger("AssociationRepository");
        }
        public DataReturn<CampaignResult> GetCampaigns(DataFilter<CampaignResult> filter)
        {
            var ret = new DataReturn<CampaignResult>();
            int numberOfRecords = 0;

            var query = from c in context.Campaigns.AsNoTracking()
                        join ca in context.CampaignAssociations.AsNoTracking()
                        on c.Id equals ca.CampaignId
                        join a in context.Association.AsNoTracking()
                        on ca.AssociationId equals a.OraganisationId
                        select new CampaignResult
                        {
                            Id = c.Id,
                            campaignId = c.Id,
                            CampaignName = c.CampaignName,
                            StartDate = c.StartDate,
                            EndDate = c.EndDate,
                            remainingDays = CampaignResult.GetDaysDifferenceFromToday((DateTime)c.EndDate),
                            isCampaignOpen = c.isCampaignOpen.Value,
                            CampaignAssociations = context.CampaignAssociations.AsNoTracking()
                                .Where(ca2 => ca2.CampaignId == c.Id)
                                .Select(ca2 => new CampaignAssociationsResult
                                {
                                    CampaignId = ca2.CampaignId,
                                    AssociationId = ca2.AssociationId,
                                    AssociationName = context.Association
                                        .FirstOrDefault(a2 => a2.OraganisationId == ca2.AssociationId).AssociationName,
                                    members = context.Applicationuser.Count(au => au.AssociationId == ca2.AssociationId)
                                }).ToList(),
                            CampaignProducts = context.CampaignProducts.AsNoTracking()
                                .Where(cp => cp.CampaignId == c.Id)
                                .Select(cp => new CampaignProductsResult
                                {
                                    CampaignId = cp.CampaignId,
                                    ProductId = cp.ProductId,
                                    ProductName = context.Product
                                        .FirstOrDefault(p => p.Id == cp.ProductId).ProductName
                                }).ToList()
                        };

            // Apply filters if provided
            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(x =>
                    (x.CampaignName != null && x.CampaignName.Contains(filter.Search)) ||
                    x.CampaignAssociations.Any(ca =>
                        ca.AssociationName != null && ca.AssociationName.Contains(filter.Search))
                );
            }

            // Apply specific filters
            if (filter.Filter != null)
            {
                // Filter by CampaignName
                if (!string.IsNullOrEmpty(filter.Filter.CampaignName))
                {
                    query = query.Where(x => x.CampaignName != null &&
                        x.CampaignName.Contains(filter.Filter.CampaignName));
                }

                // Filter by AssociationName
                if (filter.Filter.CampaignAssociations != null &&
                    filter.Filter.CampaignAssociations.Any() &&
                    !string.IsNullOrEmpty(filter.Filter.CampaignAssociations.First().AssociationName))
                {
                    string associationNameFilter = filter.Filter.CampaignAssociations.First().AssociationName;
                    query = query.Where(x => x.CampaignAssociations.Any(ca =>
                        ca.AssociationName != null && ca.AssociationName.Contains(associationNameFilter)));
                }
            }

            // Order by UpdatedAt in descending order
            query = query.OrderByDescending(x => context.Campaigns.FirstOrDefault(a => a.Id == x.campaignId).UpdatedAt);

            // Get total count before pagination
            numberOfRecords = query.Count();

            // Apply pagination
            var paginatedResults = filter.Limit > 0
                ? query.Skip((filter.PageNumber - 1) * filter.Limit)
                      .Take(filter.Limit)
                      .ToList()
                : query.ToList();

            ret.Contents = paginatedResults;
            ret.Source = "Campaigns";
            ret.ResultCount = numberOfRecords;
            ret.StatusCode = 200;

            // Set up paging information
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

            // Set up sorting information
            DataSorting sorting = new DataSorting();
            sorting.SortName = filter.SortName;
            sorting.SortDirection = filter.SortDirection;
            ret.Sorting = sorting;

            return ret;
        }
        public DataReturn<AssociationResult> GetAssociations(DataFilter<AssociationResult> filter)
        {
            var ret = new DataReturn<AssociationResult>();
            var objList = new List<Association>();
            int numberOfRecords = 0;


            var query = from o in context.Organisations.AsNoTracking()
                        join a in context.Association.AsNoTracking() on o.Id equals a.OraganisationId
                        select new AssociationResult
                        {
                            Id = a.Id,
                            AssociationId = a.Id,
                            ParentAssociationId = a.ParentAssociationId.HasValue ? a.ParentAssociationId.Value : 0,
                            AssociationName = a.AssociationName,
                            OraganisationId = o.Id,
                            OrganisationName = o.Name,
                            Address = a.Address1,

                            Members = context.Applicationuser.AsNoTracking().Count(au => au.AssociationId == a.Id),
                        };

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.AssociationName.ToLower().Contains(searchTerm) ||
                    x.OrganisationName.ToLower().Contains(searchTerm) ||
                    x.Address.Contains(searchTerm));
            }
            // Order by UpdatedAt in descending order
            query = query.OrderByDescending(x => context.Association.FirstOrDefault(a => a.Id == x.AssociationId).UpdatedAt);

            // Get total count before pagination
            numberOfRecords = query.Count();


            var paginatedResults = filter.Limit > 0
         ? query.Skip((filter.PageNumber - 1) * filter.Limit)
               .Take(filter.Limit)
               .ToList()
         : query.ToList();


            ret.Contents = paginatedResults;// objList.Select(x => new AssociationResult().Copy(x: x)).ToList();
            ret.Source = "Association";
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

        public async Task<DTOAssociation> GetAssociationById(int id)
        {
            var ret = await (from ass in context.Association.AsNoTracking()
                             join bank in context.AssociationBankDetails.AsNoTracking()
                             on ass.Id equals bank.AssociationId
                             join state in context.AddressState.AsNoTracking()
                              on ass.StateId equals state.Id
                             join country in context.AddressCountry.AsNoTracking()
                             on ass.CountryId equals country.Id

                             join onePay in context.AssociationOnePayDetails.AsNoTracking()
                             on ass.Id equals onePay.AssociationId
                              into associationOnePayGroup
                             from OnePay in associationOnePayGroup.DefaultIfEmpty()

                             join contacts in context.AssociationContactDetails.AsNoTracking()

                             on ass.Id equals contacts.AssociationId
                              into associationContactGroup
                             from Contact in associationContactGroup.DefaultIfEmpty()

                             join messages in context.AssociationMessageDetails.AsNoTracking()
                             on ass.Id equals messages.AssociationId
                             into associationMessageGroup
                             from message in associationMessageGroup.DefaultIfEmpty()

                             join org in context.Organisations.AsNoTracking()
                            on ass.OraganisationId equals org.Id
                             join parentass in context.Association.AsNoTracking()
                               on ass.ParentAssociationId equals parentass.Id into parentassGroup
                             from parentass in parentassGroup.DefaultIfEmpty()
                             where ass.Id == id
                             select new
                             {
                                 Association = ass,
                                 BankDetails = bank,
                                 Oraganisation = org,
                                 ParentAssociation = parentass,
                                 Country = country,
                                 State = state,
                                 OnePay = OnePay,
                                 ContactDetails = (from contacts in context.AssociationContactDetails.AsNoTracking()
                                                   where contacts.AssociationId == ass.Id
                                                   select contacts).ToList(),
                                 MessageDetails = (from messages in context.AssociationMessageDetails.AsNoTracking()
                                                   where messages.AssociationId == ass.Id
                                                   select messages).ToList()

                             }).FirstOrDefaultAsync();
            if (ret != null)
            {
                var dTOAssociation = new DTOAssociation()
                    .Copy(ret.Association, ret.BankDetails, ret.Oraganisation, ret.ParentAssociation,
                    ret.State, ret.Country, ret.ContactDetails, ret.MessageDetails, ret.OnePay);
                return await Task.FromResult(dTOAssociation);
            }
            else
            {
                return await Task.FromResult(new DTOAssociation());
            }
        }

        public async Task<Association> CheckAssociationName(DTOAssociation association, bool update = false)
        {
            if (update)
            {
                return await context.Association.AsNoTracking()
                .Where(x => x.AssociationName == association.AssociationName
                && x.Id != association.Id
                ).FirstOrDefaultAsync();
            }
            else
            {
                return await context.Association.AsNoTracking()
                .Where(x => x.AssociationName == association.AssociationName).FirstOrDefaultAsync();
            }
        }
        //public async Task<ApplicationUser> RegisterUserAsync(UserDTO userDTO)
        public async Task<Association?> CreateAssociationAsync(IFormFile ?MandateFile, IFormFile ?QRCodeFile, DTOAssociation dtoAssociation, int updatedBy)
        {
            CommonFileModel mandate = new CommonFileModel();
            CommonFileModel QRCode = new CommonFileModel();

            Association tempAssociation = new Association();
            tempAssociation.IsActive = true;
            tempAssociation.AssociationName = dtoAssociation.AssociationName;
            tempAssociation.OraganisationId = dtoAssociation.Organisation.Id;
            if (dtoAssociation.ParentAssociation != null)
            {
                tempAssociation.ParentAssociationId = dtoAssociation.ParentAssociation.Id == null ? 0 : dtoAssociation.ParentAssociation.Id;
            }
            else
                tempAssociation.ParentAssociationId = null;
            tempAssociation.AssociationCode = dtoAssociation.AssociationCode;
            tempAssociation.Address1 = dtoAssociation.Address1;
            tempAssociation.Address2 = dtoAssociation.Address2;
            tempAssociation.PINCode = dtoAssociation.PINCode;
            tempAssociation.CountryId = dtoAssociation.Country.Id;
            tempAssociation.City = dtoAssociation.City;
            tempAssociation.OraganisationId = dtoAssociation.Organisation.Id;
            tempAssociation.StateId = dtoAssociation.State.Id;
            tempAssociation.AcceptOnePayPayment = dtoAssociation.AcceptOnePayPayment;
            //tempAssociation.OnePayId=dtoAssociation.OnePayId;
            tempAssociation.CreatedBy = updatedBy;
            tempAssociation.CreatedAt = DateTime.Now;
            tempAssociation.UpdatedBy = updatedBy;
            tempAssociation.UpdatedAt = DateTime.Now;
            AssociationBankDetails bankDetail = new AssociationBankDetails();

            // retAssociation = _associationRepository.Add(tempAssociation, _associationRepository.UserEmail, _associationRepository.UserFullName, "Association Added");
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {

                    context.Association.Add(tempAssociation);
                    await context.SaveChangesAsync();

                    // Ensure tempAssociation.Id is not 0 and was inserted successfully
                    if (tempAssociation.Id == 0)
                    {
                        throw new Exception("Association add failed.");
                    }

                    if (dtoAssociation.OnePayId != null)
                    {
                        AssociationOnePayDetails associationOnePayDetails = new AssociationOnePayDetails();
                        associationOnePayDetails.AssociationId = tempAssociation.Id;
                        associationOnePayDetails.OnepayUrl = "";// dtoAssociation.associationOnePayDetails.OnepayUrl;
                        associationOnePayDetails.OnePayId = dtoAssociation.OnePayId;
                        context.AssociationOnePayDetails.Add(associationOnePayDetails);
                        await context.SaveChangesAsync();

                        // Ensure tempAssociation.Id is not 0 and was inserted successfully
                        if (associationOnePayDetails.Id == 0)
                        {
                            throw new Exception("Association one pay details add failed.");
                        }
                    }


                    if (dtoAssociation.Bank != null)
                    {
                        var dTOAssociationBankDetail = dtoAssociation.Bank;


                        bankDetail.AssociationId = tempAssociation.Id;
                        bankDetail.BankName = dTOAssociationBankDetail.BankName;
                        bankDetail.BranchName = dTOAssociationBankDetail.BranchName;
                        bankDetail.AccountName = dTOAssociationBankDetail.AccountName;
                        bankDetail.AccountNumber = dTOAssociationBankDetail.AccountNumber;
                        bankDetail.IFSCCode = dTOAssociationBankDetail.IFSCCode;
                        bankDetail.MICRCode = dTOAssociationBankDetail.MICRCode;
                        bankDetail.MendateId = tempAssociation.Id;
                        bankDetail.MendateName = mandate.Name;
                        bankDetail.MendateUrl = mandate.Url;
                        bankDetail.QRCodeId = tempAssociation.Id;
                        bankDetail.QRCodeName = QRCode.Name;
                        bankDetail.QRCodeUrl = QRCode.Url;

                        bankDetail.UpdatedAt = DateTime.Now;
                        bankDetail.CreatedAt = DateTime.Now;
                        bankDetail.CreatedBy = updatedBy;
                        bankDetail.UpdatedBy = updatedBy;
                        if (MandateFile != null && MandateFile.Length != 0)
                        {
                            mandate = await new DataHelpers().UploadFile(MandateFile, DataHelpers.ASSOCIATIONMANDATEFILE, tempAssociation.Id);
                        }

                        if (QRCodeFile != null && QRCodeFile.Length != 0)
                        {
                            QRCode = await new DataHelpers().UploadFile(QRCodeFile, DataHelpers.ASSOCIATIONQRCODEFILE, tempAssociation.Id);
                        }
                        if (mandate != null && mandate.Name != null)
                        {
                            bankDetail.MendateId = (int)bankDetail.AssociationId;
                            bankDetail.MendateName = mandate.Name;
                            bankDetail.MendateUrl = mandate.Url;
                        }
                        if (QRCode != null && QRCode.Name != null)

                        {
                            bankDetail.QRCodeId = (int)bankDetail.AssociationId;
                            bankDetail.QRCodeName = QRCode.Name;
                            bankDetail.QRCodeUrl = QRCode.Url;
                        }

                        context.AssociationBankDetails.Add(bankDetail);
                        await context.SaveChangesAsync();

                        if (bankDetail.Id == 0)
                        {
                            throw new Exception("Association Bank detail add failed.");
                        }


                    }

                    if (dtoAssociation.associationContactDetails != null && dtoAssociation.associationContactDetails.Any())
                    {
                        foreach (var dTOAssociationContactDetail in dtoAssociation.associationContactDetails)
                        {

                            AssociationContactDetails contactDetail = new AssociationContactDetails();
                            contactDetail.AssociationId = tempAssociation.Id;
                            contactDetail.FirstName = dTOAssociationContactDetail.FirstName;
                            contactDetail.LastName = dTOAssociationContactDetail.LastName;
                            contactDetail.Phone = dTOAssociationContactDetail.Phone;
                            contactDetail.Email = dTOAssociationContactDetail.Email;


                            contactDetail.UpdatedAt = DateTime.Now;
                            contactDetail.CreatedAt = DateTime.Now;
                            contactDetail.CreatedBy = updatedBy;
                            contactDetail.UpdatedBy = updatedBy;

                            context.AssociationContactDetails.Add(contactDetail);
                            await context.SaveChangesAsync();

                            if (contactDetail.Id == 0)
                            {
                                throw new Exception("Association Contact detail add failed.");
                            }
                        }
                    }

                    if (dtoAssociation.associationMessages != null && dtoAssociation.associationMessages.Any())
                    {
                        foreach (var dTOAssociationMessage in dtoAssociation.associationMessages)
                        {

                            AssociationMessageDetails messageDetails = new AssociationMessageDetails();
                            messageDetails.AssociationId = tempAssociation.Id;
                            messageDetails.Name = dTOAssociationMessage.Name;
                            messageDetails.Template = dTOAssociationMessage.Template;
                            messageDetails.IsApproved = dTOAssociationMessage.IsApproved;
                            // messageDetails.Id = dTOAssociationMessage.Id;


                            messageDetails.UpdatedAt = DateTime.Now;
                            messageDetails.CreatedAt = DateTime.Now;
                            messageDetails.CreatedBy = updatedBy;
                            messageDetails.UpdatedBy = updatedBy;

                            context.AssociationMessageDetails.Add(messageDetails);
                            await context.SaveChangesAsync();

                            if (messageDetails.Id == 0)
                            {
                                throw new Exception("Association Contact detail add failed.");
                            }
                        }
                    }


                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error("Error while add association", ex);
                    throw;
                }
            }

           

           
            return tempAssociation;
        }


        public async Task<Association?> UpdateAssociationAsync(int Id, IFormFile MandateFile, IFormFile QRCodeFile, DTOAssociation dtoAssociation, int updatedBy)
        {
            CommonFileModel mandate = new CommonFileModel();
            CommonFileModel QRCode = new CommonFileModel();
            mandate = dtoAssociation.MandateFile;
            mandate.Id = Id;
            QRCode = dtoAssociation.QrCodeFile;
            QRCode.Id = Id;
           



            Association tempAssociation = new Association();
            var associationobj = context.Association.AsNoTracking()
                .Where(x => x.Id == Id).FirstOrDefault();
            if (associationobj != null)
            {
                tempAssociation = associationobj;
            }

            tempAssociation.AssociationName = dtoAssociation.AssociationName;
            tempAssociation.AssociationCode = dtoAssociation.AssociationCode;
            tempAssociation.OraganisationId = dtoAssociation.Organisation.Id;
            if (dtoAssociation.ParentAssociation != null)
            {
                tempAssociation.ParentAssociationId = dtoAssociation.ParentAssociation.Id == 0 ? null : dtoAssociation.ParentAssociation.Id;
            }
            tempAssociation.Address1 = dtoAssociation.Address1;
            tempAssociation.Address2 = dtoAssociation.Address2;
            tempAssociation.PINCode = dtoAssociation.PINCode;
            tempAssociation.CountryId = dtoAssociation.Country.Id;
            tempAssociation.City = dtoAssociation.City;
            tempAssociation.OraganisationId = dtoAssociation.Organisation.Id;
            tempAssociation.StateId = dtoAssociation.State.Id;
            tempAssociation.AcceptOnePayPayment = dtoAssociation.AcceptOnePayPayment;
           
            tempAssociation.UpdatedBy = updatedBy;
            tempAssociation.UpdatedAt = DateTime.Now;
            AssociationBankDetails bankDetail = new AssociationBankDetails();

             using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {

                    context.Association.Update(tempAssociation);
                    await context.SaveChangesAsync();

                    // Ensure tempAssociation.Id is not 0 and was inserted successfully
                    if (tempAssociation.Id == 0)
                    {
                        throw new Exception("Association add failed.");
                    }

                    if (dtoAssociation.OnePayId != null)
                    {
                        AssociationOnePayDetails associationOnePayDetails = new AssociationOnePayDetails();

                        var associationOnePayDetailsobj = context.AssociationOnePayDetails.AsNoTracking()
                             .Where(x => x.AssociationId == tempAssociation.Id).FirstOrDefault();
                        if (associationOnePayDetailsobj != null)
                        {
                            associationOnePayDetails = associationOnePayDetailsobj;
                        }
                        associationOnePayDetails.AssociationId = tempAssociation.Id;

                        associationOnePayDetails.OnepayUrl = "";// dtoAssociation.associationOnePayDetails.OnepayUrl;
                        associationOnePayDetails.OnePayId = dtoAssociation.OnePayId;
                        context.AssociationOnePayDetails.Update(associationOnePayDetails);
                        await context.SaveChangesAsync();

                        // Ensure tempAssociation.Id is not 0 and was inserted successfully
                        if (associationOnePayDetails.Id == 0)
                        {
                            throw new Exception("Association one pay details add failed.");
                        }
                    }


                    if (dtoAssociation.Bank != null)
                    {
                        var dTOAssociationBankDetail = dtoAssociation.Bank;

                        var bankDetailobj = context.AssociationBankDetails.AsNoTracking()
                            .Where(x => x.AssociationId == tempAssociation.Id).FirstOrDefault();
                        if (bankDetailobj != null)
                        {
                            bankDetail = bankDetailobj;
                        }
                        bankDetail.AssociationId = tempAssociation.Id;
                      
                        bankDetail.BankName = dTOAssociationBankDetail.BankName;
                        bankDetail.BranchName = dTOAssociationBankDetail.BranchName;
                        bankDetail.AccountName = dTOAssociationBankDetail.AccountName;
                        bankDetail.AccountNumber = dTOAssociationBankDetail.AccountNumber;
                        bankDetail.IFSCCode = dTOAssociationBankDetail.IFSCCode;
                        bankDetail.MICRCode = dTOAssociationBankDetail.MICRCode;
                        if (dtoAssociation.isMandateFileUpdated == true || dtoAssociation.isQrCodeFileUpdated == true)
                        {
                            if (dtoAssociation.isMandateFileUpdated == true)
                            {
                                bankDetail.MendateId = tempAssociation.Id;
                            }
                           
                            if (dtoAssociation.isQrCodeFileUpdated == true)
                            {
                                bankDetail.QRCodeId = tempAssociation.Id;
                            }

                        }
                        bankDetail.UpdatedAt = DateTime.Now;
                        bankDetail.UpdatedBy = updatedBy;
                        if (dtoAssociation.isMandateFileUpdated == true && MandateFile != null && MandateFile.Length != 0)
                        {
                            mandate = await new DataHelpers().UploadFile(MandateFile, DataHelpers.ASSOCIATIONMANDATEFILE, tempAssociation.Id);
                        }

                        if (dtoAssociation.isQrCodeFileUpdated == true && QRCodeFile != null && QRCodeFile.Length != 0)
                        {
                            QRCode = await new DataHelpers().UploadFile(QRCodeFile, DataHelpers.ASSOCIATIONQRCODEFILE, tempAssociation.Id);
                        }
                        if (mandate != null && mandate.Name != null)
                        {
                            bankDetail.MendateId = (int)bankDetail.AssociationId;
                            bankDetail.MendateName = mandate.Name;
                            bankDetail.MendateUrl = mandate.Url;
                        }
                        if (QRCode != null && QRCode.Name != null)

                        {
                            bankDetail.QRCodeId = (int)bankDetail.AssociationId;
                            bankDetail.QRCodeName = QRCode.Name;
                            bankDetail.QRCodeUrl = QRCode.Url;
                        }

                        context.AssociationBankDetails.Update(bankDetail);
                        await context.SaveChangesAsync();

                        if (bankDetail.Id == 0)
                        {
                            throw new Exception("Association Bank detail update failed.");
                        }


                        if (dtoAssociation.associationContactDetails != null && dtoAssociation.associationContactDetails.Any())
                        {
                            foreach (var dTOAssociationContactDetail in dtoAssociation.associationContactDetails)
                            {

                                AssociationContactDetails contactDetail = new AssociationContactDetails();
                                var AssociationContactDetailsobj = context.AssociationContactDetails.AsNoTracking()
                            .Where(x => x.AssociationId == tempAssociation.Id).FirstOrDefault();
                                if (AssociationContactDetailsobj != null)
                                {
                                    contactDetail = AssociationContactDetailsobj;
                                }
                                contactDetail.AssociationId = tempAssociation.Id;
                                contactDetail.FirstName = dTOAssociationContactDetail.FirstName;
                                contactDetail.LastName = dTOAssociationContactDetail.LastName;
                                contactDetail.Phone = dTOAssociationContactDetail.Phone;
                                contactDetail.Email = dTOAssociationContactDetail.Email;




                                if (dTOAssociationContactDetail.Id == 0)
                                {
                                    contactDetail.UpdatedAt = DateTime.Now;
                                    contactDetail.CreatedAt = DateTime.Now;
                                    contactDetail.CreatedBy = updatedBy;
                                    contactDetail.UpdatedBy = updatedBy;
                                    context.AssociationContactDetails.Add(contactDetail);
                                }
                                else
                                {
                                    contactDetail.UpdatedAt = DateTime.Now;
                                    contactDetail.UpdatedBy = updatedBy;
                                    context.AssociationContactDetails.Update(contactDetail);
                                }

                                await context.SaveChangesAsync();


                                if (contactDetail.Id == 0)
                                {
                                    throw new Exception("Association Contact detail add failed.");
                                }
                            }
                        }


                        await transaction.CommitAsync();
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error("Error while add association", ex);
                    throw;
                }
            }

            if (dtoAssociation.isMandateFileUpdated == true && MandateFile != null && MandateFile.Length != 0)
            {
                mandate = await new DataHelpers().UploadFile(MandateFile, DataHelpers.ASSOCIATIONMANDATEFILE, tempAssociation.Id);
            }

            if (dtoAssociation.isQrCodeFileUpdated == true && QRCodeFile != null && QRCodeFile.Length != 0)
            {
                QRCode = await new DataHelpers().UploadFile(QRCodeFile, DataHelpers.ASSOCIATIONQRCODEFILE, tempAssociation.Id);
            }
            
            return tempAssociation;
        }

      
    }
}
