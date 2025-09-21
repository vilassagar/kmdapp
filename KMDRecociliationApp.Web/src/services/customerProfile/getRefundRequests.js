import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get refund requests
 * @param {number} page
 * @param {number} pageSize
 * @param {string} searchTerm
 * @param {number} associationId
 * @returns
 */

const getRefundRequests = async (page, pageSize, searchTerm, associationId,campaignId) => {
  try {
    const response = await httpClient.get(
      `/customerprofile/getrefundrequests?page=${page}&pageSize=${pageSize}&search=${searchTerm}&associationId=${associationId}&campaignId=${campaignId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getRefundRequests;
