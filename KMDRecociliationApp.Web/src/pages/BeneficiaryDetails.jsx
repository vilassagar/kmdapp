/* eslint-disable no-undef */
import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Combobox } from "@/components/ui/comboBox";
import { DatePicker } from "@/components/ui/datePicker";
import FileUpload from "@/components/ui/fileUpload";
import { Label } from "@/components/ui/label";
import RButton from "@/components/ui/rButton";
import RInput from "@/components/ui/rInput";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { toast } from "@/components/ui/use-toast";
import {
  appendFormData,
  getBeneficiariesPost,
  getBeneficiaryFlags,
} from "@/lib/helperFunctions";
import {
  useLocationStore,
  usePermissionStore,
  usePolicyStore,
} from "@/lib/store";
import {
  addProductPolicy,
  getGenders,
  getNomineeRelations,
} from "@/services/customerProfile";
import { beneficarySchema, nomineeSchema } from "@/validations";
import { produce } from "immer";
import { Terminal } from "lucide-react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ValidationError } from "yup";

const PAGE_NAME = "products";
const PAGE_NAME_nomineedetails = "nomineedetails";
const PAGE_NAME_dependentdetails = "dependentdetails";
function BeneficiaryDetails() {
  const locations = useLocationStore((state) => state.locations);
  if (locations.length == 0) {
    window.location.href = "/productlist";
  }
  const search = window.location.search;
  const params = new URLSearchParams(search);
  const userId = params.get("userId");
  const campaignId = params.get("campaignId");
  const updatedPolicyId = params.get("policyId");

  const SCHEMA = beneficarySchema();
  const NOMINEE_SCHEMA = nomineeSchema();

  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("dependents");

  const permissions = usePermissionStore((state) => state.permissions);

  const userDetails = usePolicyStore((state) => state.userDetails);

  const updateBeneficiaries = usePolicyStore(
    (state) => state.updateBeneficiaries
  );
  const step = usePolicyStore((state) => state.step);
  const setStep = usePolicyStore((state) => state.setStep);
  const updateBeneficiaryId = usePolicyStore(
    (state) => state.updateBeneficiaryId
  );
  const mode = usePolicyStore((state) => state.mode);

  const [isLoading, setIsLoading] = useState(false);

  const [flags, setFlags] = useState({});
  const [genders, setGenders] = useState([]);
  const [relations, setRelations] = useState([]);
  const [policyTypeId, setPolicyTypeId] = useState(null);

  useEffect(() => {
    (async () => {
      setStep(2);
      let newFlags = getBeneficiaryFlags(userDetails?.products);
      setPolicyTypeId(userDetails?.products.policyTypeId);
      setFlags(newFlags);

      //get genders
      let response = await getGenders();
      if (response.status === "success") {
        setGenders(response.data);
      }

      //get relations
      let relationResponse = await getNomineeRelations();
      if (relationResponse.status === "success") {
        setRelations(relationResponse.data);
      }
    })();
  }, []);

  const handleTabChange = (tabName) => {
    if (tabName === "nominees") {
      if (!Object.keys(flags).length) {
        setActiveTab("nominees");
        setStep(3);
      } else {
        handleSubmitBeneficiaries();
      }
    } else {
      setActiveTab(tabName);
    }
  };
  const handleChange = (name, field) => (event) => {
    let nextErrors = { ...userDetails?.beneficiaries?.errors } || {};

    let nextState = produce(userDetails, (draft) => {
      // Initialize beneficiaries object if it doesn't exist
      if (!draft.beneficiaries) {
        draft.beneficiaries = {
          spouse: null,
          child1: null,
          child2: null,
          parent1: null,
          parent2: null,
          inLaw1: null,
          inLaw2: null,
          nominee: null,
          errors: {},
        };
      }

      // Initialize the specific beneficiary object if it's null
      if (draft.beneficiaries[name] === null) {
        draft.beneficiaries[name] = {};
      }

      // Initialize errors object if it doesn't exist
      if (!draft.beneficiaries.errors) {
        draft.beneficiaries.errors = {};
      }

      // Handle different field types
      switch (field) {
        case "name":
          draft.beneficiaries[name][field] = event.target.value;
          break;

        case "gender":
          draft.beneficiaries[name][field] = event;

          // Special handling for nominee relation gender sync
          if (name === "nominee" && relations) {
            const selectedRelation = relations.find(
              (relation) => relation.gender === event.gender
            );
            if (selectedRelation && genders) {
              const matchedGender = genders.find(
                (gender) => gender.id === selectedRelation.gender
              );
              if (matchedGender) {
                draft.beneficiaries.nominee.gender = matchedGender;
              }
            }
          }
          break;

        case "dateOfBirth":
          draft.beneficiaries[name][field] = event;
          break;

        case "disabilityCertificate":
          draft.beneficiaries[name][field] = event;
          draft.beneficiaries[name].isCertificateUpdated = true;
          break;

        case "nomineeRelation":
          draft.beneficiaries[name][field] = event;

          if (relations && genders) {
            const selectedRelation = relations.find(
              (relation) => relation.id === event.id
            );
            if (selectedRelation) {
              const matchedGender = genders.find(
                (gender) => gender.id === selectedRelation.gender
              );
              if (matchedGender) {
                draft.beneficiaries.nominee.gender = matchedGender;
              }
            }
          }
          break;

        default:
          break;
      }

      try {
        const path = `${name}.${field}`;
        if (name === "nominee") {
          NOMINEE_SCHEMA.validateSyncAt(path, draft.beneficiaries);
        } else {
          SCHEMA.validateSyncAt(path, draft.beneficiaries, { context: flags });
        }
        nextErrors[path] = [];
      } catch (e) {
        nextErrors[path] = e.errors;
      }
    });

    // Update the errors in the state
    nextState = produce(nextState, (draft) => {
      draft.beneficiaries.errors = nextErrors;
    });

    // Update the store
    updateBeneficiaries(nextState.beneficiaries);
  };

  // Helper function to get initial beneficiaries state
  const getInitialBeneficiariesState = () => ({
    spouse: null,
    child1: null,
    child2: null,
    parent1: null,
    parent2: null,
    inLaw1: null,
    inLaw2: null,
    nominee: null,
    errors: {},
  });

  const handleSubmitBeneficiaries = async () => {
    setIsLoading(true);
    try {
      SCHEMA.validateSync(userDetails.beneficiaries, {
        abortEarly: false,
        context: flags,
      });

      //save request for beneficieries
      let postObject = {
        policyId: userDetails?.policyId || 0,
        userId: userDetails?.userId || 0,
        beneficiaries: getBeneficiariesPost(userDetails?.beneficiaries, flags),
        isUpDate: mode === "edit" ? true : false,
        ...(mode === "edit"
          ? { beneficiaryId: userDetails?.beneficiaryId || 0 }
          : {}),
      };

      let formData = new FormData();

      appendFormData(formData, postObject);

      let response = await addProductPolicy(formData, step);
      if (response.status === "success") {
        setStep(3);
        updateBeneficiaryId(response?.data?.beneficiaryId || 0);
        setActiveTab("nominees");
      } else {
        toast({
          variant: "destructive",
          title: "Something went wrong.",
          description: "Unable to save details.",
        });
      }
    } catch (e) {
      if (e instanceof ValidationError) {
        let newEr = produce({}, (draft) => {
          e.inner.forEach((error) => {
            draft[error.path] = [...error.errors];
          });
        });

        //set errors
        let nextState = produce(userDetails, (draft) => {
          draft["beneficiaries"]["errors"] = newEr;
        });
        updateBeneficiaries(nextState.beneficiaries);
      }
    }

    setIsLoading(false);
  };

  const handleSaveNominee = async () => {
    setIsLoading(true);
    try {
      NOMINEE_SCHEMA.validateSync(userDetails.beneficiaries, {
        abortEarly: false,
      });

      //save request for beneficieries
      let postObject = {
        policyId: userDetails?.policyId || updatedPolicyId,
        userId: userDetails?.userId || 0,
        beneficiaryId: userDetails?.beneficiaryId || 0,
        nominee: userDetails?.beneficiaries?.nominee,
        isUpDate: mode === "edit" ? true : false,
      };
      const formData = new FormData();

      appendFormData(formData, postObject, "");

      let response = await addProductPolicy(formData, step);
      if (response.status === "success") {
        setStep(4);
        if (userId > 0) {
          navigate(
            `/payment?policyId=${updatedPolicyId}&userId=${userId}&campaignId=${campaignId}`
          );
        } else {
          navigate(
            `/payment?policyId=${updatedPolicyId}&campaignId=${campaignId}`
          );
        }
      } else {
        toast({
          variant: "destructive",
          title: "Something went wrong.",
          description: "Unable to save details.",
        });
      }
    } catch (e) {
      if (e instanceof ValidationError) {
        let newEr = produce({}, (draft) => {
          e.inner.forEach((error) => {
            draft[error.path] = [...error.errors];
          });
        });

        //set errors
        let nextState = produce(userDetails, (draft) => {
          draft["beneficiaries"]["errors"] = newEr;
        });

        updateBeneficiaries(nextState.beneficiaries);
      }
    }
    setIsLoading(false);
  };

  const handleNextNominee = async () => {
    setIsLoading(true);
    try {
      setStep(4);
      // navigate("/payment");
      if (userId > 0) {
        navigate(`/payment?userId=${userId}&campaignId=${campaignId}`);
      } else {
        navigate(`/payment?campaignId=${campaignId}`);
      }
    } catch (e) {
      if (e instanceof ValidationError) {
        let newEr = produce({}, (draft) => {
          e.inner.forEach((error) => {
            draft[error.path] = [...error.errors];
          });
        });

        //set errors
        let nextState = produce(userDetails, (draft) => {
          draft["beneficiaries"]["errors"] = newEr;
        });

        updateBeneficiaries(nextState.beneficiaries);
      }
    }
    setIsLoading(false);
  };

  return (
    <div>
      <h1 className="text-2xl font-bold mb-6">Beneficiary Details</h1>
      <div className=" mx-auto ">
        <Tabs
          defaultValue="dependents"
          className="w-full"
          value={activeTab}
          onValueChange={handleTabChange}
        >
          <TabsList className="mb-10">
            <TabsTrigger value="dependents">Dependent Details</TabsTrigger>
            <TabsTrigger value="nominees">Nominee Details</TabsTrigger>
          </TabsList>
          <TabsContent value="dependents">
            {!Object.keys(flags).length ? (
              <div className="grid gap-6">
                <Alert>
                  <Terminal className="h-4 w-4" />
                  <AlertTitle>Heads up!</AlertTitle>
                  <AlertDescription>
                    No Beneficiary selected for policy.
                  </AlertDescription>
                </Alert>
              </div>
            ) : (
              <div className="grid gap-6">
                {flags?.isSpousePremiumSelected ||
                flags?.isSelfSpousePremiumSelected ||
                flags?.isSelfSpouse2ChildrenPremiumSelected ||
                flags?.isSelfSpouse1ChildrenPremiumSelected ||
                flags?.isTopUpSelfSpousePremiumSelected ||
                flags?.isSelfSpouse2ChildrenPremiumSelected ||
                flags?.isSelfSpouse1ChildrenPremiumSelected ? (
                  <Card className="shadow-lg border-0">
                    <CardHeader>
                      <CardTitle>Spouse</CardTitle>
                    </CardHeader>
                    <CardContent className="grid gap-4">
                      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        <div className="space-y-2">
                          <RInput
                            label="Name"
                            id="spouse-name"
                            onChange={handleChange("spouse", "name")}
                            value={
                              userDetails?.beneficiaries?.spouse?.name || ""
                            }
                            type="text"
                            isRequired={true}
                            error={
                              userDetails?.beneficiaries?.errors[`spouse.name`]
                            }
                          />
                        </div>
                        <div className="space-y-2 flex flex-col justify-end">
                          <Label htmlFor="spouse-gender ">
                            <span>Gender</span>
                            <span className="text-red-600	ml-1">*</span>
                          </Label>
                          <Combobox
                            options={genders}
                            valueProperty="id"
                            labelProperty="name"
                            id="spouse-gender"
                            onChange={handleChange("spouse", "gender")}
                            value={userDetails?.beneficiaries?.spouse?.gender}
                            error={
                              userDetails?.beneficiaries?.errors[
                                `spouse.gender`
                              ]
                            }
                          />
                        </div>
                        <div className="space-y-2 flex items-end">
                          <DatePicker
                            label="Date of birth"
                            id="dob"
                            type="date"
                            placeholder="dd-mm-yyyy"
                            onChange={handleChange("spouse", "dateOfBirth")}
                            isRequired={true}
                            date={
                              userDetails?.beneficiaries?.spouse?.dateOfBirth
                            }
                            isFutureDateAllowed={false}
                            size="sm"
                            error={
                              userDetails?.beneficiaries?.errors[
                                `spouse.dateOfBirth`
                              ]
                            }
                          />
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                ) : null}

                {flags?.isChild1PremiumSelected ||
                flags?.isSelfSpouse2ChildrenPremiumSelected ||
                flags?.isSelfSpouse1ChildrenPremiumSelected ||
                flags?.isSelf1ChildrenPremiumSelected ||
                flags?.isSelf2ChildrenPremiumSelected ? (
                  <Card className="shadow-lg border-0">
                    <CardHeader>
                      <CardTitle>Child 1</CardTitle>
                    </CardHeader>
                    <CardContent className="grid gap-4">
                      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        <div className="space-y-2">
                          <RInput
                            label="Name"
                            id="chid1-name"
                            onChange={handleChange("child1", "name")}
                            value={
                              userDetails?.beneficiaries?.child1?.name || ""
                            }
                            type="text"
                            isRequired={true}
                            error={
                              userDetails?.beneficiaries?.errors[`child1.name`]
                            }
                          />
                        </div>
                        <div className="space-y-2 flex flex-col justify-end ">
                          <Label htmlFor="child1-gender ">
                            <span>Gender</span>
                            <span className="text-red-600	ml-1">*</span>
                          </Label>
                          <Combobox
                            options={genders}
                            valueProperty="id"
                            labelProperty="name"
                            id="child1-gender"
                            onChange={handleChange("child1", "gender")}
                            value={userDetails?.beneficiaries?.child1?.gender}
                            isRequired={true}
                            error={
                              userDetails?.beneficiaries?.errors[
                                `child1.gender`
                              ]
                            }
                          />
                        </div>
                        <div className="space-y-2 flex items-end">
                          <DatePicker
                            label="Date of birth"
                            id="dob"
                            type="date"
                            placeholder="dd-mm-yyyy"
                            onChange={handleChange("child1", "dateOfBirth")}
                            isRequired={true}
                            isFutureDateAllowed={false}
                            date={
                              userDetails?.beneficiaries?.child1?.dateOfBirth
                            }
                            error={
                              userDetails?.beneficiaries?.errors[
                                `child1.dateOfBirth`
                              ]
                            }
                          />
                        </div>
                        {policyTypeId && (
                          <div className="grid gap-2">
                            <Label className="mt-3" htmlFor="chequeFile">
                              Upload Disability Certificate
                            </Label>
                            <FileUpload
                              id="child1-cert"
                              onChange={handleChange(
                                "child1",
                                "disabilityCertificate"
                              )}
                              value={
                                userDetails?.beneficiaries?.child1
                                  ?.disabilityCertificate || null
                              }
                              error={
                                userDetails?.beneficiaries?.errors[
                                  `child1.disabilityCertificate`
                                ]
                              }
                              accept=".pdf"
                            />
                          </div>
                        )}
                      </div>
                    </CardContent>
                  </Card>
                ) : null}

                {flags?.isChild2PremiumSelected ||
                flags?.isSelfSpouse2ChildrenPremiumSelected ||
                flags?.isSelf2ChildrenPremiumSelected ? (
                  <Card className="shadow-lg border-0">
                    <CardHeader>
                      <CardTitle>Child 2</CardTitle>
                    </CardHeader>
                    <CardContent className="grid gap-4">
                      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        <div className="space-y-2">
                          <RInput
                            label="Name"
                            id="chil2-name"
                            onChange={handleChange("child2", "name")}
                            value={
                              userDetails?.beneficiaries?.child2?.name || ""
                            }
                            type="text"
                            isRequired={true}
                            error={
                              userDetails?.beneficiaries?.errors[`child2.name`]
                            }
                          />
                        </div>

                        <div className="space-y-2 flex flex-col justify-end ">
                          <Label htmlFor="child2-gender ">
                            <span>Gender</span>
                            <span className="text-red-600	ml-1">*</span>
                          </Label>
                          <Combobox
                            options={genders}
                            valueProperty="id"
                            labelProperty="name"
                            id="child2-gender"
                            onChange={handleChange("child2", "gender")}
                            value={userDetails?.beneficiaries?.child2?.gender}
                            isRequired={true}
                            error={
                              userDetails?.beneficiaries?.errors[
                                `child2.gender`
                              ]
                            }
                          />
                        </div>
                        <div className="space-y-2 flex items-end">
                          <DatePicker
                            label="Date of birth"
                            id="dob"
                            type="date"
                            placeholder="dd-mm-yyyy"
                            onChange={handleChange("child2", "dateOfBirth")}
                            isRequired={true}
                            isFutureDateAllowed={false}
                            date={
                              userDetails?.beneficiaries?.child2?.dateOfBirth
                            }
                            error={
                              userDetails?.beneficiaries?.errors[
                                `child2.dateOfBirth`
                              ]
                            }
                          />
                        </div>
                        {policyTypeId && (
                          <div className="grid gap-2">
                            <Label className="mt-3" htmlFor="child2-cert">
                              Upload Disability Certificate
                            </Label>
                            <FileUpload
                              id="child2-cert"
                              onChange={handleChange(
                                "child2",
                                "disabilityCertificate"
                              )}
                              value={
                                userDetails?.beneficiaries?.child2
                                  ?.disabilityCertificate || null
                              }
                              error={
                                userDetails?.beneficiaries?.errors[
                                  `child2.disabilityCertificate`
                                ]
                              }
                              accept=".pdf"
                            />
                          </div>
                        )}
                      </div>
                    </CardContent>
                  </Card>
                ) : null}

                {flags?.isParent1PremiumSelected ? (
                  <Card className="shadow-lg border-0">
                    <CardHeader>
                      <CardTitle>Parent 1</CardTitle>
                    </CardHeader>
                    <CardContent className="grid gap-4">
                      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        <div className="space-y-2">
                          <RInput
                            label="Name"
                            id="parent1-name"
                            onChange={handleChange("parent1", "name")}
                            value={
                              userDetails?.beneficiaries?.parent1?.name || ""
                            }
                            type="text"
                            isRequired={true}
                            error={
                              userDetails?.beneficiaries?.errors[`parent1.name`]
                            }
                          />
                        </div>

                        <div className="space-y-2 flex flex-col justify-end ">
                          <Label htmlFor="parent1-gender ">
                            <span>Gender</span>
                            <span className="text-red-600	ml-1">*</span>
                          </Label>
                          <Combobox
                            options={genders}
                            valueProperty="id"
                            labelProperty="name"
                            id="parent1-gender"
                            onChange={handleChange("parent1", "gender")}
                            value={userDetails?.beneficiaries?.parent1?.gender}
                            isRequired={true}
                            error={
                              userDetails?.beneficiaries?.errors[
                                `parent1.gender`
                              ]
                            }
                          />
                        </div>
                        <div className="space-y-2 flex items-end">
                          <DatePicker
                            label="Date of birth"
                            id="dob"
                            type="date"
                            placeholder="dd-mm-yyyy"
                            onChange={handleChange("parent1", "dateOfBirth")}
                            isRequired={true}
                            isFutureDateAllowed={false}
                            date={
                              userDetails?.beneficiaries?.parent1?.dateOfBirth
                            }
                            error={
                              userDetails?.beneficiaries?.errors[
                                `parent1.dateOfBirth`
                              ]
                            }
                          />
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                ) : null}

                {flags?.isParent2PremiumSelected ? (
                  <Card className="shadow-lg border-0">
                    <CardHeader>
                      <CardTitle>Parent 2</CardTitle>
                    </CardHeader>
                    <CardContent className="grid gap-4">
                      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        <div className="space-y-2">
                          <RInput
                            label="Name"
                            id="parent2-name"
                            onChange={handleChange("parent2", "name")}
                            value={
                              userDetails?.beneficiaries?.parent2?.name || ""
                            }
                            type="text"
                            isRequired={true}
                            error={
                              userDetails?.beneficiaries?.errors[`parent2.name`]
                            }
                          />
                        </div>

                        <div className="space-y-2 flex flex-col justify-end ">
                          <Label htmlFor="parent2-gender ">
                            <span>Gender</span>
                            <span className="text-red-600	ml-1">*</span>
                          </Label>
                          <Combobox
                            options={genders}
                            valueProperty="id"
                            labelProperty="name"
                            id="parent2-gender"
                            onChange={handleChange("parent2", "gender")}
                            value={userDetails?.beneficiaries?.parent2?.gender}
                            isRequired={true}
                            error={
                              userDetails?.beneficiaries?.errors[
                                `parent2.gender`
                              ]
                            }
                          />
                        </div>
                        <div className="space-y-2 flex items-end">
                          <DatePicker
                            label="Date of birth"
                            id="dob"
                            type="date"
                            placeholder="dd-mm-yyyy"
                            onChange={handleChange("parent2", "dateOfBirth")}
                            isRequired={true}
                            isFutureDateAllowed={false}
                            date={
                              userDetails?.beneficiaries?.parent2?.dateOfBirth
                            }
                            error={
                              userDetails?.beneficiaries?.errors[
                                `parent2.dateOfBirth`
                              ]
                            }
                          />
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                ) : null}

                {flags?.isInLaw1PremiumSelected ? (
                  <Card className="shadow-lg border-0">
                    <CardHeader>
                      <CardTitle>In Law 1</CardTitle>
                    </CardHeader>
                    <CardContent className="grid gap-4">
                      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        <div className="space-y-2">
                          <RInput
                            label="Name"
                            id="inlaw1-name"
                            onChange={handleChange("inLaw1", "name")}
                            value={
                              userDetails?.beneficiaries?.inLaw1?.name || ""
                            }
                            type="text"
                            isRequired={true}
                            error={
                              userDetails?.beneficiaries?.errors[`inLaw1.name`]
                            }
                          />
                        </div>

                        <div className="space-y-2 flex flex-col justify-end ">
                          <Label htmlFor="inlaw1-gender ">
                            <span>Gender</span>
                            <span className="text-red-600	ml-1">*</span>
                          </Label>
                          <Combobox
                            options={genders}
                            valueProperty="id"
                            labelProperty="name"
                            id="inlaw1-gender"
                            onChange={handleChange("inLaw1", "gender")}
                            value={userDetails?.beneficiaries?.inLaw1?.gender}
                            isRequired={true}
                            error={
                              userDetails?.beneficiaries?.errors[
                                `inLaw1.gender`
                              ]
                            }
                          />
                        </div>
                        <div className="space-y-2 flex items-end">
                          <DatePicker
                            label="Date of birth"
                            id="dob"
                            type="date"
                            placeholder="dd-mm-yyyy"
                            onChange={handleChange("inLaw1", "dateOfBirth")}
                            isRequired={true}
                            isFutureDateAllowed={false}
                            date={
                              userDetails?.beneficiaries?.inLaw1?.dateOfBirth
                            }
                            error={
                              userDetails?.beneficiaries?.errors[
                                `inLaw1.dateOfBirth`
                              ]
                            }
                          />
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                ) : null}

                {flags?.isInLaw2PremiumSelected ? (
                  <Card className="shadow-lg border-0">
                    <CardHeader>
                      <CardTitle>In Law 2</CardTitle>
                    </CardHeader>
                    <CardContent className="grid gap-4">
                      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        <div className="space-y-2">
                          <RInput
                            label="Name"
                            id="inlaw2-name"
                            onChange={handleChange("inLaw2", "name")}
                            value={
                              userDetails?.beneficiaries?.inLaw2?.name || ""
                            }
                            type="text"
                            isRequired={true}
                            error={
                              userDetails?.beneficiaries?.errors[`inLaw2.name`]
                            }
                          />
                        </div>

                        <div className="space-y-2 flex flex-col justify-end ">
                          <Label htmlFor="inlaw2-gender ">
                            <span>Gender</span>
                            <span className="text-red-600	ml-1">*</span>
                          </Label>
                          <Combobox
                            options={genders}
                            valueProperty="id"
                            labelProperty="name"
                            id="inlaw2-gender"
                            onChange={handleChange("inLaw2", "gender")}
                            value={userDetails?.beneficiaries?.inLaw2?.gender}
                            isRequired={true}
                            error={
                              userDetails?.beneficiaries?.errors[
                                `inLaw2.gender`
                              ]
                            }
                          />
                        </div>
                        <div className="space-y-2 flex items-end">
                          <DatePicker
                            label="Date of birth"
                            id="dob"
                            type="date"
                            placeholder="dd-mm-yyyy"
                            onChange={handleChange("inLaw2", "dateOfBirth")}
                            isRequired={true}
                            isFutureDateAllowed={false}
                            date={
                              userDetails?.beneficiaries?.inLaw2?.dateOfBirth
                            }
                            error={
                              userDetails?.beneficiaries?.errors[
                                `inLaw2.dateOfBirth`
                              ]
                            }
                          />
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                ) : null}
              </div>
            )}
            <div className="flex justify-end gap-2">
              {Object.keys(flags).length &&
              permissions?.[PAGE_NAME]?.read &&
              mode === "edit" ? (
                <RButton
                  className="mt-3 "
                  onClick={() => {
                    setActiveTab("nominees");
                    setStep(3);
                  }}
                  isLoading={isLoading}
                >
                  Next
                </RButton>
              ) : null}

              {permissions?.[PAGE_NAME_dependentdetails]?.update ? (
                <RButton
                  className="mt-3 "
                  onClick={
                    !Object.keys(flags).length
                      ? () => {
                          setActiveTab("nominees");
                          setStep(3);
                        }
                      : handleSubmitBeneficiaries
                  }
                  isLoading={isLoading}
                >
                  {!Object.keys(flags).length ? "Next" : " Save and Next"}
                </RButton>
              ) : null}
            </div>
          </TabsContent>
          <TabsContent value="nominees">
            <Card className="shadow-lg border-0">
              <CardHeader>
                <CardTitle>Nominee</CardTitle>
              </CardHeader>
              <CardContent className="grid gap-4">
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  <div className="space-y-2 flex flex-col justify-end">
                    <Label htmlFor="inlaw2-gender ">
                      <span>Relation</span>
                      <span className="text-red-600	ml-1">*</span>
                    </Label>
                    <Combobox
                      options={relations}
                      valueProperty="id"
                      labelProperty="name"
                      id="nominee-relation"
                      onChange={handleChange("nominee", "nomineeRelation")}
                      value={
                        userDetails?.beneficiaries?.nominee?.nomineeRelation
                      }
                      error={
                        userDetails?.beneficiaries?.errors[
                          `nominee.nomineeRelation`
                        ]
                      }
                    />
                  </div>

                  <div className="space-y-2">
                    <RInput
                      label="Name"
                      id="nominee-name"
                      onChange={handleChange("nominee", "name")}
                      value={userDetails?.beneficiaries?.nominee?.name || ""}
                      type="text"
                      isRequired={true}
                      error={userDetails?.beneficiaries?.errors[`nominee.name`]}
                    />
                  </div>
                  <div className="space-y-2 flex flex-col justify-end">
                    <Label htmlFor="inlaw2-gender ">
                      <span>Gender</span>
                      <span className="text-red-600	ml-1">*</span>
                    </Label>
                    <Combobox
                      options={genders}
                      valueProperty="id"
                      labelProperty="name"
                      id="nominee-gender"
                      onChange={handleChange("nominee", "gender")}
                      value={userDetails?.beneficiaries?.nominee?.gender}
                      error={
                        userDetails?.beneficiaries?.errors[`nominee.gender`]
                      }
                    />
                  </div>

                  <div className="space-y-2 flex items-end">
                    <DatePicker
                      label="Date of birth"
                      id="dob"
                      type="date"
                      placeholder="dd-mm-yyyy"
                      onChange={handleChange("nominee", "dateOfBirth")}
                      isRequired={true}
                      isFutureDateAllowed={false}
                      date={userDetails?.beneficiaries?.nominee?.dateOfBirth}
                      error={
                        userDetails?.beneficiaries?.errors[
                          `nominee.dateOfBirth`
                        ]
                      }
                    />
                  </div>
                </div>
              </CardContent>
            </Card>
            <div className="flex justify-end gap-2">
              {/* {mode === "edit" && permissions?.[PAGE_NAME]?.read && (userDetails?.beneficiaries?.nominee?id >0) ? (
                <RButton
                  className="mt-3 "
                  onClick={handleNextNominee}
                  isLoading={isLoading}
                >
                  Next
                </RButton>
              ) : null} */}
              {mode === "edit" &&
              permissions?.[PAGE_NAME]?.read &&
              userDetails?.beneficiaries?.nominee?.id > 0 ? (
                <RButton
                  className="mt-3"
                  onClick={handleNextNominee}
                  isLoading={isLoading}
                >
                  Next
                </RButton>
              ) : null}
              {permissions?.[PAGE_NAME_nomineedetails]?.update ? (
                <RButton
                  className="mt-3 "
                  onClick={handleSaveNominee}
                  isLoading={isLoading}
                >
                  Save and Pay
                </RButton>
              ) : null}
            </div>
          </TabsContent>
        </Tabs>
      </div>
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(BeneficiaryDetails))
);
