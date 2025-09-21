"use client";

import React, {
  useState,
  useRef,
  useEffect,
  useCallback,
  useMemo,
} from "react";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";
import { debounce } from "lodash";
import { CheckCircle } from "lucide-react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  DialogClose,
  DialogFooter,
} from "@/components/ui/dialog";
import { Combobox } from "@/components/ui/comboBox.jsx";
import RInput from "@/components/ui/rInput";

import { useToast } from "@/components/ui/use-toast.js";
import {
  getAssociations,
  getOrganisations,
  getUserTypes,
  register,
  getCampignAssociations,
} from "@/services/user";
import {
  verifysignupOTP,
  sendverificationcodesignup,
} from "@/services/authenticate";
import countryCodes from "country-codes-list";
import { Loader2 } from "lucide-react";
import { Link, useNavigate } from "react-router-dom";
import { ValidationError } from "yup";
import { signUpSchema } from "../validations";
import {
  InputOTP,
  InputOTPGroup,
  InputOTPSlot,
} from "@/components/ui/input-otp";
import { userStore } from "@/lib/store";

// Optimized memoized components with proper comparison functions
const MemoizedInput = React.memo(RInput, (prev, next) => {
  return (
    prev.value === next.value &&
    JSON.stringify(prev.error) === JSON.stringify(next.error) &&
    prev.disabled === next.disabled
  );
});

const MemoizedCombobox = React.memo(Combobox, (prev, next) => {
  return (
    prev.value?.id === next.value?.id &&
    prev.options?.length === next.options?.length
  );
});

// Custom hook for form validation
const useFormValidation = (schema) => {
  const validateField = useCallback(
    (name, value) => {
      try {
        schema.validateSyncAt(name, { [name]: value });
        return [];
      } catch (e) {
        return e.errors;
      }
    },
    [schema]
  );

  return { validateField };
};

// Custom hook for countdown timer
const useCountdownTimer = (initialTime, isActive, onComplete) => {
  const [timeLeft, setTimeLeft] = useState(initialTime);

  useEffect(() => {
    let rafId;
    let startTime;

    const updateTimer = (timestamp) => {
      if (!startTime) startTime = timestamp;
      const elapsed = Math.floor((timestamp - startTime) / 1000);

      if (elapsed < initialTime) {
        setTimeLeft(initialTime - elapsed);
        rafId = requestAnimationFrame(updateTimer);
      } else {
        setTimeLeft(0);
        onComplete?.();
      }
    };

    if (isActive && timeLeft > 0) {
      rafId = requestAnimationFrame(updateTimer);
    }

    return () => {
      if (rafId) {
        cancelAnimationFrame(rafId);
      }
    };
  }, [isActive, initialTime, onComplete]);

  return timeLeft;
};

// Custom hook for API calls
const useAPI = () => {
  const { toast } = useToast();

  const handleError = useCallback(
    (error, message) => {
      console.error(error);
      toast({
        variant: "destructive",
        title: "Error",
        description: message,
      });
    },
    [toast]
  );

  return { handleError };
};

