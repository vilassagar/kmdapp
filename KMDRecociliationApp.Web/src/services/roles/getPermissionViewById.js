import { handleApiError, httpClient, Result } from "../../utils";
/**
 *
 * @returns list of permissions for the role by id
 */
const getPermissionViewById = async (id) => {
  try {
    let response = await httpClient.get(`/roles/permissionview/${id}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPermissionViewById;
