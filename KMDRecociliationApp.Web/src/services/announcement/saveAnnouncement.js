import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save and update entity
 * @param {number} id - entity id
 * @param {object} entity - entity data
 * @returns
 */
const saveAnnouncement = async (id, announcement) => {
  try {
    const headers = {
      "Content-Type": "application/x-www-form-urlencoded",
    };
    if (id === 0) {
     
      let response = await httpClient.post(
        `/announcement/createannouncement`,
        announcement,
        headers
      );
      const { data } = response;
      return Result.success(data);
    } else {
      let response = await httpClient.patch(
        `/announcement/updateannouncement/${id}`,
        announcement,
        headers
      );
      const { data } = response;
      return Result.success(data);
    }
  } catch (e) {
    return handleApiError(e);
  }
};

export default saveAnnouncement;
