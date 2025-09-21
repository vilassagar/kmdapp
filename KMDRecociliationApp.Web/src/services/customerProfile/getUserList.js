import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get user list
 * @param {number} page
 * @param {number} pageSize
 * @param {string} searchTerm
 * @param {object} filters
 * @returns
 */
const getUserList = async (page, pageSize, searchTerm, filters) => {

  console.log("getUserList", filters);
  try {
    const response = await httpClient.get(
      `/customerprofile?page=${page}&pageSize=${pageSize}&search=${searchTerm}
      &userTypes=${filters?.userTypes}&userTypeId=${filters?.userTypes} &associations=${filters.associations}&associationId=${filters.associationId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getUserList;
