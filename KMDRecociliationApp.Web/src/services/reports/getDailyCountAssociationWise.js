import { handleApiError, httpClient, Result } from "../../utils";

const getDailyCountAssociationWise = async (date) => {
  try {
    const response = await httpClient.get(
      `/reports/getdailycountassociationwise?reportDate=${date}`,
      { responseType: "blob" }
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getDailyCountAssociationWise;
