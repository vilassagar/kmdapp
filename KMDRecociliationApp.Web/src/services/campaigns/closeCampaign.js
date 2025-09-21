import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to close campaign
 * @param {number} campaignId
 * @returns
 */
const closeCampaign = async (campaignId) => {
  try {
    let response = await httpClient.patch(
      `/campaigns/closecampaign/${campaignId}`
    );

    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default closeCampaign;
