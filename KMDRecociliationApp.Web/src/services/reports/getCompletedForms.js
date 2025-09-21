import { handleApiError, httpClient, Result } from "../../utils";

const getCompletedForms = async (association) => {
  try {
    const response = await httpClient.post(
      `/reports/getcompletedforms`,
      association,
      {
        responseType: "blob",
      }
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getCompletedForms;
