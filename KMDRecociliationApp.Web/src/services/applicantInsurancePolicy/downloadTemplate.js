import { handleApiError, httpClient, Result } from "../../utils";

const downloadTemplate = async () => {
  try {
    let response = await httpClient.post(
      "/applicants/downloadtemplate",
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
export default downloadTemplate;
