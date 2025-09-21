/* eslint-disable no-case-declarations */
import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import { Checkbox } from "@/components/ui/checkbox";
import RButton from "@/components/ui/rButton";
import RInput from "@/components/ui/rInput";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { toast } from "@/components/ui/use-toast";
import { getInitialRoleObject } from "@/lib/helperFunctions";
import {
  getPermissionView,
  getRole,
  saveRole,
  updateRole,
} from "@/services/roles";
import { produce } from "immer";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ValidationError } from "yup";
import { roleSchema } from "../validations/RoleSchema";
import { usePermissionStore } from "@/lib/store";

const PAGE_NAME = "roles";

function CreateRole() {
  const navigate = useNavigate();
  const permissions = usePermissionStore((state) => state.permissions);

  const [role, setRole] = useState(() => ({
    ...getInitialRoleObject(),
    permissions: [],
  }));
  const [errors, setErrors] = useState({});

  useEffect(() => {
    const fetchRoleData = async (id) => {
      const response = await getRole(id);
      if (response.status === "success") {
        setRole({
          ...response.data,
          permissions: response.data.permissions || [],
        });
      } else {
        console.error("Error fetching role:", response.error);
      }
    };

    const fetchPermissionView = async (id) => {
      const response = await getPermissionView(id);
      if (response.status === "success") {
        setRole((prevRole) => ({
          ...prevRole,
          permissions: response.data,
        }));
      } else {
        console.error("Error fetching permission view:", response.error);
      }
    };

    const id = window.location.pathname.split("/")[2];
    if (id) {
      fetchRoleData(id);
    }
    fetchPermissionView(id || 0);
  }, []);

  const handleChange = (name, section, entity, action) => (event) => {
    let nextErrors = { ...errors };
    let nextState = produce(role, (draft) => {
      switch (name) {
        case "name":
        case "description":
          draft[name] = event.target.value;
          break;
        case "permissions":
          const index = draft.permissions.findIndex(
            (p) => p.type === section && p.name === entity
          );
          if (index !== -1) {
            draft.permissions[index].actions[action] = event;
            if (draft.permissions[index].id) {
              draft.permissions[index].id = draft.permissions[index].id || 0;
            }
          }
          break;
        default:
          break;
      }
    });

    setRole(nextState);

    if (name === "permissions") {
      const permissionIndex = nextState.permissions.findIndex(
        (p) => p.type === section && p.name === entity
      );

      if (permissionIndex !== -1) {
        roleSchema
          .validateAt(
            `permissions[${permissionIndex}].actions.${action}`,
            nextState
          )
          .then(() => {
            delete nextErrors[`${section}.${entity}.${action}`];
            setErrors(nextErrors);
          })
          .catch((validationError) => {
            nextErrors[`${section}.${entity}.${action}`] =
              validationError.message;
            setErrors(nextErrors);
          });
      }
    } else {
      roleSchema
        .validateAt(name, nextState)
        .then(() => {
          delete nextErrors[name];
          setErrors(nextErrors);
        })
        .catch((validationError) => {
          nextErrors[name] = validationError.message;
          setErrors(nextErrors);
        });
    }
  };
  // Updated handleSubmit function - no need to transform case
  const handleSubmit = async () => {
    try {
      await roleSchema.validate(role, { abortEarly: false });

      // No case transformation needed since backend now uses lowercase properties
      // Just ensure the structure matches exactly what the API expects
      const roleData = {
        id: role.id,
        name: role.name,
        description: role.description,
        permissions: role.permissions.map((permission) => ({
          id: permission.id,
          type: permission.type,
          name: permission.name,
          actions: {
            create: permission.actions.create,
            read: permission.actions.read,
            update: permission.actions.update,
            delete: permission.actions.delete,
          },
        })),
      };

      let response;
      if (role.id === 0) {
        response = await saveRole(roleData);
      } else {
        response = await updateRole(role.id, roleData);
      }

      if (response.status === "success") {
        navigate("/roles");
        setRole(getInitialRoleObject());
        toast({
          title: "Role saved successfully",
          description: "Role has been saved successfully.",
        });
      } else {
        // 400
        if (response.status === "conflict") {
          //409
          toast({
            variant: "destructive",
            title: "Something went wrong.",
            description: response.errors.message,
          });
        } else {
          // 500
          toast({
            variant: "destructive",
            title: "Something went wrong.",
            description: "Unable to save Role.",
          });
        }
      }
    } catch (validationErrors) {
      if (validationErrors instanceof ValidationError) {
        let newErrors = {};
        validationErrors.inner.forEach((error) => {
          newErrors[error.path] = error.message;
        });
        setErrors(newErrors);
      }
    }
  };
  const PermissionRow = ({ section, entity, permissions }) => (
    <TableRow key={entity}>
      <TableCell>{entity.charAt(0).toUpperCase() + entity.slice(1)}</TableCell>
      <TableCell>
        <Checkbox
          id={`${section}-${entity}-create`}
          onCheckedChange={handleChange(
            "permissions",
            section,
            entity,
            "create"
          )}
          checked={permissions.create}
        />
        {errors[`${section}.${entity}.create`] && (
          <div className="text-red-600">
            {errors[`${section}.${entity}.create`]}
          </div>
        )}
      </TableCell>
      <TableCell>
        <Checkbox
          id={`${section}-${entity}-read`}
          onCheckedChange={handleChange("permissions", section, entity, "read")}
          checked={permissions.read}
        />
        {errors[`${section}.${entity}.read`] && (
          <div className="text-red-600">
            {errors[`${section}.${entity}.read`]}
          </div>
        )}
      </TableCell>
      <TableCell>
        <Checkbox
          id={`${section}-${entity}-update`}
          onCheckedChange={handleChange(
            "permissions",
            section,
            entity,
            "update"
          )}
          checked={permissions.update}
        />
        {errors[`${section}.${entity}.update`] && (
          <div className="text-red-600">
            {errors[`${section}.${entity}.update`]}
          </div>
        )}
      </TableCell>
      <TableCell>
        <Checkbox
          id={`${section}-${entity}-delete`}
          onCheckedChange={handleChange(
            "permissions",
            section,
            entity,
            "delete"
          )}
          checked={permissions.delete}
        />
        {errors[`${section}.${entity}.delete`] && (
          <div className="text-red-600">
            {errors[`${section}.${entity}.delete`]}
          </div>
        )}
      </TableCell>
    </TableRow>
  );

  PermissionRow.defaultProps = {
    section: "",
    entity: "",
    permissions: [],
  };

  PermissionRow.propTypes = {
    section: PropTypes.string,
    entity: PropTypes.string,
    permissions: PropTypes.object,
  };

  const PermissionSection = ({ title, section, permissions }) => (
    <AccordionItem value={title} key={section}>
      <AccordionTrigger>{title}</AccordionTrigger>
      <AccordionContent>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Entity</TableHead>
              <TableHead>Create</TableHead>
              <TableHead>Read</TableHead>
              <TableHead>Update</TableHead>
              <TableHead>Delete</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {permissions
              .filter((p) => p.type === section)
              .map((permission) => (
                <PermissionRow
                  key={permission.name}
                  section={section}
                  entity={permission.name}
                  permissions={permission.actions}
                />
              ))}
          </TableBody>
        </Table>
      </AccordionContent>
    </AccordionItem>
  );

  PermissionSection.defaultProps = {
    title: "",
    section: "",
    permissions: [],
  };

  PermissionSection.propTypes = {
    title: PropTypes.string,
    section: PropTypes.string,
    permissions: PropTypes.array,
  };

  const permissionTypes = Array.from(
    new Set(role.permissions.map((perm) => perm.type))
  );

  return (
    <div className="w-full max-w-4xl">
      <div>
        <h1 className="mb-8 text-2xl font-bold ">Create Role</h1>
      </div>
      <div className="space-y-2">
        <RInput
          label="Role Name"
          id="name"
          type="text"
          placeholder="Enter a Role Name"
          className="w-full"
          onChange={(event) => handleChange("name")(event)}
          value={role.name}
          error={errors?.name}
        />
        {errors.name && <div className="text-red-600">{errors.name}</div>}
        <RInput
          label="Description"
          id="description"
          type="text"
          placeholder="Enter a description"
          className="w-full"
          onChange={(event) => handleChange("description")(event)}
          value={role.description}
          error={errors.description}
        />
        {errors.description && (
          <div className="text-red-600">{errors.description}</div>
        )}
        <div className="text-xl font-bold pt-7">Permissions</div>
        <div className="text-sm text-gray-500 pt-2">
          Manage the permissions for your application. Adjust the API and UI
          access levels as needed.
        </div>
      </div>
      <div>
        <Accordion type="single" collapsible>
          {permissionTypes.map((type) => (
            <PermissionSection
              key={type}
              title={type.toUpperCase()}
              section={type}
              permissions={role.permissions}
            />
          ))}
        </Accordion>
      </div>
      {permissions?.[PAGE_NAME]?.create ? (
        <div className="flex justify-end mt-10">
          <RButton onClick={handleSubmit}>Save</RButton>
        </div>
      ) : null}
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(CreateRole))
);
