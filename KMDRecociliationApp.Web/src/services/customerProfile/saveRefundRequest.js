import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save refund request
 * @param {object} refundrequest
 * @returns
 */

const saveRefundRequest = async (refundrequest) => {
  try {
    let response = await httpClient.post(
      `/customerprofile/AddRefundRequest`,
      refundrequest
    );

    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default saveRefundRequest;
