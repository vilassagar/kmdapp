import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get product by id
 * @param {number} productId
 * @returns
 */
const getProductById = async (productId) => {
  try {
    const response = await httpClient.get(`/product/${productId}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getProductById;