export default function SignUp() {
  const navigate = useNavigate();
  const { toast } = useToast();
  const { handleError } = useAPI();
  // Get campaign ID from URL
  const campaignId = new URLSearchParams(window.location.search).get(
    "campaignId"
  );
  const userType = new URLSearchParams(window.location.search).get("UserType");

  const SCHEMA = useMemo(() => signUpSchema(), []);
  const updateUser = userStore((state) => state.updateUser);
  // const { validateField } = useFormValidation(SCHEMA);

  const declarations = [
    "I hereby declare, on my behalf and on behalf of my spouse, that the above particulars given by me are true and complete in all respects to the best of my knowledge and that I am authorized to propose on behalf of my spouse.",
    "I understand that the information provided by me will form the basis of the insurance policy, and that the policy will come into force only after full receipt of the premium chargeable and the application is in order.",
    "I agree to the terms and conditions of the Policy and have understood the same.",
  ];
  // Split state for better performance
  const [uiState, setUIState] = useState({
    isLoading: false,
    isFetchingUserTypes: true,
    showOTP: false,
    agreedToDeclarations: false,
  });

  const [formData, setFormData] = useState({
    organisations: [],
    associations: [],
    userTypes: [],
  });

  const [errors, setErrors] = useState({});
  const [phoneError, setPhoneError] = useState("");
  const [verified, setVerified] = useState(false);

  const countrySelectOptions = useMemo(() => {
    const codes = countryCodes.customList(
      "countryNameEn",
      "+{countryCallingCode}"
    );
    return Object.entries(codes)
      .map(([country, code]) => ({
        id: `${country}-${code}`, // Create unique id combining country and code
        country,
        code,
        displayText: `${country} (${code})`, // Add display text for better UX
      }))
      .sort((a, b) => a.country.localeCompare(b.country));
  }, []);

  const [signUpData, setSignUpData] = useState({
    userType: null,
    firstName: "",
    lastName: "",
    email: "",
    countryCode:
      countrySelectOptions.find((option) => option.country === "India")?.code ||
      "+91",
    mobileNumber: "",
    organization: null,
    association: null,
    otp: "",
  });

  // Countdown timer
  const timeLeft = useCountdownTimer(
    30,
    uiState.showOTP && !uiState.canResend,
    () => {
      setUIState((prev) => ({ ...prev, canResend: true }));
    }
  );
  // Modify the validation function
  const validateField = useCallback(
    (name, value) => {
      try {
        SCHEMA.validateSyncAt(name, { [name]: value });
        setErrors((prev) => {
          const newErrors = { ...prev };
          delete newErrors[name]; // Remove the error for this field if validation passes
          return newErrors;
        });
        return true;
      } catch (e) {
        setErrors((prev) => ({
          ...prev,
          [name]: e.errors,
        }));
        return false;
      }
    },
    [SCHEMA]
  );

  // Optimized validation with debounce
  const debouncedValidation = useCallback(
    debounce((name, value) => {
      const fieldErrors = validateField(name, value);
      setErrors((prev) => ({
        ...prev,
        [name]: fieldErrors,
      }));
    }, 500),
    [validateField]
  );

  // Fetch organizations and user types
  const fetchOrgAndUserTypes = async () => {
    try {
      const [orgResponse, userTypesResponse] = await Promise.all([
        getOrganisations(),
        getUserTypes(),
      ]);

      if (
        orgResponse.status !== "success" ||
        userTypesResponse.status !== "success"
      ) {
        throw new Error("Failed to fetch organization or user type data");
      }

      return {
        organisations: orgResponse.data,
        userTypes: userTypesResponse.data,
      };
    } catch (error) {
      handleError(error, "Failed to fetch organization or user type data");
      throw error;
    }
  };

  // Fetch campaign associations
  const fetchCampaignAssociations = async (campaignId) => {
    if (!campaignId || isNaN(campaignId) || campaignId <= 0) return null;

    try {
      const response = await getCampignAssociations(campaignId);
      if (response.status !== "success") {
        throw new Error("Failed to fetch campaign associations");
      }
      return response.data;
    } catch (error) {
      handleError(error, "Failed to fetch campaign associations");
      throw error;
    }
  };

  // Set default user type (pensioner)
  const setDefaultUserType = (userTypes) => {
    const pensioner = userTypes.find(
      (type) => type?.name?.toLowerCase()?.trim() === "pensioner"
    );
    if (pensioner) {
      setSignUpData((prev) => ({ ...prev, userType: pensioner }));
    }
  };

  // Earlier code remains the same...

  // Modify the useEffect hook to handle community user type
  useEffect(() => {
    const fetchInitialData = async () => {
      try {
        // Fetch organization and user type data
        const { organisations, userTypes } = await fetchOrgAndUserTypes();

        // Update form data with fetched data
        setFormData((prev) => ({
          ...prev,
          organisations,
          userTypes,
        }));

        // Handle user type based on URL parameter
        if (
          userType?.toLowerCase() === "community" ||
          userType?.toLowerCase() === "bpp"
        ) {
          const communityUserType = userTypes.find(
            (type) => type?.name?.toLowerCase()?.trim() === "community"
          );
          if (communityUserType) {
            setSignUpData((prev) => ({ ...prev, userType: communityUserType }));
          }

          // Fetch associations and set first organization and association as default when campaignId exists
          if (campaignId > 0) {
            const associations = await fetchCampaignAssociations(campaignId);
            if (associations && associations.length > 0) {
              setSignUpData((prev) => ({
                ...prev,
                organization: organisations[0],
                association: associations[0],
              }));
              setFormData((prev) => ({
                ...prev,
                associations,
              }));
            }
          } else if (organisations.length > 0) {
            // If no campaign ID, get associations for first organization
            const response = await getAssociations(organisations[0].id);
            if (response.status === "success" && response.data.length > 0) {
              setSignUpData((prev) => ({
                ...prev,
                organization: organisations[0],
                association: response.data[0],
              }));
              setFormData((prev) => ({
                ...prev,
                associations: response.data,
              }));
            }
          }
        } else {
          // Default to pensioner if not community
          const pensioner = userTypes.find(
            (type) => type?.name?.toLowerCase()?.trim() === "pensioner"
          );
          if (pensioner) {
            setSignUpData((prev) => ({ ...prev, userType: pensioner }));
          }
        }
      } catch (error) {
        handleError(error, "Failed to fetch initial data");
      } finally {
        setUIState((prev) => ({ ...prev, isFetchingUserTypes: false }));
      }
    };

    fetchInitialData();
  }, []); // Empty dependency array since campaignId and userType are from URL

  // useEffect(() => {
  //   const fetchInitialData = async () => {
  //     try {
  //       // Fetch organization and user type data
  //       const { organisations, userTypes } = await fetchOrgAndUserTypes();

  //       // Update form data with fetched data
  //       setFormData((prev) => ({
  //         ...prev,
  //         organisations,
  //         userTypes,
  //       }));

  //       // Set default user type
  //       setDefaultUserType(userTypes);

  //       // Fetch campaign associations if campaign ID exists
  //       if (campaignId) {
  //         const associations = await fetchCampaignAssociations(campaignId);
  //         if (associations) {
  //           setSignUpData((pre) => ({
  //             ...pre,
  //             associations: associations[0],
  //           }));
  //           setFormData((prev) => ({
  //             ...prev,
  //             associations,
  //           }));
  //         }
  //       }
  //     } catch (error) {
  //       handleError(error, "Failed to fetch initial data");
  //     } finally {
  //       setUIState((prev) => ({ ...prev, isFetchingUserTypes: false }));
  //     }
  //   };

  //   fetchInitialData();
  // }, []); // Empty dependency array since campaignId is from URL
  // // Optimized change handlers

  const handleChange = useCallback(
    (name) => (event) => {
      const value = event?.target?.value ?? event;

      setSignUpData((prev) => ({ ...prev, [name]: value }));

      if (["email", "mobileNumber", "firstName", "lastName"].includes(name)) {
        validateField(name, value);
      }

      if (name === "organization" && event?.id) {
        getAllAssociations(event.id);
      }
    },
    [validateField]
  );

  const handleMobileChange = useCallback((event) => {
    const value = event.target.value.replace(/\D/g, ""); //.slice(0, 16);
    // setPhoneError(
    //   value.length === 0
    //     ? ""
    //     : value.length >= 5 && value.length <= 16
    //     ? "Please enter valid mobile number"
    //     : ""
    // );
    setSignUpData((prev) => ({ ...prev, mobileNumber: value }));
  }, []);

  const getAllAssociations = async (orgId) => {
    try {
      if (campaignId > 0) {
        const response = await getCampignAssociations(campaignId);
        if (response.status === "success") {
          setFormData((prev) => ({ ...prev, associations: response.data }));
        }
      } else {
        const response = await getAssociations(orgId);
        if (response.status === "success") {
          setFormData((prev) => ({ ...prev, associations: response.data }));
        }
      }
    } catch (error) {
      handleError(error, "Failed to fetch associations");
    }
  };

  const handleRequestOTP = async () => {
    setUIState((prev) => ({ ...prev, isLoading: true }));

    try {
      const response = await sendverificationcodesignup({
        phoneNumber: signUpData.mobileNumber,
      });

      if (response.status === "success") {
        setUIState((prev) => ({
          ...prev,
          showOTP: true,
          canResend: false,
        }));
      }
      if (response.status === "conflict") {
        toast({
          variant: "destructive",
          title: "Mobile",
          description: "Mobile number already exists.",
        });
      }
    } catch (error) {
      handleError(error, "Failed to send OTP");
    } finally {
      setUIState((prev) => ({ ...prev, isLoading: false }));
    }
  };

  const handleVerifyOTP = async () => {
    if (signUpData.otp?.length !== 4) {
      toast({
        variant: "destructive",
        title: "Invalid OTP",
        description: "Please enter a valid 4-digit OTP.",
      });
      return;
    }

    setUIState((prev) => ({ ...prev, isLoading: true }));

    try {
      const response = await verifysignupOTP({
        phoneNumber: signUpData.mobileNumber,
        otp: signUpData.otp,
      });

      if (response.status === "success") {
        toast({ description: "OTP verified successfully." });
        setVerified(true);
        setUIState((prev) => ({ ...prev, showOTP: false }));
      }
    } catch (error) {
      handleError(error, "Failed to verify OTP");
    } finally {
      setUIState((prev) => ({ ...prev, isLoading: false }));
    }
  };

  const handleSubmit = useCallback(
    async (event) => {
      event.preventDefault();

      if (!uiState.agreedToDeclarations) {
        toast({
          variant: "destructive",
          title: "Agreement Required",
          description: "Please agree to the declarations before submitting.",
        });
        return;
      }

      setUIState((prev) => ({ ...prev, isLoading: true }));

      try {
        // Validate all fields before submitting
        await SCHEMA.validate(signUpData, { abortEarly: false });

        // Only proceed if there are no errors
        if (Object.keys(errors).length === 0) {
          const response = await register({
            firstName: signUpData.firstName,
            lastName: signUpData.lastName,
            email: signUpData.email,
            countryCode: signUpData.countryCode,
            mobileNumber: signUpData.mobileNumber,
            organisationId: signUpData.organization?.id || 0,
            associationId: signUpData.association?.id || 0,
            userTypeId: signUpData.userType?.id || 1,
          });

          if (response.status === "success") {
            toast({
              description: "Account created successfully!",
            });
            let user = response.data;
            let role = user?.userType?.name.toLowerCase().trim();

            if (role !== "pensioner" && role !== "community") {
              user["isProfileComplete"] = true;
            }
            updateUser(user);

            window.location.href = "/dashboard";
            //}
          } else if (response.status === "conflict") {
            toast({
              variant: "destructive",
              title: "Mobile",
              description: "Mobile number already exists.",
            });
          } else {
            toast({
              variant: "destructive",
              title: "Submit",
              description: "Something went wrong",
            });
          }
        }
      } catch (e) {
        if (e instanceof ValidationError) {
          const newErrors = e.inner.reduce(
            (acc, error) => ({
              ...acc,
              [error.path]: error.errors,
            }),
            {}
          );
          setErrors(newErrors);
        }
      } finally {
        setUIState((prev) => ({ ...prev, isLoading: false }));
      }
    },
    [signUpData, uiState.agreedToDeclarations, errors, navigate]
  );

  const isFormValid = useCallback(() => {
    try {
      // Validate all required fields
      const fieldsToValidate = {
        firstName: signUpData.firstName,
        lastName: signUpData.lastName,
        email: signUpData.email,
        mobileNumber: signUpData.mobileNumber,
      };

      SCHEMA.validateSync(fieldsToValidate, { abortEarly: false });
      return Object.keys(errors).length === 0;
    } catch (e) {
      return false;
    }
  }, [signUpData, errors, SCHEMA]);

  if (uiState.isFetchingUserTypes) {
    return (
      <div className="flex items-center justify-center h-screen">
        <Loader2 className="mr-2 h-12 w-12 animate-spin" />
      </div>
    );
  }

  return (
    <div className="flex items-center justify-center min-h-screen py-12">
      {uiState.isFetchingUserTypes ? (
        <div className="flex items-center justify-center h-screen">
          <Loader2 className="mr-2 h-12 w-12 animate-spin" />
        </div>
      ) : (
        <Card className="w-full max-w-md">
          <CardHeader>
            <CardTitle className="text-xl">Sign Up</CardTitle>
            <CardDescription>
              Enter your information to create an account
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              {/* Name Fields Section */}
              <div className="grid grid-cols-2 gap-4">
                <div className="grid gap-2">
                  <MemoizedInput
                    label="First Name"
                    placeholder="Enter First Name"
                    name="firstName"
                    isRequired={true}
                    value={signUpData.firstName}
                    error={errors?.firstName}
                    onChange={handleChange("firstName")}
                    id="first-name"
                    type="text"
                    disabled={uiState.isLoading}
                  />
                </div>
                <div className="grid gap-2">
                  <MemoizedInput
                    label="Last Name"
                    placeholder="Enter Last Name"
                    name="lastName"
                    isRequired={true}
                    value={signUpData.lastName}
                    error={errors?.lastName}
                    onChange={handleChange("lastName")}
                    id="last-name"
                    type="text"
                    disabled={uiState.isLoading}
                  />
                </div>
              </div>

              {/* Email Field */}
              <div className="grid gap-2">
                <MemoizedInput
                  label="Email"
                  placeholder="Enter Email"
                  name="email"
                  value={signUpData.email}
                  error={errors?.email}
                  onChange={handleChange("email")}
                  id="email"
                  type="text"
                  disabled={uiState.isLoading}
                />
              </div>

              {/* Mobile Number Section */}
              <div className="grid gap-2">
                <Label htmlFor="mobile">
                  Mobile Number
                  <span className="text-red-600 ml-1">*</span>
                </Label>
                <div className="flex gap-1">
                  <MemoizedInput
                    id="mobile"
                    type="tel"
                    name="mobileNumber"
                    onChange={handleMobileChange}
                    placeholder="Enter Mobile Number"
                    error={errors?.mobileNumber}
                    value={signUpData.mobileNumber}
                    disabled={uiState.isLoading}
                  />
                </div>

                {phoneError && (
                  <div className="text-red-600 text-xs">{phoneError}</div>
                )}
              </div>

              {/* OTP Section */}
              {!verified ? ( // Check if OTP is not verified
                <div className="grid gap-2">
                  {!uiState.showOTP ? (
                    <Button
                      type="button"
                      onClick={handleRequestOTP}
                      className="w-full mt-4"
                      disabled={uiState.isLoading}
                    >
                      {uiState.isLoading ? (
                        <span className="flex items-center justify-center">
                          <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                          Please wait
                        </span>
                      ) : (
                        "Send OTP"
                      )}
                    </Button>
                  ) : (
                    <div className="grid gap-2">
                      <Label htmlFor="otp" className="text-gray-700">
                        One-Time Password
                      </Label>
                      <InputOTP
                        maxLength={4}
                        value={signUpData.otp}
                        onChange={(value) =>
                          setSignUpData((prev) => ({ ...prev, otp: value }))
                        }
                        disabled={uiState.isLoading}
                      >
                        <InputOTPGroup>
                          {Array.from({ length: 4 }).map((_, index) => (
                            <InputOTPSlot
                              key={index}
                              index={index}
                              className="bg-white/50 border-2 border-gray-200 focus:border-[#8B0000]"
                            />
                          ))}
                        </InputOTPGroup>
                      </InputOTP>

                      <CardDescription className="text-red-500 text-xs">
                        In case you do not get the OTP on your mobile, please
                        contact your operator or admin.
                      </CardDescription>

                      <Button
                        type="button"
                        onClick={handleVerifyOTP}
                        className="w-full bg-[#8B0000] hover:bg-[#660000] text-white"
                        disabled={
                          uiState.isLoading || signUpData.otp?.length !== 4
                        }
                      >
                        {uiState.isLoading ? (
                          <span className="flex items-center justify-center">
                            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                            Verifying
                          </span>
                        ) : (
                          "Verify OTP"
                        )}
                      </Button>

                      <Button
                        type="button"
                        onClick={handleRequestOTP}
                        variant="outline"
                        className="w-full mt-2 border-[#8B0000] text-[#8B0000] hover:bg-[#8B0000] hover:text-white"
                        disabled={!timeLeft === 0}
                      >
                        {timeLeft > 0
                          ? `Resend OTP in ${timeLeft}s`
                          : "Resend OTP"}
                      </Button>
                    </div>
                  )}
                </div>
              ) : (
                // Show verified status
                <div className="flex items-center mt-2 text-green-600">
                  <CheckCircle className="mr-2 h-5 w-5" />
                  <span>Mobile number verified successfully</span>
                </div>
              )}

              {/* Organization and Association Section - Only show after verification */}
              {verified && (
                <div className="space-y-4">
                  {userType?.toLowerCase() !== "community" &&
                    userType?.toLowerCase() !== "bpp" && (
                      <div>
                        <div className="grid gap-2">
                          <Label htmlFor="organization">
                            Organization Name
                          </Label>
                          <MemoizedCombobox
                            options={formData.organisations}
                            onChange={handleChange("organization")}
                            valueProperty="id"
                            labelProperty="name"
                            value={signUpData.organization}
                            disabled={uiState.isLoading}
                          />
                        </div>

                        <div className="grid gap-2">
                          <Label htmlFor="association">
                            Association
                            <span className="text-red-600 ml-1">*</span>
                          </Label>
                          <MemoizedCombobox
                            options={formData.associations}
                            onChange={handleChange("association")}
                            valueProperty="id"
                            labelProperty="name"
                            value={signUpData.association}
                            error={errors?.association}
                            disabled={uiState.isLoading}
                          />
                        </div>
                      </div>
                    )}
                  {/* Declarations Section */}
                  <div className="flex items-start space-x-2">
                    <Dialog>
                      <DialogTrigger asChild>
                        <div className="flex items-center space-x-2">
                          <Checkbox
                            id="declarations"
                            checked={uiState.agreedToDeclarations}
                            onCheckedChange={(checked) =>
                              setUIState((prev) => ({
                                ...prev,
                                agreedToDeclarations: checked,
                              }))
                            }
                            disabled={uiState.isLoading}
                          />
                          <Label
                            htmlFor="declarations"
                            className="text-sm font-medium leading-none cursor-pointer"
                          >
                            I agree to the declarations
                          </Label>
                        </div>
                      </DialogTrigger>
                      <DialogContent className="sm:max-w-[425px]">
                        <DialogHeader>
                          <DialogTitle>Declarations</DialogTitle>
                          <DialogDescription>
                            Please read the following declarations carefully:
                          </DialogDescription>
                        </DialogHeader>
                        <div className="space-y-4">
                          {declarations.map((declaration, index) => (
                            <p key={index} className="text-sm">
                              {declaration}
                            </p>
                          ))}
                        </div>
                        <DialogFooter className="mt-6">
                          <DialogClose asChild>
                            <Button type="button">Accept</Button>
                          </DialogClose>
                        </DialogFooter>
                      </DialogContent>
                    </Dialog>
                  </div>
                </div>
              )}

              {/* Submit Button */}
              {/* <Button
                type="submit"
                className="w-full bg-[#8B0000] hover:bg-[#660000] text-white"
                disabled={
                  uiState.isLoading ||
                  !uiState.agreedToDeclarations ||
                  !verified ||
                  Object.keys(errors).length > 0
                }
              >
                {uiState.isLoading ? (
                  <span className="flex items-center justify-center">
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Processing
                  </span>
                ) : (
                  "Sign Up"
                )}
              </Button> */}
            </form>

            {/* Login Link */}
            <div className="mt-4 text-center text-sm">
              Already have an account?{" "}
              <Link
                to="/login"
                className="text-[#8B0000] hover:underline font-medium"
              >
                Sign in
              </Link>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
