import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get states
 * @returns
 */
const getStates = async () => {
  try {
    const response = await httpClient.get(`/customerprofile/getstates`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getStates;
