import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get Announcement
 * @param {number} id - Announcement id
 * @returns
 */
const getAnnouncement = async (id) => {
  try {
    let response = await httpClient.get(`/announcement/${id}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getAnnouncement;
