import { handleApiError, httpClient, Result } from "../../utils";

const getcampaignlistbyuserid = async (userId,associationId) => {
  try {
    const response = await httpClient.get(
      `/campaigns/getcampaignlistbyuserid?userId=${userId}&associationId=${associationId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getcampaignlistbyuserid;
