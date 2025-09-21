import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get getApplicantList
 * @param {number} page
 * @param {number} pageSize
 * @param {string} searchTerm
 * @returns
 */
const getApplicantList = async (page, pageSize, searchTerm) => {
  try {
    const response = await httpClient.get(
      `/applicants/filter?page=${page}&pageSize=${pageSize}&search=${searchTerm}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getApplicantList;
