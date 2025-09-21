import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get products for user to purchase
 * @param {number | string} userId
 * @returns
 */
const getProductList = async (userId) => {
  try {
    const response = await httpClient.get(
      `/customerprofile/getproductlist?userId=${userId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getProductList;
