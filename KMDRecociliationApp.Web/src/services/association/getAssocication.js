import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get association
 * @param {number} id - association id
 * @returns
 */
const getAssociation = async (id) => {
  try {
    let response = await httpClient.get(`/association/${id}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getAssociation;
