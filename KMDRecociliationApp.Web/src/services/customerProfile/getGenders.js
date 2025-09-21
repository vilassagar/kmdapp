import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get genders
 * @returns
 */
const getGenders = async () => {
  try {
    const response = await httpClient.get(`/customerprofile/getgenders`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getGenders;
