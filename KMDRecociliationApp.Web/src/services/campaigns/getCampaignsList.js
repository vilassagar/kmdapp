import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get campaigns list
 * @param {number} page
 * @param {number} pageSize
 * @param {string} searchTerm
 * @returns
 */
const getCampaignsList = async (associationId, filter) => {
  try {
    const response = await httpClient.get(
      `/campaigns/getcampaignlist?associationId=${associationId}&filter=${filter}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getCampaignsList;
