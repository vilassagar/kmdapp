import { handleApiError, httpClient, Result } from "../../utils";

/**
 * Api call to save and update permission
 * @param {number} id - permission id
 * @param {object} entity - permission data
 * @returns
 */
const updateRole = async (id, role) => {
  
  try {
    // Ensure valid JSON
    //const validRole = JSON.parse(JSON.stringify(role));
     // Ensure permissions actions are stringified properly
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

    // Wrap in roleObj property
    const requestBody = { roleObj: role };
    
    // Send the role object directly without wrapping it in roleObj
    let response = await httpClient.patch(`/roles/${id}`, role);
    const { data } = response;
    return Result.success(data);
  } catch (e) {
    return handleApiError(e);
  }
};

export default updateRole;
