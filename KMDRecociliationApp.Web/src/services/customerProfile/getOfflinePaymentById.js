import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get offline payment by ID
 * @returns
 */
const getOfflinePaymentById = async (paymentId) => {
  try {
    const response = await httpClient.get(
      `/customerprofile/getofflinepayment?paymentId=${paymentId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getOfflinePaymentById;
