import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get associations
 * @returns
 */
const getAssociations = async (filter) => {
  try {


   const response= await httpClient.get(`/customerprofile/getassociations?filter=${filter} `);


    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getAssociations;
