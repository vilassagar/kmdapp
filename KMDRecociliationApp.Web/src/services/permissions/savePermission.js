import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save and update permission
 * @param {number} id - permission id
 * @param {object} entity - permission data
 * @returns
 */
const savePermission = async (permissionId, permission) => {
  try {
    if (permissionId === 0) {
      let response = await httpClient.post(`/permissions`, permission);
      const { data } = response;
      return Result.success(data);
    } else {
      let response = await httpClient.patch(
        `/permissions/${permissionId}`,
        permission
      );
      const { data } = response;
      return Result.success(data);
    }
  } catch (e) {
    return handleApiError(e);
  }
};

export default savePermission;
