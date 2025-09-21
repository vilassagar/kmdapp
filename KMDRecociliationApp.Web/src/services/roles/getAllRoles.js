import { handleApiError, httpClient, Result } from "../../utils";

const getAllRoles = async () => {
  try {
    let response = await httpClient.get(`/roles/getallroles`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getAllRoles;
