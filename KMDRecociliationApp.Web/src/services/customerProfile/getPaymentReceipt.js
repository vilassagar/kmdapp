import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get payent receipt details
 * @param {number} policyId
 * @returns
 */
const getPaymentReceipt = async (policyId) => {
  try {
    const response = await httpClient.get(
      `/customerprofile/getpaymentreceipt?policyId=${policyId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPaymentReceipt;
