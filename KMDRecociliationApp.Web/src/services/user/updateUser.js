import { handleApiError, httpClient, Result } from "../../utils";

const updateUser = async (userId, user) => {
  try {
    const response = await httpClient.post(`/user/updateuser/${userId}`, user);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default updateUser;
