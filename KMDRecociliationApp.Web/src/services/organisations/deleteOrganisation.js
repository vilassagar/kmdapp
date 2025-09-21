import { handleApiError, httpClient, Result } from "../../utils";

const deleteOrganistion = async (id) => {
  try {
    const response = await httpClient.delete(`/organisation/${id}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default deleteOrganistion;
