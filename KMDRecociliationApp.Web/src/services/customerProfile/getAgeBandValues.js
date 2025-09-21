import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get user types
 * @returns
 */
const getAgeBandValues = async () => {
  try {
    const response = await httpClient.get(`/customerprofile/getageband`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getAgeBandValues;
