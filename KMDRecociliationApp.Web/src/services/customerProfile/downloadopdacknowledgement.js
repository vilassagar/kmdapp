import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get payent receipt details
 * @param {number} policyId
 * @returns
 */
const downloadopdacknowledgement = async (policyId) => {
  try {
    const response = await httpClient.get(
      `/digitreceipt/downloadopdacknowledgement?policyId=${policyId}`, {
        responseType: "blob", // Important for binary data
      }
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};
export default downloadopdacknowledgement;
