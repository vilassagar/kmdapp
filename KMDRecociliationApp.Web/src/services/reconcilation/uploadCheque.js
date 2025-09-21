import { handleApiError, httpClient, Result } from "../../utils";

/**
 *
 * @param {*} file
 * @returns
 */

const uploadCheque = async (file,campaignId) => {
  try {
    let response = await httpClient.post(
      "/bulkreconcilation/bulkreconcilationcheque",
      {file,campaignId},
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};
export default uploadCheque;
