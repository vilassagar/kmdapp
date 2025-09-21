import { handleApiError, httpClient, Result } from "../../utils";

/**
 *
 * @returns
 */

const getOfflinePayments = async (associationId) => {
  try {
    const response = await httpClient.get(
      `/dashboard/getofflinepayments?associationId=${associationId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getOfflinePayments;
