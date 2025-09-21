import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to download file
 * @param {number} id
 * @param {string} name
 * @param {string} url
 * @returns
 */
const download = async (id, name, url) => {
  try {
    let response = await httpClient.get(
      `/files/download?id=${id}&name=${name}&url=${url}`,
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

export default download;
