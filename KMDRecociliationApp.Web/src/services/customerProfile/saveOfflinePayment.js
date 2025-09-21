import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save offline payment
 * @param {object} offlinepayment
 * @returns
 */
const saveOfflinePayment = async (offlinepayment) => {
  try {
    let response = await httpClient.post(
      `/customerprofile/offlinepayment`,
      offlinepayment
    );

    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default saveOfflinePayment;
