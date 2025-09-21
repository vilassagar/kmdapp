import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to delete Campaign by id
 * @param {number} campaignId
 * @returns
 */
const deleteCampaign = async (campaignId) => {
  try {
    const response = await httpClient.delete(`/Campaign/${campaignId}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default deleteCampaign;
