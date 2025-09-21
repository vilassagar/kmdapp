import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get role by id
 * @param {Number} id
 * @returns
 */
const getRole = async (id) => {
  try {
    let response = await httpClient.get(`/roles/${id}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getRole;
