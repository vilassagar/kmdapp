import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to delete  by id
 * @param {number} Id
 * @returns
 */
const deleteApplicant = async (Id) => {
  try {
    const response = await httpClient.post(`/applicants/deleteapplicant/${Id}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default deleteApplicant;
