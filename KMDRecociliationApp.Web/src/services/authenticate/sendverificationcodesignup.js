import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to sendverificationcodesignup
 * @param {object} credentials user mobile number and otp
 * @returns
 */
const sendverificationcodesignup = async (credentials) => {
  try {
    const response = await httpClient.post(`/authenticate/sendverificationcodesignup`, credentials, {
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

export default sendverificationcodesignup;
