import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get organisations
 * @returns
 */
const getApplicantOrganizations = async () => {
  try {
    const response = await httpClient.get(`/customerprofile/getapplicantorganizations`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getApplicantOrganizations;
