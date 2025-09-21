import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to delete user by id
 * @param {number} userId
 * @returns
 */
const deleteUser = async (userId) => {
  try {
    const response = await httpClient.post(
      `/customerprofile/deleteuser/${userId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default deleteUser;
