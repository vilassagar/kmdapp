import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get association based on organisation id
 * @param {number} id organisation id
 * @returns
 */
const getassociations = async (id) => {
  try {
    const response = await httpClient.get(`/user/getassociations/${id}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getassociations;
