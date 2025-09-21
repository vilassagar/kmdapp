import { handleApiError, httpClient, Result } from "../../utils";

const getAssociationExtract = async (associationId) => {
  try {
    const response = await httpClient.get(
      `/reports/getassociationextract?id=${associationId}`,
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

export default getAssociationExtract;
