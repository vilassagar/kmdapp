import { handleApiError, httpClient, Result } from "../../utils";

const getPermissions = async (page, pageSize, searchTerm) => {
  try {
    const response = await httpClient.get(
      `/permissions?page=${page}&pageSize=${pageSize}&searchTerm=${searchTerm}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getPermissions;
