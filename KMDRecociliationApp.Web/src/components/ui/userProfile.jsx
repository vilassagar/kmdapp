/* eslint-disable no-case-declarations */
import { getUserPostObject } from "@/lib/helperFunctions";
import { userStore } from "@/lib/store";
import {
  getAssociationsByIdFilter,
  getGenders,
  getOrganisations,
  getStates,
  getUser,
  saveUser,
  getPensionerIdTypes,
} from "@/services/customerProfile";
import { getAllRoles, getRoles } from "@/services/roles";
import { userProfileSchema } from "@/validations";
import countryCodes from "country-codes-list";
import { produce } from "immer";
import { GlobeIcon } from "lucide-react";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { ValidationError } from "yup";
import { Combobox } from "./comboBox";
import DatePicker from "./datePicker";
import { Label } from "./label";
import { MultiCombobox } from "./multiComboBox";
import RButton from "./rButton";
import RInput from "./rInput";
import RTextArea from "./rTextArea";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
} from "./select";
import { toast } from "./use-toast";
// import DatePicker from "react-datepicker";
import checkPaymentByUsers from "@/services/customerProfile/checkPaymentByUser";

// import CustomDatePicker from "./customDatePicker";

export default function UserProfile({
  userType,
  mode,
  onSave,
  userId,
  permissions,
  pageName,
}) {
  const SCHEMA = userProfileSchema();
  const userStoreData = userStore((state) => state.user);
  // Get campaign ID from URL
  const campaignId = new URLSearchParams(window.location.search).get(
    "campaignId"
  );

  const shouldShowOrgFields = () => {
    // Parse localStorage data properly
    let loggedInUserTypeValue = "";
    try {
      const userDataString = localStorage.getItem("user");
      if (userDataString) {
        const userData = JSON.parse(userDataString);
        loggedInUserTypeValue =
          userData?.state?.user?.userType?.name?.toLowerCase().trim() || "";
      }
    } catch (error) {
      console.error("Error parsing user data from localStorage:", error);
    }

    // Handle userType properly
    const userTypeValue =
      typeof userType === "string"
        ? userType?.toLowerCase().trim()
        : userType?.name?.toLowerCase().trim() || "";

    // Case 1: Both are internal users - don't show org fields
    if (
      (userTypeValue === "internaluser" &&
        loggedInUserTypeValue === "internaluser") ||
      (userTypeValue === "datacollection" &&
        loggedInUserTypeValue === "datacollection")
    ) {
      return false;
    }

    // Case 2: Internal user viewing a community user - show org fields
    if (
      loggedInUserTypeValue === "internaluser" &&
      userTypeValue === "community"
    ) {
      return true;
    }

    if (
      loggedInUserTypeValue === "internaluser" &&
      userTypeValue === "datacollection"
    ) {
      return false;
    }

    // Case 3: Show for pensioner or association types
    if (userTypeValue === "pensioner" || userTypeValue === "association") {
      return true;
    }

    // Case 4: Show when internal user is viewing non-internal users
    if (
      loggedInUserTypeValue === "internaluser" &&
      userTypeValue !== "internaluser"
    ) {
      return true;
    }

    // Default: Don't show
    return false;
  };
  const associationId =
    userStoreData?.userType?.name?.toLowerCase()?.trim() === "association"
      ? userStoreData?.associationId
      : 0;
  const countryCodesObject = countryCodes.customList(
    "countryNameEn",
    "+{countryCallingCode}"
  );

  const loggedInUser = userStore((state) => state.user);

  const [user, setUser] = useState({
    userId: 0,
    userType: null,
    roles: [],
    firstName: "",
    lastName: "",
    empId: "",
    organisation: null,
    association: null,
    dateOfBirth: "",
    gender: null,
    email: "",
    address: "",
    mobileNumber: "",
    countryCode: countryCodesObject["India"],
    state: null,
    pincode: "",
    pensioneridtype: null,
    uniqueidnumber: "",
    password: "",
  });
  const [errors, setErrors] = useState({});
  const search = window.location.search;
  const params = new URLSearchParams(search);
  const paramId = params.get("userId");
  const [organisations, setOrganisations] = useState([]);
  const [associations, setAssociations] = useState([]);
  const [genders, setGenders] = useState([]);
  const [roles, setRoles] = useState([]);
  const [states, setStates] = useState([]);
  const [pensioneridtypes, setpensioneridtypes] = useState([]);
  const [type, setType] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    (async () => {
      try {
        const [response1, response2, response3, response4, response5] =
          await Promise.all([
            getOrganisations(),
            getAssociationsByIdFilter(associationId, "all"),
            getGenders(),
            getStates(),
            // checkPaymentByUsers(paramId ? paramId : userId),
            getPensionerIdTypes(),
          ]);

        setOrganisations(response1.data);
        setAssociations(response2.data);
        setGenders(response3.data);
        setStates(response4.data);
        setpensioneridtypes(response5.data);

        if (mode === "new" && pageName === "users") {
          let nextState = produce(user, (draft) => {
            draft["userType"] = userType;
          });
          setType(userType);
          setUser(nextState);
          setUser((prev) => ({
            ...prev,
            userType: userType,
            pensioneridtype: pensioneridtypes?.[0] || null,
          }));
        }

        // Get roles and set default pensioner role
        const rolesResponse = await getAllRoles();
        if (rolesResponse.status === "success") {
          setRoles(rolesResponse.data);

          // If new mode and user type is pensioner or community
          if (mode === "new" && userType?.name?.toLowerCase()) {
            const userTypeLower = userType.name.toLowerCase().trim();

            if (
              userTypeLower === "pensioner" ||
              userTypeLower === "community"
            ) {
              // Find pensioner role
              const pensionerRole = rolesResponse.data.find((role) =>
                role.name.toLowerCase().includes("pensioner")
              );

              if (pensionerRole) {
                setUser((prev) => ({
                  ...prev,
                  roles: [pensionerRole],
                  userType: userType,
                }));
              }
            }
          }
        }
      } catch (err) {
        console.error("Error loading initial data:", err);
      }

      if (mode === "edit") {
        let [response1, response2, response6] = await Promise.all([
          getUser(paramId ? paramId : userId),
          getAllRoles(),
          getPensionerIdTypes(),
        ]);

        if (response1.status === "success") {
          const userData = response1.data;
          const pensionerIdTypesData = response6.data;
          setType(userData?.userType);
          setpensioneridtypes(pensionerIdTypesData);

          const userWithPensionerIdType = {
            ...userData,
            pensioneridtype: userData.pensioneridtype
              ? pensionerIdTypesData.find(
                  (type) => type.id === userData.pensioneridtype.id
                ) || pensionerIdTypesData[0]
              : pensionerIdTypesData[0],
          };

          setUser(userWithPensionerIdType);
        }
      }
    })();
  }, [mode, userType]);

  const handleChange = (name) => async (event) => {
    let nextErrors = { ...errors };
    let nextState = produce(user, (draft) => {
      switch (name) {
        case "roles":
          draft[name] = event;
          break;

        case "firstName":
          draft[name] = event.target.value;
          break;

        case "lastName":
          draft[name] = event.target.value;
          break;

        case "empId":
          draft[name] = event.target.value;
          break;

        case "organisation":
          draft[name] = event;
          break;

        case "association":
          draft[name] = event;
          break;

        case "dateOfBirth":
          const formattedDate = event;
          draft[name] = formattedDate;
          break;

        case "gender":
          draft[name] = event;
          break;

        case "email":
          draft[name] = event.target.value;
          break;

        case "countryCode":
          draft[name] = countryCodesObject[event];
          break;

        case "mobileNumber":
          draft[name] = event.target.value;
          break;

        case "state":
          draft[name] = event;
          break;

        case "pincode":
          draft[name] = event.target.value;
          break;

        case "address":
          draft[name] = event.target.value;
          break;
        case "pensioneridtype":
          draft[name] = event;
          break;
        case "uniqueidnumber":
          draft[name] = event.target.value;
          break;
        case "password":
          draft[name] = event.target.value;
          break;
      }

      try {
        const context = { userType: type?.name || "" };
        SCHEMA.validateSyncAt(name, draft, { context: context });
        nextErrors[name] = [];
      } catch (e) {
        nextErrors[name] = [...e.errors];
      }
    });

    setErrors(nextErrors);
    setUser(nextState);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    try {
      setIsLoading(true);
      const context = { userType: type?.name || "" };
      SCHEMA.validateSync(user, { abortEarly: false, context });
      let postObject = getUserPostObject(user);
      let response = await saveUser(postObject);
      if (response.status === "success") {
        toast({
          title: "User saved sucessfully",
          description: "User has been saved successfully.",
        });
        onSave();
      } else {
        if (response.status === "conflict") {
          //409
          toast({
            variant: "destructive",
            title: "Something went wrong.",
            description: response?.errors?.message || "User already exists.",
          });
        } else if (response.status === "invalid") {
          setErrors(response.validationErrors);
        } else {
          //400, 404, 500
          toast({
            variant: "destructive",
            title: "Something went wrong.",
            description: "Unable to save user.",
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
    setIsLoading(false);
  };

  const isFieldDisabled = (field) => {
    if (mode === "new" || paramId) {
      return false;
    }

    if (
      loggedInUser?.userType?.name?.toLowerCase() === "pensioner" ||
      loggedInUser?.userType?.name?.toLowerCase() === "community"
    ) {
      return (
        field === "organisation" ||
        field === "association" ||
        field === "mobileNumber" ||
        field === "countryCode"
      );
    }
    if (
      loggedInUser?.userType?.name?.toLowerCase() === "pensioner" ||
      loggedInUser?.userType?.name?.toLowerCase() === "community"
    ) {
      return field === "mobileNumber" || field === "countryCode";
    }

    return false;
  };

  const handleNext = async (event) => {
    event.preventDefault();
    try {
      setIsLoading(true);
      const context = { userType: type?.name || "" };
      // Validate the form data against the schema
      SCHEMA.validateSync(user, { abortEarly: false, context });

      // Skip the API call that was in handleSubmit
      // Just show success toast and call onSave
      toast({
        title: "Validation successful",
        description: "You can proceed to the next step.",
      });
      onSave();
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
    setIsLoading(false);
  };

  return (
    <form className="space-y-6">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="space-y-2">
          <RInput
            label="First Name"
            placeholder="Enter first name"
            value={user.firstName}
            onChange={handleChange("firstName")}
            id="first-name"
            error={errors?.firstName}
            isRequired={true}
            type="text"
            isDisabled={isFieldDisabled("firstName")}
          />
          {errors?.firstName && (
            <div className="text-red-600 text-xs py-1">{errors.firstName}</div>
          )}
        </div>
        <div className="space-y-2">
          <RInput
            label="Last Name"
            placeholder="Enter last name"
            value={user.lastName}
            onChange={handleChange("lastName")}
            id="last-name"
            error={errors?.lastName}
            isRequired={true}
            type="text"
            isDisabled={isFieldDisabled("lastName")}
          />
          {errors?.lastName && (
            <div className="text-red-600 text-xs py-1">{errors.lastName}</div>
          )}
        </div>
      </div>

      {type?.name?.toLowerCase().trim() === "pensioner" ||
      type?.name?.toLowerCase().trim() === "association" ? (
        <div className="grid grid-cols-2 gap-4">
          <div className="space-y-2">
            <RInput
              label="EMP ID / PF No."
              placeholder="Enter EMP ID / PF No."
              value={user.empId}
              onChange={handleChange("empId")}
              id="emp-id"
              error={errors?.empId}
              isRequired={true}
              type="text"
              isDisabled={isFieldDisabled("empId")}
            />
          </div>
        </div>
      ) : null}

      {shouldShowOrgFields() ? (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="flex flex-col justify-between space-y-2">
            <Label htmlFor="org-name">
              <span>
                {type?.name?.toLowerCase().trim() === "association"
                  ? ""
                  : " Pensioner"}{" "}
                Organisation Name{" "}
              </span>
              {type?.name?.toLowerCase().trim() === "association" && (
                <span className="text-red-600 ml-1">*</span>
              )}
            </Label>
            <Combobox
              options={organisations}
              valueProperty="id"
              labelProperty="name"
              id="org-name"
              onChange={handleChange("organisation")}
              value={user.organisation}
              error={errors?.organisation}
              isDisabled={isFieldDisabled("organisation")}
            />
          </div>

          <div className="flex flex-col justify-between space-y-2">
            <Label htmlFor="association-name">
              <span>Association Name </span>
              <span className="text-red-600 ml-1">*</span>
            </Label>
            <Combobox
              options={associations}
              valueProperty="id"
              labelProperty="name"
              id="association-name"
              onChange={handleChange("association")}
              value={user.association}
              error={errors?.association}
              isDisabled={campaignId > 0 || isFieldDisabled("association")}
            />
            {errors?.associations && (
              <div className="text-red-600 text-xs py-1">
                {errors.associations}
              </div>
            )}
          </div>
        </div>
      ) : null}

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="space-y-2">
          <DatePicker
            label="Date of birth"
            id="dob"
            type="date"
            placeholder="dd/mm/yyyy"
            onChange={handleChange("dateOfBirth")}
            isRequired={true}
            error={errors?.dateOfBirth}
            isDisabled={isFieldDisabled("dateOfBirth")}
            date={user?.dateOfBirth}
            isFutureDateAllowed={false}
          />
          {errors?.dateOfBirth && (
            <div className="text-red-600 text-xs py-1">
              {errors.dateOfBirth}
            </div>
          )}
        </div>

        <div className="flex flex-col justify-between space-y-2">
          <Label htmlFor="gender">
            Gender{" "}
            {type?.name?.toLowerCase().trim() === "pensioner" ||
            type?.name?.toLowerCase().trim() === "association" ||
            type?.name?.toLowerCase().trim() === "community" ? (
              <span className="text-red-600 ml-1">*</span>
            ) : (
              ""
            )}
          </Label>
          <Combobox
            options={genders}
            valueProperty="id"
            labelProperty="name"
            id="gender"
            onChange={handleChange("gender")}
            value={user.gender}
            error={errors?.gender}
            isDisabled={isFieldDisabled("gender")}
          />
          {errors?.gender && (
            <div className="text-red-600 text-xs py-1">{errors.gender}</div>
          )}
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="space-y-2">
          <RInput
            label="Email"
            placeholder="Enter Email"
            value={user.email}
            onChange={handleChange("email")}
            id="email"
            type="text"
            error={errors?.email}
            isDisabled={isFieldDisabled("email")}
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="mobile-number">
            <span>Mobile Number</span>
            <span className="text-red-600 ml-1">*</span>
          </Label>
          <div className="flex">
            {/* <Select
              onValueChange={handleChange("countryCode")}
              value={user.countryCode}
              className="mr-5"
              isDisabled={isFieldDisabled("countryCode")}
            >
              <SelectTrigger className="w-[120px]">
                {user.countryCode === "" ? (
                  <GlobeIcon className="w-4 h-4 mr-2" />
                ) : (
                  user.countryCode
                )}
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  {Object.keys(countryCodesObject)?.map(
                    (countryName, index) => (
                      <SelectItem key={index} value={countryName}>
                        {countryName}
                      </SelectItem>
                    )
                  )}
                </SelectGroup>
              </SelectContent>
            </Select> */}
            <div className="ml-3 w-full">
              <RInput
                id="mobile"
                type="number"
                name="mobileNumber"
                onChange={handleChange("mobileNumber")}
                placeholder="Enter Mobile Number"
                error={errors?.mobileNumber}
                value={user.mobileNumber}
                isDisabled={isFieldDisabled("mobileNumber")}
              />
              {errors?.mobileNumber && (
                <div className="text-red-600 text-xs py-1">
                  {errors.mobileNumber}
                </div>
              )}
            </div>
          </div>
        </div>

        {(type?.name?.toLowerCase().trim() === "pensioner" ||
          type?.name?.toLowerCase().trim() === "association" ||
          type?.name?.toLowerCase().trim() === "community") && (
          <>
            <div className="flex flex-col justify-between space-y-2">
              <Label htmlFor="state-name">
                <span>State </span>
                <span className="text-red-600 ml-1">*</span>
              </Label>
              <Combobox
                options={states}
                valueProperty="id"
                labelProperty="name"
                id="state-name"
                onChange={handleChange("state")}
                value={user.state}
                error={errors?.state}
                isDisabled={isFieldDisabled("state")}
              />
              {errors?.state && (
                <div className="text-red-600 text-xs py-1">{errors.state}</div>
              )}
            </div>

            <div className="space-y-2">
              <RInput
                label="Pincode"
                placeholder="Enter pincode"
                value={user.pincode}
                onChange={handleChange("pincode")}
                id="pincode"
                type="text"
                isDisabled={isFieldDisabled("pincode")}
                isRequired={true}
                error={errors?.pincode}
              />
              {errors?.pincode && (
                <div className="text-red-600 text-xs py-1">
                  {errors.pincode}
                </div>
              )}
            </div>
            {type?.name?.toLowerCase().trim() !== "community" && (
              <div className="flex flex-col justify-between space-y-2">
                <Label htmlFor="pensioneridtype">
                  <span>Id Type </span>
                  {/* <span className="text-red-600 ml-1">*</span> */}
                </Label>

                <Combobox
                  options={pensioneridtypes}
                  valueProperty="id"
                  labelProperty="name"
                  id="pensioneridtype-name"
                  onChange={handleChange("pensioneridtype")}
                  value={user.pensioneridtype}
                  error={errors?.pensioneridtype}
                  isDisabled={isFieldDisabled("pensioneridtype")}
                />
              </div>
            )}
            {type?.name?.toLowerCase().trim() !== "community" && (
              <div className="space-y-2">
                <RInput
                  label="Unique ID Number"
                  placeholder="Unique ID Number"
                  value={user.uniqueidnumber}
                  onChange={handleChange("uniqueidnumber")}
                  id="uniqueidnumber"
                  type="text"
                  isDisabled={isFieldDisabled("uniqueidnumber")}
                  isRequired={false}
                  error={errors?.uniqueidnumber}
                />
                {errors?.uniqueidnumber && (
                  <div className="text-red-600 text-xs py-1">
                    {errors.uniqueidnumber}
                  </div>
                )}
              </div>
            )}
            <div className="space-y-2">
              <RTextArea
                isRequired={true}
                label="Address"
                placeholder="Enter address"
                value={user.address}
                onChange={handleChange("address")}
                id="address"
                isDisabled={isFieldDisabled("address")}
                error={errors?.address}
              />
              {errors?.address && (
                <div className="text-red-600 text-xs py-1">
                  {errors.address}
                </div>
              )}
            </div>
          </>
        )}
      </div>

      <div className="grid grid-cols-2 gap-4">
        {(type?.name?.toLowerCase().trim() === "internaluser" ||
          type?.name?.toLowerCase().trim() === "datacollection" ||
          type?.name?.toLowerCase().trim() === "association") && (
          <div className="flex flex-col justify-between space-y-2">
            <Label htmlFor="role">
              <span>Role</span>
              <span className="text-red-600 ml-1">*</span>
            </Label>
            <MultiCombobox
              options={roles}
              valueProperty="id"
              labelProperty="name"
              id="role"
              onChange={handleChange("roles")}
              values={user.roles}
              error={errors?.roles}
              isDisabled={isFieldDisabled("roles")}
            />
          </div>
        )}
        {(type?.name?.toLowerCase().trim() === "internaluser" ||
          type?.name?.toLowerCase().trim() === "datacollection" ||
          type?.name?.toLowerCase().trim() === "association") && (
          <div className="flex flex-col justify-between space-y-2">
            <RInput
              label="Password"
              placeholder="Enter Password"
              value={user.password}
              onChange={handleChange("password")}
              id="password"
              type="text"
              isDisabled={isFieldDisabled("password")}
              isRequired={false}
              error={errors?.password}
            />
          </div>
        )}
      </div>

      {permissions?.[pageName]?.create ? (
        <div className="flex justify-end">
          <RButton
            onClick={handleSubmit}
            type="submit"
            isLoading={isLoading}
            // isDisabled={!type?.name?.length}
          >
            Save
          </RButton>
        </div>
      ) : null}
    </form>
  );
}

UserProfile.defaultProps = {
  userType: null,
  mode: "new",
  onSave: "",
  userId: 0,
  permissions: null,
  pageName: "",
};

UserProfile.propTypes = {
  userType: PropTypes.object,
  mode: PropTypes.string,
  onSave: PropTypes.string,
  userId: PropTypes.number,
  permissions: PropTypes.object,
  pageName: PropTypes.string,
};
