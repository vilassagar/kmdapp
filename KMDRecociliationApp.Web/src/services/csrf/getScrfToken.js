import { Result } from "@/utils";
import axios from "axios";

/**
 * Api call to get antiforgery token
 * @returns token
 */
const getcsrfToken = async () => {
  try {
    const response = await axios.get(
      `${import.meta.env.VITE_API_BASE_URL}/api/csrf/token`
    );
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    throw new Error();
  }
};

export default getcsrfToken;
