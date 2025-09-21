import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to create the user
 * @param {object} user
 * @returns
 */
const register = async (user) => {
  try {
    const response = await httpClient.post(`/user/register`, user, {
      headers: {
        SecretAPIKey: import.meta.env.VITE_KMDAPISECRETKEY,
      },
    });
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default register;
