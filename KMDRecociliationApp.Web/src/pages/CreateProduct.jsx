import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import BasePolicyPremiumChart from "@/components/ui/basePolicyPremiumChart";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Combobox } from "@/components/ui/comboBox";
import FileUpload from "@/components/ui/fileUpload";
import { Label } from "@/components/ui/label";
import RButton from "@/components/ui/rButton";
import RInput from "@/components/ui/rInput";
import { Separator } from "@/components/ui/separator";
import TopUpPolicyPremiumChart from "@/components/ui/topUpPolicyPremiumChart";
import { toast } from "@/components/ui/use-toast";
import {
  appendFormData,
  formatTopUpPolicy,
  getIndexToAddTopupOption,
  getPolicyOption,
  getProduct,
  getProductPostObject,
  getTopUpPolicyOption,
  isTopUpPolicy,
  isAgeBandPremiumPolicy,
  isTopUpPolicyAdded,
  markBasePolicyOption,
  removePremium,
  removePremiumChartErrors,
} from "@/lib/helperFunctions";
import { PlusIcon } from "@/lib/icons";
import { usePermissionStore } from "@/lib/store";
import {
  getBasePolicyList,
  getPolicyTypes,
  getProductById,
  saveProduct,
} from "@/services/product";
import { getAgeBandValues } from "@/services/customerProfile";
import { productValidation } from "@/validations";
import { produce } from "immer";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ValidationError } from "yup";
import RTextArea from "@/components/ui/rTextArea";
const PAGE_NAME = "createproduct";

