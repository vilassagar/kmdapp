import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to verify otp
 * @param {object} credentials mobile number and otp
 * @returns
 */
const verifyOTP = async (credentials) => {
  try {
    const response = await httpClient.post(
      `/authenticate/verifyotp`,
      credentials,
      {
        headers: {
          SecretAPIKey: import.meta.env.VITE_KMDAPISECRETKEY,
        },
      }
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default verifyOTP;
