import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import { Button } from "@/components/ui/button";
import { Combobox } from "@/components/ui/comboBox";
import { Label } from "@/components/ui/label";
import RInput from "@/components/ui/rInput";
import { Separator } from "@/components/ui/separator";
import { toast } from "@/components/ui/use-toast";
import {
  appendFormData,
  getApplicantObject,
  getApplicantPostObject,
} from "@/lib/helperFunctions";
import { usePermissionStore } from "@/lib/store";
import {
  getApplicant,
  saveApplicant,
} from "@/services/applicantInsurancePolicy";

import {
  getGenders,
  getIdCardTypes,
  getApplicantOrganizations,
  getNomineeRelations,
} from "@/services/customerProfile";
import { applicantSchema } from "@/validations";
import { produce } from "immer";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ValidationError } from "yup";
import { Plus, Trash2 } from "lucide-react";
import DatePicker from "@/components/ui/datePicker";
const PAGE_NAME = "applicantinsurancepolicy";

const CreateApplicant = () => {
  const navigate = useNavigate();
  const searchParams = new URLSearchParams(location.search);
  const id = searchParams.get("id");
  const mode = searchParams.get("mode");

  const permissions = usePermissionStore((state) => state.permissions);
  const [applicant, setApplicant] = useState(getApplicantObject());
  const [genders, setGenders] = useState([]);
  const [idCardTypes, setIdCardTypes] = useState([]);
  const [organizations, setOrganizations] = useState([]);
  const [errors, setErrors] = useState({});
  const [relations, setRelations] = useState([]);
  let schema = applicantSchema();

  const getInitialData = async () => {
    try {
      const [
        genderResponse,
        idCardResponse,
        orgResponse,
        nomineerelationResponse,
      ] = await Promise.all([
        getGenders(),
        getIdCardTypes(),
        getApplicantOrganizations(),
        getNomineeRelations(),
      ]);

      if (genderResponse.status === "success") {
        setGenders(genderResponse.data);
      }
      if (idCardResponse.status === "success") {
        setIdCardTypes(idCardResponse.data);
      }
      if (orgResponse.status === "success") {
        setOrganizations(orgResponse.data);
      }
      if (nomineerelationResponse.status === "success") {
        setRelations(nomineerelationResponse.data);
      }
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to load form data.",
      });
    }
  };

  useEffect(() => {
    (async () => {
      await getInitialData();

      if (mode === "edit") {
        let response = await getApplicant(id);
        if (response.status === "success") {
          // Transform the data to match expected format
          const transformedData = {
            ...response.data,
            gender: genders.find((g) => g.id === response.data.gender) || {
              id: response.data.gender,
              name: response.data.genderName,
            },
            idCardType: idCardTypes.find(
              (t) => t.id === response.data.idCardType
            ) || {
              id: response.data.idCardType,
              name: response.data.idCardTypeName,
            },
          };
          console.log("Transformed Data:", transformedData);
          setApplicant(transformedData);
        } else {
          toast({
            variant: "destructive",
            title: "Error",
            description: "Unable to get applicant.",
          });
        }
      } else {
        setApplicant(getApplicantObject());
      }
    })();
  }, []);

  const handleChange = (name, index) => (event) => {
    let nextErrors = { ...errors };
    var path = name;
    let nextState = produce(applicant, (draft) => {
      switch (name) {
        case "gender":
        case "idCardType":
          draft[name] = event;
          break;
        case "dateOfBirth":
          draft[name] = event;
          break;
        case "salary":
          draft[name] = parseFloat(event.target.value) || 0;
          break;
        case "firstName":
        case "lastName":
        case "guardianName":
        case "mobileNumber":
        case "email":
        case "address":
        case "idCardNumber":
        case "associatedOrganization":
        case "productName":
          draft[name] = event.target.value;
          break;
        // Bank Details
        case "bankName":
        case "bankBranchDetails":
        case "bankAccountNumber":
        case "bankIfscCode":
        case "bankMicrCode":
          if (!draft.bankDetails) draft.bankDetails = {};
          draft.bankDetails[name] = event.target.value;
          path = "bankDetails." + name;
          break;
        case "dependentFirstName":
        case "dependentLastName":
        case "dependentRelationship":
          if (!draft.dependents[index]) draft.dependents[index] = {};
          // Fix: Ensure consistent property naming
          // eslint-disable-next-line no-case-declarations
          const propertyName =
            name.replace("dependent", "").charAt(0).toLowerCase() +
            name.replace("dependent", "").slice(1);
          draft.dependents[index][propertyName] = event.target.value;
          path = `dependents[${index}].${propertyName}`;
          break;
        case "dependentDateOfBirth":
          if (!draft.dependents[index]) draft.dependents[index] = {};
          draft.dependents[index].dateOfBirth = event;
          path = `dependents[${index}].dateOfBirth`;
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
    setApplicant(nextState);
    setErrors(nextErrors);
  };

  const addDependent = () => {
    setApplicant(
      produce(applicant, (draft) => {
        if (!draft.dependents) draft.dependents = [];
        draft.dependents.push({
          firstName: "",
          lastName: "",
          relationship: "",
          dateOfBirth: null,
          contactNumber: "",
        });
      })
    );
  };

  const removeDependent = (index) => {
    setApplicant(
      produce(applicant, (draft) => {
        draft.dependents.splice(index, 1);
      })
    );
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    try {
      schema.validateSync(applicant, { abortEarly: false });
      let postObject = getApplicantPostObject(applicant);
      console.log("Post Object:", postObject);
      //let formData = new FormData();
      // appendFormData(formData, postObject);

      let response = await saveApplicant(applicant.id, postObject);
      if (response.status === "success") {
        toast({
          title: "Success",
          description: "Applicant saved successfully.",
        });
        navigate("/insurancepolicydata");
      } else {
        if (response.status === "conflict") {
          toast({
            variant: "destructive",
            title: "Something went wrong.",
            description: response.errors.message,
          });
        } else {
          toast({
            variant: "destructive",
            title: "Something went wrong.",
            description:
              response.validationErrors || "Unable to save Applicant.",
          });
        }
      }
    } catch (e) {
      if (e instanceof ValidationError) {
        let newEr = produce({}, (draft) => {
          e.inner.forEach((error) => {
            draft[error.path] = [...error.errors];
          });
        });
        setErrors(newEr);
      }
    }
  };

  return (
    <div className="w-full">
      <h1 className="text-2xl font-bold mb-6">
        {mode === "edit" ? "Edit Applicant" : "Create Applicant"}
      </h1>

      {/* Personal Information */}
      <div>
        <h4 className="text-lg font-medium leading-none">
          Personal Information
        </h4>
        <Separator className="my-4" />
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-8">
        <div className="space-y-2">
          <RInput
            id="firstName"
            type="text"
            label="First Name"
            name="firstName"
            value={applicant.firstName}
            error={errors?.firstName}
            onChange={handleChange("firstName")}
            placeholder="Enter first name"
            isRequired={true}
          />
        </div>
        <div className="space-y-2">
          <RInput
            id="lastName"
            type="text"
            label="Last Name"
            name="lastName"
            value={applicant.lastName}
            error={errors?.lastName}
            onChange={handleChange("lastName")}
            placeholder="Enter last name"
          />
        </div>
        <div className="space-y-2">
          <div className="flex flex-col justify-between space-y-2">
            <Label htmlFor="gender">
              <span>Gender</span>
              <span className="text-red-600 ml-1">*</span>
            </Label>
            {/* <Combobox
              options={genders}
              placeholder="Select Gender"
              valueProperty="id"
              labelProperty="name"
              id="gender"
              onChange={handleChange("gender")}
              value={applicant.gender}
              error={errors?.gender}
            /> */}
            <Combobox
              options={genders}
              placeholder="Select Gender"
              valueProperty="id"
              labelProperty="name"
              id="gender"
              onChange={handleChange("gender")}
              value={
                typeof applicant.gender === "object"
                  ? applicant.gender
                  : genders.find((g) => g.id === applicant.gender) || null
              }
              error={errors?.gender}
            />
          </div>
        </div>

        <div className="space-y-2">
          <DatePicker
            label="Date of birth"
            id="dob"
            type="date"
            placeholder="dd/mm/yyyy"
            onChange={handleChange("dateOfBirth")}
            isRequired={true}
            error={errors?.dateOfBirth}
            date={applicant?.dateOfBirth}
            isFutureDateAllowed={false}
          />
          {errors?.dateOfBirth && (
            <div className="text-red-600 text-xs py-1">
              {errors.dateOfBirth}
            </div>
          )}
        </div>
        <div className="space-y-2">
          <RInput
            id="mobileNumber"
            isRequired={true}
            type="tel"
            label="Contact Number"
            name="mobileNumber"
            value={applicant.mobileNumber}
            error={errors?.mobileNumber}
            onChange={handleChange("mobileNumber")}
            placeholder="Enter contact number"
          />
        </div>
        <div className="space-y-2">
          <RInput
            label="Email"
            placeholder="Enter Email"
            value={applicant.email}
            onChange={handleChange("email")}
            id="email"
            type="text"
            error={errors?.email}
          />
        </div>
      </div>
      <div className="space-y-2">
        <RInput
          id="address"
          type="text"
          label="Location/Address"
          name="address"
          value={applicant.address}
          error={errors?.address}
          onChange={handleChange("address")}
          placeholder="Enter address"
          isRequired={true}
        />
      </div>
      {/* Employment Details */}
      <div>
        <h4 className="text-lg font-medium leading-none">Employment Details</h4>
        <Separator className="my-4" />
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-8">
        <div className="space-y-2">
          <RInput
            id="salary"
            type="number"
            label="Salary"
            name="salary"
            value={applicant.salary}
            error={errors?.salary}
            onChange={handleChange("salary")}
            placeholder="Enter salary"
            isRequired={true}
          />
        </div>
        <div className="space-y-2">
          <RInput
            id="associatedOrganization"
            type="text"
            label="Organization"
            name="associatedOrganization"
            value={applicant.associatedOrganization}
            error={errors?.associatedOrganization}
            onChange={handleChange("associatedOrganization")}
            placeholder="Enter Organization"
            isRequired={true}
          />
        </div>
        <div className="space-y-2">
          <RInput
            id="productName"
            type="text"
            label="ProductName"
            name="productName"
            value={applicant.productName}
            error={errors?.productName}
            onChange={handleChange("productName")}
            placeholder="Enter Product Name"
            isRequired={true}
          />
        </div>
      </div>

      {/* Identification Details */}
      <div>
        <h4 className="text-lg font-medium leading-none">
          Identification Details
        </h4>
        <Separator className="my-4" />
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-8">
        <div className="space-y-2">
          <div className="flex flex-col justify-between space-y-2">
            <Label htmlFor="idCardType">
              <span>ID Card Type</span>
              <span className="text-red-600 ml-1">*</span>
            </Label>
            <Combobox
              options={idCardTypes}
              placeholder="Select ID Card Type"
              valueProperty="id"
              labelProperty="name"
              id="idCardType"
              onChange={handleChange("idCardType")}
              value={applicant.idCardType} // The component should handle both object and primitive values
              error={errors?.idCardType}
            />
          </div>
        </div>
        <div className="space-y-2">
          <RInput
            id="idCardNumber"
            type="text"
            label="ID Card Number"
            name="idCardNumber"
            value={applicant.idCardNumber}
            error={errors?.idCardNumber}
            onChange={handleChange("idCardNumber")}
            placeholder="Enter ID card number"
            isRequired={true}
          />
        </div>
      </div>

      {/* Bank Details */}
      <div>
        <h4 className="text-lg font-medium leading-none">Bank Details</h4>
        <Separator className="my-4" />
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-8">
        <div className="space-y-2">
          <RInput
            id="bankName"
            type="text"
            label="Bank Name"
            name="bankName"
            value={applicant.bankDetails?.bankName}
            error={errors?.["bankDetails.bankName"]}
            onChange={handleChange("bankName")}
            placeholder="Enter bank name"
          />
        </div>
        <div className="space-y-2">
          <RInput
            id="bankBranchDetails"
            type="text"
            label="Bank Branch name & Address"
            name="bankBranchDetails"
            value={applicant.bankDetails?.bankBranchDetails}
            error={errors?.["bankDetails.bankBranchDetails"]}
            onChange={handleChange("bankBranchDetails")}
            placeholder="Enter branch name"
          />
        </div>

        <div className="space-y-2">
          <RInput
            id="bankAccountNumber"
            type="text"
            label="Account Number"
            name="bankAccountNumber"
            value={applicant.bankDetails?.bankAccountNumber}
            error={errors?.["bankDetails.bankAccountNumber"]}
            onChange={handleChange("bankAccountNumber")}
            placeholder="Enter account number"
          />
        </div>
        <div className="space-y-2">
          <RInput
            id="bankIfscCode"
            type="text"
            label="IFSC Code"
            name="bankIfscCode"
            value={applicant.bankDetails?.bankIfscCode}
            error={errors?.["bankDetails.bankIfscCode"]}
            onChange={handleChange("bankIfscCode")}
            placeholder="Enter IFSC code"
          />
        </div>
        <div className="space-y-2">
          <RInput
            id="bankMicrCode"
            type="text"
            label="MICR Code"
            name="bankMicrCode"
            value={applicant.bankDetails?.bankMicrCode}
            error={errors?.["bankDetails.bankMicrCode"]}
            onChange={handleChange("bankMicrCode")}
            placeholder="Enter MICR code"
          />
        </div>
      </div>

      {/* Dependents */}
      <div className="mb-8">
        <div className="flex justify-between items-center mb-4">
          <h4 className="text-lg font-medium leading-none">Dependents</h4>
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={addDependent}
          >
            <Plus className="h-4 w-4 mr-2" />
            Add Dependent
          </Button>
        </div>
        <Separator className="my-4" />

        {applicant.dependents?.map((dependent, index) => (
          <div key={index} className="mb-6 p-4 border rounded-lg">
            <div className="flex justify-between items-center mb-4">
              <h5 className="text-md font-medium">Dependent {index + 1}</h5>
              <Button
                type="button"
                variant="destructive"
                size="sm"
                onClick={() => removeDependent(index)}
              >
                <Trash2 className="h-4 w-4" />
              </Button>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              <div className="space-y-2">
                <RInput
                  id={`dependentFirstName-${index}`}
                  type="text"
                  label="First Name"
                  name={`dependentFirstName-${index}`}
                  value={dependent?.firstName}
                  error={errors?.[`dependents[${index}].firstName`]} // Fix: Match the property name case
                  onChange={handleChange("dependentFirstName", index)}
                  placeholder="Enter first name"
                />
              </div>
              <div className="space-y-2">
                <RInput
                  id={`dependentLastName-${index}`}
                  type="text"
                  label="Last Name"
                  name={`dependentLastName-${index}`}
                  value={dependent?.lastName}
                  error={errors?.[`dependents[${index}].lastName`]} // Change lowercase 'lastname' to camelCase 'lastName'
                  onChange={handleChange("dependentLastName", index)}
                  placeholder="Enter last name"
                />
              </div>
              <div className="space-y-2">
                <RInput
                  id={`dependentRelationship-${index}`}
                  type="text"
                  label="Relationship"
                  name={`dependentRelationship-${index}`}
                  value={dependent?.relationship}
                  error={errors?.[`dependents[${index}].relationship`]}
                  onChange={handleChange("dependentRelationship", index)}
                  placeholder="Enter relationship"
                />
              </div>
              <div className="space-y-2">
                <DatePicker
                  label="Date of birth"
                  id={`dependentDateOfBirth-${index}`}
                  type="date"
                  placeholder="dd/mm/yyyy"
                  onChange={handleChange("dependentDateOfBirth", index)}
                  name={`dependentDateOfBirth-${index}`}
                  value={dependent?.dateOfBirth}
                  error={errors?.[`dependents[${index}].dateOfBirth`]}
                  date={dependent?.dateOfBirth}
                  isFutureDateAllowed={false}
                />
                {errors?.dateOfBirth && (
                  <div className="text-red-600 text-xs py-1">
                    {errors.dateOfBirth}
                  </div>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Action Buttons */}
      <div className="mt-6 flex justify-end">
        <Button
          variant="secondary"
          className="mr-2"
          onClick={() => navigate("/applicants")}
        >
          Cancel
        </Button>
        {permissions?.[PAGE_NAME]?.create ? (
          <Button onClick={handleSubmit}>
            {mode === "edit" ? "Update" : "Save"}
          </Button>
        ) : null}
      </div>
    </div>
  );
};

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(CreateApplicant))
);
