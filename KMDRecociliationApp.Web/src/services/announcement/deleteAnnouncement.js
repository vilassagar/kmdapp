import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to delete Campaign by id
 * @param {number} campaignId
 * @returns
 */
const deleteAnnouncement = async (announcementId) => {
  try {
    const response = await httpClient.delete(`/announcement/${announcementId}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default deleteAnnouncement;
    