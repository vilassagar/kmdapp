import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save and update permission
 * @param {number} id - permission id
 * @param {object} entity - permission data
 * @returns
 */
const getPaymentById = async (id) => {
  try {
    let response = await httpClient.get(
      `/customerprofile/getpaymentbyid/${id}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPaymentById;
