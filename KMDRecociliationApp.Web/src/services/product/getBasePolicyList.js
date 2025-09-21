import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get base policy list
 * @returns
 */
const getBasePolicyList = async () => {
  try {
    const response = await httpClient.get(`/product/getbasepolicylist`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getBasePolicyList;
