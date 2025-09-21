import { handleApiError, httpClient, Result } from "../../utils";

const getInsuranceCompanyReport = async (association) => {
  try {
    const response = await httpClient.post(
      `/reports/getinsurancecompanyreport`,
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

export default getInsuranceCompanyReport;
