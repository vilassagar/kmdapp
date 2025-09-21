import { handleApiError, httpClient, Result } from "../../utils";

/**
 *
 * @returns
 */

const getCompletedPayments = async (associationId) => {
  try {
    const response = await httpClient.get(
      `/dashboard/getcompletedpayments?associationId=${associationId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getCompletedPayments;
