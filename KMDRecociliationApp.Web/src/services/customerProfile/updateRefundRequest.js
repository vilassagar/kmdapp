import { handleApiError, httpClient, Result } from "../../utils";

/**
 * api call to update refund request
 * @param {number} id
 * @param {object} refundrequest
 * @returns
 */

const updateRefundRequest = async (id, refundrequest) => {
  try {
    let response = await httpClient.patch(
      `/customerprofile/UpdateRefundRequest/${id}`,
      refundrequest
    );

    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default updateRefundRequest;
