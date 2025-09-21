import { handleApiError, httpClient, Result } from "../../utils";

const deletePermission = async (id) => {
  try {
    const response = await httpClient.delete(`/permissions/${id}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default deletePermission;
