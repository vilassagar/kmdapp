import { handleApiError, httpClient, Result } from "../../utils";

/**
 *
 * @param {*} file
 * @returns
 */

const uploadAssociations = async (file) => {
  try {
    let response = await httpClient.post(
      "/importdata/bulkuploadproducts",
      file,
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
export default uploadAssociations;
