import { handleApiError, httpClient, Result } from "../../utils";

const deleteRole = async (id) => {
  try {
    const response = await httpClient.delete(`/roles/${id}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default deleteRole;
