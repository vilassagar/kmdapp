import { handleApiError, httpClient, Result } from "../../utils";

const getCorrectionReport = async (association) => {
  try {
    const response = await httpClient.post(
      `/reports/getcorrectionreport`,
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

export default getCorrectionReport;
