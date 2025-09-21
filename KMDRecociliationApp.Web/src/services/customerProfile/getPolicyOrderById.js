import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get offline payment by ID
 * @returns
 */
const getPolicyOrderById = async (paymentId) => {
  try {
    const response = await httpClient.get(
      `/customerprofile/getpolicyorderbyid?policyId=${paymentId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPolicyOrderById;
