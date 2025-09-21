import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get validate by users
 * @returns
 */
const validatepolicypurchase = async (userId) => {
  try {
    const response = await httpClient.get(
      `/customerprofile/validatepolicypurchase?userId=${userId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default validatepolicypurchase;
