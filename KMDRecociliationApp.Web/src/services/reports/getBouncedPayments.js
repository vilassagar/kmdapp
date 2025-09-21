import { handleApiError, httpClient, Result } from "../../utils";

const getBouncedPayments = async (association) => {
  try {
    const response = await httpClient.post(
      `/reports/getbouncedpayments`,
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

export default getBouncedPayments;
