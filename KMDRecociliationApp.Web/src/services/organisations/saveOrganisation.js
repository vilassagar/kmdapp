import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save and update permission
 * @param {number} id - permission id
 * @param {object} role - permission data
 * @returns
 */
const saveOrganistion = async (organisation) => {
  try {
    let response = await httpClient.post(`/organisation/`, organisation);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default saveOrganistion;
