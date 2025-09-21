 import { handleApiError, httpClient, Result } from "../../utils";

 /**
  * Api call to save campaign
  * @param {object} campaignData
  * @returns
  */
 const saveCampaign = async (campaignData) => {
   try {
     // Ensure campaign is not null or undefined
     if (!campaignData) {
       return Result.error("Campaign data is missing");
     }
     
     // Do NOT stringify the campaignData - use the object directly
    // const requestData = { campaign: campaignData };
     
     // Log the request data for debugging
     console.log("Sending request data:", campaignData);
     console.log("JSON string:", JSON.stringify(campaignData));
 
     let response;
     if (campaignData.campaignId) {
       response = await httpClient.patch(
         `/campaigns/${campaignData.campaignId}`,
         campaignData
       );
     } else {
       response = await httpClient.post(`/campaigns`, campaignData);
     }
     const { data } = response;
     return Result.success(data);
   } catch (e) {
     console.error("Error in saveCampaign:", e);
     return handleApiError(e);
   }
 };
 
 export default saveCampaign;