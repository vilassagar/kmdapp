import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save refund request
 * @param {object} refundrequest
 * @returns
 */

const updatePolicyOrder = async (policyorder) => {
  try {
    let response = await httpClient.post(
      `/customerprofile/updatepolicyorder`,
      policyorder
    );

    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default updatePolicyOrder;
