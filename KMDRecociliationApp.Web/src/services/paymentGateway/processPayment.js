import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to initiate online payment
 * @param {object} paymentDetails
 * @returns
 */
const processPayment = async (paymentDetails) => {
  try {
    const response = await httpClient.post(
      `/gateway/processpayment`,
      paymentDetails
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default processPayment;
