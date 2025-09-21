import { handleApiError, httpClient, Result } from "../../utils";

const downloadApplicationUserTemplate = async () => {
  try {
    let response = await httpClient.post(
      "/importdata/downloadapplicationuserstemplate",
      null,
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
export default downloadApplicationUserTemplate;
