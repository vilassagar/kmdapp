import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import RButton from "@/components/ui/rButton";
import RInput from "@/components/ui/rInput";
import { toast } from "@/components/ui/use-toast";
import { getInitialOrganisationObject } from "@/lib/helperFunctions";
import { produce } from "immer";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ValidationError } from "yup";
import { usePermissionStore } from "@/lib/store";
import {
  getOrganisationById,
  saveOrganistion,
  updateOrganisation,
} from "@/services/organisations";
import { organisationSchema } from "@/validations";
import RTextArea from "@/components/ui/rTextArea";

const PAGE_NAME = "associations";

function CreateOrganisation() {
  const navigate = useNavigate();
  const permissions = usePermissionStore((state) => state.permissions);
  const searchParams = new URLSearchParams(location.search);
  const id = searchParams.get("id");
  const mode = searchParams.get("mode");

  const schema = organisationSchema();

  const [organisation, setOrganisation] = useState(
    getInitialOrganisationObject()
  );
  const [errors, setErrors] = useState({});

  useEffect(() => {
    const getOrganisationData = async (id) => {
      const response = await getOrganisationById(id);
      if (response.status === "success") {
        setOrganisation(response.data);
      } else {
        console.error("Error fetching organisation:", response.error);
      }
    };

    if (id && mode === "edit") {
      getOrganisationData(id);
    } else {
      setOrganisation(getInitialOrganisationObject());
    }
  }, []);

  const handleChange = (name) => (event) => {
    let nextErrors = { ...errors };
    let path = name;
    let nextState = produce(organisation, (draft) => {
      switch (name) {
        case "name":
        case "description":
          draft[name] = event.target.value;
          break;
        default:
          break;
      }
      try {
        schema.validateSyncAt(path, draft);
        nextErrors[path] = [];
      } catch (e) {
        nextErrors[path] = [...e.errors];
      }
    });
    setErrors(nextErrors);
    setOrganisation(nextState);
  };

  const handleSubmit = async () => {
    try {
      await schema.validate(organisation, { abortEarly: false });

      let response;
      if (organisation.id === 0) {
        response = await saveOrganistion(organisation);
      } else {
        response = await updateOrganisation(organisation.id, organisation);
      }
      if (response.status === "success") {
        navigate("/organisations");
        setOrganisation(getInitialOrganisationObject());
        toast({
          title: "Organisation saved successfully",
          description: "Organisation has been saved successfully.",
        });
        navigate("/organisations");
      } else {
        toast({
          variant: "destructive",
          title: "Something went wrong.",
          description: "Unable to save organisation.",
        });
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

  return (
    <div className="w-full max-w-4xl">
      <div>
        <h1 className="mb-8 text-2xl font-bold ">Create Organisation</h1>
      </div>
      <div className="space-y-2">
        <RInput
          label="Organisation Name"
          id="name"
          type="text"
          placeholder="Enter a Organisation Name"
          className="w-full"
          onChange={(event) => handleChange("name")(event)}
          value={organisation.name}
          error={errors?.name}
        />
        {errors.name && <div className="text-red-600">{errors.name}</div>}
        <RTextArea
          label="Description"
          id="description"
          type="text"
          placeholder="Enter a description"
          className="w-full"
          onChange={(event) => handleChange("description")(event)}
          value={organisation.description}
          error={errors.description}
        />
        {errors.description && (
          <div className="text-red-600">{errors.description}</div>
        )}
      </div>

      {permissions?.[PAGE_NAME]?.create ? (
        <div className="flex justify-end mt-10">
          <RButton onClick={handleSubmit}>Save Changes</RButton>
        </div>
      ) : null}
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(CreateOrganisation))
);
