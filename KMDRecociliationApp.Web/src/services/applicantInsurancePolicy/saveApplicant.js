import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save and update entity
 * @param {number} id - entity id
 * @param {object} entity - entity data
 * @returns
 */
const saveApplicant = async (id, applicant) => {
debugger;
  try {
   
    if (id === 0) {
     
      let response = await httpClient.post(
        `/applicants`,
        applicant
      );
      const { data } = response;
      return Result.success(data);
    } else {
      let response = await httpClient.patch(
        `/applicants/${id}`,
        applicant
      );
      const { data } = response;
      return Result.success(data);
    }
  } catch (e) {
    return handleApiError(e);
  }
};

export default saveApplicant;
