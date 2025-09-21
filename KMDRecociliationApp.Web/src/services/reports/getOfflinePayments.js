import { handleApiError, httpClient, Result } from "../../utils";

const getOfflinePayments = async (association) => {
  try {
    const response = await httpClient.post(
      `/reports/getofflinepayments`,
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

export default getOfflinePayments;
