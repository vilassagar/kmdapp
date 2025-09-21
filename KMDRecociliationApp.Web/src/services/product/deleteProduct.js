import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to delete product by id
 * @param {number} productId
 * @returns
 */
const deleteProduct = async (productId) => {
  try {
    const response = await httpClient.delete(`/product/${productId}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default deleteProduct;
