import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save and update entity
 * @param {number} id - entity id
 * @param {object} entity - entity data
 * @returns
 */
const saveAssociation = async (id, association) => {
  try {
    const headers = {
      "Content-Type": "application/x-www-form-urlencoded",
    };
    if (id === 0) {
     
      let response = await httpClient.post(
        `/association/createassociation`,
        association,
        headers
      );
      const { data } = response;
      return Result.success(data);
    } else {
      let response = await httpClient.patch(
        `/association/updateassociation/${id}`,
        association,
        headers
      );
      const { data } = response;
      return Result.success(data);
    }
  } catch (e) {
    return handleApiError(e);
  }
};

export default saveAssociation;
