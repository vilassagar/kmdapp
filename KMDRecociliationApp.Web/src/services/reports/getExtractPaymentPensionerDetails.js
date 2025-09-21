import { handleApiError, httpClient, Result } from "../../utils";

const getExtractPensionerPaymentDetails = async (
  paymentTypeId,
  paymentStatusId,campaignId,associationId
) => {
  try {
    const response = await httpClient.get(
      `/reports/getextractpensionerpaymentdetails?paymentTypeId=${paymentTypeId}
      &paymentStatusId=${paymentStatusId}&campaignId=${campaignId}&associationId=${associationId}`,
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

export default getExtractPensionerPaymentDetails;
