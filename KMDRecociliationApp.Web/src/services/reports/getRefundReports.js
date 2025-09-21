import { handleApiError, httpClient, Result } from "../../utils";

const getRefundReports = async (association) => {
  try {
    const response = await httpClient.post(
      `/reports/getrefundreports`,
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

export default getRefundReports;
