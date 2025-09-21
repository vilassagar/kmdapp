import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to verify user for login
 * @param {object} credentials user mobile number and otp
 * @returns
 */
const login = async (credentials) => {
  try {
    const response = await httpClient.post(`/authenticate/login`, credentials, {
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

export default login;
