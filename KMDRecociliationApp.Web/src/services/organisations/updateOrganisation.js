import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save and update permission
 * @param {number} id - permission id
 * @param {object} entity - permission data
 * @returns
 */
const updateOrganisation = async (id, organisation) => {
  try {
    let response = await httpClient.put(`/organisation/${id}`, organisation);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default updateOrganisation;
