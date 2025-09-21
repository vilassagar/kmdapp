import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get payments history
 * @param {number} page
 * @param {number} pageSize
 * @param {string} searchTerm
 * @param {number} userId
 * @returns
 */
const getPaymentHistory = async (page, pageSize, searchTerm, userId) => {
  try {
    const response = await httpClient.get(
      `/customerprofile/getpaymenthistory?page=${page}&pageSize=${pageSize}&search=${searchTerm}&userId=${userId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPaymentHistory;
