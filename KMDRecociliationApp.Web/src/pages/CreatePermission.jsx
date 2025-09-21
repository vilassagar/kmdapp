import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import { Checkbox } from "@/components/ui/checkbox";
import { Combobox } from "@/components/ui/comboBox";
import RButton from "@/components/ui/rButton";
import RInput from "@/components/ui/rInput";
import { toast } from "@/components/ui/use-toast";
import { getInitialPermissionObject } from "@/lib/helperFunctions";
import { usePermissionStore } from "@/lib/store";
import getPermission from "@/services/permissions/getPermission";
import savePermission from "@/services/permissions/savePermission";
import { permissionSchema } from "@/validations/PermissionSchema";
import { Label } from "@radix-ui/react-dropdown-menu";
import { produce } from "immer";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ValidationError } from "yup";

const PAGE_NAME = "permissions";

const CreatePermission = () => {
  const id = window.location.pathname.split("/")[2];

  const permissions = usePermissionStore((state) => state.permissions);

  const [permission, setPermission] = useState(getInitialPermissionObject());

  const [errors, setErrors] = useState({});
  const [isLoading, setIsLoading] = useState(false);
  const [selectedtype, setselectedtype] = useState(null);
  const types = [
    { id: 1, name: "api" },
    { id: 2, name: "ui" },
  ];
  const navigate = useNavigate();

  useEffect(() => {
    if (id) {
      getPermission(id).then((result) => {
        if (result.status === "success") {
          setPermission(result.data);
          const typeobj =types.find(item => item.name=== result.data.type);
          // let id=type?type.id:null;
          // const typeobj={id:id,name:}
          setselectedtype(typeobj);
        } else {
          throw new Error(result.message);
        }
      });
    }else
    {
      const typeobj={id:1,name:"api"}
      setselectedtype(typeobj);
    }
  }, [id]);

  const handleChange = (name, action) => (event) => {
    const value = event.target ? event.target.value : event;
    const nextErrors = { ...errors };
    const nextState = produce(permission, (draft) => {
      if (name === "actions") {
        draft[name][action] = event;
      } else {
        draft[name] = value;
      }
    });

    setPermission(nextState);

    permissionSchema
      .validateAt(name, nextState)
      .then(() => {
        delete nextErrors[name];
        setErrors(nextErrors);
      })
      .catch((error) => {
        nextErrors[name] = error.message;
        setErrors(nextErrors);
      });
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    try {
      setIsLoading(true);
      await permissionSchema.validate(permission, { abortEarly: false });
      const permissionobj={id:permission.id,actions:permission.actions,name:permission.name,type:permission.type};
      const response = await savePermission(permission.id, permissionobj);
      if (response.status === "success") {
        toast({
          title: "Permission saved successfully",
          description: "Permission has been saved successfully.",
        });
        navigate("/permissions");
      } else {
        toast({
          variant: "destructive",
          title: "Something went wrong.",
          description: "Unable to save permission.",
        });
      }
    } catch (e) {
      if (e instanceof ValidationError) {
        const newErrors = produce({}, (draft) => {
          e.inner.forEach((error) => {
            draft[error.path] = error.message;
          });
        });
        setErrors(newErrors);
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="w-full max-w-4xl">
      <div>
        <h1 className="mb-8 text-2xl font-bold ">Create Permission</h1>
      </div>
      <form className="grid  gap-4 py-2">
        <div className="flex flex-col justify-between space-y-2">
          <Label htmlFor="type ">
            <span>Permission Type</span>
            <span className="text-red-600">*</span>
          </Label>
          <Combobox
            options={types}
            placeholder="Select permission type"
            valueProperty="id"
            labelProperty="name"
            
            id="type"
            onChange={handleChange("type")}
            value={selectedtype}
            error={errors?.type}
          />
          {errors.type && <div className="text-red-600">{errors.type}</div>}
        </div>

        <div className="space-y-2">
          <RInput
            type="text"
            id="name"
            label="Permission Name"
            isRequired={true}
            placeholder="Enter Permission Name"
            onChange={handleChange("name")}
            value={permission.name}
            error={errors?.name}
          />
          {errors.name && <div className="text-red-600">{errors.name}</div>}
        </div>
        <div className="space-y-2">
          <div className="text-xl font-bold pt-7">Permission Actions</div>
          {["create", "read", "update", "delete"].map((action) => (
            <div key={action} className="flex items-center gap-2">
              <Checkbox
                id={action}
                onCheckedChange={(checked) =>
                  handleChange("actions", action)(checked)
                }
                checked={permission.actions[action]}
              />
              <Label htmlFor={action}>
                {action.charAt(0).toUpperCase() + action.slice(1)}
              </Label>
            </div>
          ))}
        </div>
        {permissions?.[PAGE_NAME]?.create ? (
          <div className="flex justify-end">
            <RButton onClick={handleSubmit} isLoading={isLoading}>
              Save
            </RButton>
          </div>
        ) : null}
      </form>
    </div>
  );
};

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(CreatePermission))
);
