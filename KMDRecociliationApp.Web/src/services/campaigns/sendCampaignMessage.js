import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save campaign
 * @param {object} campaign
 * @returns
 */
const sendCampaignMessage = async (campaign) => {
  try {
    let response;
    if (campaign) {      
      response = await httpClient.post(`/campaigns/sendwhatsappnotification`, campaign);
    }
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default sendCampaignMessage;
