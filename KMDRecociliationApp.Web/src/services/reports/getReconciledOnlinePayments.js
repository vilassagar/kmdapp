import { handleApiError, httpClient, Result } from "../../utils";

const getReconciledOnlinePayments = async (association) => {
  try {
    const response = await httpClient.post(
      `/reports/getreconcileedonlinepayments`,
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

export default getReconciledOnlinePayments;
