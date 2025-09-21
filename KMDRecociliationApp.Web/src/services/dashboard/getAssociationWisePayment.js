import { handleApiError, httpClient, Result } from "../../utils";
/**
 * Api call to get association wise payments
 * @param {number} page
 * @param {number} pageSize
 * @param {number} associationId
 * @returns
 */
const getAssociationWisePayment = async (page, pageSize, associationId) => {
  try {
    const response = await httpClient.get(
      `/dashboard/getassociationwisepayment?page=${page}&pageSize=${pageSize}&associationId=${associationId}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getAssociationWisePayment;
