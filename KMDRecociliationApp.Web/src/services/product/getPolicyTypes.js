import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get policy types
 * @returns
 */
const getPolicyTypes = async () => {
  try {
    const response = await httpClient.get(`/product/getpolicytypes`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPolicyTypes;
