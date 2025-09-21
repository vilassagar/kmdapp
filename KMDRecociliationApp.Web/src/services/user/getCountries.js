import { handleApiError, httpClient, Result } from "../../utils";

const getCountries = async () => {
  try {
    const response = await httpClient.get(`/user/getcountries`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getCountries;
