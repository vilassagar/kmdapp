import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get announcement
 * @param {number} id - announcement id
 * @returns
 */
const getCurrentannouncement = async (location) => {
  try {
    let response = await httpClient.get(`/annoncement/getcurrentannouncement?location=${location}`);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getCurrentannouncement;
