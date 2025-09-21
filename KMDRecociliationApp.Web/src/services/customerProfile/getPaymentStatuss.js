import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get genders
 * @returns
 */
const getPaymentStatuss = async () => {
  try {
    const response = await httpClient.get(`/customerprofile/getpaymentstatuss`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPaymentStatuss;
