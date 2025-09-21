import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get permissions
 * @returns
 */
const getPermissions = async () => {
  try {
    const response = await httpClient.get(`/customerprofile/getpermissions`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPermissions;
