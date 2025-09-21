import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get user types
 * @returns
 */
const getUserTypes = async () => {
  try {
    const response = await httpClient.get(`/user/getusertypes`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getUserTypes;
