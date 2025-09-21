import { handleApiError, httpClient, Result } from "../../utils";

/**
 *
 * @returns
 */

const getDashboardData = async (campaignId, associationId) => {
  try {
    const response = await httpClient.get(`/dashboard/getdashboarddata?campaignId=${campaignId}&associationId=${associationId}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getDashboardData;
