import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import RSelect from "@/components/ui/RSelect";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
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
  getBeneficiaries,
  getChildPremium,
  getPaymentPostObject,
} from "@/lib/helperFunctions";
import {
  useLocationStore,
  usePermissionStore,
  usePolicyStore,
  userStore,
} from "@/lib/store";
import { getAssociation } from "@/services/association";
import {
  addProductPolicy,
  getAssociations,
  getOfflinePaymentModes,
  getPaymentModes,
  saveRefundRequest,
} from "@/services/customerProfile";
import { authenticateFunction } from "@/utils/authenticatePaymentGateway";
import { paymentSchema } from "@/validations";
import { format } from "date-fns";
import { produce } from "immer";
import { AlertCircle, CircleArrowLeft } from "lucide-react";
import { useEffect, useState } from "react";
import { ValidationError } from "yup";
import { processPayment } from "../services/paymentGateway";
import { useNavigate } from "react-router-dom";
import qrcode from "@/assets/qrcodefile.png";
const PAGE_NAME = "products";
const PAGE_NAME_pensionerpayment = "pensionerpayment";
function Payments() {
  const locations = useLocationStore((state) => state.locations);
  if (locations.length == 0) {
    window.location.href = "/productlist";
  }
  const search = window.location.search;
  const params = new URLSearchParams(search);
  const userId = params.get("userId");
  const campaignId = params.get("campaignId");
  const updatedPolicyId = params.get("policyId");
  const loggedInUser = userStore((state) => state.user);

  const navigate = useNavigate();

  const SCHEMA = paymentSchema();

  const permissions = usePermissionStore((state) => state.permissions);

  const userDetails = usePolicyStore((state) => state.userDetails);
  const mode = usePolicyStore((state) => state.mode);

  const updatePaymentDetails = usePolicyStore(
    (state) => state.updatePaymentDetails
  );
  const step = usePolicyStore((state) => state.step);
  const setStep = usePolicyStore((state) => state.setStep);

  const user = userDetails?.user || {};
  const updatePolicyId = usePolicyStore((state) => state.updatePolicyId);
  const isRetiree =
    user?.userType?.name?.toLowerCase().trim() === "pensioner" ||
    user?.userType?.name?.toLowerCase().trim() === "community";

  const setMode = usePolicyStore((state) => state.setMode);
  const [paymentModes, setPaymentModes] = useState([]);
  const [offlinePaymentModes, setOfflinePaymentModes] = useState([]);
  const [associations, setAssociations] = useState([]);
  const [activeTab, setActiveTab] = useState();
  const [isLoading, setIsLoading] = useState(false);
  const [association, setAssociation] = useState(null);

  useEffect(() => {
    (async () => {
      setStep(4);
      try {
        const [response1, response2, response3, response4] = await Promise.all([
          getAssociations(),
          getPaymentModes(),
          getOfflinePaymentModes(),
          getAssociation(userDetails?.user?.association?.id || 0),
        ]);

        setAssociations(response1.data || []);

        if (user?.userType?.name?.toLowerCase().trim() === "community") {
          // setPaymentModes(response2.data || []);

          // Using filter to create a new array without the item with matching id
          setPaymentModes(response2.data.filter((status) => status.id !== 1));
        } else {
          setPaymentModes(response2.data || []);
        }
        // Find the offline payment mode object
        const offlinePaymentMode = response2.data.find(
          (mode) => mode.name.toLowerCase().trim() === "offline"
        );

        setPaymentModes(response2.data || []);
        setOfflinePaymentModes(response3.data || []);
        console.log(response3.data);
        setAssociation(response4.data || null);

        let nextState = produce(userDetails, (draft) => {
          draft["paymentDetails"]["chequeDetails"] =
            draft["paymentDetails"]["offline"]["chequeDetails"];

          draft["paymentDetails"]["neft"] =
            draft["paymentDetails"]["offline"]["neft"];

          draft["paymentDetails"]["upi"] =
            draft["paymentDetails"]["offline"]["upi"];

          draft["paymentDetails"]["offlinePaymentMode"] =
            draft["paymentDetails"]["offline"]["offlinePaymentMode"];

          if (user?.userType?.name?.toLowerCase().trim() === "community") {
            draft["paymentDetails"]["paymentMode"] = response2.data[0];
          } else {
            draft["paymentDetails"]["paymentMode"] = response2.data[0];
          }

          //delete draft["paymentDetails"]["offline"];

          // Set the payment mode to offline by default
          //draft["paymentDetails"]["paymentMode"] =
          //offlinePaymentMode || response2.data[0];

          //set cheque, neft, upi amount
          draft["paymentDetails"]["offlinePaymentMode"] = "cheque";
          draft["paymentDetails"]["chequeDetails"]["amount"] =
            parseFloat(draft["totalPremium"]) -
            parseFloat(draft["totalPaidPremium"]);
          draft["paymentDetails"]["chequeDetails"]["date"] =
            new Date().toISOString();
          draft["paymentDetails"]["neft"]["amount"] =
            parseFloat(draft["totalPremium"]) -
            parseFloat(draft["totalPaidPremium"]);
          draft["paymentDetails"]["neft"]["date"] = new Date().toISOString();
          draft["paymentDetails"]["upi"]["amount"] =
            parseFloat(draft["totalPremium"]) -
            parseFloat(draft["totalPaidPremium"]);
          draft["paymentDetails"]["upi"]["date"] = new Date().toISOString();

          //set account details
          let bankDetails = response4?.data?.bank;

          draft["paymentDetails"]["accountDetails"]["accountNumber"] =
            bankDetails?.accountNumber;
          draft["paymentDetails"]["accountDetails"]["bankName"] =
            bankDetails?.bankName;
          draft["paymentDetails"]["accountDetails"]["branchName"] =
            bankDetails?.branchName;
          draft["paymentDetails"]["accountDetails"]["ifscCode"] =
            bankDetails?.ifscCode;

          // set user association

          let association = response1.data.find((association) => {
            return association?.id == userDetails?.user?.association?.id;
          });
          draft["paymentDetails"]["chequeDetails"]["inFavourOf"] = association;
          //set cheque accountName and code
          draft["paymentDetails"]["chequeDetails"]["accountName"] =
            bankDetails?.accountName || "";
          draft["paymentDetails"]["chequeDetails"]["code"] =
            response4?.data?.associationCode || "";
        });

        updatePaymentDetails(nextState.paymentDetails);
        setActiveTab("cheque");
      } catch (err) {
        // show error
      }
    })();
  }, []);

  const handleChangeTab = (tabName) => {
    setActiveTab(tabName);
    let nextState = produce(userDetails, (draft) => {
      draft["paymentDetails"]["offlinePaymentMode"] = tabName;
      draft["paymentDetails"]["errors"] = {};
    });

    updatePaymentDetails(nextState.paymentDetails);
  };

  const handleConfirmPayment = (event) => {
    let nextState = produce(userDetails, (draft) => {
      draft["paymentDetails"]["isPaymentConfirmed"] = event;
    });
    updatePaymentDetails(nextState.paymentDetails);
  };

  const handleChange = (name, paymentType) => (event) => {
    let path = name;
    let nextErrors = { ...userDetails?.paymentDetails?.errors };
    let nextState = produce(userDetails, (draft) => {
      switch (name) {
        case "paymentMode":
          draft["paymentDetails"][name] = event;
          break;

        //cheque
        case "chequeNumber":
          draft["paymentDetails"]["chequeDetails"][name] = event.target.value;
          break;

        case "chequeBankName":
          path = "bankName";
          draft["paymentDetails"]["chequeDetails"]["bankName"] =
            event.target.value;
          break;

        case "chequeAmount":
          path = "amount";
          draft["paymentDetails"]["chequeDetails"]["amount"] =
            event.target.value;
          break;

        case "chequePhoto":
          draft["paymentDetails"]["chequeDetails"][name] = event;
          draft["paymentDetails"]["chequeDetails"][
            "isChequePhotoUpdated"
          ] = true;
          break;

        case "inFavourOf":
          draft["paymentDetails"]["chequeDetails"][name] = event;
          break;

        case "chequeDepositLocation":
          draft["paymentDetails"]["chequeDetails"][name] = event.target.value;
          break;

        case "chequeDate":
          path = "date";
          // eslint-disable-next-line no-case-declarations
          draft["paymentDetails"]["chequeDetails"]["date"] = event;
          break;
        case "micrCode":
          draft["paymentDetails"]["chequeDetails"][name] = event.target.value;
          break;
        case "ifscCode":
          draft["paymentDetails"]["chequeDetails"][name] = event.target.value;
          break;
        //neft
        case "neftBankName":
          path = "bankName";
          draft["paymentDetails"]["neft"]["bankName"] = event.target.value;
          break;

        case "neftBranchName":
          path = "branchName";
          draft["paymentDetails"]["neft"]["branchName"] = event.target.value;

          break;

        case "neftAccountNumber":
          path = "accountNumber";
          draft["paymentDetails"]["neft"]["accountNumber"] = event.target.value;
          break;

        case "neftAccountName":
          path = "accountName";
          draft["paymentDetails"]["neft"]["accountName"] = event.target.value;

          break;

        case "neftIfscCode":
          path = "ifscCode";
          draft["paymentDetails"]["neft"]["ifscCode"] = event.target.value;
          break;

        case "neftTransactionId":
          path = "transactionId";
          draft["paymentDetails"]["neft"]["transactionId"] = event.target.value;
          break;

        case "neftAmount":
          path = "amount";
          draft["paymentDetails"]["neft"]["amount"] = event.target.value;
          break;

        case "neftDate":
          path = "date";
          draft["paymentDetails"]["neft"]["date"] = event;
          break;

        case "neftPaymentReceipt":
          draft["paymentDetails"]["neft"][name] = event;
          draft["paymentDetails"]["neft"]["isNeftPaymentReceiptUpdated"] = true;
          break;

        //upi
        case "upiTransactioId":
          path = "transactionId";
          draft["paymentDetails"]["upi"]["transactionId"] = event.target.value;
          break;

        case "upiAmount":
          path = "amount";
          draft["paymentDetails"]["upi"]["amount"] = event.target.value;
          break;

        case "upiDate":
          path = "date";
          draft["paymentDetails"]["upi"]["date"] = event;
          break;

        case "upiPaymentReceipt":
          draft["paymentDetails"]["upi"][name] = event;
          draft["paymentDetails"]["upi"]["isUpiPaymentReceiptUpdated"] = true;
          break;

        default:
          break;
      }

      try {
        var schemaPath = `${paymentType}.${path}`;
        if (name === "paymentMode") {
          schemaPath = "paymentMode";
        }

        SCHEMA.validateSyncAt(schemaPath, draft.paymentDetails, {
          context: {
            offlinePaymentMode: userDetails?.paymentDetails?.offlinePaymentMode,
          },
        });

        nextErrors[schemaPath] = [];
      } catch (e) {
        nextErrors[schemaPath] = [...e.errors];
      }
    });

    nextState = produce(nextState, (draft) => {
      draft["paymentDetails"]["errors"] = nextErrors;
    });

    updatePaymentDetails(nextState.paymentDetails);
  };

  const handlePay = async () => {
    setIsLoading(true);

    let policyId = parseInt(userDetails?.policyId);

    if (policyId == 0) {
      policyId = updatedPolicyId;
    }

    if (policyId > 0) {
      if (
        userDetails?.paymentDetails?.paymentMode?.name?.toLowerCase().trim() ===
        "online"
      ) {
        // Online payment logic remains the same
        const postObject = {
          userId: userId ? userId : userDetails?.userId || 0,
          policyId: policyId,
          totalPremium: userDetails?.totalPremium || 0,
          amountPaid: userDetails?.amountPaid || 0,
          totalPaidPremium: userDetails?.totalPaidPremium || 0,
          online: {
            amountPayable: userDetails?.totalPremium,
            amountPaid: userDetails?.amountPaid || 0,
            transactionId: "",
            paymentStatus: userDetails?.paymentStatus,
          },
          isUpdate: true,
          paymentRequestBody: {
            mobileNumber: user?.mobileNumber || "",
            requestReference: policyId?.toString(),
            successReturnUrl: `${import.meta.env.VITE_KMD_URL}/PaymentSuccess`,
            cancelReturnUrl: `${import.meta.env.VITE_KMD_URL}/PaymentFailure`,
            email: user?.email || "",
            floatRequests: [
              {
                premiumAmount:
                  userDetails?.totalPremium - userDetails?.totalPaidPremium,
                customerCode: association?.associationCode || "",
              },
            ],
          },
        };

        const setPaymentSuccess = {
          userId: userId ? userId : userDetails.userId,
          policyId: userDetails.policyId,
          totalPremium: userDetails.totalPremium,
        };
        sessionStorage.setItem(
          "payment_success",
          JSON.stringify(setPaymentSuccess)
        );
        let response = await processPayment(postObject);
        if (response.status === "success") {
          window.location.href = response.data.paymentLink;
        } else {
          toast({
            variant: "destructive",
            title: "Something went wrong.",
            description: "Unable to initiate payment.",
          });
        }
      } else {
        try {
          SCHEMA.validateSync(userDetails.paymentDetails, {
            abortEarly: false,
            context: {
              offlinePaymentMode:
                userDetails?.paymentDetails?.offlinePaymentMode,
            },
          });

          // Create a properly formatted object that matches the C# DTO
          let postObject = {
            PolicyId: policyId,
            UserId: userId ? userId : userDetails?.userId || 0,
            TotalPremium: userDetails?.totalPremium,
            TotalPaidPremium: userDetails?.totalPaidPremium,
            AmountPaid:
              userDetails?.totalPremium - userDetails?.totalPaidPremium,
            ChildPremium: getChildPremium(userDetails?.products),
            PaymentDetails: getPaymentPostObject(
              userDetails?.paymentDetails,
              userDetails?.totalPremium,
              offlinePaymentModes
            ),
            IsUpdate: mode === "edit" ? true : false,
          };

          // Create FormData object
          const formData = new FormData();

          // Use appendFormData to properly format the data
          appendFormData(formData, postObject, "");

          // Log the FormData to check if it's being correctly populated
          for (let pair of formData.entries()) {
            console.log(pair[0] + ": " + pair[1]);
          }

          let response = await addProductPolicy(formData, step);
          if (response.status === "success") {
            window.location.href = `/paymentinitiated?typeid=${postObject.PaymentDetails.paymentTypeId}`;
          } else {
            toast({
              variant: "destructive",
              title: "Something went wrong.",
              description: "Unable to complete payment.",
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
              draft["paymentDetails"]["errors"] = newEr;
            });
            updatePaymentDetails(nextState.paymentDetails);
          }
        }
      }
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to proceed payment, please try again",
      });
    }

    setIsLoading(false);
  };

  const navigateToEdit = async () => {
    setMode("edit");
    updatePolicyId(userDetails?.policyId);
    if (userId > 0) {
      navigate(`/productlist?userId=${userId}&campaignId=${campaignId || 0}`);
    } else {
      navigate(`/productlist?campaignId=${campaignId || 0}`);
    }
  };
  const handleRefundRequest = async () => {
    const postPayload = {
      refundRequestNumber: 0,
      orderNumber: userDetails?.policyId,
      userId: user?.userId,
      refundAmount: userDetails?.totalPremium - userDetails?.totalPaidPremium,
      refundRequestDate: new Date().toISOString(),
    };
    const response = await saveRefundRequest(postPayload);
    if (response.status === "success") {
      window.location.href = "/refundinitiated";
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to create refund request.",
      });
    }
  };

  return (
    <div>
      {userId ? (
        <div className="flex justify-between">
          <CircleArrowLeft onClick={() => navigate(`/users`)} />
        </div>
      ) : null}

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="grid grid-cols-1 gap-6">
          {/* user details */}
          <Card className="bg-gradient-to-br from-[#f3f4f6] to-[#e5e7eb] dark:from-gray-800 dark:to-gray-900 shadow-lg border-0">
            <CardHeader className="bg-white dark:bg-gray-800 rounded-t-lg p-4 sm:p-6">
              <CardTitle className="text-xl font-bold">
                Policy Purchaser Details
              </CardTitle>
            </CardHeader>
            <CardContent className="grid gap-4 p-4 sm:p-6">
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label className="text-sm font-medium text-gray-500 dark:text-gray-400">
                    Name
                  </Label>
                  <div className="text-lg font-bold break-words">{`${user?.firstName} ${user?.lastName}`}</div>
                </div>
                <div className="space-y-2">
                  <Label className="text-sm font-medium text-gray-500 dark:text-gray-400">
                    Date of Birth
                  </Label>
                  <div className="text-lg font-bold">
                    {user?.dateOfBirth?.length
                      ? format(user?.dateOfBirth, "PPP")
                      : "-"}
                  </div>
                </div>
              </div>
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label className="text-sm font-medium text-gray-500 dark:text-gray-400">
                    Gender
                  </Label>
                  <div className="text-lg font-bold">{user?.gender?.name}</div>
                </div>
                <div className="space-y-2">
                  <Label className="text-sm font-medium text-gray-500 dark:text-gray-400">
                    Premium
                  </Label>
                  <div className="text-lg font-bold">
                    ₹ {userDetails?.totalPremium}
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* product details */}
          {userDetails?.products?.map((product) => {
            if (product.isProductSelected) {
              return (
                <Card
                  key={product?.productId}
                  className="bg-gradient-to-br from-[#f3f4f6] to-[#e5e7eb] dark:from-gray-800 dark:to-gray-900 shadow-lg border-0"
                >
                  <CardHeader className="bg-white dark:bg-gray-800 rounded-t-lg p-4 sm:p-6">
                    <CardTitle className="text-xl font-bold">
                      {product?.productName}
                    </CardTitle>
                  </CardHeader>
                  <CardContent className="grid gap-4 p-4 sm:p-6">
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      <div className="space-y-2">
                        <Label className="text-sm font-medium text-gray-500 dark:text-gray-400">
                          Sum Insured
                        </Label>
                        <div className="text-lg font-bold">
                          ₹ {product.selectedSumInsured?.name || ""}
                        </div>
                      </div>
                      <div className="space-y-2">
                        <Label className="text-sm font-medium text-gray-500 dark:text-gray-400">
                          Beneficiaries
                        </Label>
                        <div className="text-lg font-bold break-words">
                          {getBeneficiaries(
                            product["premiumChart"][
                              product?.selectedSumInsured?.index
                            ]
                          )}
                        </div>
                      </div>
                    </div>
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      {product?.selectedTopUpOption?.name &&
                        product?.selectedTopUpOption?.name > 0 && (
                          <div className="space-y-2">
                            <Label className="text-sm font-medium text-gray-500 dark:text-gray-400">
                              Top Up Amount
                            </Label>
                            <div className="text-lg font-bold">
                              ₹ {product?.selectedTopUpOption?.name || "-"}
                            </div>
                          </div>
                        )}
                      <div className="space-y-2">
                        <Label className="text-sm font-medium text-gray-500 dark:text-gray-400">
                          Premium
                        </Label>
                        <div className="text-lg font-bold">
                          ₹ {product?.totalProductPremium || ""}
                        </div>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              );
            }
          })}
        </div>

        {/* payment details */}

        <div>
          {userDetails?.totalPremium - userDetails?.totalPaidPremium > 0 ? (
            <Card className="shadow-lg border-0">
              <CardHeader className="p-4 sm:p-6">
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div className="w-full">
                    <Badge className="w-full py-2 flex justify-between items-center px-4">
                      <Label className="text-sm font-medium">
                        Amount Payable
                      </Label>
                      <span className="text-lg">
                        ₹{" "}
                        {userDetails?.totalPremium -
                          userDetails?.totalPaidPremium}
                      </span>
                    </Badge>
                  </div>
                  <div className="w-full">
                    <Badge className="w-full py-2 flex justify-between items-center px-4 bg-green-200 text-black hover:bg-green-200">
                      <Label className="text-sm font-medium">Amount Paid</Label>
                      <span className="text-lg">
                        ₹ {userDetails?.totalPaidPremium || 0}
                      </span>
                    </Badge>
                  </div>
                </div>
              </CardHeader>
              <CardContent className="space-y-6 p-4 sm:p-6">
                <div className="w-full">
                  <RSelect
                    options={paymentModes}
                    label="Payment Modes"
                    className="space-y-2"
                    onChange={handleChange("paymentMode")}
                    valueProperty="id"
                    nameProperty="name"
                    value={
                      userDetails?.paymentDetails?.paymentMode ||
                      paymentModes[0]
                    }
                  />
                </div>

                {userDetails?.paymentDetails?.paymentMode?.name
                  ?.toLowerCase()
                  ?.trim() === "offline" && (
                  <div className="grid gap-4 mt-5">
                    <Tabs
                      defaultValue="cheque"
                      className="w-full"
                      value={activeTab}
                      onValueChange={handleChangeTab}
                    >
                      <TabsList className="flex flex-row gap-2 mb-5">
                        <TabsTrigger value="cheque">Cheque</TabsTrigger>
                        <TabsTrigger value="neft">NEFT</TabsTrigger>
                        <TabsTrigger value="upi">UPI</TabsTrigger>
                      </TabsList>
                      <TabsContent value="cheque">
                        <div className="grid gap-4 flex flex-col">
                          <div className="grid gap-2">
                            <RInput
                              label="Cheque Number"
                              id="cheque-number"
                              onChange={handleChange(
                                "chequeNumber",
                                "chequeDetails"
                              )}
                              value={
                                userDetails?.paymentDetails?.chequeDetails
                                  ?.chequeNumber || ""
                              }
                              type="text"
                              placeholder="Enter Cheque Number"
                              isRequired={true}
                              error={
                                userDetails?.paymentDetails?.errors[
                                  `chequeDetails.chequeNumber`
                                ]
                              }
                            />
                          </div>
                          <div className="grid gap-2">
                            <RInput
                              label="Amount"
                              id="cheque-amount"
                              onChange={handleChange(
                                "chequeAmount",
                                "chequeDetails"
                              )}
                              value={
                                userDetails?.paymentDetails?.chequeDetails
                                  ?.amount || ""
                              }
                              type="text"
                              placeholder="Enter Amount"
                              isRequired={true}
                              error={
                                userDetails?.paymentDetails?.errors[
                                  `chequeDetails.amount`
                                ]
                              }
                              isDisabled={true}
                            />
                          </div>
                          <div className="grid gap-2">
                            <RInput
                              label=" Bank Name (The bank name on the cheque)"
                              id="cheque-bank-name"
                              onChange={handleChange(
                                "chequeBankName",
                                "chequeDetails"
                              )}
                              value={
                                userDetails?.paymentDetails?.chequeDetails
                                  ?.bankName || ""
                              }
                              type="text"
                              placeholder="Enter Bank Name"
                              isRequired={true}
                              error={
                                userDetails?.paymentDetails?.errors[
                                  `chequeDetails.bankName`
                                ]
                              }
                            />
                          </div>
                          <div className="grid gap-2">
                            <RInput
                              label=" MICR Code"
                              id="cheque-bank-name"
                              onChange={handleChange(
                                "micrCode",
                                "chequeDetails"
                              )}
                              value={
                                userDetails?.paymentDetails?.chequeDetails
                                  ?.micrCode || ""
                              }
                              type="text"
                              placeholder="Enter MICR Code"
                              isRequired={false}
                              error={
                                userDetails?.paymentDetails?.errors[
                                  `chequeDetails.micrCode`
                                ]
                              }
                            />
                          </div>
                          <div className="grid gap-2">
                            <RInput
                              label=" IFSC Code"
                              id="cheque-bank-name"
                              onChange={handleChange(
                                "ifscCode",
                                "chequeDetails"
                              )}
                              value={
                                userDetails?.paymentDetails?.chequeDetails
                                  ?.ifscCode || ""
                              }
                              type="text"
                              placeholder="Enter IFSC Code"
                              isRequired={true}
                              error={
                                userDetails?.paymentDetails?.errors[
                                  `chequeDetails.ifscCode`
                                ]
                              }
                            />
                          </div>
                          <div className="grid gap-2">
                            <DatePicker
                              label="Date"
                              id="cheque-date"
                              type="date"
                              placeholder="dd-mm-yyyy"
                              onChange={handleChange(
                                "chequeDate",
                                "chequeDetails"
                              )}
                              isRequired={true}
                              date={
                                userDetails?.paymentDetails?.chequeDetails
                                  ?.date || ""
                              }
                              error={
                                userDetails?.paymentDetails?.errors[
                                  `chequeDetails.date`
                                ]
                              }
                              fromDate={new Date("2024-10-01")}
                            />
                          </div>

                          <div className="grid gap-2">
                            <Label className="mt-3" htmlFor="favouredName">
                              <span>Associoation Name</span>
                            </Label>
                            <Combobox
                              options={associations}
                              valueProperty="id"
                              labelProperty="name"
                              id="cheque-associoation"
                              onChange={handleChange(
                                "inFavourOf",
                                "chequeDetails"
                              )}
                              value={
                                userDetails?.paymentDetails?.chequeDetails
                                  ?.inFavourOf || null
                              }
                              error={
                                userDetails?.paymentDetails?.errors[
                                  `chequeDetails.inFavourOf`
                                ]
                              }
                              isDisabled={true}
                            />
                          </div>

                          <div className="grid gap-2">
                            <RInput
                              label="Association Code"
                              id="association-code"
                              value={
                                userDetails?.paymentDetails?.chequeDetails
                                  ?.code || ""
                              }
                              type="text"
                              placeholder="Enter Code"
                              isDisabled={true}
                            />
                          </div>

                          <div className="grid gap-2">
                            <Label className="mt-3" htmlFor="favouredName">
                              <span>In Favour Of</span>
                            </Label>
                            <Label className="mt-3" htmlFor="favouredName">
                              <span className="font-bold pl-3">
                                {userDetails?.paymentDetails?.chequeDetails
                                  ?.accountName ||
                                  " Go Digit General Insurance Limited"}
                              </span>
                            </Label>
                            {user?.userType?.name?.toLowerCase().trim() !==
                              "community" && (
                              <Alert variant="destructive" className="my-5">
                                <AlertCircle className="h-4 w-4" />
                                <AlertTitle>Note</AlertTitle>
                                <AlertDescription>
                                  Mention Association Code at the Back of the
                                  Cheque{" "}
                                </AlertDescription>
                              </Alert>
                            )}
                          </div>
                          <div className="grid gap-2">
                            <RInput
                              label="Cheque Deposit Location"
                              id="cheque-deposit-location"
                              onChange={handleChange(
                                "chequeDepositLocation",
                                "chequeDetails"
                              )}
                              value={
                                userDetails?.paymentDetails?.chequeDetails
                                  ?.chequeDepositLocation || ""
                              }
                              type="text"
                              placeholder="Enter Location"
                              error={
                                userDetails?.paymentDetails?.errors[
                                  `chequeDetails.chequeDepositLocation`
                                ]
                              }
                            />
                          </div>
                          <div className="grid gap-2">
                            <Label className="mt-3" htmlFor="chequeFile">
                              Cheque Photo
                            </Label>
                            <FileUpload
                              id="chequeFile"
                              onChange={handleChange(
                                "chequePhoto",
                                "chequeDetails"
                              )}
                              value={
                                userDetails?.paymentDetails?.chequeDetails
                                  ?.chequePhoto || null
                              }
                              error={
                                userDetails?.paymentDetails?.errors[
                                  `chequeDetails.chequePhoto`
                                ]
                              }
                              accept=".png, .jpg, .jpeg"
                              isImage={true}
                            />
                          </div>
                        </div>
                      </TabsContent>

                      <TabsContent value="neft">
                        <div className="grid gap-4 flex flex-col">
                          <div className="grid md:grid-cols-2 gap-6 p-4">
                            <div>
                              <p className="font-medium">Bank Name:</p>
                              <p>
                                {
                                  userDetails?.paymentDetails?.accountDetails
                                    ?.bankName
                                }
                              </p>
                            </div>
                            <div>
                              <p className="font-medium">Branch Name:</p>
                              <p>
                                {
                                  userDetails?.paymentDetails?.accountDetails
                                    ?.branchName
                                }
                              </p>
                            </div>
                            <div>
                              <p className="font-medium">Account Number:</p>
                              <p>
                                {
                                  userDetails?.paymentDetails?.accountDetails
                                    ?.accountNumber
                                }
                              </p>
                            </div>
                            <div>
                              <p className="font-medium">IFSC Code:</p>
                              <p>
                                {
                                  userDetails?.paymentDetails?.accountDetails
                                    ?.ifscCode
                                }
                              </p>
                            </div>
                          </div>
                          <hr />

                          <div className="space-y-4">
                            <div>
                              <RInput
                                label="Bank Name"
                                id="bank-name-neft"
                                onChange={handleChange("neftBankName", "neft")}
                                value={
                                  userDetails?.paymentDetails?.neft?.bankName ||
                                  ""
                                }
                                type="text"
                                placeholder="Enter Bank Name"
                              />
                            </div>
                            <div className="space-y-2">
                              <RInput
                                label="Branch Name"
                                id="branch-name-neft"
                                onChange={handleChange(
                                  "neftBranchName",
                                  "neft"
                                )}
                                value={
                                  userDetails?.paymentDetails?.neft
                                    ?.branchName || ""
                                }
                                type="text"
                                placeholder="Enter Branch Name"
                              />
                            </div>
                            <div className="space-y-2">
                              <RInput
                                label="Account Name"
                                id="account-name-neft"
                                onChange={handleChange(
                                  "neftAccountName",
                                  "neft"
                                )}
                                value={
                                  userDetails?.paymentDetails?.neft
                                    ?.accountName || ""
                                }
                                type="text"
                                placeholder="Enter Account Name"
                                isRequired={true}
                                error={
                                  userDetails?.paymentDetails?.errors[
                                    `neft.accountName`
                                  ]
                                }
                              />
                            </div>
                            <div className="space-y-2">
                              <RInput
                                label="Account Number"
                                id="account-number-neft"
                                onChange={handleChange(
                                  "neftAccountNumber",
                                  "neft"
                                )}
                                value={
                                  userDetails?.paymentDetails?.neft
                                    ?.accountNumber || ""
                                }
                                type="text"
                                placeholder="Enter Account Number"
                                isRequired={true}
                                error={
                                  userDetails?.paymentDetails?.errors[
                                    `neft.accountNumber`
                                  ]
                                }
                              />
                            </div>
                            <div className="space-y-2">
                              <RInput
                                label="IFSC Code"
                                id="ifsc-code-neft"
                                onChange={handleChange("neftIfscCode", "neft")}
                                value={
                                  userDetails?.paymentDetails?.neft?.ifscCode ||
                                  ""
                                }
                                type="text"
                                placeholder="Enter IFSC Code"
                                isRequired={true}
                                error={
                                  userDetails?.paymentDetails?.errors[
                                    `neft.ifscCode`
                                  ]
                                }
                              />
                            </div>
                            <div className="space-y-2">
                              <RInput
                                label="Transaction Number"
                                id="trans-number-neft"
                                onChange={handleChange(
                                  "neftTransactionId",
                                  "neft"
                                )}
                                value={
                                  userDetails?.paymentDetails?.neft
                                    ?.transactionId || ""
                                }
                                type="text"
                                placeholder="Enter Transaction Number"
                                isRequired={true}
                                error={
                                  userDetails?.paymentDetails?.errors[
                                    `neft.transactionId`
                                  ]
                                }
                              />
                            </div>
                            <div className="space-y-2">
                              <DatePicker
                                label="Date"
                                id="neft-date"
                                type="date"
                                placeholder="dd-mm-yyyy"
                                onChange={handleChange("neftDate", "neft")}
                                isRequired={true}
                                date={
                                  userDetails?.paymentDetails?.neft?.date || ""
                                }
                                error={
                                  userDetails?.paymentDetails?.errors[
                                    `neft.date`
                                  ]
                                }
                                fromDate={new Date("2025-01-01")}
                              />
                            </div>
                            <div className="space-y-2">
                              <RInput
                                label="Amount"
                                id="amount-neft"
                                onChange={handleChange("neftAmount", "neft")}
                                value={
                                  userDetails?.paymentDetails?.neft?.amount ||
                                  ""
                                }
                                type="number"
                                placeholder="Enter Amount"
                                isRequired={true}
                                error={
                                  userDetails?.paymentDetails?.errors[
                                    `neft.amount`
                                  ]
                                }
                                isDisabled={true}
                              />
                            </div>
                            <div className="grid gap-2">
                              <Label
                                className="mt-3"
                                htmlFor="neftpaymentreceipt"
                              >
                                Payment Photo
                              </Label>
                              <FileUpload
                                id="neftpaymentreceipt"
                                onChange={handleChange(
                                  "neftPaymentReceipt",
                                  "neft"
                                )}
                                value={
                                  userDetails?.paymentDetails?.neft
                                    ?.neftPaymentReceipt || null
                                }
                                accept=".png, .jpg, .jpeg"
                                isImage={true}
                              />
                            </div>
                          </div>
                        </div>
                      </TabsContent>

                      <TabsContent value="upi">
                        <div className="grid gap-4 flex flex-col">
                          <div>
                            <div className="pb-5">
                              <p>Kotak Bank</p>
                            </div>
                            <div className="flex items-center justify-center">
                              <img
                                src={qrcode}
                                alt="Kotak Bank QR Code"
                                // width={1250}
                                // height={250}
                                className="mx-auto"
                              />
                            </div>
                          </div>
                          <div className="space-y-4">
                            <div className="space-y-2">
                              <RInput
                                label="Transaction Number"
                                id="trans-num-upi"
                                onChange={handleChange(
                                  "upiTransactioId",
                                  "upi"
                                )}
                                value={
                                  userDetails?.paymentDetails?.upi
                                    ?.transactionId || ""
                                }
                                type="text"
                                isRequired={true}
                                placeholder="Enter Transaction Number"
                                error={
                                  userDetails?.paymentDetails?.errors[
                                    `upi.transactionId`
                                  ]
                                }
                              />
                            </div>

                            <div className="space-y-2">
                              <RInput
                                label="Amount"
                                id="amount-upi"
                                onChange={handleChange("upiAmount", "upi")}
                                value={
                                  userDetails?.paymentDetails?.upi?.amount || ""
                                }
                                type="number"
                                placeholder="Enter Amount"
                                isRequired={true}
                                error={
                                  userDetails?.paymentDetails?.errors[
                                    `upi.amount`
                                  ]
                                }
                                isDisabled={true}
                              />
                            </div>

                            <div className="space-y-2">
                              <DatePicker
                                label="Date"
                                id="upi-date"
                                type="date"
                                placeholder="dd-mm-yyyy"
                                onChange={handleChange("upiDate", "upi")}
                                isRequired={true}
                                date={
                                  userDetails?.paymentDetails?.upi?.date || ""
                                }
                                error={
                                  userDetails?.paymentDetails?.errors[
                                    `upi.date`
                                  ]
                                }
                                fromDate={new Date("2024-10-01")}
                              />
                            </div>
                            <div className="grid gap-2">
                              <Label
                                className="mt-3"
                                htmlFor="upipaymentreceipt"
                              >
                                Payment Photo
                              </Label>
                              <FileUpload
                                id="upipaymentreceipt"
                                onChange={handleChange(
                                  "upiPaymentReceipt",
                                  "upi"
                                )}
                                value={
                                  userDetails?.paymentDetails?.upi
                                    ?.upiPaymentReceipt || null
                                }
                                accept=".png, .jpg, .jpeg"
                                isImage={true}
                              />
                            </div>
                          </div>
                        </div>
                      </TabsContent>
                    </Tabs>
                  </div>
                )}

                <div className="my-5">
                  <Label
                    htmlFor="checkbox"
                    className="flex flex-row justify-start"
                  >
                    <Checkbox
                      type="checkbox"
                      id="checkbox"
                      checked={
                        userDetails?.paymentDetails?.isPaymentConfirmed || false
                      }
                      onCheckedChange={handleConfirmPayment}
                      className="mr-2"
                    />
                    <span>
                      Check the box to confirm the displayed information.
                    </span>
                  </Label>
                </div>
                <div className="gap-4">
                  {permissions?.[PAGE_NAME_pensionerpayment]?.update &&
                  userDetails?.totalPremium - userDetails?.totalPaidPremium >
                    0 &&
                  userDetails?.products[0].isCampaignExpired === false ? (
                    <div className="flex gap-4">
                      {" "}
                      <RButton
                        className="my-5"
                        onClick={handlePay}
                        isDisabled={
                          !userDetails?.paymentDetails?.isPaymentConfirmed
                        }
                        isLoading={isLoading}
                      >
                        {userDetails?.paymentDetails?.paymentMode?.name
                          ?.toLowerCase()
                          ?.trim() === "offline"
                          ? "Submit"
                          : "Pay"}
                      </RButton>
                      <RButton className="my-5" onClick={navigateToEdit}>
                        Back To Edit
                      </RButton>
                    </div>
                  ) : null}
                </div>
              </CardContent>
            </Card>
          ) : userDetails?.totalPremium - userDetails?.totalPaidPremium < 0 ? (
            <Card className="shadow-lg border-0">
              <CardHeader className="grid grid-cols-1 md:grid-cols-2 gap-4 items-center">
                <Badge className="font-medium w-60 h-11 flex justify-between px-5">
                  <Label htmlFor="premium">Refund Amount</Label>
                  <div className="text-lg">
                    ₹{" "}
                    {Math.abs(
                      userDetails?.totalPremium - userDetails?.totalPaidPremium
                    )}
                  </div>
                </Badge>
              </CardHeader>
              <CardContent className="flex flex-col">
                <RButton className="my-10" onClick={handleRefundRequest}>
                  Raise Refund Request
                </RButton>
              </CardContent>
            </Card>
          ) : (
            <Card className="w-full max-w-md mx-auto">
              <CardHeader className="hidden">
                <CardTitle>Payment Information</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-lg font-medium hidden">
                  Note: Payable amount is 0. Please select the policy options.
                </p>
              </CardContent>
              <CardFooter>
                <RButton
                  className="inline-flex items-center rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground shadow-sm transition-colors hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2"
                  onClick={() => {
                    if (isRetiree) {
                      window.location.href = "/";
                    } else {
                      window.location.href = "/dashboard";
                    }
                  }}
                >
                  Go to Homepage
                </RButton>
              </CardFooter>
            </Card>
          )}
        </div>
      </div>
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(Payments))
);
