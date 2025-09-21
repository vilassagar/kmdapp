import { handleApiError, httpClient, Result } from "../../utils";

/**
 *
 * @returns
 */

const getDashboardFilterData = async (page, pageSize, searchTerm,baseFilter,campaignId, associationId) => {
    try {
        const response = await httpClient.get(
          `/dashboard/getdashboardfilterdata?page=${page}&pageSize=${pageSize}&search=${searchTerm}&baseFilter=${baseFilter}&campaignId=${campaignId}&associationId=${associationId}`
        );
        const { data } = response;
        return Result.success(data);
      } catch (e) {
        return handleApiError(e);
      }
};

export default getDashboardFilterData;
