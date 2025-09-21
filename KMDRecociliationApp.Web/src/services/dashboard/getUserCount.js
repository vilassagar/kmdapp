import { handleApiError, httpClient, Result } from "../../utils";

/**
 *
 * @returns
 */

const getUserCount = async (associationId) => {
  try {
    const response = await httpClient.get(
      `/dashboard/getusercount?associationId=${associationId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getUserCount;
