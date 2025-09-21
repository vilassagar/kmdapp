import { handleApiError, httpClient, Result } from "../../utils";

/**
 *
 * @returns
 */

const getCampaigns = async () => {
  try {
    const response = await httpClient.get(`/dashboard/getcampaigns`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getCampaigns;
