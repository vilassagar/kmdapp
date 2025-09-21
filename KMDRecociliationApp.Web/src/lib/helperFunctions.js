import { format } from "date-fns";
import { produce } from "immer";
import { v4 as uuid } from "uuid";

/**
 * Function to check if id id UUID
 * @param {string} str
 * @returns
 */
export const isUUID = (str) => {
  const uuidRegex =
    /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-4[0-9a-fA-F]{3}-[89ABab][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$/;
  return uuidRegex.test(str);
};

/**
 * Function to get association empty object
 * @returns
 */
export const getAssociationObject = () => {
  return {
    id: 0,
    associationName: "",
    associationCode: "",
    organisation: null,
    parentAssociation: null,
    address1: "",
    address2: "",
    city: "",
    state: null,
    pinCode: "",
    country: null,
    bank: {
      id: 0,
      bankName: "",
      branchName: "",
      accountName: "",
      accountNumber: "",
      ifscCode: "",
      micrCode: "",
    },
    contacts: [],
    messages: [],
    isMandateFileUpdated: false,
    mandateFile: null,
    acceptOnePayPayment: false,
    onePayId: "",
    qrCodeFile: null,
    isQrCodeFileUpdated: false,
  };
};

export const getAssociationPostObject = (association) => {
  return {
    id: association?.id,
    associationName: association?.associationName,
    associationCode: association?.associationCode,
    organisation: association?.organisation,
    parentAssociation: association?.parentAssociation,
    address1: association?.address1,
    address2: association?.address2,
    city: association?.city,
    state: association?.state,
    pinCode: association?.pinCode,
    country: association?.country,
    Bank: {
      id: association?.bank?.id || 0,
      bankName: association?.bank?.bankName,
      branchName: association?.bank?.branchName,
      accountName: association?.bank?.accountName,
      accountNumber: association?.bank?.accountNumber,
      ifscCode: association?.bank?.ifscCode,
      micrCode: association?.bank?.micrCode,
    },
    mandateFile: association?.mandateFile,
    isMandateFileUpdated: association?.isMandateFileUpdated,
    qrCodeFile: {
      id: association?.qrCodeFile?.id || 0,
      name: association?.qrCodeFile?.name || "",
      file: association?.qrCodeFile?.file || null,
      url: association?.qrCodeFile?.url || "",
    },
    isQrCodeFileUpdated: association?.isQrCodeFileUpdated,
    associationContactDetails: association?.associationContactDetails,
    associationMessages: association?.associationMessages,
    acceptOnePayPayment: association?.acceptOnePayPayment,
    onePayId: association?.onePayId || "",
  };
};

export const getApplicantObject = () => {
  return {
    id: 0,
    uniqueIdentifier: "",
    firstName: "",
    lastName: "",
    guardianName: "",
    dateOfBirth: null,
    gender: null,
    mobileNumber: "",
    salary: 0,
    associatedOrganization: null,
    address: "",
    idCardType: null,
    idCardNumber: "",
    productName: "",
    bankDetails: {
      id: 0,
      bankName: "",
      bankBranchDetails: "",
      bankAccountNumber: "",      
      bankIfscCode: "",
      bankMicrCode: "",
    },
    dependents: [],
  };
};

export const getApplicantPostObject = (applicant) => {

  return {
    id: applicant?.id || 0,
    uniqueIdentifier: applicant?.uniqueIdentifier || "",
    firstName: applicant?.firstName || "",
    lastName: applicant?.lastName || "",
    guardianName: applicant?.guardianName || "",
    dateOfBirth: applicant?.dateOfBirth ? new Date(applicant.dateOfBirth).toISOString() : null,
    gender: typeof applicant.gender === 'object' ? applicant.gender.id : applicant.gender,
    mobileNumber: applicant?.mobileNumber || "",
    salary: parseFloat(applicant?.salary) || 0,
    associatedOrganization: applicant?.associatedOrganization || "",
    address: applicant?.address || "",
    idCardType: typeof applicant.idCardType === 'object' ? applicant.idCardType.id : applicant.idCardType,
    idCardNumber: applicant?.idCardNumber || "",
    productName: applicant?.productName || "",
    bankDetails: {
      id: applicant?.bankDetails?.id || 0,
      bankName: applicant?.bankDetails?.bankName || "",
      bankBranchDetails: applicant?.bankDetails?.bankBranchDetails || "",
      bankAccountNumber: applicant?.bankDetails?.bankAccountNumber || "",
      bankIfscCode: applicant?.bankDetails?.bankIfscCode || "",
      bankMicrCode: applicant?.bankDetails?.bankMicrCode || "",
    },
    dependents: (applicant?.dependents || []).map(dependent => ({
      id: dependent?.id || 0,
      firstName: dependent?.firstName || "",
      lastName: dependent?.lastName || "",
      relationship: dependent?.relationship || "",
      dateOfBirth: dependent?.dateOfBirth ? new Date(dependent.dateOfBirth).toISOString() : null,
      
    })),
  };
};

/**
 * Function to get dependent empty object
 * @returns 
 */
export const getDependentObject = () => {
  return {
    id: 0,
    firstName: "",
    lastName: "",
    relationship: "",
    dateOfBirth: null,
    contactNumber: "",
  };
};

export const getInitialRoleObject = () => {
  return {
    id: 0,
    name: "",
    description: "",
    permissions: [
      {
        id: 0,
        type: "api",
        name: "api",
        actions: {
          create: false,
          read: false,
          update: false,
          delete: false,
        },
      },
      {
        id: 1,
        type: "ui",
        name: "ui",
        actions: {
          create: false,
          read: false,
          update: false,
          delete: false,
        },
      },
      {
        id: 1,
        type: "dev",
        name: "ui",
        actions: {
          create: false,
          read: false,
          update: false,
          delete: false,
        },
      },
    ],
  };
};

export const getInitialOrganisationObject = () => {
  return {
    id: 0,
    name: "",
    description: "",
  };
};

export const getInitialPermissionObject = () => {
  return {
    id: 0,
    type: null,
    name: "",
    actions: {
      create: false,
      read: false,
      update: false,
      delete: false,
    },
  };
};
/**
 * Function to get empty product object
 * @returns
 */
export const getProduct = () => {
  return {
    productName: "",
    policyType: null,
    basePolicy: null,
    isSpouseCoverage: false,
    isHandicappedChildrenCoverage: false,
    isParentsCoverage: false,
    isInLawsCoverage: false,
    numberOfHandicappedChildren: 0,
    numberOfParents: 0,
    numberOfInLaws: 0,
    productDocument: null,
    isProductDocumentUpdated: false,
    premiumChart: [],
  };
};

/**
 * Function to get policy option object
 * @returns
 */
export const getPolicyOption = () => {
  return {
    productPremiumId: uuid(),
    sumInsured: 0,
    selfOnlyPremium: 0,
    selfSpousePremium: 0,
    spousePremium: 0,
    child1Premium: 0,
    child2Premium: 0,
    parent1Premium: 0,
    parent2Premium: 0,
    inLaw1Premium: 0,
    inLaw2Premium: 0,
  };
};

/**
 * Function to remove premim for all rows when option is de-selected
 * @param {Array} policyOptions
 * @param {String} type1
 * @param {string} type2
 * @returns
 */
export const removePremium = (policyType, policyOptions, type1, type2) => {
  policyOptions.forEach((option, index) => {
    if (policyType.toLowerCase().trim() === "basepolicy") {
      option[type1] = 0;
    }
    if (
      policyType.toLowerCase().trim() === "topuppolicy" &&
      option.isBasePolicy === false
    ) {
      option[type1] = 0;
    }

    if (type2) {
      option[type2] = 0;
    }
  });

  return policyOptions;
};

/**
 * Function to check if policy type is top up policy
 * @param {object} policyType
 * @returns
 */
export const isTopUpPolicy = (policyType) => {
  if (policyType === null) return true;
  return policyType["name"].toLowerCase().trim() === "topuppolicy";
};

export const isAgeBandPremiumPolicy = (policyType) => {
  if (policyType === null) return true;
  return policyType["name"].toLowerCase().trim() === "agebandpremium";
};

/**
 * Function to mark policy option as base policy option
 * @param {Array} premiumChart
 * @returns
 */
export const markBasePolicyOption = (premiumChart) => {
  premiumChart.forEach((option) => {
    option["isBasePolicy"] = true;
  });

  return premiumChart;
};

/**
 * Function to get top up policy option
 * @returns
 */
export const getTopUpPolicyOption = () => {
  return {
    productPremiumId: uuid(),
    sumInsured: 0,
    selfOnlyPremium: 0,
    selfSpousePremium: 0,
    spousePremium: 0,
    child1Premium: 0,
    child2Premium: 0,
    parent1Premium: 0,
    parent2Premium: 0,
    inLaw1Premium: 0,
    inLaw2Premium: 0,
    isBasePolicy: false,
  };
};

/**
 * Function to remove premium chart errors from error object
 * @param {object} errors
 * @returns
 */
export const removePremiumChartErrors = (errors) => {
  const keys = Object.keys(errors);
  let nextErrors = produce(errors, (draft) => {
    // Iterate over the keys
    keys.forEach((key) => {
      // Check if the key contains the letter 'p'
      if (key.includes("premiumChart")) {
        // Delete the property
        delete draft[key];
      }
    });
  });

  return nextErrors;
};

/**
 * Function to get index for adding top up option below base policy option
 * @param {Array} premiumChart
 * @param {number} currentIndex
 * @returns
 */
export const getIndexToAddTopupOption = (premiumChart, currentIndex) => {
  let indexToAddTopUp = 0;

  for (var i = currentIndex; i <= premiumChart.length; i++) {
    let index1 = i;
    let index2 = i + 1;

    if (i === premiumChart.length - 1) {
      indexToAddTopUp = i;
      break;
    }

    if (
      premiumChart[index1]["isBasePolicy"] === false &&
      premiumChart[index2]["isBasePolicy"] === true
    ) {
      indexToAddTopUp = i;
      break;
    }

    if (
      premiumChart[index1]["isBasePolicy"] === true &&
      premiumChart[index2]["isBasePolicy"] === true
    ) {
      indexToAddTopUp = i;
      break;
    }
  }

  return indexToAddTopUp;
};

const formatTopUpPremiumChartForSaving = (premiumChart) => {
  let newPremiumChart = structuredClone(premiumChart);

  let formattedPremiumChart = [];
  let formattedPremiumChartLength = 0;
  newPremiumChart.forEach((option, index) => {
    if (option.isBasePolicy) {
      var basePolicyOption = {};

      basePolicyOption["parentProductPremiumId"] = option.productPremiumId;
      basePolicyOption["topUpOptions"] = [];
      formattedPremiumChart.push(basePolicyOption);
      formattedPremiumChartLength++;
    }

    if (!option.isBasePolicy) {
      if (isUUID(option.productPremiumId)) {
        option["productPremiumId"] = 0;
      }

      formattedPremiumChart[formattedPremiumChartLength - 1][
        "topUpOptions"
      ].push(option);
    }
  });

  return formattedPremiumChart;
};
const formatOtherPremiumChartForSaving = (premiumChart) => {
  let formattedPremiumChart = [];
  let newPremiumChart = structuredClone(premiumChart);
  newPremiumChart.forEach((option) => {
    if (isUUID(option.productPremiumId)) {
      option.productPremiumId = 0;
    }

    formattedPremiumChart.push(option);
  });

  return formattedPremiumChart;
};

const formatBasePremiumChartForSaving = (premiumChart) => {
  let formattedPremiumChart = [];
  let newPremiumChart = structuredClone(premiumChart);
  newPremiumChart.forEach((option) => {
    if (isUUID(option.productPremiumId)) {
      option.productPremiumId = 0;
    }

    formattedPremiumChart.push(option);
  });

  return formattedPremiumChart;
};

/**
 * Function to get product post object
 * @param {object} product
 * @returns
 */
export const getProductPostObject = (product) => {
  return {
    productId: product?.productId || 0,
    productName: product?.productName,
    disclaimer: product?.disclaimer || "",
    providerName: product?.providerName || "",
    policyTypeId: product?.policyType?.id || 0,
    basePolicyId: product?.basePolicy?.id || 0,
    isSpouseCoverage: product.isSpouseCoverage,
    isHandicappedChildrenCoverage: product.isHandicappedChildrenCoverage,
    isParentsCoverage: product.isParentsCoverage,
    isInLawsCoverage: product.isInLawsCoverage,
    numberOfHandicappedChildren: product.numberOfHandicappedChildren,
    numberOfParents: product.numberOfParents,
    numberOfInLaws: product.numberOfInLaws,
    productDocument: product?.productDocument || null,
    isProductDocumentUpdated: product?.isProductDocumentUpdated || false,
    premiumChart:
      product.policyType?.name.toLowerCase().trim() === "basepolicy"
        ? formatBasePremiumChartForSaving(product.premiumChart)
        : product.policyType?.name.toLowerCase().trim() === "basepolicy"
        ? formatTopUpPremiumChartForSaving(product.premiumChart)
        : formatOtherPremiumChartForSaving(product.premiumChart),
  };
};

export const formatTopUpPolicy = (product) => {
  let newPremiumChart = [];
  product.premiumChart.forEach((premium) => {
    premium["isBasePolicy"] = true;
    newPremiumChart.push(premium);

    if (premium?.topUpOptions?.length) {
      premium.topUpOptions.forEach((topUp) => {
        topUp["isBasePolicy"] = false;
        newPremiumChart.push(topUp);
      });
    }
  });

  product.premiumChart = newPremiumChart;

  return product;
};

const getRoleIds = (roles) => {
  let roleIds = [];

  if (roles.length) {
    roles.forEach((role) => {
      roleIds.push(Number(role.id));
    });
  }
  return roleIds;
};

/**
 * Function to get post object for user
 * @param {object} user
 * @returns
 */
export const getUserPostObject = (user) => {
  return {
    userId: user?.userId || 0,
    userTypeId: user?.userType?.id || 0,
    roleIds: getRoleIds(user?.roles),
    firstName: user?.firstName,
    lastName: user?.lastName,
    empId: user?.empId,
    organisationId: user?.organisation?.id || 0,
    associationId: user?.association?.id || 0,
    dateOfBirth: user?.dateOfBirth,
    genderId: user?.gender?.id || 0,
    email: user?.email,
    address: user?.address,
    mobileNumber: user?.mobileNumber,
    countryCode: user?.countryCode || "",
    stateId: user?.state?.id || 0,
    pincode: user?.pincode,
    pensioneridtypeId: user?.pensioneridtype?.id || 0,
    uniqueidnumber: user?.uniqueidnumber,
    password: user.password,
  };
};

/**
 * Function to find filters in filters array
 * @param {array} filters
 * @param {number} id
 * @returns
 */
export const isFilterPresent = (filters, id) => {
  let index = filters.findIndex((filter) => filter.id == id);
  return index;
};

/**
 * Function to format products array. Add sum insured options for base, top u ppolicy
 * @param {array} products
 * @param {string} mode
 * @returns
 */
export const setSumInsuredOptions = (products, mode) => {
  products.forEach((product) => {
    let sumInsuredOptions = [];

    if (mode === "new") {
      product["isProductSelected"] = false;
      product["totalProductPremium"] = 0;
    }

    product.premiumChart.forEach((premium, premiumIndex) => {
      let sumInsured = {
        id: premium["productPremiumId"],
        index: premiumIndex,
        name: premium["sumInsured"],
      };

      sumInsuredOptions.push(sumInsured);

      if (mode === "new") {
        premium["isSpousePremiumSelected"] = false;
        premium["isChild1PremiumSelected"] = false;
        premium["isChild2PremiumSelected"] = false;
        premium["isParent1PremiumSelected"] = false;
        premium["isParent2PremiumSelected"] = false;
        premium["isInLaw1PremiumSelected"] = false;
        premium["isInLaw2PremiumSelected"] = false;
      }

      if (premium.topUpOptions?.length) {
        let topUpSumInsuredOptions = [];
        premium.topUpOptions.forEach((topUp, topUpIndex) => {
          let sumInsured = {
            id: topUp["productPremiumId"],
            name: topUp["sumInsured"],
            index: topUpIndex,
          };

          topUpSumInsuredOptions.push(sumInsured);

          if (mode === "new") {
            topUp["isTopUpSpousePremiumSelected"] = false;
          }
        });

        premium["topUpSumInsuredOptions"] = topUpSumInsuredOptions;

        //set selected top up sum insured option
        if (mode === "new") {
          if (
            product["selectedTopUpOption"] === undefined ||
            product["selectedTopUpOption"] === null
          ) {
            product["selectedTopUpOption"] = topUpSumInsuredOptions[0];
          }
        } else {
          if (topUpSumInsuredOptions.length) {
            if (product["selectedTopUpOption"]?.id !== 0) {
              let sumInsuredOption = topUpSumInsuredOptions.find((topUp) => {
                return topUp.id == product["selectedTopUpOption"]?.id;
              });

              if (sumInsuredOption) {
                product["selectedTopUpOption"] = sumInsuredOption;
                premium["isTopUpSelected"] = true;
              }
            } else {
              product["selectedTopUpOption"] = topUpSumInsuredOptions[0];
            }
          }
        }

        if (mode === "new") {
          premium["isTopUpSelected"] = false;
        }
      }
    });

    product["sumInsuredOptions"] = sumInsuredOptions;

    //set selected sum insured option
    if (mode === "new") {
      product["selectedSumInsured"] = sumInsuredOptions[0];
    } else {
      if (sumInsuredOptions.length) {
        let sumInsuredOption = sumInsuredOptions.find((sumInsured) => {
          return sumInsured.id == product["selectedSumInsured"]?.id;
        });
        product["selectedSumInsured"] = sumInsuredOption;
      }
    }
  });

  return products;
};

/**
 * Function to check whther top up policy is added
 * @param {array} premiumChart
 * @returns
 */
export const isTopUpPolicyAdded = (premiumChart) => {
  let isTopupAdded = false;

  premiumChart.forEach((premium) => {
    if (!premium["isBasePolicy"]) {
      isTopupAdded = true;

      return;
    }
  });

  return isTopupAdded;
};

/**
 * Function to get property name from checkbox selection in product premuim card for total product premium calculation
 * @param {string} name
 * @returns
 */
export const getPropertyName = (name) => {
  let propName = "";

  switch (name) {
    case "isSpousePremiumSelected":
      propName = "spousePremium";
      break;

    case "isChild1PremiumSelected":
      propName = "child1Premium";
      break;

    case "isChild2PremiumSelected":
      propName = "child2Premium";
      break;

    case "isParent1PremiumSelected":
      propName = "parent1Premium";
      break;

    case "isParent2PremiumSelected":
      propName = "parent2Premium";
      break;

    case "isInLaw1PremiumSelected":
      propName = "inLaw1Premium";
      break;

    case "isInLaw2PremiumSelected":
      propName = "inLaw2Premium";
      break;

    default:
      break;
  }

  return propName;
};

/**
 * Function to reset premium selection option if the product is checked/unchecked
 * @param {object} product
 * @param {boolean} isProductSelected
 * @returns
 */
export const resetSelectedPremiumOptions = (product, isProductSelected) => {
  product["totalProductPremium"] = 0;

  if (product.selectedSumInsured !== null) {
    let index = product.selectedSumInsured.index;

    product["premiumChart"][index]["isSelfPremiumSelected"] = isProductSelected
      ? true
      : false;
    product["premiumChart"][index]["isSelfSpousePremiumSelected"] = false;
    product["premiumChart"][index]["isSpousePremiumSelected"] = false;
    product["premiumChart"][index]["isChild1PremiumSelected"] = false;
    product["premiumChart"][index]["isChild2PremiumSelected"] = false;
    product["premiumChart"][index]["isParent1PremiumSelected"] = false;
    product["premiumChart"][index]["isParent2PremiumSelected"] = false;
    product["premiumChart"][index]["isInLaw1PremiumSelected"] = false;
    product["premiumChart"][index]["isInLaw2PremiumSelected"] = false;

    product["premiumChart"][index]["isTopUpSelected"] = false;

    //clear top up selection if any
    if (product.selectedTopUpOption && product.selectedTopUpOption !== null) {
      let topUpIndex = product.selectedTopUpOption?.index;
      product["premiumChart"][index]["topUpOptions"][topUpIndex][
        "isTopUpSpousePremiumSelected"
      ] = false;
      product["premiumChart"][index]["topUpOptions"][topUpIndex][
        "isTopUpSelfSpousePremiumSelected"
      ] = false;
      product["premiumChart"][index]["topUpOptions"][topUpIndex][
        "isTopUpSelfPremiumSelected"
      ] = false;
    }
  }

  return product;
};

/**
 * Function to calculate total product premium based on action
 * @param {number} premium
 * @param {string} action
 * @param {number} value
 * @returns
 */
export const calculateTotalPremium = (premium, action, value) => {
  switch (action) {
    case "add":
      premium = premium + value;
      break;

    case "subtract":
      premium = premium - value;
      break;

    default:
      break;
  }

  return premium;
};

/**
 * Function to calculate total premium for all products
 * @param {aray} products
 * @returns
 */
export const getTotalPremium = (products) => {
  let totalPremium = 0;

  products.forEach((product) => {
    totalPremium = totalPremium + Number(product.totalProductPremium);
  });
  return totalPremium;
};

export const getBeneficiaryFlags = (products) => {
  let flags = {};
  products.forEach((product) => {
    product.premiumChart.forEach((premiumChart) => {
      let {
        isSelfSpousePremiumSelected,
        isSpousePremiumSelected,
        isSelfSpouse2ChildrenPremiumSelected,
        isSelfSpouse1ChildrenPremiumSelected,
        isSelf2ChildrenPremiumSelected,
        isSelf1ChildrenPremiumSelected,
        isChild1PremiumSelected,
        isChild2PremiumSelected,
        isParent1PremiumSelected,
        isParent2PremiumSelected,
        isInLaw1PremiumSelected,
        isInLaw2PremiumSelected,
      } = premiumChart;

      if (isSelfSpousePremiumSelected) {
        flags["isSelfSpousePremiumSelected"] = true;
      }

      if (isSelfSpouse2ChildrenPremiumSelected) {
        flags["isSelfSpouse2ChildrenPremiumSelected"] = true;
      }

      if (isSelfSpouse1ChildrenPremiumSelected) {
        flags["isSelfSpouse1ChildrenPremiumSelected"] = true;
      }
      if (isSelf2ChildrenPremiumSelected) {
        flags["isSelf2ChildrenPremiumSelected"] = true;
      }

      if (isSelf1ChildrenPremiumSelected) {
        flags["isSelf1ChildrenPremiumSelected"] = true;
      }

      if (isSpousePremiumSelected) {
        flags["isSpousePremiumSelected"] = true;
      }

      if (isChild1PremiumSelected) {
        flags["isChild1PremiumSelected"] = true;
      }
      if (isChild2PremiumSelected) {
        flags["isChild2PremiumSelected"] = true;
      }

      if (isParent1PremiumSelected) {
        flags["isParent1PremiumSelected"] = true;
      }
      if (isParent2PremiumSelected) {
        flags["isParent2PremiumSelected"] = true;
      }
      if (isInLaw1PremiumSelected) {
        flags["isInLaw1PremiumSelected"] = true;
      }
      if (isInLaw2PremiumSelected) {
        flags["isInLaw2PremiumSelected"] = true;
      }

      premiumChart?.topUpOptions?.length &&
        premiumChart?.topUpOptions.forEach((topUp) => {
          if (topUp.isTopUpSpousePremiumSelected) {
            flags["isSpousePremiumSelected"] = true;
          }

          if (topUp.isTopUpSelfSpousePremiumSelected) {
            flags["isTopUpSelfSpousePremiumSelected"] = true;
          }
        });
    });
  });

  return flags;
};

export const getBeneficiaries = (premium) => {
  let beneficiaries = "";

  if (premium.isSelfPremiumSelected) {
    beneficiaries = beneficiaries.concat("Self");
  }

  if (premium.isSelfSpousePremiumSelected) {
    beneficiaries = beneficiaries.concat("Self, spouse");
  }

  //children
  if (premium.isChild1PremiumSelected && !premium.isChild2PremiumSelected) {
    beneficiaries = beneficiaries.concat(", 1 child");
  }

  if (premium.isChild1PremiumSelected && premium.isChild2PremiumSelected) {
    beneficiaries = beneficiaries.concat(", 2 children");
  }

  //parents
  if (premium.isParent1PremiumSelected && !premium.isParent2PremiumSelected) {
    beneficiaries = beneficiaries.concat(", 1 parent");
  }

  if (premium.isParent1PremiumSelected && premium.isParent2PremiumSelected) {
    beneficiaries = beneficiaries.concat(", 2 parents");
  }

  //in law
  if (premium.isInLaw1PremiumSelected && !premium.isInLaw2PremiumSelected) {
    beneficiaries = beneficiaries.concat(", 1 In Law");
  }

  if (premium.isInLaw1PremiumSelected && premium.isInLaw2PremiumSelected) {
    beneficiaries = beneficiaries.concat(", 2 In Laws");
  }

  return beneficiaries;
};

/**
 * Function to check whether at least one product is selected for purchase
 * @param {array} products
 * @returns
 */
export const isProductSelected = (products) => {
  let isSelected = false;

  products.forEach((product) => {
    if (product.isProductSelected) {
      isSelected = true;
      return;
    }
  });

  return isSelected;
};

/**
 * Function to get the post object for saving campaign
 * @param {object} campaign
 * @returns
 */
export const getCampaignPostObject = (campaign) => {

  // Create a clean object with only the required properties
  const postObject = {
    campaignId: campaign?.campaignId ? parseInt(campaign.campaignId, 10) : 0,
    campaignName: campaign?.campaignName || "",
    startDate: campaign?.startDate || "",
    endDate: campaign?.endDate || "",  
    productIds: campaign.productIds,
    associationIds: campaign?.associationIds,
  };
  
  // IMPORTANT: Do NOT stringify the object here
  // Return the object directly
  return postObject;
};
/**
 * Function to format association message templates
 * @param {array} templates
 * @returns
 */
export const getAssociationWiseTemapltes = (templates) => {
  let messageTemplates = {};
  templates.forEach((template) => {
    messageTemplates[template?.associationId] = template.template;
  });

  return messageTemplates;
};

/**
 * Function to get selected products
 * @param {array} products
 * @returns
 */
export const getSelectedProducts = (products) => {
  let selectedProducts = [];
  products.forEach((product) => {
    if (product?.isProductSelected) {
      selectedProducts.push(product);
    }
  });

  return selectedProducts;
};

export const getBeneficiariesPost = (beneficiaries, flags) => {
  let beneficieryPostObject = {};
  if (
    flags?.isSelfSpousePremiumSelected ||
    flags?.isSelfSpouse2ChildrenPremiumSelected ||
    flags?.isSelfSpouse1ChildrenPremiumSelected
  ) {
    beneficieryPostObject["spouse"] = beneficiaries["spouse"];
  }

  if (
    flags?.isTopUpSelfSpousePremiumSelected ||
    flags?.isSelfSpouse2ChildrenPremiumSelected ||
    flags?.isSelfSpouse1ChildrenPremiumSelected
  ) {
    beneficieryPostObject["spouse"] = beneficiaries["spouse"];
  }

  if (flags?.isSpousePremiumSelected) {
    beneficieryPostObject["spouse"] = beneficiaries["spouse"];
  }

  if (
    flags?.isChild1PremiumSelected ||
    flags?.isSelf2ChildrenPremiumSelected ||
    flags?.isSelf1ChildrenPremiumSelected ||
    flags?.isSelfSpouse2ChildrenPremiumSelected ||
    flags?.isSelfSpouse1ChildrenPremiumSelected
  ) {
    beneficieryPostObject["child1"] = beneficiaries["child1"];
  }
  if (
    flags?.isChild2PremiumSelected ||
    flags?.isSelf2ChildrenPremiumSelected ||
    flags?.isSelfSpouse2ChildrenPremiumSelected
  ) {
    beneficieryPostObject["child2"] = beneficiaries["child2"];
  }
  if (flags?.isParent1PremiumSelected) {
    beneficieryPostObject["parent1"] = beneficiaries["parent1"];
  }
  if (flags?.isParent2PremiumSelected) {
    beneficieryPostObject["parent2"] = beneficiaries["parent2"];
  }
  if (flags?.isInLaw1PremiumSelected) {
    beneficieryPostObject["inLaw1"] = beneficiaries["inLaw1"];
  }
  if (flags?.isInLaw2PremiumSelected) {
    beneficieryPostObject["inLaw2"] = beneficiaries["inLaw2"];
  }

  return beneficieryPostObject;
};

export const getBeneficiaryInitialObject = () => {
  return {
    spouse: { name: "", gender: null, dateOfBirth: "" },
    child1: {
      name: "",
      gender: null,
      dateOfBirth: "",
      disabilityCertificate: null,
    },
    child2: {
      name: "",
      gender: null,
      dateOfBirth: "",
      disabilityCertificate: null,
    },
    parent1: { name: "", gender: null, dateOfBirth: "" },
    parent2: { name: "", gender: null, dateOfBirth: "" },
    inLaw1: { name: "", gender: null, dateOfBirth: "" },
    inLaw2: { name: "", gender: null, dateOfBirth: "" },
    nominee: { name: "", gender: null, dateOfBirth: "" },
    errors: {},
  };
};

export const getPaymentsInitialObject = () => {
  return {
    paymentMode: null,
    isPaymentConfirmed: false,
    accountDetails: {
      accountNumber: "",
      bankName: "",
      ifscCode: "",
      branchName: "",
    },
    online: {
      amountPayable: 0,
      amountPaid: 0,
      transactionId: "abc23jk",
      paymentStatus: "success",
    },
    offline: {
      offlinePaymentMode: null,

      chequeDetails: {
        chequeNumber: "",
        amount: "",
        bankName: "",
        date: null,
        inFavourOf: null,
        chequePhoto: null,
        chequeDepositLocation: "",
      },
      neft: {
        bankName: "",
        branchName: "",
        accountNumber: "",
        accountName: "",
        ifscCode: "",
        transactionId: "",
        amount: "",
        date: null,
        neftPaymentReceipt: null,
      },
      upi: {
        transactionNumber: "",
        amount: "",
        date: null,
        upiPaymentReceipt: null,
      },
    },
    gateway: {
      transactionnumber: "",
    },
    errors: {},
  };
};

export const getPaymentPostObject = (
  paymentDetails,
  totalPayableAmount,
  offlinePaymentModes
) => {
  let offlinePaymentTypeId =
    offlinePaymentModes.find(
      (mode) =>
        mode?.name?.toLowerCase()?.trim() ===
        paymentDetails?.offlinePaymentMode?.toLowerCase()?.trim()
    )?.id || 0;
  
  return {
    paymentModeId: paymentDetails?.paymentMode?.id || 0,
    paymentTypeId: offlinePaymentTypeId,
    online: {
      amountPayable: totalPayableAmount,
      amountPaid: totalPayableAmount,
      transactionId: "abc23jk",
      paymentStatus: "success",
    },
    offline: {
      offlinePaymentModeId: offlinePaymentTypeId,
      chequeDetails: {
        chequeNumber: paymentDetails?.chequeDetails?.chequeNumber || "",
        bankName: paymentDetails?.chequeDetails?.bankName || "",
        amount: paymentDetails?.chequeDetails?.amount || 0,
        date: paymentDetails?.chequeDetails?.date || new Date().toISOString(),
        inFavourOfId: paymentDetails?.chequeDetails?.inFavourOf?.id || 0,
        chequeDepositLocation: paymentDetails?.chequeDetails?.chequeDepositLocation || "",
        micrcode: paymentDetails?.chequeDetails?.micrCode || "",
        ifsccode: paymentDetails?.chequeDetails?.ifscCode || ""
      },
      neft: {
        bankName: paymentDetails?.neft?.bankName || "",
        branchName: paymentDetails?.neft?.branchName || "",
        accountNumber: paymentDetails?.neft?.accountNumber || "",
        accountName: paymentDetails?.neft?.accountName || "",
        ifscCode: paymentDetails?.neft?.ifscCode || "",
        transactionId: paymentDetails?.neft?.transactionId || "",
        amount: paymentDetails?.neft?.amount || 0,
        date: paymentDetails?.neft?.date || new Date().toISOString()
      },
      upi: {
        transactionId: paymentDetails?.upi?.transactionId || "",
        amount: paymentDetails?.upi?.amount || 0,
        date: paymentDetails?.upi?.date || new Date().toISOString()
      },
    },
  };
};
/**
 * Function to create permissions pagewise object
 * @param {array} permissions
 * @returns
 */
export const formatPermissions = (permissions) => {
  let newPermissions = {};

  permissions.forEach((permission) => {
    newPermissions[permission.pageName] = {
      read: permission.read,
      create: permission.create,
      delete: permission.delete,
      update: permission.update,
    };
  });

  return newPermissions;
};

/**
 * Function to append payload to formData
 * @param {object} formData
 * @param {object} data
 * @param {string} parentKey
 */


export const appendFormData = (formData, data, parentKey = "") => {
  // Handle null or undefined values
  if (data === null || data === undefined) {
    if (parentKey) {
      formData.append(parentKey, "");
    }
    return;
  }

  // Handle File objects
  if (data instanceof File) {
    formData.append(parentKey, data);
    return;
  }

  // Handle Date objects
  if (data instanceof Date) {
    formData.append(parentKey, data.toISOString());
    return;
  }

  // Handle Arrays
  if (Array.isArray(data)) {
    // Special handling for empty arrays
    if (data.length === 0 && parentKey) {
      formData.append(`${parentKey}`, "");
    } else {
      // For ASP.NET MVC/Core binding, use indexed notation
      data.forEach((item, index) => {
        const arrayKey = parentKey ? `${parentKey}[${index}]` : `[${index}]`;
        appendFormData(formData, item, arrayKey);
      });
    }
    return;
  }

  // Handle Objects (excluding Files, Dates, Arrays)
  if (data && typeof data === "object") {
    const entries = Object.entries(data);
    if (entries.length === 0 && parentKey) {
      formData.append(`${parentKey}`, "");
    } else {
      entries.forEach(([key, value]) => {
        // For ASP.NET model binding format with dot notation for nested objects
        const newKey = parentKey ? `${parentKey}.${key}` : key;
        appendFormData(formData, value, newKey);
      });
    }
    return;
  }

  // Handle string "true"/"false" conversions - important for your data!
  if (data === "true" || data === "false") {
    formData.append(parentKey, data); // Keep as string - ASP.NET can parse these
    return;
  }

  // Handle primitive values (strings, numbers, booleans)
  if (parentKey) {
    if (typeof data === "boolean") {
      // Convert boolean to string lowercase "true"/"false" for .NET binding
      formData.append(parentKey, data ? "true" : "false");
    } else {
      // Convert to string for FormData compatibility
      formData.append(parentKey, String(data));
    }
  }
};
export const formatMonths = (months) => {
  let formattedMonths = {};
  months.forEach((month, index) => {
    formattedMonths[index] = month;
  });

  return formattedMonths;
};

export const formatYears = (years) => {
  let formattedYears = {};
  years.forEach((year) => {
    formattedYears[year.name] = year;
  });

  return formattedYears;
};

export const downloadFile = (url, docName) => {
  const link = document.createElement("a");
  link.href = url;
  link.download = `${docName}`;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  URL.revokeObjectURL(url);
};

export const initialRefundPaymentObject = () => {
  return {
    chequeDetails: {
      chequeNumber: "",
      chequeBankName: "",
      chequeAmount: "",
      chequePhoto: null,
      chequeDate: null,
      inFavourOf: null,
      isChequePhotoUpdated: false,
    },
    neft: {
      neftBankName: "",
      neftBranchName: "",
      neftAccountNumber: "",
      neftAccountName: "",
      neftIfscCode: "",
      neftTransactionId: "",
      neftAmount: "",
      neftDate: null,
      neftPaymentReceipt: null,
      isNeftPaymentReceiptUpdated: false,
    },
    upi: {
      upiTransactionId: "",
      upiAmount: "",
      upiDate: null,
      upiPaymentReceipt: null,
      isUpiPaymentReceiptUpdated: false,
    },
  };
};

export const updateRefundRequestObject = (
  refundRequest,
  refundPayment,
  activeTab
) => {
  if (activeTab === "cheque") {
    return {
      refundRequestId: refundRequest.refundRequestNumber,
      refundRequestNumber: refundRequest.refundRequestNumber,
      orderNumber: refundRequest.orderNumber,
      refundAmount: refundRequest.refundAmount,
      isAccepted: refundRequest.isAccepted,
      chequeDetails: {
        chequeNumber: refundPayment.chequeDetails.chequeNumber,
        bankName: refundPayment.chequeDetails.chequeBankName,
        amount: refundRequest.refundAmount,
        chequePhoto: refundPayment.chequeDetails.chequePhoto,
        date: refundPayment.chequeDetails.chequeDate,
        retireeName: refundPayment.chequeDetails.inFavourOf,
        isChequePhotoUpdated: refundPayment.chequeDetails.isChequePhotoUpdated,
        refundPaymentMode: 1,
      },
    };
  } else if (activeTab === "neft") {
    return {
      refundRequestId: refundRequest.refundRequestNumber,
      refundRequestNumber: refundRequest.refundRequestNumber,
      orderNumber: refundRequest.orderNumber,
      refundAmount: refundRequest.refundAmount,
      isAccepted: refundRequest.isAccepted,
      neft: {
        bankName: refundPayment.neft.neftBankName,
        branchName: refundPayment.neft.neftBranchName,
        accountNumber: refundPayment.neft.neftAccountNumber,
        accountName: refundPayment.neft.neftAccountName,
        ifscCode: refundPayment.neft.neftIfscCode,
        transactionId: refundPayment.neft.neftTransactionId,
        amount: refundRequest.refundAmount,
        date: refundPayment.neft.neftDate,
        neftPaymentReceipt: refundPayment.neft.neftPaymentReceipt,
        isNeftPaymentReceiptUpdated:
          refundPayment.neft.isNeftPaymentReceiptUpdated,
        refundPaymentMode: 2,
      },
    };
  } else if (activeTab === "upi") {
    return {
      refundRequestId: refundRequest.refundRequestNumber,
      refundRequestNumber: refundRequest.refundRequestNumber,
      orderNumber: refundRequest.orderNumber,
      refundAmount: refundRequest.refundAmount,
      isAccepted: refundRequest.isAccepted,
      upi: {
        transactionNumber: refundPayment.upi.upiTransactionId,
        amount: refundRequest.refundAmount,
        date: refundPayment.upi.upiDate,
        upiPaymentReceipt: refundPayment.upi.upiPaymentReceipt,
        isUpiPaymentReceiptUpdated:
          refundPayment.upi.isUpiPaymentReceiptUpdated,
        refundPaymentMode: 3,
      },
    };
  }
};

export const getAssociationWisePaymentDetailsPostObject = (association) => {
  return {
    associationId: association?.association?.id || 0,
    paymentStatusId: association?.paymentStatus?.id || 0,
    startDate:
      getCurrentTimeZoneIsoStringFromDate(
        new Date(association?.startDate)
      ).toISOString() || "",
    endDate:
      getCurrentTimeZoneIsoStringFromDate(
        new Date(association?.endDate)
      ).toISOString() || "",
  };
};

export const getOfflinePaymentPostObject = (offlinePayment) => {
  return {
    associationId: offlinePayment?.association?.id || 0,
    startDate:
      getCurrentTimeZoneIsoStringFromDate(
        new Date(offlinePayment?.startDate)
      ).toISOString() || "",
    endDate:
      getCurrentTimeZoneIsoStringFromDate(
        new Date(offlinePayment?.endDate)
      ).toISOString() || "",
  };
};

export const getRefundReportsPostObject = (refundReport) => {
  return {
    associationId: refundReport?.association?.id || 0,
    organisationId: refundReport?.organisation?.id || 0,
    startDate:
      getCurrentTimeZoneIsoStringFromDate(
        new Date(refundReport?.startDate)
      ).toISOString() || "",
    endDate:
      getCurrentTimeZoneIsoStringFromDate(
        new Date(refundReport?.endDate)
      ).toISOString() || "",
  };
};

export const getBouncedPaymentsPostObject = (bouncedPayment) => {
  return {
    associationId: bouncedPayment?.association?.id || 0,
    startDate:
      getCurrentTimeZoneIsoStringFromDate(
        new Date(bouncedPayment?.startDate)
      ).toISOString() || "",
    endDate:
      getCurrentTimeZoneIsoStringFromDate(
        new Date(bouncedPayment?.endDate)
      ).toISOString() || "",
  };
};

export const getInsuranceCompanyReportPostObject = (payment) => {
  return {
    associationId: payment?.association?.id || 0,
    startDate:
      getCurrentTimeZoneIsoStringFromDate(
        new Date(payment?.startDate)
      ).toISOString() || "",
    endDate:
      getCurrentTimeZoneIsoStringFromDate(
        new Date(payment?.endDate)
      ).toISOString() || "",
  };
};

export const getCorrectionReportPostObject = (correctionReport) => {
  return {
    associationId: correctionReport?.association?.id || 0,
    startDate:
      getCurrentTimeZoneIsoStringFromDate(
        new Date(correctionReport?.startDate)
      ).toISOString() || "",
    endDate:
      getCurrentTimeZoneIsoStringFromDate(
        new Date(correctionReport?.endDate)
      ).toISOString() || "",
  };
};

export const getExtractPensionersAndPaymentsPostObject = (report) => {
  var statusId = report?.paymentStatus?.id;
  if (statusId === undefined) statusId = 99;
  return {
    paymentTypeId: report?.paymentType?.id || 0,
    paymentStatusId: statusId,
    campaignId: report?.campaignId?.id || 0,
  };
};
export const getCompletedFormsPostObject = (completedForms) => {
  return {
    associationId: completedForms?.association?.id || 0,
    reportDate: completedForms?.date || "",
  };
};
export const getReconciledOnlinePaymentsPostObject = (onlinePayment) => {
  return {
    associationId: onlinePayment?.association?.id || 0,
    startDate:
      getCurrentTimeZoneIsoStringFromDate(
        new Date(onlinePayment?.startDate)
      ).toISOString() || "",
    endDate:
      getCurrentTimeZoneIsoStringFromDate(
        new Date(onlinePayment?.endDate)
      ).toISOString() || "",
  };
};
export const getCurrentTimeZoneIsoStringFromDate = (date) => {
  let newDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
  return newDate;
};

/**
 * Function to format payment receipt data
 * @param {array} payments
 * @returns
 */
export const formatReceiptData = (payments) => {
  payments.forEach((payment) => {
    payment["paymentDate"] = format(payment.paymentDate, "PPP");
    payment["amountPaid"] = payment.amountPaid.toString();
    payment["paymentAcceptedDate"] = format(payment.paymentAcceptedDate, "PPP");
  });

  return payments;
};

function createHeaders(keys) {
  var result = [];
  for (var i = 0; i < keys.length; i += 1) {
    result.push({
      id: keys[i],
      name: keys[i],
      prompt: keys[i],
      width: 65,
      align: "center",
      padding: 0,
    });
  }
  return result;
}

/**
 * Function to create payment receipt document
 * @param {object} doc
 * @param {array} data
 * @returns
 */
export const getPaymentReceiptDoc = (doc, data) => {
  var headers = createHeaders([
    "paymentDate",
    "amountPaid",
    "paymentMode",
    "paymentAcceptedDate",
  ]);

  let receiptData = formatReceiptData(data.paymentReceiptDetails);

  doc.text(`Name : ${data.name} `, 20, 20);
  doc.text(`Order Number : ${data.orderNumber} `, 20, 30);
  doc.text(`Total Premium : ${data.totalPremium.toString()} `, 20, 40);
  doc.text(`Amount Paid : ${data.amountPaid.toString()} `, 20, 50);

  doc.table(20, 60, receiptData, headers, {
    autoSize: true,
  });

  return doc;
};

export const getChildPremium = (products) => {
  let childPremium = 0;

  products.forEach((product) => {
    if (product.isProductSelected) {
      let selectedPremiumIndex = product.selectedSumInsured?.index;

      if (
        product["premiumChart"][selectedPremiumIndex]["isChild1PremiumSelected"]
      ) {
        childPremium =
          childPremium +
          Number(
            product["premiumChart"][selectedPremiumIndex]["child1Premium"]
          );
      }

      if (
        product["premiumChart"][selectedPremiumIndex]["isChild2PremiumSelected"]
      ) {
        childPremium =
          childPremium +
          Number(
            product["premiumChart"][selectedPremiumIndex]["child2Premium"]
          );
      }
    }
  });
  return childPremium;
};
