import { handleApiError, httpClient, Result } from "../../utils";

const downloadAssociationTemplate = async () => {
  try {
    let response = await httpClient.post(
      "/importdata/downloadassociationtemplate",
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
export default downloadAssociationTemplate;
