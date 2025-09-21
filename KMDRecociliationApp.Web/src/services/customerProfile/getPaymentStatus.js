import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get genders
 * @returns
 */
const getPaymentStatuses = async () => {
  try {
    const response = await httpClient.get(`/customerprofile/getpaymentstatus`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPaymentStatuses;
