import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save and update permission
 * @param {number} id - permission id
 * @param {object} entity - permission data
 * @returns
 */
const savePermission = async (id, permission) => {
  try {
    let response = await httpClient.patch(`/permissions/${id}`, permission);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default savePermission;
