import { handleApiError, httpClient, Result } from "../../utils";

/**
 *
 * @param {*} file
 * @returns
 */

const uploadTemplate = async (file) => {
  console.log("file", file);
  try {
    // Create a FormData object
    const formData = new FormData();
    
    // Add the file with the key 'template' to match the backend model
    formData.append("template", file);
    
    // Send the formData to the API endpoint
    let response = await httpClient.post(
      "/applicants/import",
      formData,
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
export default uploadTemplate;
