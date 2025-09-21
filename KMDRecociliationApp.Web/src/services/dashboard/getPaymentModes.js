import { handleApiError, httpClient, Result } from "../../utils";

/**
 *
 * @returns
 */

const getPaymentModes = async (associationId) => {
  try {
    const response = await httpClient.get(
      `/dashboard/getpaymentmodes?associationId=${associationId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPaymentModes;
