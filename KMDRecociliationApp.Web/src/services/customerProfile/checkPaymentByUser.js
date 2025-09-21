import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get payments by users
 * @returns
 */
const checkPaymentByUsers = async (userId) => {
  try {
    const response = await httpClient.get(
      `/customerprofile/checkpaymentbyuser?userId=${userId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default checkPaymentByUsers;
