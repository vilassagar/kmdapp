import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save product
 * @param {number} productId
 * @param {object} product
 * @returns
 */
const saveProduct = async (productId, product) => {
  try {
    let response;
    if (productId) {
      response = await httpClient.patch(`/product/${productId}`, product);
    } else {
      response = await httpClient.post(`/product`, product);
    }
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default saveProduct;