function CreateProduct() {
  const SCHEMA = productValidation();
  const navigate = useNavigate();

  const permissions = usePermissionStore((state) => state.permissions);

  const [product, setProduct] = useState(getProduct());
  const [errors, setErrors] = useState({});
  const [policyTypes, setPolicyTypes] = useState([]);
  const [ageBandPremiumRateOptions, setAgeBandPremiumRateOptions] = useState(
    []
  );
  const [basePolicies, setBasePolicies] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  let params = new URLSearchParams(window.location.search);
  let pageMode = params.get("mode");
  let productId = params.get("productId");

  useEffect(() => {
    (async () => {
      //get policyTypes
      let policyTypeResponse = await getPolicyTypes();
      if (policyTypeResponse.status === "success") {
        setPolicyTypes(policyTypeResponse.data);
      }
      let ageBandResponse = await getAgeBandValues();
      if (ageBandResponse.status === "success") {
        setAgeBandPremiumRateOptions(ageBandResponse.data);
      }

      if (pageMode === "edit") {
        let productResponse = await getProductById(productId);
        if (productResponse.status === "success") {
          if (isTopUpPolicy(productResponse.data.policyType)) {
            let product = formatTopUpPolicy(productResponse.data);
            setProduct(product);

            //get base policies
            let basePolicyResponse = await getBasePolicyList();
            if (basePolicyResponse.status === "success") {
              setBasePolicies(basePolicyResponse.data);
            } else {
              //show error
              toast({
                variant: "destructive",
                title: "Something went wrong.",
                description: "Unable to get base policies.",
              });
            }
          } else {
            setProduct(productResponse.data);
          }
        } else {
          //show page error and toast error message
          toast({
            variant: "destructive",
            title: "Something went wrong.",
            description: "Unable to get product.",
          });
        }
      }
    })();
  }, []);

  const handleAddPolicyOption = (event) => {
    event.preventDefault();
    let nextState = produce(product, (draft) => {
      draft["premiumChart"].push(getPolicyOption());
    });

    //if option at the index is added, remove the premium chart errors from errors oject
    let nextErrors = removePremiumChartErrors(errors);
    setErrors(nextErrors);

    setProduct(nextState);
  };

  const handleRemovePolicyOption = (index) => (event) => {
    event.preventDefault();
    let nextState = produce(product, (draft) => {
      draft["premiumChart"].splice(index, 1);
    });

    //if option at the index is deleted, remove the premium chart errors from errors oject
    let nextErrors = removePremiumChartErrors(errors);
    setErrors(nextErrors);

    setProduct(nextState);
  };

  const handleChange = (name, index) => async (event) => {
    console.log(event);
    let nextErrors = { ...errors };
    let nextState = produce(product, (draft) => {
      switch (name) {
        case "productName":
          draft[name] = event.target.value;
          break;
        case "disclaimer":
          draft[name] = event.target.value;
          break;
        case "providerName":
          draft[name] = event.target.value;
          break;

        case "policyType":
          draft[name] = event;

          // reset product values when product type is changed
          draft["premiumChart"] = [];
          draft["isSpouseCoverage"] = false;
          draft["isHandicappedChildrenCoverage"] = false;
          draft["isParentsCoverage"] = false;
          draft["isInLawsCoverage"] = false;
          draft["numberOfHandicappedChildren"] = 0;
          draft["numberOfParents"] = 0;
          draft["numberOfInLaws"] = 0;
          draft["numberOfChildren"] = 0;

          if (event && event?.name.toLowerCase().trim() === "basepolicy") {
            draft["basePolicy"] = null;
          }
          break;

        case "basePolicy":
          draft[name] = event;
          break;

        //coverage
        case "isSpouseCoverage":
          draft[name] = event;

          if (!event) {
            draft["premiumChart"] = removePremium(
              draft["policyType"]["name"],
              draft["premiumChart"],
              "spousePremium"
            );
          }
          break;

        case "isHandicappedChildrenCoverage":
          draft[name] = event;

          if (!event) {
            draft["premiumChart"] = removePremium(
              draft["policyType"]["name"],
              draft["premiumChart"],
              "child1Premium",
              "child2Premium"
            );
            draft["numberOfHandicappedChildren"] = 0;
          } else {
            draft["numberOfHandicappedChildren"] = 1;
          }

          break;

        case "isParentsCoverage":
          draft[name] = event;

          if (!event) {
            draft["premiumChart"] = removePremium(
              draft["policyType"]["name"],
              draft["premiumChart"],
              "parent1Premium",
              "parent2Premium"
            );
            draft["numberOfParents"] = 0;
          } else {
            draft["numberOfParents"] = 1;
          }
          break;

        case "isInLawsCoverage":
          draft[name] = event;

          if (!event) {
            draft["premiumChart"] = removePremium(
              draft["policyType"]["name"],
              draft["premiumChart"],
              "inLaw1Premium",
              "inLaw2Premium"
            );
            draft["numberOfInLaws"] = 0;
          } else {
            draft["numberOfInLaws"] = 1;
          }
          break;

        //-----------

        case "numberOfHandicappedChildren":
        case "numberOfParents":
        case "numberOfInLaws":
          draft[name] = Number(event.target.value);
          break;

        case "productDocument":
          draft[name] = event;
          draft["isProductDocumentUpdated"] = true;
          break;

        case "sumInsured":
        case "selfOnlyPremium":
        case "selfSpousePremium":
        case "spousePremium":
        case "child1Premium":
        case "child2Premium":
        case "parent1Premium":
        case "parent2Premium":
        case "inLaw1Premium":
        case "inLaw2Premium":
        case "selfSpouse1ChildrenPremium":
        case "selfSpouse2ChildrenPremium":
        case "self1ChildrenPremium":
        case "self2ChildrenPremium":
          draft["premiumChart"][index][name] = event.target.value;
          break;
        case "ageBandPremiumRateValue":
          draft["premiumChart"][index][name] = event;
          break;
        default:
          break;
      }
      try {
        const context = {
          isSpouseCoverage: draft.isSpouseCoverage,
          numberOfChildren: draft.numberOfChildren,
          isHandicappedChildrenCoverage: draft.isHandicappedChildrenCoverage,
          numberOfHandicappedChildren: draft.numberOfHandicappedChildren,
          isParentsCoverage: draft.isParentsCoverage,
          numberOfParents: draft.numberOfParents,
          isInLawsCoverage: draft.isInLawsCoverage,
          numberOfInLaws: draft.numberOfInLaws,
          policyType: draft.policyType?.name || "",
        };
        var path = name;
        if (index >= 0) {
          path = `premiumChart[${index}].${name}`;
        }
        SCHEMA.validateSyncAt(path, draft, { context: context });
        nextErrors[path] = [];
      } catch (e) {
        nextErrors[path] = [...e.errors];
      }

      //clear errors when  policy type is changed
      if (name === "policyType") {
        nextErrors = {};
      }
    });

    if (name === "basePolicy" && event) {
      //get the base Policy and set premium chart

      let baseProduct = {};
      let baseProductResponse = await getProductById(event.id);

      if (baseProductResponse.status === "success") {
        baseProduct = baseProductResponse.data;
      }

      let basePremiumChart = [...baseProduct["premiumChart"]];

      nextState = produce(nextState, (draft) => {
        draft["premiumChart"] = basePremiumChart;
        draft["isSpouseCoverage"] = baseProduct["isSpouseCoverage"];
        draft["isHandicappedChildrenCoverage"] =
          baseProduct["isHandicappedChildrenCoverage"];
        draft["isParentsCoverage"] = baseProduct["isParentsCoverage"];
        draft["isInLawsCoverage"] = baseProduct["isInLawsCoverage"];
        draft["numberOfHandicappedChildren"] =
          baseProduct["numberOfHandicappedChildren"];
        draft["numberOfParents"] = baseProduct["numberOfParents"];
        draft["numberOfInLaws"] = baseProduct["numberOfInLaws"];
        draft["numberOfChildren"] = baseProduct["numberOfChildren"];

        draft["premiumChart"] = markBasePolicyOption(draft["premiumChart"]);
      });
    }

    if (name === "policyType" && event) {
      if (event.name.toLowerCase().trim() === "topuppolicy") {
        //get base policies

        let basePolicyResponse = await getBasePolicyList();
        if (basePolicyResponse.status === "success") {
          setBasePolicies(basePolicyResponse.data);
        } else {
          //show error
        }
      }
    }

    setErrors(nextErrors);
    setProduct(nextState);
  };

  const handleAddTopup = (index) => (event) => {
    event.preventDefault();
    let newTopUpOption = getTopUpPolicyOption();

    let indexForAddingTopupPolicyOption = getIndexToAddTopupOption(
      product["premiumChart"],
      index
    );

    let nextState = produce(product, (draft) => {
      draft["premiumChart"].splice(
        indexForAddingTopupPolicyOption + 1,
        0,
        newTopUpOption
      );
    });

    //if option at the index is added, remove the premium chart errors from errors oject
    let nextErrors = removePremiumChartErrors(errors);
    setErrors(nextErrors);

    setProduct(nextState);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    setIsLoading(true);
    try {
      // if (isTopUpPolicy(product?.policyType)) {
      //let isTopPolicyExists = isTopUpPolicyAdded(product.premiumChart);
      // if (!isTopPolicyExists) {
      //   toast({
      //     variant: "destructive",
      //     title: "No Top up added.",
      //     description: "Please add atleast one top up option.",
      //   });
      //   setIsLoading(false);

      //   return;
      // }
      // }

      const context = {
        isSpouseCoverage: product.isSpouseCoverage,
        numberOfChildren: product.numberOfChildren,
        isHandicappedChildrenCoverage: product.isHandicappedChildrenCoverage,
        numberOfHandicappedChildren: product.numberOfHandicappedChildren,
        isParentsCoverage: product.isParentsCoverage,
        numberOfParents: product.numberOfParents,
        isInLawsCoverage: product.isInLawsCoverage,
        numberOfInLaws: product.numberOfInLaws,
        policyType: product.policyType?.name || "",
      };

      SCHEMA.validateSync(product, { abortEarly: false, context });

      let postObject = getProductPostObject(product);

      const formData = new FormData();

      appendFormData(formData, postObject, "");

      //save product
      let productResponse = await saveProduct(postObject?.productId, formData);
      if (productResponse.status === "success") {
        navigate("/createproductlist");
      } else {
        // 400
        if (productResponse.status === "conflict") {
          //409
          toast({
            variant: "destructive",
            title: "Something went wrong.",
            description: productResponse.errors.message,
          });
        } else {
          // 500
          toast({
            variant: "destructive",
            title: "Something went wrong.",
            description: "Unable to save product.",
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

  return (
    <div>
      <h1 className="mb-8 text-2xl font-bold">
        {pageMode === "new" ? "Create" : "Edit"} Product
      </h1>
      <form className="space-y-6">
        <div className="grid grid-cols-2 gap-6">
          <div>
            <RInput
              placeholder="Enter Product Name"
              label="Product Name"
              type="text"
              onChange={handleChange("productName")}
              name="productName"
              id="product-name"
              value={product.productName}
              error={errors?.productName}
              isRequired={true}
            />
          </div>

          <div>
            <RInput
              placeholder="Enter Insurer Name"
              label="Insurer Name"
              type="text"
              onChange={handleChange("providerName")}
              name="providerName"
              id="provider-name"
              value={product.providerName}
            />
          </div>
        </div>

        <div className="grid grid-cols-2 gap-6">
          <div className="flex flex-col justify-between">
            <Label htmlFor="policyType">
              <span>Policy Type</span>
              <span className="text-red-600	ml-1">*</span>
            </Label>
            <Combobox
              options={policyTypes}
              valueProperty="id"
              labelProperty="name"
              id="policyType"
              onChange={handleChange("policyType")}
              value={product.policyType}
              error={errors?.policyType}
              isDisabled={pageMode == "edit"}
            />
          </div>
          <div className="flex flex-col justify-between space-y-2">
            <Label htmlFor="basePolicyLink">
              <span>Link to Base Policy</span>
              {isTopUpPolicy(product?.policyType) ? (
                <span className="text-red-600	ml-1">*</span>
              ) : null}
            </Label>
            <Combobox
              options={basePolicies}
              valueProperty="id"
              labelProperty="name"
              id="basePolicyLink"
              onChange={handleChange("basePolicy")}
              value={product.basePolicy}
              error={errors?.basePolicy}
              isDisabled={
                product?.policyType === null ||
                product.policyType?.name?.toLowerCase().trim() ===
                  "basepolicy" ||
                pageMode == "edit"
              }
            />
          </div>
        </div>
        <div className="grid grid-cols-2 gap-6">
          <div className="flex flex-col justify-between">
            <RTextArea
              id="disclaimer"
              name="disclaimer"
              label="Disclaimer"
              placeholder="Enter Disclaimer"
              value={product.disclaimer || ""}
              onChange={handleChange("disclaimer")}
              error={errors?.disclaimer}
              isRequired={false}
            />
          </div>
          <div className="flex flex-col justify-between space-y-2">
            <Label htmlFor="productDocument" className="whitespace-nowrap">
              Product Document
            </Label>

            <FileUpload
              id="productDocument"
              onChange={handleChange("productDocument")}
              value={product?.productDocument || null}
              accept=".pdf"
            />
          </div>
        </div>

        <div className="grid grid-cols-3 gap-6">
          <div className="hidden">
            <Checkbox
              id="spouseCoverage"
              checked={product.isSpouseCoverage}
              onCheckedChange={handleChange("isSpouseCoverage")}
            />
            <Label htmlFor="spouseCoverage" className="ml-2">
              Spouse Coverage
            </Label>
          </div>
          {isTopUpPolicy(product?.policyType) ? null : (
            <div>
              <Checkbox
                id="handicappedChildren"
                checked={product.isHandicappedChildrenCoverage}
                onCheckedChange={handleChange("isHandicappedChildrenCoverage")}
              />
              <Label htmlFor="handicappedChildren" className="ml-2">
                Handicapped Children Coverage
              </Label>
              {product.isHandicappedChildrenCoverage && (
                <div className="mt-2">
                  <RInput
                    min="1"
                    max="2"
                    value={product.numberOfHandicappedChildren}
                    onChange={handleChange("numberOfHandicappedChildren")}
                  />
                  <span className="ml-2">Handicapped Children</span>
                </div>
              )}
            </div>
          )}
          {isTopUpPolicy(product?.policyType) ? null : (
            <div className="hidden">
              <Checkbox
                id="parents"
                checked={product.isParentsCoverage}
                onCheckedChange={handleChange("isParentsCoverage")}
              />
              <Label htmlFor="parents" className="ml-2">
                Parents Coverage
              </Label>
              {product.isParentsCoverage && (
                <div className="mt-2">
                  <RInput
                    min="1"
                    max="2"
                    value={product.numberOfParents}
                    onChange={handleChange("numberOfParents")}
                  />
                  <span className="ml-2"> Parents</span>
                </div>
              )}
            </div>
          )}
          {isTopUpPolicy(product?.policyType) ? null : (
            <div className="hidden">
              <Checkbox
                id="inLaws"
                checked={product.isInLawsCoverage}
                onCheckedChange={handleChange("isInLawsCoverage")}
              />
              <Label htmlFor="inLaws" className="ml-2">
                In-Laws Coverage
              </Label>
              {product.isInLawsCoverage && (
                <div className="mt-2">
                  <RInput
                    min="1"
                    max="2"
                    value={product.numberOfInLaws}
                    onChange={handleChange("numberOfInLaws")}
                  />
                  <span className="ml-2"> In-Laws</span>
                </div>
              )}
            </div>
          )}
        </div>

        <Separator className="my-4" />
        <div className="flex justify-between items-start">
          <Label className=" text-xl">Create Premium Chart</Label>
          {isTopUpPolicy(product?.policyType) ? null : (
            <Button onClick={handleAddPolicyOption}>
              <PlusIcon className="h-4 w-4" />
            </Button>
          )}
        </div>
        <div>
          <div className="space-y-4">
            {isTopUpPolicy(product?.policyType) ? (
              <TopUpPolicyPremiumChart
                product={product}
                onChange={handleChange}
                errors={errors}
                onTopupAdd={handleAddTopup}
                onRemovePolicyOption={handleRemovePolicyOption}
              />
            ) : (
              <BasePolicyPremiumChart
                product={product}
                policyType={product?.policyType}
                ageBandPremiumRate={ageBandPremiumRateOptions}
                // ageBandPremiumRateValue={
                //   product?.premiumChart?.ageBandPremiumRateValue
                // }
                onChange={handleChange}
                errors={errors}
                onRemovePolicyOption={handleRemovePolicyOption}
              />
            )}
          </div>
        </div>
        {product.premiumChart.length && permissions?.[PAGE_NAME]?.create ? (
          <div className="flex justify-end">
            <RButton
              className="mt-10"
              onClick={handleSubmit}
              type="submit"
              isLoading={isLoading}
            >
              Save Product
            </RButton>
          </div>
        ) : null}
      </form>
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(CreateProduct))
);
