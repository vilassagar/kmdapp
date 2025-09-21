import { handleApiError, httpClient, Result } from "../../utils";

/**
 *
 * @param {*} file
 * @returns
 */

const uploadProducts = async (file) => {
  try {
    let response = await httpClient.post(
      "/importdata/bulkuploadassociation",
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
export default uploadProducts;
