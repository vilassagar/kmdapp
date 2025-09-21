import { handleApiError, httpClient, Result } from "../../utils";

/**
 *
 * @returns
 */
const getOrganisations = async (page, pagesize, searchTerm ) => {
  try {
    const response = await httpClient.get(
      `/organisation?page=${page}&pagesize=${pagesize}&search=${searchTerm}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getOrganisations;
