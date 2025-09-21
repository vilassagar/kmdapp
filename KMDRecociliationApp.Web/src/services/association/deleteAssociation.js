import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to delete Campaign by id
 * @param {number} campaignId
 * @returns
 */
const deleteAssociation = async (associationId) => {
  try {
    const response = await httpClient.delete(`/association/${associationId}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default deleteAssociation;
