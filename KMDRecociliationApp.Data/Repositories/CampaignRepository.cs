using DocumentFormat.OpenXml.Wordprocessing;
using KMDRecociliationApp.Data.Common;
using KMDRecociliationApp.Data.Helpers;
using KMDRecociliationApp.Domain.Common;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using KMDRecociliationApp.Domain.Results;
using KMDRecociliationApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Repositories
{
    public class CampaignRepository : MainHeaderRepo<Campaigns>
    {
        ApplicationDbContext context = null;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly WhatsAppMessageService _whatsAppMessageService;
        public CampaignRepository(ILoggerFactory logger, ApplicationDbContext appContext,WhatsAppMessageService whatsAppMessageService) : base(appContext)
        {
            context = appContext;
            _logger = logger.CreateLogger("CampaignRepository");
            _whatsAppMessageService = whatsAppMessageService;
        }

        public async Task<Campaigns> CheckCampaignName(DTOCampaignData campaign, bool update = false)
        {            
            if (update)
            {
                return await context.Campaigns.AsNoTracking()
                    .Where(x => x.CampaignName == campaign.CampaignName 
                    && x.Id != campaign.CampaignId).FirstOrDefaultAsync();
                    
            }
            else
            {
                return await context.Campaigns.AsNoTracking()
                    .Where(x => x.CampaignName == campaign.CampaignName)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<Campaigns?> CreateCampaignAsync(DTOCampaignData campaign)
        {
            Campaigns tempCampaign = new Campaigns();
            tempCampaign.IsActive = true;
            tempCampaign.CampaignName = campaign.CampaignName;
            tempCampaign.StartDate= campaign.StartDate;
            tempCampaign.EndDate= campaign.EndDate;
            tempCampaign.isCampaignOpen = true;
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    //tempCampaign.TemplateName = campaign.TemplateName;
                    //if (campaign.istemplateDocumentUpdated)
                    //{
                    //    tempCampaign.DocumentURL = campaign.TemplateDocumentUrl;
                    //    tempCampaign.DocumentName = campaign.TemplateDocumentName;
                    //}

                    tempCampaign.SentStatus = CampaignStatus.NotSent;
                    context.Campaigns.Add(tempCampaign);
                    await context.SaveChangesAsync();

                    if (tempCampaign.Id == 0)
                    {
                        throw new Exception("Campaign add failed.");
                    }

                    if (campaign.AssociationIds != null && campaign.AssociationIds.Any())
                    {
                        foreach (var campaignAssociation in campaign.AssociationIds.Split(","))
                        {

                            CampaignAssociations campaignAssociations = new CampaignAssociations();
                            campaignAssociations.CampaignId = tempCampaign.Id;
                            campaignAssociations.AssociationId =Convert.ToInt32(campaignAssociation);

                          

                            context.CampaignAssociations.Add(campaignAssociations);
                            await context.SaveChangesAsync();

                            if (campaignAssociations.Id == 0)
                            {
                                throw new Exception("Campaign Assoc detail add failed.");
                            }
                        }

                    }

                    if (campaign.ProductIds != null && campaign.ProductIds.Any())
                    {
                        foreach (var campaignProduct in campaign.ProductIds.Split(","))
                        {

                            CampaignProducts campaignProducts = new CampaignProducts();
                            campaignProducts.CampaignId = tempCampaign.Id;
                            campaignProducts.ProductId = Convert.ToInt32(campaignProduct);

                          

                            context.CampaignProducts.Add(campaignProducts);
                            await context.SaveChangesAsync();

                            if (campaignProducts.Id == 0)
                            {
                                throw new Exception("Campaign Product add failed.");
                            }
                        }

                    }

                    await transaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error("Error while add campaign", ex);
                    throw;
                }
            }
            return tempCampaign;
        }
        public DataReturn<CampaignResult> GetCampaigns(DataFilter<CampaignResult> filter)
        {
            var ret = new DataReturn<CampaignResult>();
            int numberOfRecords = 0;

            var query = from c in context.Campaigns.AsNoTracking()                       
                        select new CampaignResult
                        {
                            Id = c.Id,
                            campaignId = c.Id,
                            CampaignName = c.CampaignName,
                            StartDate = c.StartDate,
                            EndDate = c.EndDate,
                            remainingDays = CampaignResult.GetDaysDifferenceFromToday((DateTime)c.EndDate),
                            isCampaignOpen = c.isCampaignOpen.Value,
                           
                        };

            // Apply filters if provided
            if (filter.Filter!=null&&!string.IsNullOrEmpty(filter.Filter.CampaignName))
            {
                query = query.Where(x => x.CampaignName != null &&
                    x.CampaignName.Contains(filter.Filter.CampaignName));
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
        public async Task<DTOCampaign> GetCampaignById(int id)
        {
            // Validate input
            if (id <= 0)
            {
                throw new ArgumentException("Invalid campaign ID", nameof(id));
            }

            try
            {
               
                var campaign = await context.Set<Campaigns>()
                    .AsNoTracking()
                    .Where(c => c.Id == id)
                    .Select(c => new DTOCampaign
                    {
                        CampaignId = c.Id,
                        CampaignName = c.CampaignName,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                        isCampaignOpen = c.isCampaignOpen,
                        TemplateName=c.TemplateName,
                        TemplateDocument = new CommonFileModel()
                        {
                            Id = c.Id,
                            Name =c.DocumentName,
                            Url = c.DocumentURL
                        },
                        Associations = context.Set<CampaignAssociations>()
                            .AsNoTracking()
                            .Where(ca => ca.CampaignId == c.Id)
                            .Join(context.Set<Association>().AsNoTracking(),
                                ca => ca.AssociationId,
                                a => a.Id,
                                (ca, a) => new { CampaignAssociation = ca, Association = a })
                            .Join(context.Set<Organisation>().AsNoTracking(),
                                joined => joined.Association.OraganisationId,
                                o => o.Id,
                                (joined, o) => new
                                {
                                    joined.CampaignAssociation,
                                    joined.Association,
                                    Organisation = o
                                })
                            .Select(joined => new DtoAssociation
                            {
                                associationId = joined.Association.Id,
                                associationName = joined.Association.AssociationName,
                                organisationName = joined.Organisation.Name,
                               
                                members = joined.Association.Id > 0
                                    ? context.Applicationuser.AsNoTracking()
                                        .Count(x => x.AssociationId == joined.Association.Id)
                                    : 0
                            })
                            .ToList(),
                        Products = context.Set<CampaignProducts>()
                            .AsNoTracking()
                            .Where(cp => cp.CampaignId == c.Id)
                            .Join(context.Set<Product>().AsNoTracking(),
                                cp => cp.ProductId,
                                p => p.Id,
                                (cp, p) => new DtoProducts
                                {
                                    productId = p.Id,
                                    productName = p.ProductName,
                                    providerName = p.ProviderName,
                                    policyType = ((ProductPolicyType)(p.PolicyTypeId)).ToString(),
                                    // basePolicy = p.BasePolicy,
                                })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                return campaign;
            }
            catch (Exception ex)
            {
                // Log the exception
               // logger.LogError(ex, $"Error retrieving campaign with ID {id}");
                throw;
            }
        }
   
        public async Task<Campaigns?> UpdateCampaignAsync(int campaignId, DTOCampaignData DTOCampaign)
        {
            var campaign = await context.Campaigns.FindAsync(campaignId);

            if (campaign == null)
            {
                return null;
            }

            campaign.CampaignName = DTOCampaign.CampaignName;
            campaign.StartDate = DTOCampaign.StartDate;
            campaign.EndDate = DTOCampaign.EndDate;
            campaign.isCampaignOpen = true;
            
         
            // Update associations
            var existingAssociations = context.CampaignAssociations.Where(ca => ca.CampaignId == campaignId).ToList();
            context.CampaignAssociations.RemoveRange(existingAssociations);
            if (DTOCampaign.AssociationIds != null && DTOCampaign.AssociationIds.Any())
            {
                foreach (var associationId in DTOCampaign.AssociationIds.Split(","))
                {
                    context.CampaignAssociations.Add(new CampaignAssociations
                    { CampaignId = campaignId, AssociationId = Convert.ToInt32(associationId) });
                }
            }

            // Update products
            var existingProducts = context.CampaignProducts.Where(cp => cp.CampaignId == campaignId).ToList();
            context.CampaignProducts.RemoveRange(existingProducts);
            if (DTOCampaign.ProductIds != null && DTOCampaign.ProductIds.Any())
            {
                foreach (var productId in DTOCampaign.ProductIds.Split(","))
                {
                    context.CampaignProducts.Add(new CampaignProducts 
                    { CampaignId = campaignId, ProductId =Convert.ToInt32(productId) });
                }
            }

            await context.SaveChangesAsync();

            return campaign;
        }

        public async Task<bool> CloseCampaignAsync(int campaignId)
        {
            var campaign = await context.Campaigns.FindAsync(campaignId);
            if (campaign == null)
            {
                // Return false if the campaign is not found
                return false;
            }

            // Update the isCampaignOpen property to false
            campaign.isCampaignOpen = false;

            // Save changes to the database
            await context.SaveChangesAsync();

            // Return true to indicate success
            return true;
        }

        public async Task<IEnumerable<CampaignListDTO>> GetCampaignListByUserId(int userId, int associationId)
        {

            var obj =await (from c in context.Campaigns
                       join ca in context.CampaignAssociations
                 on c.Id equals ca.CampaignId
                 join u in context.Applicationuser on ca.AssociationId equals u.AssociationId
                       where u.Id == userId 
                       //&& u.AssociationId==associationId
                       orderby c.Id descending
                       select c).ToListAsync();
            List<CampaignListDTO> objlist = new List<CampaignListDTO>();
            foreach (var item in obj)
            {
                var data = new CampaignListDTO
                {
                    Id = item.Id,
                    Name = item.CampaignName,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    IsRunning = DateTime.Now.IsBetween(item.StartDate.Value, item.EndDate.Value)

                };
                objlist.Add(data);
            }
            return objlist;


        }

        public async Task<IEnumerable<CampaignListDTO>> GetAllCampaignsAsync(int associationId)
        {
            if (associationId == 0)
            {
                return await context.Campaigns
                   .OrderByDescending(c => c.Id)
              .Select(c => new CampaignListDTO
              {
                  Id = c.Id,
                  Name = c.CampaignName,
                  StartDate = c.StartDate,
                  EndDate = c.EndDate
              })
              .ToListAsync();
            }
            else
            {
                var obj = (from c in context.Campaigns
                           join ca in context.CampaignAssociations
                     on c.Id equals ca.CampaignId
                           where ca.AssociationId == associationId
                           orderby c.Id descending
                           select c).ToList();
                List<CampaignListDTO> objlist = new List<CampaignListDTO>();
                foreach (var item in obj)
                {
                    var data = new CampaignListDTO
                    {
                        Id = item.Id,
                        Name = item.CampaignName,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        IsRunning = DateTime.Now.IsBetween(item.StartDate.Value, item.EndDate.Value)

                    };
                    objlist.Add(data);
                }
                return objlist;
            }

        }

        public async Task<bool> NotifyCampaignUser(string recipient,string templatename
            , string _typeMediaTemplateUrl
            , string _typeMediaTemplateFilename)
        {
            return await _whatsAppMessageService.SendWhatsAppMessage(recipient, templatename, _typeMediaTemplateUrl, _typeMediaTemplateFilename);
        }

        public async Task<IEnumerable<string>> GetRecipients(int? campaignId, int? associationId)
        {
            // Validate input parameters
            if (!campaignId.HasValue || !associationId.HasValue)
            {
                return Enumerable.Empty<string>();
            }

            // Directly select the mobile numbers as strings
            var recipients = await (from c in context.Campaigns
                                    join ca in context.CampaignAssociations
                                    on c.Id equals ca.CampaignId
                                    join a in context.Association
                                    on ca.AssociationId equals a.Id
                                    join u in context.Applicationuser
                                    on a.Id equals u.AssociationId
                                    where c.Id == campaignId && a.Id == associationId
                                    select u.MobileNumber)
                                  .ToListAsync();
            return recipients;
        }
    }
}
