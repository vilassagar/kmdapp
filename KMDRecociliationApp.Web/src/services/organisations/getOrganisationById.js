import { handleApiError, httpClient, Result } from "../../utils";

/**
 *
 * @param {*} id
 * @returns
 */
const getOrganisationById = async (id) => {
  try {
    let response = await httpClient.get(`/organisation/${id}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getOrganisationById;
