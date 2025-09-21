import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get getexceldata
 * @param {number} page
 * @param {number} pageSize
 * @param {string} searchTerm
 * @returns
 */
const getexceldata = async (page, pageSize, searchTerm) => {
  try {
    const response = await httpClient.get(
      `/applicants/export?page=${page}&pageSize=${pageSize}&search=${searchTerm}&format=xlsx`,
      {
        responseType: "blob",
      }
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getexceldata;
