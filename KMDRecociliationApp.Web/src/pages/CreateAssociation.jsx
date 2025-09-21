import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Combobox } from "@/components/ui/comboBox";
import ContactDetails from "@/components/ui/contactDetails";
import FileUpload from "@/components/ui/fileUpload";
import { Label } from "@/components/ui/label";
import Messages from "@/components/ui/messagesList";
import RInput from "@/components/ui/rInput";
import { Separator } from "@/components/ui/separator";
import { toast } from "@/components/ui/use-toast";
import {
  appendFormData,
  getAssociationObject,
  getAssociationPostObject,
} from "@/lib/helperFunctions";
import { usePermissionStore } from "@/lib/store";
import { getAssociation } from "@/services/association";
import saveAssociation from "@/services/association/saveAssociation";
import {
  getAssociations,
  getCountries,
  getOrganisations,
  getStates,
} from "@/services/user";
import { associationSchema } from "@/validations";
import { produce } from "immer";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ValidationError } from "yup";

const PAGE_NAME = "associations";

const CreateAssociation = () => {
  const navigate = useNavigate();
  const searchParams = new URLSearchParams(location.search);
  const id = searchParams.get("id");
  const mode = searchParams.get("mode");

  const permissions = usePermissionStore((state) => state.permissions);
  const [associations, setAssociations] = useState();
  const [association, setAssociation] = useState(getAssociationObject());
  const [countries, setCountries] = useState();
  const [states, setStates] = useState();
  const [errors, setErrors] = useState({});
  const [organisations, setOrganisations] = useState();

  let schema = associationSchema();

  const getCountriesData = async () => {
    let countryResponse = await getCountries();
    if (countryResponse.status === "success") {
      setCountries(countryResponse.data);
    }
  };

  const getAssociationsData = async () => {
    let associationResponse = await getAssociations(0);
    if (associationResponse.status === "success") {
      setAssociations(associationResponse.data);
    }
  };
  const getStatesData = async () => {
    let stateResponse = await getStates();
    if (stateResponse.status === "success") {
      setStates(stateResponse.data);
    }
  };
  const getOrganisationsData = async () => {
    let orgResponse = await getOrganisations();
    if (orgResponse.status === "success") {
      setOrganisations(orgResponse.data);
    }
  };

  useEffect(() => {
    (async () => {
      getOrganisationsData();
      getCountriesData();
      getStatesData();
      getAssociationsData();

      if (mode === "edit") {
        let response = await getAssociation(id);
        if (response.status === "success") {
          setAssociation(response.data);
        } else {
          toast({
            variant: "destructive",
            title: "Error",
            description: "Unable to get association.",
          });
        }
      } else {
        setAssociation(getAssociationObject());
      }
    })();
  }, []);

  const handleChange = (name, index) => (event) => {
    let nextErrors = { ...errors };
    var path = name;
    let nextState = produce(association, (draft) => {
      switch (name) {
        case "organisation":
        case "parentAssociation":
        case "state":
        case "country":
        case "acceptOnePayPayment":
        case "mandateFile":
          draft[name] = event;
          draft["isMandateFileUpdated"] = true;
          break;
        case "qrCodeFile":
          draft[name] = event;
          draft["isQrCodeFileUpdated"] = true;
          break;
        case "associationName":
        case "associationCode":
        case "address1":
        case "address2":
        case "city":
        case "pinCode":
        case "onePayId":
          draft[name] = event.target.value;
          break;
        case "bankName":
        case "branchName":
        case "accountNumber":
        case "ifscCode":
        case "micrCode":
        case "accountName":
          draft["bank"][name] = event.target.value;
          path = "bank." + name;
          break;
        case "associationMessages":
        case "associationContactDetails":
          draft[name] = [...event];
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
    setAssociation(nextState);
    setErrors(nextErrors);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    try {
      schema.validateSync(association, { abortEarly: false });
      let postObject = getAssociationPostObject(association);
      let formData = new FormData();
      appendFormData(formData, postObject);

      let response = await saveAssociation(association.id, formData);
      if (response.status === "success") {
        setAssociation(getAssociationObject());
        navigate("/association");
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
            description: "Unable to save Association.",
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
      <h1 className="text-2xl font-bold mb-10">Create Association</h1>

      <div>
        <h4 className="text-sm font-medium leading-none">Basic Details</h4>
        <Separator className="my-4 " />
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-10">
        <div className="space-y-2">
          <RInput
            id="name"
            type="text"
            label="Association Name"
            name="associationName"
            value={association.associationName}
            error={errors?.associationName}
            onChange={handleChange("associationName")}
            placeholder="Enter association name"
            isRequired={true}
          />
        </div>
        <div className="space-y-2">
          <RInput
            id="ass-code"
            type="text"
            label="Association Code"
            name="associationCode"
            value={association.associationCode}
            error={errors?.associationCode}
            onChange={handleChange("associationCode")}
            placeholder="Enter association name"
          />
        </div>
        <div className="space-y-2">
          <div className="flex flex-col justify-between space-y-2">
            <Label className="mb-3" htmlFor="parent-org">
              <span>Parent Association </span>
            </Label>
            <Combobox
              options={associations}
              placeholder="Select Parent Association"
              valueProperty="id"
              labelProperty="name"
              id="parent-org"
              onChange={handleChange("parentAssociation")}
              value={association.parentAssociation}
              error={errors?.parentAssociation}
            />
          </div>
        </div>
        <div className="space-y-2">
          <div className="flex flex-col justify-between space-y-2">
            <Label className="mb-3" htmlFor="org-name">
              <span>Organisation Name </span>
              <span className="text-red-600	ml-1">*</span>
            </Label>
            <Combobox
              options={organisations}
              valueProperty="id"
              placeholder="Select Organisation"
              labelProperty="name"
              id="org-name"
              onChange={handleChange("organisation")}
              value={association.organisation}
              error={errors?.organisation}
            />
          </div>
        </div>
      </div>

      <div>
        <h4 className="text-sm font-medium leading-none">Address</h4>
        <Separator className="my-4" />
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-10">
        <div className="space-y-2">
          <RInput
            type="text"
            label="Address Line 1"
            id="address-line-1"
            placeholder="Enter Address Line 1"
            onChange={handleChange("address1")}
            value={association.address1}
            error={errors?.address1}
            isRequired={true}
          />
        </div>
        <div className="space-y-2">
          <RInput
            type="text"
            id="address-line-2"
            placeholder="Enter Address Line 2"
            label="Address Line 2"
            onChange={handleChange("address2")}
            value={association.address2}
            error={errors?.address2}
            isRequired={true}
          />
        </div>
        <div className="space-y-2">
          <RInput
            type="text"
            placeholder="Enter City"
            id="city"
            label="City"
            onChange={handleChange("city")}
            value={association.city}
            error={errors?.city}
            isRequired={true}
          />
        </div>
        <div className="space-y-2 mt-1">
          <div className="flex flex-col space-y-2">
            <Label className="mt-2" htmlFor="state">
              <span>State </span>
              <span className="text-red-600	ml-1">*</span>
            </Label>
            <Combobox
              options={states}
              placeholder="Select State"
              valueProperty="id"
              labelProperty="name"
              id="state"
              onChange={handleChange("state")}
              value={association.state}
              error={errors?.state}
            />
          </div>
        </div>
        <div className="space-y-2">
          <RInput
            id="pin-code"
            label="Pin Code"
            placeholder="Enter Pin Code"
            onChange={handleChange("pinCode")}
            value={association.pinCode}
            error={errors?.pinCode}
            isRequired={true}
          />
        </div>
        <div className="space-y-2 ">
          <div className="flex flex-col  space-y-2  ">
            <Label className="mt-2" htmlFor="country">
              <span>Country </span>
              <span className="text-red-600	ml-1">*</span>
            </Label>
            <Combobox
              options={countries}
              placeholder="Select Country"
              valueProperty="id"
              labelProperty="name"
              id="country"
              onChange={handleChange("country")}
              value={association.country}
              error={errors?.country}
            />
          </div>
        </div>
      </div>
      <div className="mt-10">
        <h4 className="text-sm font-medium leading-none">Account Details</h4>
        <Separator className="my-4" />
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-20">
        <div className="space-y-2">
          <RInput
            type="text"
            id="bankName"
            label="Bank Name"
            placeholder="Enter Bank Name"
            onChange={handleChange("bankName")}
            value={association.bank?.bankName}
            error={errors?.["bank.bankName"]}
            isRequired={true}
          />
        </div>
        <div className="space-y-2">
          <RInput
            type="text"
            id="branchName"
            placeholder="Enter Branch Name"
            label="Branch Name"
            onChange={handleChange("branchName")}
            value={association?.bank?.branchName}
            error={errors?.["bank.branchName"]}
            isRequired={true}
          />
        </div>
        <div className="space-y-2">
          <RInput
            type="text"
            id="accountName"
            label="Account Name"
            placeholder="Enter Account Name"
            onChange={handleChange("accountName")}
            value={association.bank?.accountName}
            error={errors?.["bank.accountName"]}
            isRequired={true}
          />
        </div>
        <div className="space-y-2">
          <RInput
            type="text"
            id="accountNumber"
            placeholder="Enter Account Number"
            label="Account Number"
            onChange={handleChange("accountNumber")}
            value={association.bank?.accountNumber}
            error={errors?.["bank.accountNumber"]}
            isRequired={true}
          />
        </div>
        <div className="space-y-2">
          <RInput
            type="text"
            id="ifscCode"
            placeholder="Enter IFSC Code"
            label="IFSC Code"
            onChange={handleChange("ifscCode")}
            value={association.bank?.ifscCode}
            error={errors?.["bank.ifscCode"]}
            isRequired={true}
          />
        </div>
        <div className="space-y-2">
          <RInput
            type="text"
            placeholder="Enter MICR Code"
            id="micrCode"
            label="MICR Code"
            onChange={handleChange("micrCode")}
            value={association.bank?.micrCode}
            error={errors?.["bank.micrCode"]}
            isRequired={true}
          />
        </div>
        <div className="flex flex-col space-y-2">
          <div className="mt-5 mb-5 flex items-center gap-2">
            <Checkbox
              onCheckedChange={handleChange("acceptOnePayPayment")}
              checked={association.acceptOnePayPayment}
            />
            Accept Onepay Payment
          </div>
          {association.acceptOnePayPayment === true ? (
            <div>
              <RInput
                id="onePayId"
                type="text"
                name="onePayId"
                label="Onepay ID"
                onChange={handleChange("onePayId")}
                placeholder="Enter Onepay ID"
                value={association.onePayId}
                error={errors?.onePayId}
                isRequired={association.acceptOnePayPayment ? true : false}
              />
            </div>
          ) : null}
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 gap-4 mb-5">
        <div className="mb-7">
          <Label className="mt-3" htmlFor="mandateFile">
            Mandate
          </Label>
          <FileUpload
            id="mandateFile"
            value={association.mandateFile}
            onChange={handleChange("mandateFile")}
            error={errors?.mandateFile}
            accept=".pdf"
          />
          {errors.mandateFile && (
            <div className="text-red-500">{errors.mandateFile}</div>
          )}
        </div>

        <div className="mb-10 ">
          <Label htmlFor="qrCodeFile">UPI QR Code</Label>
          <FileUpload
            id="qrCodeFile"
            isImage={true}
            value={association.qrCodeFile}
            onChange={handleChange("qrCodeFile")}
            error={errors?.qrCodeFile}
            accept=".png, .jpg, .jpeg"
          />
        </div>
      </div>

      <div className="mt-15">
        <ContactDetails
          contacts={association.associationContactDetails}
          updateContacts={handleChange("associationContactDetails")}
        />
        {errors?.associationContactDetails && (
          <div className="pt-5 text-red-500">
            {errors.associationContactDetails}
          </div>
        )}
      </div>

      <div className="mt-20">
        <Messages
          messages={association.associationMessages}
          updateMessages={handleChange("associationMessages")}
        />
      </div>

      <div className="mt-3 flex justify-end ">
        <Button
          variant="secondary"
          className="mr-2"
          onClick={() => {
            navigate("/association");
          }}
        >
          Cancel
        </Button>
        {permissions?.[PAGE_NAME]?.create ? (
          <Button onClick={handleSubmit}>Save</Button>
        ) : null}
      </div>
    </div>
  );
};

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(CreateAssociation))
);
