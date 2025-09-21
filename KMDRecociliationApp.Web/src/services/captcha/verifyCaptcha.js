import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to verify captcha
 * @param {string} token captcha token
 * @returns
 */
const verifyCaptcha = async (token) => {
  try {
    const response = await httpClient.post(
      `/captcha/verifycaptcha`,
      {
        token: token,
      },
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

export default verifyCaptcha;
