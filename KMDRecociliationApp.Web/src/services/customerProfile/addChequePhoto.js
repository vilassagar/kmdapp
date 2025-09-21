import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save cheque photo
 * @param {object} cheque
 * @returns
 */
const addChequePhoto = async (cheque) => {
  try {
    let response = await httpClient.post(
      `/customerprofile/addchequephoto`,
      cheque
    );

    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default addChequePhoto;
