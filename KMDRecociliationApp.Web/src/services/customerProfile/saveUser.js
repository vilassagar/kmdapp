import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save user
 * @param {object} user
 * @returns
 */
const saveUser = async (user) => {
  try {
    let response;
    if (user.userId) {
      response = await httpClient.patch(
        `/customerprofile/updateuser/${user.userId}`,
        user
      );
    } else {
      response = await httpClient.post(`/customerprofile/createuser`, user);
    }
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default saveUser;
