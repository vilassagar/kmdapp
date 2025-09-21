import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get payment modes
 * @returns
 */
const getPaymentModes = async () => {
  try {
    const response = await httpClient.get(`/customerprofile/getpaymentmodes`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPaymentModes;
