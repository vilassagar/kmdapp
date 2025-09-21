import { handleApiError, httpClient, Result } from "../../utils";

const getStates = async () => {
  try {
    const response = await httpClient.get(`/user/getstates`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getStates;
