import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save fileModel
 * @param {object} fileModel
 * @returns
 */
const uploadCampaignTemplate = async (fileModel) => {
  try {
    let response;
    
      response = await httpClient.post(`/campaigns/uploadcampaigntemplate`, fileModel);
    
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default uploadCampaignTemplate;
