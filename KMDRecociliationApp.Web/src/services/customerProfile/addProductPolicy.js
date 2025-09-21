import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save product purchase details
 * @param {object} policy
 * @param {string | number} step
 * @returns
 */
const addProductPolicy = async (policy, step) => {
  try {
    let response = await httpClient.post(
      `/customerprofile/addproductpolicy?step=${step}`,
      policy
    );

    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default addProductPolicy;
