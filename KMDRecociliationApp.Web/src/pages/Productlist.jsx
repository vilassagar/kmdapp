import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithProfile from "@/components/hoc/withProfile";
import WithLayout from "@/components/layout/WithLayout";
import RSelect from "@/components/ui/RSelect";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";
import NoProductData from "@/components/ui/noProductData";
import RButton from "@/components/ui/rButton";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { toast } from "@/components/ui/use-toast";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import {
  appendFormData,
  getBeneficiaryInitialObject,
  getPaymentsInitialObject,
  getPropertyName,
  getSelectedProducts,
  getTotalPremium,
  isProductSelected,
  resetSelectedPremiumOptions,
  setSumInsuredOptions,
} from "@/lib/helperFunctions";
import { usePermissionStore, usePolicyStore, userStore } from "@/lib/store";
import { cn } from "@/lib/utils";
import {
  addProductPolicy,
  checkPaymentByUsers,
  verifyPolicyPaymentEntry,
  getMyPolicies,
  getPolicyDetails,
  getProductList,
  getUser,
  validatepolicypurchase,
} from "@/services/customerProfile";
import { produce } from "immer";
import { CircleArrowLeft } from "lucide-react";
import { Fragment, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { EyeIcon } from "@/lib/icons";
import { Button } from "@/components/ui/button";
import { getProductById } from "@/services/product";
import { Loader2 } from "lucide-react";
import ProductDocumentDownload from "@/components/ui/ProductDocumentDownload";
import PremiumChartDialog from "@/components/ui/product/PremiumChartDialog";
import ProductSummary from "@/components/ui/product/ProductSummary";
import { ErrorDisplay } from "@/components/ui/common/ErrorDisplay";
import { LoadingSpinner } from "@/components/ui/common/LoadingSpinner";
import SearchComponent from "@/components/ui/SearchComponent";
import { getCampaignsList } from "@/services/campaigns";
import DisclaimerComponent from "@/components/ui/DisclaimerComponent";
const PAGE_NAME = "products";
const PAGE_NAME_policyproduct = "policyproduct";
function Productlist() {
  const search = window.location.search;
  const params = new URLSearchParams(search);
  const userId = params.get("userId");
  const campaignId = params.get("campaignId");

  const navigate = useNavigate();
  const loggedInUser = userStore((state) => state.user);

  const user = userStore((state) => state.user);
  const permissions = usePermissionStore((state) => state.permissions);
  const userDetails = usePolicyStore((state) => state.userDetails);
  const setUserDetails = usePolicyStore((state) => state.setUserDetails);
  const updatePolicyId = usePolicyStore((state) => state.updatePolicyId);
  const step = usePolicyStore((state) => state.step);
  const setStep = usePolicyStore((state) => state.setStep);
  const mode = usePolicyStore((state) => state.mode);

  const [paymentStatus, setPaymentStatus] = useState(null);
  const [PolicyPaymentEntry, setPolicyPaymentEntry] = useState(null);
  const [doesUserHavePolicies, setDoesUserHavePolicies] = useState(true);

  const [isLoading, setIsLoading] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [showDialog, setShowDialog] = useState(false);
  const [isCampaignExpired, setIsCampaignExpired] = useState(false);
  const [campaignIdused, setcampaignIdused] = useState(false);

  const [products, setProducts] = useState([]);
  const [isLoadingDependencies, setIsLoadingDependencies] = useState(true);
  const [isLoadingPolicies, setIsLoadingPolicies] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const loadDependencies = async () => {
      setIsLoadingDependencies(true);
      setStep(1);
      try {
        //get user
        let currentUser = {};
        let currUser = userId > 0 ? userId : user.userId;

        let response = await getUser(currUser);
        if (response.status === "success") {
          currentUser = response.data;
          // await getPaginatedPolicies(paginationModel, searchTerm);
        }

        if (mode === "edit") {
          let response = await getPolicyDetails(
            userDetails?.policyId || 0,
            campaignId || 0
          );
          if (response.status === "success") {
            let userDetails = response.data;

            let newProducts = setSumInsuredOptions(
              response.data?.products,
              mode
            );
            setIsCampaignExpired(newProducts[0]?.isCampaignExpired);
            let nextState = produce(userDetails, (draft) => {
              draft["user"] = currentUser;
              draft["products"] = newProducts;
              draft["beneficiaries"]["nominee"] =
                draft["nominee"] === null
                  ? { name: "", gender: null, dateOfBirth: "" }
                  : draft["nominee"];
              delete draft["nominee"];
              draft["beneficiaries"]["errors"] = {};
              draft["paymentDetails"] = getPaymentsInitialObject();
            });
            setUserDetails(nextState);
          } else {
            toast({
              variant: "destructive",
              title: "Error!",
              description: "Unable to get products.",
            });
          }
        }

        if (mode === "new") {
          // get products by user ID
          let productsResponse = await getProductList(
            userId > 0 ? userId : user?.userId
          );

          if (productsResponse.status === "success") {
            //set values inside user details object
            setIsCampaignExpired(
              productsResponse.data?.products[0]?.isCampaignExpired
            );
            setcampaignIdused(productsResponse.data?.products[0]?.campaignId);

            let nextState = produce(userDetails, (draft) => {
              let newProducts = setSumInsuredOptions(
                productsResponse.data?.products,
                mode
              );
              draft["products"] = newProducts;
              draft["user"] = currentUser;
              draft["userId"] = currentUser?.userId;
              draft["totalPremium"] = 0;
              draft["totalPaidPremium"] = 0;
              draft["amountPaid"] = 0;
              draft["beneficiaries"] = getBeneficiaryInitialObject();
              draft["paymentDetails"] = getPaymentsInitialObject();
            });
            setUserDetails(nextState);
            setStep(1);
          } else {
            toast({
              variant: "destructive",
              title: "Something went wrong.",
              description:
                "Unable to get products.: " +
                productsResponse?.validationErrors,
            });
            setError(productsResponse?.validationErrors);
          }
        }
      } catch (err) {
        setError(err.message);
        toast({
          variant: "destructive",
          title: "Error loading data",
          description: err.message,
        });
      } finally {
        setIsLoadingDependencies(false);
      }
    };

    loadDependencies();
  }, []);

  useEffect(() => {
    const getPaymentStatus = async () => {
      const response = await checkPaymentByUsers(
        userId > 0 ? userId : user?.userId
      );
      if (response.status === "success") {
        setPaymentStatus(response.data);
      }
    };
    const getPolicyPaymentEntry = async () => {
      const response = await verifyPolicyPaymentEntry(
        userId > 0 ? userId : user.userId
      );
      if (response.status === "success") {
        setPolicyPaymentEntry(response.data);
      }
    };
    getPaymentStatus();
    getPolicyPaymentEntry();
  }, []);

  // Safe helper to get nested object values
  const getNestedValue = (obj, path, defaultValue = null) => {
    try {
      return (
        path.split(".").reduce((curr, key) => curr?.[key], obj) ?? defaultValue
      );
    } catch (e) {
      return defaultValue;
    }
  };

  // Safe helper to check if a path exists
  const pathExists = (obj, path) => {
    return getNestedValue(obj, path) !== null;
  };

  const handleChange = (name, productIndex) => (event) => {
    let nextState = produce(userDetails, (draft) => {
      if (!draft.products?.[productIndex]) {
        console.warn(`Product at index ${productIndex} is undefined`);
        return;
      }

      const product = draft.products[productIndex];
      const selectedPremiumIndex = product.selectedSumInsured?.index ?? -1;
      const selectedTopUpIndex = product.selectedTopUpOption?.index ?? -1;

      if (!product.premiumChart) {
        product.premiumChart = [];
      }

      switch (name) {
        case "isProductSelected":
          product[name] = event;

          //Set to disclaimer
          draft["products"][productIndex]["isDisclaimerAccepted"] = event;

          // Only proceed if we have a valid selection
          if (product.selectedSumInsured?.index !== undefined) {
            const premiumChart =
              product.premiumChart[product.selectedSumInsured.index];

            // Reset all premium selections first
            resetPremiumSelections(premiumChart);

            if (event) {
              //
              if (
                product?.productName === "Out Patient Department Policy" ||
                product?.productName === "OUT-PATIENT CARE INSURANCE POLICY"
              ) {
                if (
                  premiumChart.selfSpousePremium &&
                  premiumChart.selfSpousePremium !== 0
                ) {
                  premiumChart.isSelfSpousePremiumSelected = true;
                  product.totalProductPremium = Number(
                    premiumChart.selfSpousePremium
                  );
                }
              } else {
                // If product is being selected
                // Check if selfOnlyPremium exists and is non-zero
                if (
                  premiumChart.selfOnlyPremium &&
                  premiumChart.selfOnlyPremium !== 0
                ) {
                  premiumChart.isSelfPremiumSelected = true;
                  product.totalProductPremium = Number(
                    premiumChart.selfOnlyPremium
                  );
                }
                // If selfOnlyPremium is not available or is zero, check selfSpousePremium
                else if (
                  premiumChart.selfSpousePremium &&
                  premiumChart.selfSpousePremium !== 0
                ) {
                  premiumChart.isSelfSpousePremiumSelected = true;
                  product.totalProductPremium = Number(
                    premiumChart.selfSpousePremium
                  );
                }
              }
            } else {
              // Reset total premium when product is deselected
              product.totalProductPremium = 0;
            }
          }
          break;

        case "sumInsured":
          // eslint-disable-next-line no-case-declarations
          const previousIndex = product.selectedSumInsured?.index;

          // Reset previous selections if they exist
          if (previousIndex >= 0 && product.premiumChart[previousIndex]) {
            const premiumOptions = [
              "isSelfPremiumSelected",
              "isSelfSpousePremiumSelected",
              "isSelfSpouse2ChildrenPremiumSelected",
              "isSelfSpouse1ChildrenPremiumSelected",
              "isSelf2ChildrenPremiumSelected",
              "isSelf1ChildrenPremiumSelected",
              "isSpousePremiumSelected",
              "isChild1PremiumSelected",
              "isChild2PremiumSelected",
              "isParent1PremiumSelected",
              "isParent2PremiumSelected",
              "isInLaw1PremiumSelected",
              "isInLaw2PremiumSelected",
              "isTopUpSelected",
            ];

            premiumOptions.forEach((option) => {
              product.premiumChart[previousIndex][option] = false;
            });

            // Reset top up options if they exist
            if (
              product.premiumChart[previousIndex].topUpOptions?.length > 0 &&
              product.premiumChart[previousIndex].topUpOptions[
                selectedTopUpIndex
              ]
            ) {
              const topUpOptions = [
                "isTopUpSelfPremiumSelected",
                "isTopUpSelfSpousePremiumSelected",
                "isTopUpSpousePremiumSelected",
              ];

              topUpOptions.forEach((option) => {
                product.premiumChart[previousIndex].topUpOptions[
                  selectedTopUpIndex
                ][option] = false;
              });
            }

            product.totalProductPremium = 0;
          }

          // Set new sum insured and update related values
          product.selectedSumInsured = event;

          if (
            event?.index !== undefined &&
            product.premiumChart[event.index] &&
            product.premiumChart[event.index].selfOnlyPremium > 0
          ) {
            product.premiumChart[event.index].isSelfPremiumSelected = true;

            const selfOnlyPremium = Number(
              product.premiumChart[event.index].selfOnlyPremium || 0
            );
            product.totalProductPremium =
              Number(product.totalProductPremium || 0) + selfOnlyPremium;

            // Update top up option based on availability
            product.selectedTopUpOption = !product.premiumChart[event.index]
              ?.topUpOptions?.length
              ? null
              : product.premiumChart[event.index].topUpSumInsuredOptions?.[0] ||
                null;
          } else {
            if (
              event?.index !== undefined &&
              product.premiumChart[event.index]
            ) {
              product.premiumChart[
                event.index
              ].isSelfSpousePremiumSelected = true;

              const selfSpousePremium = Number(
                product.premiumChart[event.index].selfSpousePremium || 0
              );
              product.totalProductPremium =
                Number(product.totalProductPremium || 0) + selfSpousePremium;

              // Update top up option based on availability
              product.selectedTopUpOption = !product.premiumChart[event.index]
                ?.topUpOptions?.length
                ? null
                : product.premiumChart[event.index]
                    .topUpSumInsuredOptions?.[0] || null;
            }
          }
          break;

        case "isSelfPremiumSelected":
          draft["products"][productIndex]["premiumChart"][
            draft["products"][productIndex]["selectedSumInsured"]["index"]
          ][name] = event;

          [
            "isSelfSpousePremiumSelected",
            "isSelfSpouse2ChildrenPremiumSelected",
            "isSelfSpouse1ChildrenPremiumSelected",
            "isSelf2ChildrenPremiumSelected",
            "isSelf1ChildrenPremiumSelected",
          ].forEach((option) => {
            draft["products"][productIndex]["premiumChart"][
              draft["products"][productIndex]["selectedSumInsured"]["index"]
            ][option] = false;
          });

          //calculate the totalProduct premium as per selection
          if (event) {
            // if self premium is selected then , uncheck the self+spouse selection and adjust the total product premium
            draft["products"][productIndex]["premiumChart"][
              draft["products"][productIndex]["selectedSumInsured"]["index"]
            ]["isSelfSpousePremiumSelected"] = false;

            draft["products"][productIndex]["totalProductPremium"] = 0;

            //add the self premium to total product premium
            draft["products"][productIndex]["totalProductPremium"] = Number(
              draft["products"][productIndex]["premiumChart"][
                draft["products"][productIndex]["selectedSumInsured"]["index"]
              ]["selfOnlyPremium"]
            );

            if (
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["isTopUpSelected"]
            ) {
              //set top up self selected flag as true
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["topUpOptions"][selectedTopUpIndex][
                "isTopUpSelfPremiumSelected"
              ] = true;

              //set top up self + spouse flag as false
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["topUpOptions"][selectedTopUpIndex][
                "isTopUpSelfSpousePremiumSelected"
              ] = false;

              // subtract self + spouse top up premium from total product premium
              draft["products"][productIndex]["totalProductPremium"] =
                Number(draft["products"][productIndex]["totalProductPremium"]) -
                Number(
                  draft["products"][productIndex]["premiumChart"][
                    selectedPremiumIndex
                  ]["topUpOptions"][selectedTopUpIndex]["selfSpousePremium"]
                );

              //add self top up premium to total product premium
              draft["products"][productIndex]["totalProductPremium"] =
                Number(draft["products"][productIndex]["totalProductPremium"]) +
                Number(
                  draft["products"][productIndex]["premiumChart"][
                    selectedPremiumIndex
                  ]["topUpOptions"][selectedTopUpIndex]["selfOnlyPremium"]
                );
            }
          } else {
            // if self only premium is unchecked then , select the self + spouse

            draft["products"][productIndex]["totalProductPremium"] =
              Number(draft["products"][productIndex]["totalProductPremium"]) -
              Number(
                draft["products"][productIndex]["premiumChart"][
                  draft["products"][productIndex]["selectedSumInsured"]["index"]
                ]["selfOnlyPremium"]
              );

            draft["products"][productIndex]["premiumChart"][
              draft["products"][productIndex]["selectedSumInsured"]["index"]
            ]["isSelfSpousePremiumSelected"] = true;

            draft["products"][productIndex]["totalProductPremium"] =
              Number(draft["products"][productIndex]["totalProductPremium"]) +
              Number(
                draft["products"][productIndex]["premiumChart"][
                  draft["products"][productIndex]["selectedSumInsured"]["index"]
                ]["selfSpousePremium"]
              );

            if (
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["isTopUpSelected"]
            ) {
              //set top up self + spouse selected flag as true
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["topUpOptions"][selectedTopUpIndex][
                "isTopUpSelfSpousePremiumSelected"
              ] = true;

              //set top up self  flag as false
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["topUpOptions"][selectedTopUpIndex][
                "isTopUpSelfPremiumSelected"
              ] = false;

              // subtract self  top up premium from total product premium
              draft["products"][productIndex]["totalProductPremium"] =
                Number(draft["products"][productIndex]["totalProductPremium"]) -
                Number(
                  draft["products"][productIndex]["premiumChart"][
                    selectedPremiumIndex
                  ]["topUpOptions"][selectedTopUpIndex]["selfOnlyPremium"]
                );

              //add self + spouse top up premium to total product premium
              draft["products"][productIndex]["totalProductPremium"] =
                Number(draft["products"][productIndex]["totalProductPremium"]) +
                Number(
                  draft["products"][productIndex]["premiumChart"][
                    selectedPremiumIndex
                  ]["topUpOptions"][selectedTopUpIndex]["selfSpousePremium"]
                );
            }
          }
          break;

        case "isSelfSpousePremiumSelected":
          //update checkox value
          draft["products"][productIndex]["premiumChart"][
            draft["products"][productIndex]["selectedSumInsured"]["index"]
          ][name] = event;

          [
            "isSelfPremiumSelected",
            "isSelfSpouse2ChildrenPremiumSelected",
            "isSelfSpouse1ChildrenPremiumSelected",
            "isSelf2ChildrenPremiumSelected",
            "isSelf1ChildrenPremiumSelected",
          ].forEach((option) => {
            draft["products"][productIndex]["premiumChart"][
              draft["products"][productIndex]["selectedSumInsured"]["index"]
            ][option] = false;
          });

          //calculate the totalProduct premium as per selection
          if (event) {
            // if self premium is selected then , uncheck the self+spouse selection and adjust the total product premium
            draft["products"][productIndex]["premiumChart"][
              draft["products"][productIndex]["selectedSumInsured"]["index"]
            ]["isSelfPremiumSelected"] = false;

            //subtract the self  premium from total product premium
            draft["products"][productIndex]["totalProductPremium"] = 0;

            //add the self premium to total product premium
            draft["products"][productIndex]["totalProductPremium"] =
              //   Number(draft["products"][productIndex]["totalProductPremium"]) +
              Number(
                draft["products"][productIndex]["premiumChart"][
                  draft["products"][productIndex]["selectedSumInsured"]["index"]
                ]["selfSpousePremium"]
              );

            // changing top up selected options
            // if top up option is already selected and base policy option changes then change the top up selected option also
            // set top up selected value
            if (
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["isTopUpSelected"]
            ) {
              //set top up self +  spouse selected flag as true
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["topUpOptions"][selectedTopUpIndex][
                "isTopUpSelfSpousePremiumSelected"
              ] = true;

              //set top up self flag as false
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["topUpOptions"][selectedTopUpIndex][
                "isTopUpSelfPremiumSelected"
              ] = false;

              // subtract self top up premium from total product premium
              draft["products"][productIndex]["totalProductPremium"] =
                Number(draft["products"][productIndex]["totalProductPremium"]) -
                Number(
                  draft["products"][productIndex]["premiumChart"][
                    selectedPremiumIndex
                  ]["topUpOptions"][selectedTopUpIndex]["selfOnlyPremium"]
                );

              //add self + spouse top up premium to total product premium
              draft["products"][productIndex]["totalProductPremium"] =
                Number(draft["products"][productIndex]["totalProductPremium"]) +
                Number(
                  draft["products"][productIndex]["premiumChart"][
                    selectedPremiumIndex
                  ]["topUpOptions"][selectedTopUpIndex]["selfSpousePremium"]
                );
            }
          } else {
            // if self only premium is unchecked then , select the self + spouse

            draft["products"][productIndex]["totalProductPremium"] =
              Number(draft["products"][productIndex]["totalProductPremium"]) -
              Number(
                draft["products"][productIndex]["premiumChart"][
                  draft["products"][productIndex]["selectedSumInsured"]["index"]
                ]["selfSpousePremium"]
              );

            draft["products"][productIndex]["premiumChart"][
              draft["products"][productIndex]["selectedSumInsured"]["index"]
            ]["isSelfPremiumSelected"] = true;

            draft["products"][productIndex]["totalProductPremium"] =
              Number(draft["products"][productIndex]["totalProductPremium"]) +
              Number(
                draft["products"][productIndex]["premiumChart"][
                  draft["products"][productIndex]["selectedSumInsured"]["index"]
                ]["selfOnlyPremium"]
              );

            // changing top up selected options
            // if top up option is already selected and base policy option changes then change the top up selected option also
            // set top up selected value
            if (
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["isTopUpSelected"]
            ) {
              //set top up self +  spouse selected flag as true
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["topUpOptions"][selectedTopUpIndex][
                "isTopUpSelfPremiumSelected"
              ] = true;

              //set top up self flag as false
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["topUpOptions"][selectedTopUpIndex][
                "isTopUpSelfSpousePremiumSelected"
              ] = false;

              // subtract self + spouse top up premium from total product premium
              draft["products"][productIndex]["totalProductPremium"] =
                Number(draft["products"][productIndex]["totalProductPremium"]) -
                Number(
                  draft["products"][productIndex]["premiumChart"][
                    selectedPremiumIndex
                  ]["topUpOptions"][selectedTopUpIndex]["selfSpousePremium"]
                );

              //add self  top up premium to total product premium
              draft["products"][productIndex]["totalProductPremium"] =
                Number(draft["products"][productIndex]["totalProductPremium"]) +
                Number(
                  draft["products"][productIndex]["premiumChart"][
                    selectedPremiumIndex
                  ]["topUpOptions"][selectedTopUpIndex]["selfOnlyPremium"]
                );
            }
          }
          break;

        case "isSelfSpouse2ChildrenPremiumSelected":
          //update checkbox value
          draft["products"][productIndex]["premiumChart"][
            draft["products"][productIndex]["selectedSumInsured"]["index"]
          ][name] = event;

          if (event) {
            // Uncheck other premium selections
            [
              "isSelfPremiumSelected",
              "isSelfSpousePremiumSelected",
              "isSelfSpouse1ChildrenPremiumSelected",
              "isSelf2ChildrenPremiumSelected",
              "isSelf1ChildrenPremiumSelected",
            ].forEach((option) => {
              draft["products"][productIndex]["premiumChart"][
                draft["products"][productIndex]["selectedSumInsured"]["index"]
              ][option] = false;
            });

            // Subtract any existing premium
            draft["products"][productIndex]["totalProductPremium"] = 0;

            // Add self + spouse + 2 children premium
            draft["products"][productIndex]["totalProductPremium"] = Number(
              draft["products"][productIndex]["premiumChart"][
                draft["products"][productIndex]["selectedSumInsured"]["index"]
              ]["selfSpouse2ChildrenPremium"]
            );
          } else {
            // If unchecked, default back to self + spouse
            draft["products"][productIndex]["premiumChart"][
              draft["products"][productIndex]["selectedSumInsured"]["index"]
            ]["isSelfPremiumSelected"] = true;

            // Update premium calculation
            draft["products"][productIndex]["totalProductPremium"] = Number(
              draft["products"][productIndex]["premiumChart"][
                draft["products"][productIndex]["selectedSumInsured"]["index"]
              ]["selfOnlyPremium"]
            );
          }
          break;

        case "isSelfSpouse1ChildrenPremiumSelected":
          //update checkbox value
          draft["products"][productIndex]["premiumChart"][
            draft["products"][productIndex]["selectedSumInsured"]["index"]
          ][name] = event;

          if (event) {
            [
              "isSelfPremiumSelected",
              "isSelfSpousePremiumSelected",
              "isSelfSpouse2ChildrenPremiumSelected",
              "isSelf2ChildrenPremiumSelected",
              "isSelf1ChildrenPremiumSelected",
            ].forEach((option) => {
              draft["products"][productIndex]["premiumChart"][
                draft["products"][productIndex]["selectedSumInsured"]["index"]
              ][option] = false;
            });

            // Subtract any existing premium
            draft["products"][productIndex]["totalProductPremium"] = 0;

            // Add self + spouse + 2 children premium
            draft["products"][productIndex]["totalProductPremium"] = Number(
              draft["products"][productIndex]["premiumChart"][
                draft["products"][productIndex]["selectedSumInsured"]["index"]
              ]["selfSpouse1ChildrenPremium"]
            );
          } else {
            // If unchecked, default back to self + spouse
            draft["products"][productIndex]["premiumChart"][
              draft["products"][productIndex]["selectedSumInsured"]["index"]
            ]["isSelfPremiumSelected"] = true;

            // Update premium calculation
            draft["products"][productIndex]["totalProductPremium"] = Number(
              draft["products"][productIndex]["premiumChart"][
                draft["products"][productIndex]["selectedSumInsured"]["index"]
              ]["selfOnlyPremium"]
            );
          }
          break;

        case "isSelf2ChildrenPremiumSelected":
          //update checkbox value
          draft["products"][productIndex]["premiumChart"][
            draft["products"][productIndex]["selectedSumInsured"]["index"]
          ][name] = event;

          if (event) {
            // Uncheck other premium selections
            [
              "isSelfPremiumSelected",
              "isSelfSpousePremiumSelected",
              "isSelfSpouse1ChildrenPremiumSelected",
              "isSelfSpouse2ChildrenPremiumSelected",
              "isSelf1ChildrenPremiumSelected",
            ].forEach((option) => {
              draft["products"][productIndex]["premiumChart"][
                draft["products"][productIndex]["selectedSumInsured"]["index"]
              ][option] = false;
            });

            // Subtract any existing premium
            draft["products"][productIndex]["totalProductPremium"] = 0;

            // Add self + spouse + 2 children premium
            draft["products"][productIndex]["totalProductPremium"] = Number(
              draft["products"][productIndex]["premiumChart"][
                draft["products"][productIndex]["selectedSumInsured"]["index"]
              ]["self2ChildrenPremium"]
            );
          } else {
            // If unchecked, default back to self + spouse
            draft["products"][productIndex]["premiumChart"][
              draft["products"][productIndex]["selectedSumInsured"]["index"]
            ]["isSelfPremiumSelected"] = true;

            // Update premium calculation
            draft["products"][productIndex]["totalProductPremium"] = Number(
              draft["products"][productIndex]["premiumChart"][
                draft["products"][productIndex]["selectedSumInsured"]["index"]
              ]["selfOnlyPremium"]
            );
          }
          break;

        case "isSelf1ChildrenPremiumSelected":
          //update checkbox value
          draft["products"][productIndex]["premiumChart"][
            draft["products"][productIndex]["selectedSumInsured"]["index"]
          ][name] = event;

          if (event) {
            // Uncheck other premium selections
            [
              "isSelfPremiumSelected",
              "isSelfSpousePremiumSelected",
              "isSelfSpouse2ChildrenPremiumSelected",
              "isSelf2ChildrenPremiumSelected",
              "isSelfSpouse1ChildrenPremiumSelected",
            ].forEach((option) => {
              draft["products"][productIndex]["premiumChart"][
                draft["products"][productIndex]["selectedSumInsured"]["index"]
              ][option] = false;
            });

            // Subtract any existing premium
            draft["products"][productIndex]["totalProductPremium"] = 0;

            // Add Self 1 Children Premium premium
            draft["products"][productIndex]["totalProductPremium"] = Number(
              draft["products"][productIndex]["premiumChart"][
                draft["products"][productIndex]["selectedSumInsured"]["index"]
              ]["self1ChildrenPremium"]
            );
          } else {
            // If unchecked, default back to self + spouse
            draft["products"][productIndex]["premiumChart"][
              draft["products"][productIndex]["selectedSumInsured"]["index"]
            ]["isSelfPremiumSelected"] = true;

            // Update premium calculation
            draft["products"][productIndex]["totalProductPremium"] = Number(
              draft["products"][productIndex]["premiumChart"][
                draft["products"][productIndex]["selectedSumInsured"]["index"]
              ]["selfOnlyPremium"]
            );
          }
          break;

        case "isSpousePremiumSelected":
        case "isParent1PremiumSelected":
        case "isParent2PremiumSelected":
        case "isInLaw1PremiumSelected":
        case "isInLaw2PremiumSelected":
          // set the checkbox value
          draft["products"][productIndex]["premiumChart"][
            draft["products"][productIndex]["selectedSumInsured"]["index"]
          ][name] = event;

          //if checked, add the new value to totalProductPremium
          if (event) {
            draft["products"][productIndex]["totalProductPremium"] =
              Number(draft["products"][productIndex]["totalProductPremium"]) +
              Number(
                draft["products"][productIndex]["premiumChart"][
                  draft["products"][productIndex]["selectedSumInsured"]["index"]
                ][getPropertyName(name)]
              );
          } else {
            // if unchecked subtract the chebox value from totalProductPremium
            draft["products"][productIndex]["totalProductPremium"] =
              Number(draft["products"][productIndex]["totalProductPremium"]) -
              Number(
                draft["products"][productIndex]["premiumChart"][
                  draft["products"][productIndex]["selectedSumInsured"]["index"]
                ][getPropertyName(name)]
              );
          }
          break;

        case "isTopUpSelected":
          // set top up selected value

          draft["products"][productIndex][name] = event;

          draft["products"][productIndex]["premiumChart"][selectedPremiumIndex][
            name
          ] = event;

          var isBaseSelfSelected =
            draft["products"][productIndex]["premiumChart"][
              selectedPremiumIndex
            ]["isSelfPremiumSelected"];

          var isBaseSelfSpouseSelected =
            draft["products"][productIndex]["premiumChart"][
              selectedPremiumIndex
            ]["isSelfSpousePremiumSelected"];

          // if top up option is checked , add the self premium of pre-selected top up option
          // to total premium of product
          if (event) {
            // if base policy self option is selected , select the top up self option
            if (isBaseSelfSelected) {
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["topUpOptions"][selectedTopUpIndex][
                "isTopUpSelfPremiumSelected"
              ] = true;

              //add the self top up premium to total product premium
              draft["products"][productIndex]["totalProductPremium"] =
                Number(draft["products"][productIndex]["totalProductPremium"]) +
                Number(
                  draft["products"][productIndex]["premiumChart"][
                    selectedPremiumIndex
                  ]["topUpOptions"][selectedTopUpIndex]["selfOnlyPremium"]
                );
            }

            // if base policy self + spouse option is selected , select the top up self + spouse option

            if (isBaseSelfSpouseSelected) {
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["topUpOptions"][selectedTopUpIndex][
                "isTopUpSelfSpousePremiumSelected"
              ] = true;

              //add the self+ spouse top up premium to total product premium
              draft["products"][productIndex]["totalProductPremium"] =
                Number(draft["products"][productIndex]["totalProductPremium"]) +
                Number(
                  draft["products"][productIndex]["premiumChart"][
                    selectedPremiumIndex
                  ]["topUpOptions"][selectedTopUpIndex]["selfSpousePremium"]
                );
            }
          } else {
            // subtract self only premium from total premium if top up is unchecked
            if (isBaseSelfSelected) {
              draft["products"][productIndex]["totalProductPremium"] =
                Number(draft["products"][productIndex]["totalProductPremium"]) -
                Number(
                  draft["products"][productIndex]["premiumChart"][
                    selectedPremiumIndex
                  ]["topUpOptions"][selectedTopUpIndex]["selfOnlyPremium"]
                );
            }

            if (isBaseSelfSpouseSelected) {
              draft["products"][productIndex]["totalProductPremium"] =
                Number(draft["products"][productIndex]["totalProductPremium"]) -
                Number(
                  draft["products"][productIndex]["premiumChart"][
                    selectedPremiumIndex
                  ]["topUpOptions"][selectedTopUpIndex]["selfSpousePremium"]
                );
            }

            //if spouse premium is checked in top up option , subtract it from total product premium

            if (
              draft["products"][productIndex]["premiumChart"][
                selectedPremiumIndex
              ]["topUpOptions"][selectedTopUpIndex][
                "isTopUpSpousePremiumSelected"
              ]
            ) {
              draft["products"][productIndex]["totalProductPremium"] =
                Number(draft["products"][productIndex]["totalProductPremium"]) -
                Number(
                  draft["products"][productIndex]["premiumChart"][
                    selectedPremiumIndex
                  ]["topUpOptions"][selectedTopUpIndex]["spousePremium"]
                );
            }

            //if top is unchecked , remove topup premium option selction if any
            draft["products"][productIndex]["premiumChart"][
              selectedPremiumIndex
            ]["topUpOptions"][selectedTopUpIndex][
              "isTopUpSpousePremiumSelected"
            ] = false;

            draft["products"][productIndex]["premiumChart"][
              selectedPremiumIndex
            ]["topUpOptions"][selectedTopUpIndex][
              "isTopUpSelfPremiumSelected"
            ] = false;

            draft["products"][productIndex]["premiumChart"][
              selectedPremiumIndex
            ]["topUpOptions"][selectedTopUpIndex][
              "isTopUpSelfSpousePremiumSelected"
            ] = false;
          }
          break;

        case "topupSumInsured":
          var premiumOptionIndex =
            draft["products"][productIndex]["selectedSumInsured"]?.index;
          var previousTopUpIndex =
            draft["products"][productIndex]["selectedTopUpOption"]?.index;

          var isSelfSelected =
            draft["products"][productIndex]["premiumChart"][premiumOptionIndex][
              "topUpOptions"
            ][previousTopUpIndex]["isTopUpSelfPremiumSelected"];

          var isSelfSpouseSelected =
            draft["products"][productIndex]["premiumChart"][premiumOptionIndex][
              "topUpOptions"
            ][previousTopUpIndex]["isTopUpSelfSpousePremiumSelected"];

          var isSpouseSelected =
            draft["products"][productIndex]["premiumChart"][premiumOptionIndex][
              "topUpOptions"
            ][previousTopUpIndex]["isTopUpSpousePremiumSelected"];

          //subtract the previously selected self only premium from total product premium
          if (isSelfSelected) {
            draft["products"][productIndex]["totalProductPremium"] =
              Number(draft["products"][productIndex]["totalProductPremium"]) -
              Number(
                draft["products"][productIndex]["premiumChart"][
                  premiumOptionIndex
                ]["topUpOptions"][previousTopUpIndex]["selfOnlyPremium"]
              );

            draft["products"][productIndex]["premiumChart"][premiumOptionIndex][
              "topUpOptions"
            ][previousTopUpIndex]["isTopUpSelfPremiumSelected"] = false;
          }

          if (isSelfSpouseSelected) {
            draft["products"][productIndex]["totalProductPremium"] =
              Number(draft["products"][productIndex]["totalProductPremium"]) -
              Number(
                draft["products"][productIndex]["premiumChart"][
                  premiumOptionIndex
                ]["topUpOptions"][previousTopUpIndex]["selfSpousePremium"]
              );

            draft["products"][productIndex]["premiumChart"][premiumOptionIndex][
              "topUpOptions"
            ][previousTopUpIndex]["isTopUpSelfSpousePremiumSelected"] = false;
          }

          if (isSpouseSelected) {
            draft["products"][productIndex]["totalProductPremium"] =
              Number(draft["products"][productIndex]["totalProductPremium"]) -
              Number(
                draft["products"][productIndex]["premiumChart"][
                  premiumOptionIndex
                ]["topUpOptions"][previousTopUpIndex]["spousePremium"]
              );

            draft["products"][productIndex]["premiumChart"][premiumOptionIndex][
              "topUpOptions"
            ][previousTopUpIndex]["isTopUpSpousePremiumSelected"] = false;
          }

          draft["products"][productIndex]["selectedTopUpOption"] = event;

          if (isSelfSelected) {
            draft["products"][productIndex]["premiumChart"][premiumOptionIndex][
              "topUpOptions"
            ][event?.index]["isTopUpSelfPremiumSelected"] = true;

            draft["products"][productIndex]["totalProductPremium"] =
              Number(draft["products"][productIndex]["totalProductPremium"]) +
              Number(
                draft["products"][productIndex]["premiumChart"][
                  premiumOptionIndex
                ]["topUpOptions"][event?.index]["selfOnlyPremium"]
              );
          }

          if (isSelfSpouseSelected) {
            draft["products"][productIndex]["premiumChart"][premiumOptionIndex][
              "topUpOptions"
            ][event?.index]["isTopUpSelfSpousePremiumSelected"] = true;

            draft["products"][productIndex]["totalProductPremium"] =
              Number(draft["products"][productIndex]["totalProductPremium"]) +
              Number(
                draft["products"][productIndex]["premiumChart"][
                  premiumOptionIndex
                ]["topUpOptions"][event?.index]["selfSpousePremium"]
              );
          }

          if (isSpouseSelected) {
            draft["products"][productIndex]["premiumChart"][premiumOptionIndex][
              "topUpOptions"
            ][event?.index]["isSpousePremiumSelected"] = true;

            draft["products"][productIndex]["totalProductPremium"] =
              Number(draft["products"][productIndex]["totalProductPremium"]) +
              Number(
                draft["products"][productIndex]["premiumChart"][
                  premiumOptionIndex
                ]["topUpOptions"][event?.index]["spousePremium"]
              );
          }

          break;

        case "isTopUpSpousePremiumSelected":
          // set to up spouse premium checkbox value
          draft["products"][productIndex]["premiumChart"][
            draft["products"][productIndex]["selectedSumInsured"]["index"]
          ]["topUpOptions"][
            draft["products"][productIndex]["selectedTopUpOption"]["index"]
          ][name] = event;

          // add spouse premium to total premiumfor product if checked else subtract from total premium
          if (event) {
            draft["products"][productIndex]["totalProductPremium"] =
              Number(draft["products"][productIndex]["totalProductPremium"]) +
              Number(
                draft["products"][productIndex]["premiumChart"][
                  draft["products"][productIndex]["selectedSumInsured"]["index"]
                ]["topUpOptions"][
                  draft["products"][productIndex]["selectedTopUpOption"][
                    "index"
                  ]
                ]["spousePremium"]
              );
          } else {
            draft["products"][productIndex]["totalProductPremium"] =
              Number(draft["products"][productIndex]["totalProductPremium"]) -
              Number(
                draft["products"][productIndex]["premiumChart"][
                  draft["products"][productIndex]["selectedSumInsured"]["index"]
                ]["topUpOptions"][
                  draft["products"][productIndex]["selectedTopUpOption"][
                    "index"
                  ]
                ]["spousePremium"]
              );
          }

          break;

        case "isDisclaimerAccepted":
          //Set to disclaimer
          draft["products"][productIndex][name] = event;

          break;
      }
    });

    nextState = produce(nextState, (draft) => {
      draft.totalPremium = getTotalPremium(draft.products || []);
    });

    setUserDetails(nextState);
  };

  // Helper function to reset all premium selections
  const resetPremiumSelections = (premiumChart) => {
    if (!premiumChart) return;

    const premiumOptions = [
      "isSelfPremiumSelected",
      "isSelfSpousePremiumSelected",
      "isSpousePremiumSelected",
      "isChild1PremiumSelected",
      "isChild2PremiumSelected",
      "isParent1PremiumSelected",
      "isParent2PremiumSelected",
      "isInLaw1PremiumSelected",
      "isInLaw2PremiumSelected",
      "isTopUpSelected",
    ];

    premiumOptions.forEach((option) => {
      premiumChart[option] = false;
    });
  };

  const markUserPolicy = userStore((state) => state.markUserPolicy);

  const isDigitProductSelected = (products) => {
    let isSelected = false;

    products.forEach((product) => {
      if (
        product.productName === "Digit Payment Protection" ||
        product.productName === "DIGIT PAYMENT PROTECTION POLICY"
      ) {
        isSelected = true;
        return;
      }
    });

    return isSelected;
  };

  const isStandaloneSuperTopProductSelected = (products) => {
    let isSelected = false;

    products.forEach((product) => {
      if (product.productName === "Standalone Super Top Up Policy_BPP_New") {
        isSelected = true;
        return;
      }
    });

    return isSelected;
  };
  const isGroupHealthInsurancePolicyProductSelected = (products) => {
    let isSelected = false;
    products.forEach((product) => {
      if (product.productName === "Group Health Insurance Policy for BPP_New") {
        isSelected = true;
        return;
      }
    });

    return isSelected;
  };
  // Add this function to check if both Group Health Insurance Policy and Standalone Super Top Up Policy have the same sum insured value
  const hasSameSumInsured = (products) => {
    let groupHealthPolicy = null;
    let superTopUpPolicy = null;

    // Find the relevant policies
    products.forEach((product) => {
      if (
        product.productName === "Group Health Insurance Policy for BPP_New" &&
        product.isProductSelected
      ) {
        groupHealthPolicy = product;
      }
      if (
        product.productName === "Standalone Super Top Up Policy_BPP_New" &&
        product.isProductSelected
      ) {
        superTopUpPolicy = product;
      }
    });

    // If both policies are selected, check if they have the same sum insured value
    if (groupHealthPolicy && superTopUpPolicy) {
      return (
        groupHealthPolicy.selectedSumInsured.name ===
        superTopUpPolicy.selectedSumInsured.name
      );
    }

    // If one or both policies aren't selected, validation is not needed
    return true;
  };
  const handleSubmit = async () => {
    try {
      setIsLoading(true);

      if (!isProductSelected(userDetails.products)) {
        toast({
          variant: "destructive",
          title: "Product not selected",
          description: "Please select at least one product.",
        });
        return;
      }
      //Digit payment validation
      if (isDigitProductSelected(userDetails.products)) {
        let userResponse = await validatepolicypurchase(
          userId > 0 ? userId : user?.userId
        );

        if (
          userResponse.status === "success" &&
          userResponse.data.message != "ok"
        ) {
          toast({
            variant: "destructive",
            title: "digit Product selected",
            description: userResponse.data.message,
          });
          return;
        }
      }

      // if (
      //   isGroupHealthInsurancePolicyProductSelected(userDetails.products) &&
      //   isStandaloneSuperTopProductSelected(userDetails.products)
      // ) {
      //   if (!hasSameSumInsured(userDetails.products)) {
      //     toast({
      //       variant: "destructive",
      //       title: "Validation Error",
      //       description:
      //         "The Sum Insured for the Group Health Insurance Policy and the Standalone Super Top-Up Policy should be identical.",
      //     });
      //     return;
      //   }
      // }

      // Prepare post object
      const postObject = {
        userId: userDetails?.userId || 0,
        policyId: userDetails?.policyId,
        totalPremium: userDetails?.totalPremium || 0,
        amountPaid: userDetails?.amountPaid || 0,
        totalPaidPremium: userDetails?.totalPaidPremium || 0,
        products: getSelectedProducts(userDetails.products),
        isUpDate: mode === "edit",
      };

      // Create and populate FormData
      const formData = new FormData();
     
      appendFormData(formData, postObject, "");
      // Submit form data
      const response = await addProductPolicy(formData, step);

      if (response.status !== "success") {
        throw new Error("Failed to save policy details");
      }

      // Mark user policy and update policy ID
      markUserPolicy();

      if (response?.data?.policyId) {
        updatePolicyId(response.data.policyId);
      }
      // Verify the update
      const updatedPolicyId = usePolicyStore.getState().userDetails.policyId;

      // Update beneficiaries state
      const dTOPolicy = response.data.dTOPolicy;
      if (dTOPolicy) {
        const nextState = produce(userDetails, (draft) => {
          // Update spouse details if present
          if (dTOPolicy.beneficiaries?.spouse) {
            draft.beneficiaries.spouse = {
              ...draft.beneficiaries.spouse,
              ...dTOPolicy.beneficiaries.spouse,
            };
          }

          // Update nominee details if present
          if (dTOPolicy.nominee) {
            draft.beneficiaries.nominee = {
              ...draft.beneficiaries.nominee,
              ...dTOPolicy.nominee,
            };
          }
        });

        setUserDetails(nextState);
      }

      // Navigate to next step
      setStep(2);
      navigate(
        userId
          ? `/beneficiarydetails?policyId=${updatedPolicyId}&userId=${userId}&campaignId=${
              campaignId || campaignIdused
            }`
          : `/beneficiarydetails?policyId=${updatedPolicyId}&campaignId=${
              campaignId || campaignIdused
            }`
      );
    } catch (error) {
      console.error("Form submission error:", error);
      toast({
        variant: "destructive",
        title: "Something went wrong",
        description: error.message || "Unable to save details.",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleNext = async () => {
    setIsLoading(true);
    if (isProductSelected(userDetails.products)) {
      //save step 1
      let postObject = {
        userId: userId ? userId : userDetails?.userId || 0,
        policyId: userDetails?.policyId,
        totalPremium: userDetails?.totalPremium || 0,
        amountPaid: userDetails?.amountPaid || 0,
        totalPaidPremium: userDetails?.totalPaidPremium || 0,
        products: getSelectedProducts(userDetails.products),
        isUpDate: mode === "edit" ? true : false,
      };

      let policyId = userDetails?.policyId;
      updatePolicyId(policyId);
      setStep(2);

      navigate(
        userId
          ? `/beneficiarydetails?userId=${userId}&campaignId=${
              campaignId || campaignIdused
            }`
          : `/beneficiarydetails?campaignId=${campaignId || campaignIdused}`
      );
    } else {
      toast({
        variant: "destructive",
        title: "Product not selected",
        description: "Please select at least on product.",
      });
    }

    setIsLoading(false);
  };

  const handleViewClick = (prod) => {
    setSelectedProduct(prod);
    setShowDialog(true);
  };

  const handleCloseDialog = () => {
    setShowDialog(false);
    setSelectedProduct(null);
  };

  if (isLoadingDependencies) {
    return <LoadingSpinner />;
  }

  if (error) {
    <div>
      {" "}
      <div className="text-red-600">Please complete your profile</div>
      <ErrorDisplay />
    </div>;
  }

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-xl font-bold sm:text-2xl">Product List</h1>
        {userId && (
          <Button
            variant="ghost"
            onClick={() => navigate("/users")}
            className="p-2"
          >
            <CircleArrowLeft className="w-5 h-5" />
          </Button>
        )}
      </div>

      <section className="space-y-6">
        {userDetails?.products?.length ? (
          <div className=" gap-6 grid">
            {userDetails.products.map((product, productIndex) => (
              <>
                {product.premiumChart && product.premiumChart[0].id > 0 && (
                  <Card
                    className="w-full border-0 shadow-xl"
                    key={product.productId}
                  >
                    <CardHeader className="space-x-3 p-4 flex flex-row items-start sm:items-center sm:p-6">
                      <Checkbox
                        checked={product.isProductSelected}
                        onCheckedChange={handleChange(
                          "isProductSelected",
                          productIndex
                        )}
                        disabled={product?.isCampaignExpired}
                        className="mt-1"
                      />
                      <div className="flex-1">
                        <CardTitle className="gap-2 flex flex-col justify-between sm:flex-row sm:items-center">
                          <span className="text-base font-medium">
                            {product.productName}
                          </span>
                          <div className="gap-2 mt-2 flex items-center sm:mt-0">
                            <TooltipProvider>
                              <Tooltip>
                                <TooltipTrigger asChild>
                                  <Button
                                    variant="ghost"
                                    size="icon"
                                    className="border-gray-200 text-white bg-red-900 w-8 h-8 rounded-full border hover:bg-red-800 dark:border-gray-800"
                                    onClick={() => handleViewClick(product)}
                                  >
                                    <EyeIcon className="w-4 h-4" />
                                  </Button>
                                </TooltipTrigger>
                                <TooltipContent>
                                  View Premium Chart
                                </TooltipContent>
                              </Tooltip>
                            </TooltipProvider>
                            {product.productBroucher && (
                              <ProductDocumentDownload
                                productDocumentUrl={product.productBroucher}
                                documentName={product.productName}
                                name=""
                                className="flex-shrink-0"
                              />
                            )}
                          </div>
                        </CardTitle>
                      </div>
                    </CardHeader>
                    {/* <CardContent className="gap-4 grid"> */}
                    <CardContent
                      className={`gap-4 grid ${
                        !product.isProductSelected
                          ? "opacity-50 pointer-events-none"
                          : ""
                      }`}
                    >
                      <div className="gap-2 grid">
                        <RSelect
                          options={product.sumInsuredOptions}
                          label="Sum Insured"
                          className="space-y-2"
                          value={product.selectedSumInsured}
                          onChange={handleChange("sumInsured", productIndex)}
                          valueProperty="id"
                          nameProperty="name"
                          disabled={product?.isCampaignExpired}
                        />{" "}
                        {product.ageBandPremiumRateValue != null ? (
                          <span>
                            Age Band :{" "}
                            <Label> {product.ageBandPremiumRateValue}</Label>
                          </span>
                        ) : null}
                      </div>
                      <div className="overflow-auto">
                        {/* table */}
                        <div className="gap-4 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5">
                          {product["premiumChart"][
                            product.selectedSumInsured?.index
                          ]["selfOnlyPremium"] == 0 ? null : (
                            <div className="bg-white rounded-lg border dark:bg-gray-800">
                              <div className="px-3 py-2 bg-gray-100 rounded-t-lg font-semibold dark:bg-gray-700">
                                Self
                              </div>
                              <div className="px-3 py-2 flex items-center">
                                <Checkbox
                                  className="mr-3"
                                  id="self-spouse-premium-base"
                                  checked={
                                    product.selectedSumInsured !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ].isSelfPremiumSelected
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isSelfPremiumSelected",
                                    productIndex
                                  )}
                                  disabled={product?.isCampaignExpired}
                                />
                                {product.selectedSumInsured !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["selfOnlyPremium"]
                                  : null}
                              </div>
                            </div>
                          )}
                          {/*selfSpousePremium */}
                          {product["premiumChart"][
                            product.selectedSumInsured?.index
                          ]["selfSpousePremium"] == 0 ? null : (
                            <div className="bg-white rounded-lg border dark:bg-gray-800">
                              <div className="px-3 py-2 bg-gray-100 rounded-t-lg font-semibold dark:bg-gray-700">
                                Self + Spouse
                              </div>
                              <div className="px-3 py-2 flex items-center">
                                <Checkbox
                                  className="mr-3"
                                  id="self-spouse-premium-base"
                                  checked={
                                    product.selectedSumInsured !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ].isSelfSpousePremiumSelected
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isSelfSpousePremiumSelected",
                                    productIndex
                                  )}
                                  disabled={product?.isCampaignExpired}
                                />
                                {product.selectedSumInsured !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["selfSpousePremium"]
                                  : null}
                              </div>
                            </div>
                          )}

                          {/*selfSpouse2ChildrenPremium */}

                          {product["premiumChart"][
                            product.selectedSumInsured?.index
                          ]["selfSpouse2ChildrenPremium"] == 0 ? null : (
                            <div className="bg-white rounded-lg border dark:bg-gray-800">
                              <div className="px-3 py-2 bg-gray-100 rounded-t-lg font-semibold dark:bg-gray-700">
                                Self + Spouse + 2 Children
                              </div>
                              <div className="px-3 py-2 flex items-center">
                                <Checkbox
                                  className="mr-3"
                                  id="self-Spouse2-Children-Premium-base"
                                  checked={
                                    product.selectedSumInsured !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ].isSelfSpouse2ChildrenPremiumSelected
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isSelfSpouse2ChildrenPremiumSelected",
                                    productIndex
                                  )}
                                  disabled={product?.isCampaignExpired}
                                />
                                {product.selectedSumInsured !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["selfSpouse2ChildrenPremium"]
                                  : null}
                              </div>
                            </div>
                          )}

                          {/*selfSpouse1ChildrenPremium */}
                          {product["premiumChart"][
                            product.selectedSumInsured?.index
                          ]["selfSpouse1ChildrenPremium"] == 0 ? null : (
                            <div className="bg-white rounded-lg border dark:bg-gray-800">
                              <div className="px-3 py-2 bg-gray-100 rounded-t-lg font-semibold dark:bg-gray-700">
                                Self + Spouse + 1 Child
                              </div>
                              <div className="px-3 py-2 flex items-center">
                                <Checkbox
                                  className="mr-3"
                                  id="self-Spouse1-Children-Premium-base"
                                  checked={
                                    product.selectedSumInsured !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ].isSelfSpouse1ChildrenPremiumSelected
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isSelfSpouse1ChildrenPremiumSelected",
                                    productIndex
                                  )}
                                  disabled={product?.isCampaignExpired}
                                />
                                {product.selectedSumInsured !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["selfSpouse1ChildrenPremium"]
                                  : null}
                              </div>
                            </div>
                          )}

                          {/*self2ChildrenPremium */}

                          {product["premiumChart"][
                            product.selectedSumInsured?.index
                          ]["self2ChildrenPremium"] == 0 ? null : (
                            <div className="bg-white rounded-lg border dark:bg-gray-800">
                              <div className="px-3 py-2 bg-gray-100 rounded-t-lg font-semibold dark:bg-gray-700">
                                Self + 2 Children
                              </div>
                              <div className="px-3 py-2 flex items-center">
                                <Checkbox
                                  className="mr-3"
                                  id="self2-Children-Premium-base"
                                  checked={
                                    product.selectedSumInsured !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ].isSelf2ChildrenPremiumSelected
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isSelf2ChildrenPremiumSelected",
                                    productIndex
                                  )}
                                  disabled={product?.isCampaignExpired}
                                />
                                {product.selectedSumInsured !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["self2ChildrenPremium"]
                                  : null}
                              </div>
                            </div>
                          )}

                          {/*self1ChildrenPremium */}
                          {product["premiumChart"][
                            product.selectedSumInsured?.index
                          ]["self1ChildrenPremium"] == 0 ? null : (
                            <div className="bg-white rounded-lg border dark:bg-gray-800">
                              <div className="px-3 py-2 bg-gray-100 rounded-t-lg font-semibold dark:bg-gray-700">
                                Self + 1 Child
                              </div>
                              <div className="px-3 py-2 flex items-center">
                                <Checkbox
                                  className="mr-3"
                                  id="self1-Children-Premium-base"
                                  checked={
                                    product.selectedSumInsured !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ].isSelf1ChildrenPremiumSelected
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isSelf1ChildrenPremiumSelected",
                                    productIndex
                                  )}
                                  disabled={product?.isCampaignExpired}
                                />
                                {product.selectedSumInsured !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["self1ChildrenPremium"]
                                  : null}
                              </div>
                            </div>
                          )}

                          {/* base spouse */}
                          {product["premiumChart"][
                            product.selectedSumInsured?.index
                          ]["spousePremium"] == 0 ? null : (
                            <div className="bg-white rounded-lg border dark:bg-gray-800">
                              <div className="px-3 py-2 bg-gray-100 rounded-t-lg font-semibold dark:bg-gray-700">
                                Spouse
                              </div>
                              <div className="px-4 py-2">
                                <Checkbox
                                  className="mr-3"
                                  id="check-spouse-premium"
                                  checked={
                                    product.selectedSumInsured !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ].isSpousePremiumSelected
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isSpousePremiumSelected",
                                    productIndex
                                  )}
                                  disabled={
                                    !product.isProductSelected ||
                                    product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["spousePremium"] == 0
                                  }
                                />
                                {product.selectedSumInsured !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["spousePremium"]
                                  : null}
                              </div>
                            </div>
                          )}

                          {/* base child1 */}
                          {product["premiumChart"][
                            product.selectedSumInsured?.index
                          ]["child1Premium"] == 0 ? null : (
                            <div className="bg-white rounded-lg border dark:bg-gray-800">
                              <div className="px-3 py-2 bg-gray-100 rounded-t-lg font-semibold dark:bg-gray-700">
                                Child 1
                              </div>
                              <div className="px-4 py-2">
                                <Checkbox
                                  className="mr-3"
                                  id="life-child1"
                                  checked={
                                    product.selectedSumInsured !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ].isChild1PremiumSelected
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isChild1PremiumSelected",
                                    productIndex
                                  )}
                                  disabled={
                                    !product.isProductSelected ||
                                    product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["child1Premium"] == 0
                                  }
                                />
                                {product.selectedSumInsured !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["child1Premium"]
                                  : null}
                              </div>
                            </div>
                          )}
                          {/* base child2 */}
                          {product["premiumChart"][
                            product.selectedSumInsured?.index
                          ]["child2Premium"] == 0 ? null : (
                            <div>
                              <div className="px-4 py-2 bg-gray-100 text-left font-bold dark:bg-gray-80">
                                Child 2
                              </div>
                              <div className="px-4 py-2">
                                <Checkbox
                                  className="mr-3"
                                  id="life-child2"
                                  checked={
                                    product.selectedSumInsured !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ].isChild2PremiumSelected
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isChild2PremiumSelected",
                                    productIndex
                                  )}
                                  disabled={
                                    !product.isProductSelected ||
                                    product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["child2Premium"] == 0
                                  }
                                />
                                {product.selectedSumInsured !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["child2Premium"]
                                  : null}
                              </div>
                            </div>
                          )}
                          {/* base parent1 */}
                          {product["premiumChart"][
                            product.selectedSumInsured?.index
                          ]["parent1Premium"] == 0 ? null : (
                            <div>
                              <div className="px-4 py-2 bg-gray-100 text-left font-bold dark:bg-gray-80">
                                Parent 1
                              </div>
                              <div className="px-4 py-2">
                                <Checkbox
                                  className="mr-3"
                                  id="parent1"
                                  checked={
                                    product.selectedSumInsured !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ].isParent1PremiumSelected
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isParent1PremiumSelected",
                                    productIndex
                                  )}
                                  disabled={
                                    !product.isProductSelected ||
                                    product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["parent1Premium"] == 0
                                  }
                                />
                                {product.selectedSumInsured !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["parent1Premium"]
                                  : null}
                              </div>
                            </div>
                          )}
                          {/* base parent2 */}
                          {product["premiumChart"][
                            product.selectedSumInsured?.index
                          ]["parent2Premium"] == 0 ? null : (
                            <div>
                              <div className="px-4 py-2 bg-gray-100 text-left font-bold dark:bg-gray-80">
                                Parent 2
                              </div>
                              <div className="px-4 py-2">
                                <Checkbox
                                  className="mr-3"
                                  id="parent2"
                                  checked={
                                    product.selectedSumInsured !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ].isParent2PremiumSelected
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isParent2PremiumSelected",
                                    productIndex
                                  )}
                                  disabled={
                                    !product.isProductSelected ||
                                    product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["parent2Premium"] == 0
                                  }
                                />
                                {product.selectedSumInsured !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["parent2Premium"]
                                  : null}
                              </div>
                            </div>
                          )}
                          {/* base In law1 */}
                          {product["premiumChart"][
                            product.selectedSumInsured?.index
                          ]["inLaw1Premium"] == 0 ? null : (
                            <div>
                              <div className="px-4 py-2 bg-gray-100 text-left font-bold dark:bg-gray-80">
                                In Law 1
                              </div>
                              <div className="px-4 py-2">
                                <Checkbox
                                  className="mr-3"
                                  id="in-law1"
                                  checked={
                                    product.selectedSumInsured !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ].isInLaw1PremiumSelected
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isInLaw1PremiumSelected",
                                    productIndex
                                  )}
                                  disabled={
                                    !product.isProductSelected ||
                                    product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["inLaw1Premium"] == 0
                                  }
                                />
                                {product.selectedSumInsured !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["inLaw1Premium"]
                                  : null}
                              </div>
                            </div>
                          )}
                          {/* base In law2 */}
                          {product["premiumChart"][
                            product.selectedSumInsured?.index
                          ]["inLaw2Premium"] == 0 ? null : (
                            <div>
                              <div className="px-4 py-2 bg-gray-100 text-left font-bold dark:bg-gray-80">
                                In Law 2
                              </div>
                              <div className="px-4 py-2">
                                <Checkbox
                                  className="mr-3"
                                  id="in-law2"
                                  checked={
                                    product.selectedSumInsured !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ].isInLaw2PremiumSelected
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isInLaw2PremiumSelected",
                                    productIndex
                                  )}
                                  disabled={
                                    !product.isProductSelected ||
                                    product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["inLaw2Premium"] == 0
                                  }
                                />
                                {product.selectedSumInsured !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["inLaw2Premium"]
                                  : null}
                              </div>
                            </div>
                          )}
                        </div>
                      </div>

                      {/* top up table */}
                      {product["premiumChart"][
                        product.selectedSumInsured?.index
                      ]["topUpOptions"]?.length ? (
                        <div className="my-5 overflow-auto">
                          <div className="mb-2 flex items-center">
                            <Checkbox
                              className="mr-3"
                              id="check-spouse-premium"
                              checked={
                                product["premiumChart"][
                                  product.selectedSumInsured?.index
                                ]["isTopUpSelected"]
                              }
                              onCheckedChange={handleChange(
                                "isTopUpSelected",
                                productIndex
                              )}
                              disabled={!product.isProductSelected}
                            />
                            <Label>Top up</Label>
                          </div>

                          <div className="gap-2 grid">
                            <RSelect
                              options={
                                product["premiumChart"][
                                  product.selectedSumInsured?.index
                                ]["topUpSumInsuredOptions"]
                              }
                              className="space-y-2"
                              value={product.selectedTopUpOption}
                              onChange={handleChange(
                                "topupSumInsured",
                                productIndex
                              )}
                              valueProperty="id"
                              nameProperty="name"
                              isDisabled={
                                !product["premiumChart"][
                                  product.selectedSumInsured?.index
                                ]["isTopUpSelected"] || mode === "edit"
                              }
                            />
                          </div>

                          <div className="mt-4 grid grid-cols-2 lg:grid-cols-5">
                            {/* top up self*/}
                            <div>
                              <div className="px-4 py-2 bg-gray-100 text-left font-bold dark:bg-gray-80">
                                Self
                              </div>
                              <div className="px-4 py-2">
                                <Checkbox
                                  className="mr-3"
                                  id="check-spouse-premium"
                                  checked={
                                    product.selectedTopUpOption !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ]["topUpOptions"][
                                          product.selectedTopUpOption?.index
                                        ]["isTopUpSelfPremiumSelected"]
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isTopUpSelfPremiumSelected",
                                    productIndex
                                  )}
                                  disabled={true}
                                />
                                {product.selectedTopUpOption !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["topUpOptions"][
                                      product.selectedTopUpOption?.index
                                    ]["selfOnlyPremium"]
                                  : null}
                              </div>
                            </div>

                            {/* top up self + spouse */}
                            <div>
                              <div className="px-4 py-2 bg-gray-100 text-left font-bold dark:bg-gray-80">
                                Self + Spouse
                              </div>
                              <div className="px-4 py-2">
                                <Checkbox
                                  className="mr-3"
                                  id="check-spouse-premium"
                                  checked={
                                    product.selectedTopUpOption !== null
                                      ? product["premiumChart"][
                                          product.selectedSumInsured?.index
                                        ]["topUpOptions"][
                                          product.selectedTopUpOption?.index
                                        ]["isTopUpSelfSpousePremiumSelected"]
                                      : false
                                  }
                                  onCheckedChange={handleChange(
                                    "isTopUpSelfSpousePremiumSelected",
                                    productIndex
                                  )}
                                  disabled={true}
                                />
                                {product.selectedTopUpOption !== null
                                  ? product["premiumChart"][
                                      product.selectedSumInsured?.index
                                    ]["topUpOptions"][
                                      product.selectedTopUpOption?.index
                                    ]["selfSpousePremium"]
                                  : null}
                              </div>
                            </div>

                            {/* top up spouse */}
                            {product["premiumChart"][
                              product.selectedSumInsured?.index
                            ]["topUpOptions"][
                              product.selectedTopUpOption?.index
                            ]["spousePremium"] == 0 ? null : (
                              <div>
                                <div className="px-4 py-2 bg-gray-100 text-left font-bold dark:bg-gray-80">
                                  Spouse
                                </div>
                                <div className="px-4 py-2">
                                  <Checkbox
                                    className="mr-3"
                                    id="check-spouse-premium"
                                    checked={
                                      product.selectedTopUpOption !== null
                                        ? product["premiumChart"][
                                            product.selectedSumInsured?.index
                                          ]["topUpOptions"][
                                            product.selectedTopUpOption?.index
                                          ]["isTopUpSpousePremiumSelected"]
                                        : false
                                    }
                                    onCheckedChange={handleChange(
                                      "isTopUpSpousePremiumSelected",
                                      productIndex
                                    )}
                                    disabled={true}
                                  />
                                  {product.selectedTopUpOption !== null
                                    ? product["premiumChart"][
                                        product.selectedSumInsured?.index
                                      ]["topUpOptions"][
                                        product.selectedTopUpOption?.index
                                      ]["spousePremium"]
                                    : null}
                                </div>
                              </div>
                            )}

                            <div>
                              <div></div>
                              <div></div>
                            </div>
                            <div>
                              <div></div>
                              <div></div>
                            </div>
                            <div>
                              <div></div>
                              <div></div>
                            </div>
                            <div>
                              <div></div>
                              <div></div>
                            </div>
                            <div>
                              <div></div>
                              <div></div>
                            </div>
                            <div>
                              <div></div>
                              <div></div>
                            </div>
                          </div>
                        </div>
                      ) : null}
                      {product.disclaimer && (
                        <div className="py-5 flex items-center justify-start border-t">
                          <DisclaimerComponent
                            product={product}
                            productIndex={productIndex}
                            handleChange={handleChange}
                            disclaimerText={product.disclaimer}
                            title="Disclaimer" // optional
                            checkboxLabel="I acknowledge that I have read, understood, and agree to the Purchase Policy" // optional
                            linkText="Disclaimer" // optional
                          />
                        </div>
                      )}

                      <div className="py-5 flex items-center justify-end border-t">
                        <div className="font-medium">
                          Total Premium: ₹ {product.totalProductPremium}
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                )}
              </>
            ))}

            <ProductSummary
              mode={mode}
              userDetails={userDetails}
              permissions={permissions}
              PAGE_NAME={PAGE_NAME}
            />
            <div className="grid grid-cols-1 md:grid-cols-1">
              <div className="space-x-2 flex items-center justify-end">
                {mode === "edit" && permissions?.[PAGE_NAME]?.read ? (
                  <RButton
                    className="px-5"
                    onClick={handleNext}
                    isLoading={isLoading}
                  >
                    Next
                  </RButton>
                ) : null}

                {permissions?.[PAGE_NAME_policyproduct]?.update ? (
                  <RButton
                    className="px-5"
                    onClick={handleSubmit}
                    isLoading={isLoading}
                  >
                    Save and Next
                  </RButton>
                ) : null}
              </div>
            </div>
          </div>
        ) : (
          <NoProductData />
        )}
      </section>
      <PremiumChartDialog
        showDialog={showDialog}
        selectedProduct={selectedProduct}
        handleCloseDialog={handleCloseDialog}
      />
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithProfile(WithLayout(Productlist)))
);
