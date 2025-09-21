import { create } from "zustand";
import { createJSONStorage, persist } from "zustand/middleware";

// user store
export const userStore = create(
  persist(
    (set, get) => ({
      user: null,
      updateUser: (newUser) => set(() => ({ user: newUser })),
      removeUser: () => set(() => ({ user: null })),
      markUserProfileComplete: () =>
        set((state) => ({ user: { ...state.user, isProfileComplete: true } })),
      
      markUserPolicy: () =>
        set((state) => ({ user: { ...state.user, isPolicy: true } })),
  
    }),
     
    {
      name: "user",
      storage: createJSONStorage(() => localStorage),
    }
  )
);

export const usePermissionStore = create((set) => ({
  permissions: null,
  setPermissions: (newPermissions) =>
    set(() => ({
      permissions: newPermissions,
    })),
}));

// campaign store
export const useCampaignsStore = create((set) => ({
  // campaign list
  campaigns: null,
  addCampaigns: (newCampaigns) =>
    set(() => ({
      campaigns: newCampaigns,
    })),
  paginationModel: {
    pageNumber: 1,
    recordsPerPage: 50,
  },
  setPaginationModel: (newModel) =>
    set(() => ({
      paginationModel: newModel,
    })),
  searchTerm: "",
  setSearchTerm: (newTerm) =>
    set(() => ({
      searchTerm: newTerm,
    })),
  filters: {
    isCampaignOpen: false,
  },
  campaignIndex: 0,
  setCampaignIndex: (newIndex) => set(() => ({ campaignIndex: newIndex })),

  //campaign form
  currentCampaignId: 0,
  setCurrentCampaignId: (newCampaignId) =>
    set(() => ({ currentCampaignId: newCampaignId })),
  mode: "",
  setMode: (newMode) => set(() => ({ mode: newMode })),
  campaign: {
    campaignId: 0,
    campaignName: "",
    startDate: null,
    endDate: null,
    products: [],
    associations: [],
  },
  initializeCampaign: () =>
    set(() => ({
      campaign: {
        campaignId: 0,
        campaignName: "",
        startDate: null,
        endDate: null,
        products: [],
        associations: [],
      },
    })),
  updateCampaign: (newCampaign) => set(() => ({ campaign: newCampaign })),
  products: [],
  addProducts: (newProducts) =>
    set(() => ({
      products: newProducts,
    })),
  associations: [],
  addAssociations: (newAssociations) =>
    set(() => ({
      associations: newAssociations,
    })),
}));

// policy purchase store
export const usePolicyStore = create((set, get) => ({
  mode: "new",
  setMode: (newMode) =>
    set(() => ({
      mode: newMode,
    })),
  step: 0,
  setStep: (newStep) => set(() => ({ step: newStep })),
  userDetails: {
    totalPremium: 0,
    amountPaid: 0,
    paymentStatus: "pending",
    policyId: 0,
    userId: 0,
    user: {},
    products: [],
    beneficiaries: {
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
    },
    paymentDetails: null,
  },
  setUserDetails: (newUserDetails) =>
    set(() => ({ userDetails: newUserDetails })),
  updateBeneficiaries: (newBeneficiaries) =>
    set((state) => ({
      userDetails: { ...state.userDetails, beneficiaries: newBeneficiaries },
    })),
  updatePaymentDetails: (newPaymentDetails) =>
    set((state) => ({
      userDetails: { ...state.userDetails, paymentDetails: newPaymentDetails },
    })),

  updatePolicyId: (newPolicyId) =>
 
    set((state) => ({
      userDetails: {
        ...state.userDetails,
        policyId: newPolicyId,
      },
    })),
    

  updateBeneficiaryId: (newBeneficiaryId) =>
    set((state) => ({
      userDetails: {
        ...state.userDetails,
        beneficiaryId: newBeneficiaryId,
      },
    })),
}));

// location store
export const useLocationStore = create((set) => ({
  locations: [],
  addLocation: (newLocation) =>
    set((state) => ({
      locations: [...state.locations, newLocation],
    })),
  clearLocations: () =>
    set(() => ({
      locations: [],
    })),
}));

export const useOfflinePaymentsStore = create((set) => ({
  // offline payments  list
  offlinePayments: null,
  addOfflinePayments: (newPayments) =>
    set(() => ({
      offlinePayments: newPayments,
    })),
  paginationModel: {
    pageNumber: 1,
    recordsPerPage: 50,
  },
  setPaginationModel: (newModel) =>
    set(() => ({
      paginationModel: newModel,
    })),
  searchTerm: "",
  setSearchTerm: (newTerm) =>
    set(() => ({
      searchTerm: newTerm,
    })),

  //payment form
  currentPaymentId: 0,
  setCurrentPaymentId: (newPaymentId) =>
    set(() => ({ currentPaymentId: newPaymentId })),

  currentPaymentStatus: "",
  setCurrentPaymentStatus: (newStatus) =>
    set(() => ({ currentPaymentStatus: newStatus })),
  mode: "",
  setMode: (newMode) => set(() => ({ mode: newMode })),
  payment: {
    paymentMode: { id: 1, name: "cheque" },
    chequeDetails: {
      chequeNumber: "",
      amount: 0,
      bankName: "",
      date: "",
      inFavourOf: "",
      chequePhoto: "",
      accountName: "",
      code: "",
      micrCode: "",
      ifscCode: "",
    },
    neft: {
      bankName: "",
      branchName: "",
      accountNumber: "",
      accountName: "",
      ifscCode: "",
      transactionId: "",
      amount: "",
      date: "",
    },
    upi: {
      transactionNumber: "",
      amount: "",
      date: "",
    },
  },
  setPayment: (newPayment) =>
    set(() => ({
      payment: newPayment,
    })),
}));

export const useDateStore = create((set) => ({
  months: null,
  years: null,
  setMonths: (newMonths) =>
    set(() => ({
      months: { ...newMonths },
    })),
  setYears: (newYears) =>
    set(() => ({
      years: { ...newYears },
    })),
}));
