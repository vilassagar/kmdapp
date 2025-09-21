import { handleApiError, httpClient, Result } from "../../utils";
/**
 *
 * @returns list of permissions for the role
 */
const getPermissionView = async (id) => {
  try {
    let response = await httpClient.get(`/roles/getpermissionview/${id}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPermissionView;
