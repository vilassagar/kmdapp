import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get user by id
 * @param {number} userId
 * @returns
 */
const getUser = async (userId) => {
  try {
    const response = await httpClient.get(
      `/customerprofile/getuser?id=${userId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getUser;
