import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save refund request
 * @param {object} refundrequest
 * @returns
 */

const freezPolicyOrder = async (policyorder,IsFreez) => {
  try {
    let response = await httpClient.post(
      `/customerprofile/freezpolicyorder?policyId=${policyorder}&isFreez=${IsFreez}`     
    );

    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default freezPolicyOrder;
