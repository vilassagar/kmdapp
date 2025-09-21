import { handleApiError, httpClient, Result } from "../../utils";

const getRoles = async (page, pageSize, searchTerm) => {
  try {
    const response = await httpClient.get(
      `/roles?page=${page}&pageSize=${pageSize}&search=${searchTerm}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getRoles;
