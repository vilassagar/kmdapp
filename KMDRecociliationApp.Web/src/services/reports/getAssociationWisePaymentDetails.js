import { handleApiError, httpClient, Result } from "../../utils";

const getAssociationWisePaymentDetails = async (association) => {
  try {
    const response = await httpClient.post(
      `/reports/getassociationwisepaymentdetails`,
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

export default getAssociationWisePaymentDetails;
