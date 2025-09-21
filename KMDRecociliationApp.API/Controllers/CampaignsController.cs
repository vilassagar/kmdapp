using FluentValidation;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Data;
using KMDRecociliationApp.Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Results;
using Serilog;
using KMDRecociliationApp.Services;
using KMDRecociliationApp.Domain.Common;
using KMDRecociliationApp.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using KMDRecociliationApp.Domain.Enum;

namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[AllowAnonymous]
    public class CampaignsController : ApiBaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly CampaignRepository _campaignRepository;
        private readonly ApplicationDbContext _context;
        private readonly WhatsAppMessageService _whatsAppMessageService;
        public CampaignsController(CampaignRepository campaignRepository,
           ApplicationDbContext context, ILogger<UserController> logger
            , RoleRepository roleRepository, WhatsAppMessageService whatsAppMessageService) : base(context)

        {
            _logger = logger;
            _campaignRepository = campaignRepository;
            _context = context;
            _whatsAppMessageService = whatsAppMessageService;
        }


        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateCampaign(DTOCampaignData campaign)
        {
           
                _campaignRepository.CurrentUser = HttpContext.User;
                Campaigns retCampaign = null;
                if (campaign.CampaignId == 0)
                {
                    var tempCampaign = await _campaignRepository.CheckCampaignName(campaign);
                    _campaignRepository.UserEmail = Convert.ToString(User.FindFirst("email")?.Value);
                    // _rolePermissionRepo.UserEmail = _associationRepository.UserEmail;



                    if (tempCampaign == null)
                    {
                        retCampaign = await _campaignRepository.CreateCampaignAsync(campaign);
                        if (retCampaign != null && retCampaign.Id > 0)
                        {
                            return Ok(new { Message = "Campaign created successfully", campaignId = retCampaign.Id });
                        }
                        else
                        {
                            return BadRequest(new { Message = "Campaign create Failed" });
                        }


                    }
                    else
                    {

                        string error = $"Campaign Name: {campaign.CampaignName} already exists";
                        return Conflict(new { Message = error });
                    }
                }
                
                return Ok();
            
            
        }
       
        [HttpPatch("{campaignId}")]
        public async Task<IActionResult> UpdateCampaign(DTOCampaignData campaign, int campaignId)

        {

            _campaignRepository.CurrentUser = HttpContext.User;

            Campaigns retCampaign = null;

            if (campaignId != 0)

            {

                var tempCampaign = await _campaignRepository.CheckCampaignName(campaign, true);

                _campaignRepository.UserEmail = Convert.ToString(User.FindFirst("email")?.Value);

                if (tempCampaign == null)

                {

                    retCampaign = await _campaignRepository.UpdateCampaignAsync(campaignId, campaign);

                    if (retCampaign != null && retCampaign.Id > 0)

                    {

                        return Ok(new { Message = "Campaign updated successfully" });

                    }

                    else

                    {

                        return BadRequest(new { Message = "Campaign update failed" });

                    }

                }

                else

                {

                    string error = $"Campaign Name: {campaign.CampaignName} already exists";
                    return Conflict(new { Message = error });

                }

            }

          
            return Ok();

        }

        [HttpGet("")]
        public async Task<IActionResult> GetCampaigns([FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)

        {

            if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
            {
                Sorting = new DataSorting() { SortName = "Id", SortDirection = "desc" };
            }
            DataFilter<CampaignResult> filter = new DataFilter<CampaignResult>()
            {
                PageNumber = searchDTO.Page,
                Limit = searchDTO.pageSize,
                Filter = null,
                SortName = Sorting.SortName,
                Search = searchDTO.Search,
                SortDirection = Sorting.SortDirection
            };
            DataReturn<CampaignResult> dataReturn = new DataReturn<CampaignResult>();
            dataReturn = _campaignRepository.GetCampaigns(filter);
            return await Task.FromResult(Ok(dataReturn));


        }

        [HttpPost("UploadCampaignTemplate")]
        public async Task<IActionResult> UploadCampaignTemplate([FromForm] CommonFileModel fileModel)
        {
            if (fileModel != null && fileModel.isUpdateFile && fileModel.Id > 0)
            {
                if (fileModel.File != null &&
                    fileModel.File.Length > 0)
                {
                    CommonFileModel commonFile = new CommonFileModel();
                    if (fileModel.File.Length != 0)
                    {
                        commonFile = await new DataHelpers().UploadFile(fileModel.File, DataHelpers.CAMPAIGNTEMPLATE, fileModel.Id.Value);
                    }


                    if (fileModel != null && fileModel.Id > 0 && commonFile != null
                        && !string.IsNullOrWhiteSpace(commonFile.Name)
                        && !string.IsNullOrWhiteSpace(commonFile.Url))
                    {
                        var campaign = await _context.Campaigns.Where(x => x.Id == fileModel.Id)
                            .FirstOrDefaultAsync();
                        if (campaign != null)
                        {
                            campaign.DocumentName = commonFile.Name;
                            campaign.DocumentURL = commonFile.Url;
                            _context.Campaigns.Update(campaign);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                return Ok(new { Message = "Campaigns template file uploaded successfully", CampaignId = fileModel.Id });

            }
            else
            {
                return Problem("error while uploading the file in the Campaigns");
            }

        }
        [HttpGet("GetCampaignList")]
        public async Task<IActionResult> GetCampaignList(int associationId)
        {
            try
            {
                if (associationId == -1)
                {
                    associationId = 0;
                }
                var campaigns = await _campaignRepository.GetAllCampaignsAsync(associationId);

                if (campaigns == null || !campaigns.Any())
                {
                    return NotFound(new { Message = "No campaigns found" });
                }

                return Ok(campaigns); // `campaigns` is already a list of `CampaignDto`.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching campaigns list");
                return StatusCode(500, new { Message = "An error occurred while fetching campaigns list" });
            }
        }

        [HttpGet("GetCampaignListByUserId")]
        public async Task<IActionResult> GetCampaignListByUserId(int userId, int associationId)
        {
            try
            {
                if (userId == -1)
                {
                    userId = 0;
                }
                var campaigns = await _campaignRepository.GetCampaignListByUserId(userId, associationId);

                if (campaigns == null || !campaigns.Any())
                {
                    return NotFound(new { Message = "No campaigns found" });
                }

                return Ok(campaigns); // `campaigns` is already a list of `CampaignDto`.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching campaigns list");
                return StatusCode(500, new { Message = "An error occurred while fetching campaigns list" });
            }
        }

        [HttpGet("{campaignId:int}")]
        public async Task<IActionResult> GetCampaignById(int campaignId)
        {

            try
            {
                var roledetails = _campaignRepository.GetCampaignById(campaignId).Result;
                return await Task.FromResult(Ok(roledetails));
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in Getbyid {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }

        }

       
        [HttpPatch("CloseCampaign/{campaignId}")]
        public async Task<IActionResult> CloseCampaign(int campaignId)
        {
            var result = await _campaignRepository.CloseCampaignAsync(campaignId);
            if (result)
            {
                return Ok("Campaign updated successfully.");
            }
            else
            {
                return NotFound("Campaign not found.");
            }
        }



        [HttpPost("SendWhatsAppNotification")]
        public async Task<IActionResult> SendWhatsAppNotification(DTOExecuteCampaign campaign)
        {
            // Fetch recipients based on campaignId and associationId
            var recipients = await _campaignRepository.GetRecipients(campaign.CampaignId, campaign.AssociationId);

            if (recipients == null || !recipients.Any())
            {
                return NotFound("No recipients found for the given campaign and association.");
            }

            var objcampaign = await _context.Campaigns.AsNoTracking()
                .Where(x => x.Id == campaign.CampaignId).FirstOrDefaultAsync();
            string documentURL = "";
            string documentName = "";
            int executeCampaignId = 0;
            if (objcampaign != null)
            {
                documentURL = objcampaign.DocumentURL;
                documentName = objcampaign.CampaignName;
                objcampaign.SentStatus = CampaignStatus.Sent;
                _context.Campaigns.Update(objcampaign);
                await _context.SaveChangesAsync();

                ExecuteCampaign executeCampaign = new ExecuteCampaign();
                executeCampaign.AssociationId = campaign.AssociationId;
                executeCampaign.CampaignId = campaign.CampaignId.Value;
                executeCampaign.SentDate = DateTime.Now;
                _context.ExecuteCampaign.Update(executeCampaign);
                await _context.SaveChangesAsync();
                if (executeCampaign.Id > 0)
                {
                    executeCampaignId = executeCampaign.Id;
                }
            }
            var responseList = new List<WhatsAppResponse>();
            if (executeCampaignId > 0)
            {
                foreach (var recipient in recipients)
                {
                    var receipt = recipient;// "9168410206";
                    campaign.MessageTemplateName = "cyber_opd_new_design";
                    bool isSent = await _campaignRepository
                        .NotifyCampaignUser(receipt, campaign.MessageTemplateName, documentURL, documentName);

                    responseList.Add(new WhatsAppResponse
                    {
                        Recipient = recipient,
                        Status = isSent ? "Success" : "Failed"
                    });
                    var user = await _context.Applicationuser.AsNoTracking().
                        FirstOrDefaultAsync(x => x.MobileNumber == receipt);
                    if (user != null)
                    {
                        CampaignMembersDetails campaignMembers = new CampaignMembersDetails();
                        campaignMembers.AssociationId = campaign.AssociationId;
                        campaignMembers.CampaignId = campaign.CampaignId.Value;
                        campaignMembers.SentDate = DateTime.Now;
                        campaignMembers.SentStatus = isSent;
                        campaignMembers.UserId = user.Id;
                        campaignMembers.ExecuteCampaignId = executeCampaignId;
                        _context.CampaignMembersDetails.Update(campaignMembers);
                        await _context.SaveChangesAsync();
                    }

                }
            }
            return Ok(responseList);
        }


    }
}
