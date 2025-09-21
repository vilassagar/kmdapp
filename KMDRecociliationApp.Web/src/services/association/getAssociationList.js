import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get product list
 * @param {number} page
 * @param {number} pageSize
 * @param {string} searchTerm
 * @returns
 */
const getAssociationList = async (page, pageSize, searchTerm) => {
  try {
    const response = await httpClient.get(
      `/association?page=${page}&pageSize=${pageSize}&search=${searchTerm}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getAssociationList;
