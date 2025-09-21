import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to get message templates for associations
 * @param {string} associationIds
 * @returns
 */
const getMessageTemplates = async (associationIds) => {
  try {
    const response = await httpClient.get(
      `/association/getmessagetemplates?associationIds=${associationIds}`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getMessageTemplates;
