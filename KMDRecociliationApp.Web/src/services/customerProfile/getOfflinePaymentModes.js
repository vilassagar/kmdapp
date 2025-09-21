import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get offline payment modes
 * @returns
 */
const getOfflinePaymentModes = async () => {
  try {
    const response = await httpClient.get(
      `/customerprofile/getofflinepaymentmodes`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getOfflinePaymentModes;
