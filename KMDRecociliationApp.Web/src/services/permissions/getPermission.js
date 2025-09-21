import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save and update permission
 * @param {number} id - permission id
 * @param {object} entity - permission data
 * @returns
 */
const getPermission = async (id) => {
  try {
    let response = await httpClient.get(`/permissions/${id}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPermission;
