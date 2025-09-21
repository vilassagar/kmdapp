import { handleApiError, httpClient, Result } from "../../utils";
/**
 * Api call to get nominee relations
 * @returns
 */
const getNomineeRelations = async () => {
  try {
    const response = await httpClient.get(
      `/customerprofile/getnomineerelations`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default getNomineeRelations;
