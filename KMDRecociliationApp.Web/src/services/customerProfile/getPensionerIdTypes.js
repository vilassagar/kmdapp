import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get genders
 * @returns
 */
const getPensionerIdTypes = async () => {
  try {
    const response = await httpClient.get(`/customerprofile/getPensionerIdTypes`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPensionerIdTypes;
