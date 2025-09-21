using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICampaignRepository
{
    Task<bool> SendCampaignMessagesAsync(DTOCampaign campaign);
}
