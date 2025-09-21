import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get associations
 * @returns
 */
const captureOnlinePaymentStatus = async (postobject) => {
  try {
    const response = await httpClient.get(
      `/customerprofile/captureonlinepaymentstatus`,
      postobject
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default captureOnlinePaymentStatus;
