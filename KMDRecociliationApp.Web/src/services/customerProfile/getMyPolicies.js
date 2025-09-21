import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Ap call to get my policies
 * @param {number} page
 * @param {number} pageSize
 * @param {string} searchTerm
 * @param {number} userId
 * @returns
 */
const getMyPolicies = async (page, pageSize, searchTerm, userId,campaignId) => {
  try {
    const response = await httpClient.get(
      `/customerprofile/getmypolicies?page+${page}&pageSize=${pageSize}&search=${searchTerm}&userId=${userId}&campaignId=${campaignId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getMyPolicies;
