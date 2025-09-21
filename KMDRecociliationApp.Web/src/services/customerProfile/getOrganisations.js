import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get organisations
 * @returns
 */
const getOrganisations = async () => {
  try {
    const response = await httpClient.get(`/customerprofile/getorganisations`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getOrganisations;
