import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get Applicant
 * @param {number} id - Applicant id
 * @returns
 */
const getApplicant = async (id) => {
  try {
    let response = await httpClient.get(`/applicants/${id}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getApplicant;
