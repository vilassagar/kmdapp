import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save and update role
 * @param {object} role - role data
 * @returns {Promise}
 */
const saveRole = async (role) => {
 
  try {
    
     if (role?.permissions) {
      role.permissions.forEach(permission => {
        if (permission.actions) {
          // Ensure each action property is a boolean
          permission.actions.create = Boolean(permission.actions.create);
          permission.actions.read = Boolean(permission.actions.read);
          permission.actions.update = Boolean(permission.actions.update);
          permission.actions.delete = Boolean(permission.actions.delete);
        }
      });
    }


    // Send the role object directly without wrapping it in roleObj
    let response = await httpClient.post('/roles/', { role });
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default saveRole;
