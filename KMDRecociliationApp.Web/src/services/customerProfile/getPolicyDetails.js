import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get order details by id
 * @param {number} policyId
 * @returns
 */
const getPolicyDetails = async (policyId,campaignId) => {
  try {
    const response = await httpClient.get(
      `/customerprofile/getpolicydetails?policyId=${policyId}&campaignId=${campaignId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPolicyDetails;
