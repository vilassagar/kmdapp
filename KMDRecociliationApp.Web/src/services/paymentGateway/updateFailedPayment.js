import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to initiate online payment
 * @param {object} paymentDetails
 * @returns
 */
const updateFailedPayment = async (paymentDetails) => {
  try {
    const response = await httpClient.post(
      `/gateway/updatefailedpayment`,
      paymentDetails
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default updateFailedPayment;
