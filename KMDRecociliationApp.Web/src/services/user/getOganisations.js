import { handleApiError, httpClient, Result } from "../../utils";

const getOrganisations = async () => {
  try {
    const response = await httpClient.get(`/user/getorganisations`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getOrganisations;
