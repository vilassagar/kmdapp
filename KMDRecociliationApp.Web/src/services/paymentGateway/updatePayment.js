import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to initiate online payment
 * @param {object} paymentDetails
 * @returns
 */
const updatePayment = async (paymentDetails) => {
  try {
    const response = await httpClient.post(
      `/gateway/updatepayment`,
      paymentDetails
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default updatePayment;
