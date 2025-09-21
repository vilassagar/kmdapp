import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get refund request
 * @param {number} id
 * @returns
 */
const getRefundRequest = async (id) => {
  try {
    let response = await httpClient.get(
      `/customerprofile/getrefundrequest?requestId=${id}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getRefundRequest;
