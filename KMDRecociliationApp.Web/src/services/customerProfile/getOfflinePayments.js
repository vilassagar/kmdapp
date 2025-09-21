import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get offline payments
 * @param {number} page
 * @param {number} pageSize
 * @param {string} searchTerm
 * @param {number} userId
 * @param {number} associationId
 * @returns
 */
const getOfflinePayments = async (
  page,
  pageSize,
  searchTerm,
  userId,
  associationId,campaignId
) => {
  try {
    const response = await httpClient.get(
      `/customerprofile/getofflinepayments?page=${page}&pageSize=${pageSize}&search=${searchTerm}&userId=${userId}&associationId=${associationId}&campaignId=${campaignId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getOfflinePayments;
