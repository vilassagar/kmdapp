import { handleApiError, httpClient, Result } from "../../utils";

const getIncompleteTransactions = async (association) => {
  try {
    const response = await httpClient.post(
      `/reports/getincompletetransactions`,
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

export default getIncompleteTransactions;
