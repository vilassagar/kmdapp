import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get associations
 * @returns
 */
const getAssociationsByIdFilter = async (associationId,filter) => {
  try {


   const response= await httpClient.get(`/customerprofile/getassociations?associationId=${associationId}&filter=${filter} `);


    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getAssociationsByIdFilter;
