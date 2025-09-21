import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get campaign by id
 * @param {number} campaignId
 * @returns
 */
const getCampaignById = async (campaignId) => {
  try {
    const response = await httpClient.get(`/campaigns/${campaignId}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getCampaignById;
