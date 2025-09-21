import {
  getAssociations,
  getRefundRequest,
  updateRefundRequest,
} from "@/services/customerProfile";
import { useEffect, useState } from "react";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { useNavigate, useParams } from "react-router-dom";
import { Label } from "@/components/ui/label";
import { cn } from "@/lib/utils";
import { Badge } from "@/components/ui/badge";
import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import RButton from "@/components/ui/rButton";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import RInput from "@/components/ui/rInput";
import { DatePicker } from "@/components/ui/datePicker";
import { Combobox } from "@/components/ui/comboBox";
import FileUpload from "@/components/ui/fileUpload";
import { produce } from "immer";
import { ValidationError } from "yup";
import {
  appendFormData,
  initialRefundPaymentObject,
  updateRefundRequestObject,
} from "@/lib/helperFunctions";
import { toast } from "@/components/ui/use-toast";
import { refundPaymentSchema } from "@/validations";

const PAGE_NAME = "refundrequests";
const RefundDetails = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [refundRequest, setRefundRequest] = useState(null);
  const [activeTab, setActiveTab] = useState("cheque");
  const [associations, setAssociations] = useState([]);
  const [refundPayment, setRefundPayment] = useState(
    initialRefundPaymentObject()
  );
  const [errors, setErrors] = useState({});
  const [isRefundPaymentTabVisible, setIsRefundPaymentTabVisible] =
    useState(false);

  useEffect(() => {
    (async () => {
      const response = await getRefundRequest(id);
      if (response.status === "success") {
        setRefundRequest(response.data);
      }
      fetchAssociations();
    })();
  }, []);

  const fetchAssociations = async () => {
    const response = await getAssociations();
    if (response.status === "success") {
      setAssociations(response.data);
    }
  };

  const handleChange = (name, paymentType) => (event) => {
    let nextErrors = { ...errors };
    let path = `${paymentType}.${name}`;
    let nextState = produce(refundPayment, (draft) => {
      switch (name) {
        //cheque
        case "chequeNumber":
          draft.chequeDetails[name] = event.target.value;
          break;
        case "chequeBankName":
          draft.chequeDetails[name] = event.target.value;
          break;

        case "chequePhoto":
          draft.chequeDetails[name] = event;
          draft.chequeDetails.isChequePhotoUpdated = true;
          break;
        case "inFavourOf":
          draft.chequeDetails[name] = event.target.value;
          break;
        case "chequeDate":
          draft.chequeDetails[name] = event;
          break;

        //neft
        case "neftBankName":
          draft.neft[name] = event.target.value;
          break;
        case "neftBranchName":
          draft.neft[name] = event.target.value;
          break;
        case "neftAccountNumber":
          draft.neft[name] = event.target.value;
          break;
        case "neftAccountName":
          draft.neft[name] = event.target.value;
          break;
        case "neftIfscCode":
          draft.neft[name] = event.target.value;
          break;
        case "neftTransactionId":
          draft.neft[name] = event.target.value;
          break;

        case "neftDate":
          draft.neft[name] = event;
          break;
        case "neftPaymentReceipt":
          draft.neft[name] = event;
          draft.neft.isNeftPaymentReceiptUpdated = true;
          break;

        //upi
        case "upiTransactionId":
          draft.upi[name] = event.target.value;
          break;

        case "upiDate":
          draft.upi[name] = event;
          break;
        case "upiPaymentReceipt":
          draft.upi[name] = event;
          draft.upi.isUpiPaymentReceiptUpdated = true;
          break;
        default:
          break;
      }
      try {
        const schema = refundPaymentSchema(activeTab);
        schema.validateSyncAt(path, draft, { context: { activeTab } });
        nextErrors[path] = [];
      } catch (e) {
        nextErrors[path] = [...e.errors];
      }
    });
    setErrors(nextErrors);
    setRefundPayment(nextState);
  };

  const handleAccept = () => {
    setIsRefundPaymentTabVisible(true);
    setRefundRequest({
      ...refundRequest,
      isAccepted: true,
    });
  };

  const handleReject = () => {
    setIsRefundPaymentTabVisible(false);
    navigate("/refundrequests");
  };

  const handlePayment = async () => {
    try {
      const schema = refundPaymentSchema(activeTab);

      schema.validateSync(refundPayment, {
        abortEarly: false,
        context: { activeTab },
      });

      const postObject = updateRefundRequestObject(
        refundRequest,
        refundPayment,
        activeTab
      );
      let formData = new FormData();
      appendFormData(formData, postObject);
      const response = await updateRefundRequest(
        refundRequest.refundRequestNumber,
        formData
      );

      if (response.status === "success") {
        toast({
          variant: "success",
          title: "Payment Successful",
          description: "Payment Successful.",
        });
        navigate("/refundrequests");
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

        let nextState = produce(refundPayment, (draft) => {
          draft["errors"] = newEr;
        });
        setErrors(nextState.errors);
      }
    }
  };

  const handleChangeTab = (tabName) => {
    setActiveTab(tabName);
  };

  return (
    <div className="flex flex-col gap-6">
      <div>
        <h1 className="text-2xl font-bold mb-6">Refund Details</h1>
      </div>
      <div className="space-y-2">
        <div className="grid grid-cols-2 gap-4">
          <div>
            <Label>Pensioner Name</Label>
            <p>{refundRequest?.retireeName}</p>
          </div>
          <div>
            <Label>Amount</Label>
            <p>₹{refundRequest?.refundAmount.toFixed(2)}</p>
          </div>
          <div>
            <Label>Date</Label>
            <p>
              {new Date(refundRequest?.refundRequestDate).toLocaleDateString()}
            </p>
          </div>
          <div className="flex flex-col">
            <Label>Status</Label>
            <Badge
              className={cn(
                refundRequest?.refundRequestStatus?.toLowerCase()?.trim() ===
                  "initiated" && "bg-yellow-400 text-black w-fit mt-3",
                refundRequest?.refundRequestStatus?.toLowerCase()?.trim() ===
                  "completed" && "bg-green-600 text-black w-fit mt-3",
                refundRequest?.refundRequestStatus?.toLowerCase()?.trim() ===
                  "rejected" && "bg-red-600 text-white w-fit mt-3"
              )}
            >
              {refundRequest?.refundRequestStatus}
            </Badge>
          </div>
        </div>
      </div>
      {refundRequest?.isAccepted ? (
        <Alert className="mt-5 border-green-500 bg-green-100">
          <AlertTitle>Great!</AlertTitle>
          <AlertDescription>
            The refund request has been accepted.
          </AlertDescription>
        </Alert>
      ) : (
        <div>
          {refundRequest?.refundRequestStatus?.toLowerCase().trim() ===
          "initiated" ? (
            <RButton onClick={handleAccept}>Accept</RButton>
          ) : null}

          {refundRequest?.refundRequestStatus?.toLowerCase().trim() ===
          "initiated" ? (
            <RButton variant="danger" onClick={handleReject}>
              Reject
            </RButton>
          ) : null}
        </div>
      )}

      {isRefundPaymentTabVisible || refundRequest?.isAccepted ? (
        <div className="grid gap-4 mt-5">
          <Tabs
            defaultValue="cheque"
            className="w-full"
            value={activeTab}
            onValueChange={handleChangeTab}
          >
            <TabsList className="grid w-full grid-cols-3 mb-5">
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
                    onChange={handleChange("chequeNumber", "chequeDetails")}
                    value={refundPayment?.chequeDetails?.chequeNumber || ""}
                    type="text"
                    placeholder="Enter Cheque Number"
                    isRequired={true}
                    error={errors["chequeDetails.chequeNumber"]}
                  />
                </div>
                <div className="grid gap-2 mt-3">
                  <Label>Amount</Label>
                  <p>₹{refundRequest?.refundAmount.toFixed(2)}</p>
                </div>
                <div className="grid gap-2">
                  <RInput
                    label=" Bank Name (The bank name on the cheque)"
                    id="cheque-bank-name"
                    onChange={handleChange("chequeBankName", "chequeDetails")}
                    value={refundPayment?.chequeDetails?.chequeBankName || ""}
                    type="text"
                    placeholder="Enter Bank Name"
                    isRequired={true}
                    error={errors["chequeDetails.chequeBankName"]}
                  />
                </div>
                <div className="grid gap-2">
                  <DatePicker
                    label="Date"
                    id="cheque-date"
                    type="date"
                    placeholder="dd-mm-yyyy"
                    onChange={handleChange("chequeDate", "chequeDetails")}
                    isRequired={true}
                    date={refundPayment?.chequeDetails?.chequeDate || ""}
                    error={errors["chequeDetails.chequeDate"]}
                    fromDate={new Date()}
                  />
                </div>

                <div className="grid gap-2">
                  {/* <Label className="mt-3" htmlFor="favouredName">
                    <span>In Favour Of</span>
                  </Label> */}
                  <RInput
                    label="In Favour Of"
                    id="inFavourOf"
                    onChange={handleChange("inFavourOf", "chequeDetails")}
                    value={refundPayment?.chequeDetails?.inFavourOf || ""}
                    type="text"
                    placeholder="Enter In Favour Of Name"
                    isRequired={true}
                    error={errors["chequeDetails.inFavourOf"]}
                  />
                  {/* <Combobox
                    options={associations}
                    valueProperty="id"
                    labelProperty="name"
                    id="cheque-associoation"
                    onChange={handleChange("inFavourOf", "chequeDetails")}
                    value={refundPayment?.chequeDetails?.inFavourOf || null}
                    error={errors["chequeDetails.inFavourOf"]}
                  /> */}
                </div>
                <div className="grid gap-2">
                  <Label className="mt-3" htmlFor="chequeFile">
                    Cheque Photo
                  </Label>
                  <FileUpload
                    id="chequeFile"
                    onChange={handleChange("chequePhoto", "chequeDetails")}
                    value={refundPayment?.chequeDetails?.chequePhoto || null}
                    error={errors["chequeDetails.chequePhoto"]}
                    accept=".png, .jpg, .jpeg"
                    isImage={true}
                  />
                </div>
              </div>
            </TabsContent>

            <TabsContent value="neft">
              <div className="grid gap-4 flex flex-col">
                <div className="space-y-4">
                  <div>
                    <RInput
                      label="Bank Name"
                      id="bank-name-neft"
                      onChange={handleChange("neftBankName", "neft")}
                      value={refundPayment?.neft?.neftBankName || ""}
                      type="text"
                      placeholder="Enter Bank Name"
                    />
                  </div>
                  <div className="space-y-2">
                    <RInput
                      label="Branch Name"
                      id="branch-name-neft"
                      onChange={handleChange("neftBranchName", "neft")}
                      value={refundPayment?.neft?.neftBranchName || ""}
                      type="text"
                      placeholder="Enter Branch Name"
                    />
                  </div>
                  <div className="space-y-2">
                    <RInput
                      label="Account Name"
                      id="account-name-neft"
                      onChange={handleChange("neftAccountName", "neft")}
                      value={refundPayment?.neft?.neftAccountName || ""}
                      type="text"
                      placeholder="Enter Account Name"
                      isRequired={true}
                      error={errors["neft.neftAccountName"]}
                    />
                  </div>
                  <div className="space-y-2">
                    <RInput
                      label="Account Number"
                      id="account-number-neft"
                      onChange={handleChange("neftAccountNumber", "neft")}
                      value={refundPayment?.neft?.neftAccountNumber || ""}
                      type="text"
                      placeholder="Enter Account Number"
                      isRequired={true}
                      error={errors["neft.neftAccountNumber"]}
                    />
                  </div>
                  <div className="space-y-2">
                    <RInput
                      label="IFSC Code"
                      id="ifsc-code-neft"
                      onChange={handleChange("neftIfscCode", "neft")}
                      value={refundPayment?.neft?.neftIfscCode || ""}
                      type="text"
                      placeholder="Enter IFSC Code"
                      isRequired={true}
                      error={errors["neft.neftIfscCode"]}
                    />
                  </div>
                  <div className="space-y-2">
                    <RInput
                      label="Transaction Number"
                      id="trans-number-neft"
                      onChange={handleChange("neftTransactionId", "neft")}
                      value={refundPayment?.neft?.neftTransactionId || ""}
                      type="text"
                      placeholder="Enter Transaction Number"
                      isRequired={true}
                      error={errors["neft.neftTransactionId"]}
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
                      date={refundPayment?.neft?.neftDate || ""}
                      error={errors["neft.neftDate"]}
                      fromDate={new Date()}
                    />
                  </div>
                  <div className="grid gap-2 mt-3">
                    <Label>Amount</Label>
                    <p>₹{refundRequest?.refundAmount.toFixed(2)}</p>
                  </div>
                  <div className="grid gap-2">
                    <Label className="mt-3" htmlFor="neftpaymentreceipt">
                      Payment Photo
                    </Label>
                    <FileUpload
                      id="neftpaymentreceipt"
                      onChange={handleChange("neftPaymentReceipt", "neft")}
                      value={refundPayment?.neft?.neftPaymentReceipt || null}
                      accept=".png, .jpg, .jpeg"
                      isImage={true}
                    />
                  </div>
                </div>
              </div>
            </TabsContent>

            <TabsContent value="upi">
              <div className="grid gap-4 flex flex-col">
                <div className="space-y-4">
                  <div className="space-y-2">
                    <RInput
                      label="Transaction Number"
                      id="trans-num-upi"
                      onChange={handleChange("upiTransactionId", "upi")}
                      value={refundPayment?.upi?.upiTransactionId || ""}
                      type="text"
                      isRequired={true}
                      placeholder="Enter Transaction Number"
                      error={errors["upi.upiTransactionId"]}
                    />
                  </div>

                  <div className="grid gap-2 mt-3">
                    <Label>Amount</Label>
                    <p>₹{refundRequest?.refundAmount.toFixed(2)}</p>
                  </div>

                  <div className="space-y-2">
                    <DatePicker
                      label="Date"
                      id="upi-date"
                      type="date"
                      placeholder="dd-mm-yyyy"
                      onChange={handleChange("upiDate", "upi")}
                      isRequired={true}
                      date={refundPayment?.upi?.upiDate || ""}
                      error={errors["upi.upiDate"]}
                      fromDate={new Date()}
                    />
                  </div>
                  <div className="grid gap-2">
                    <Label className="mt-3" htmlFor="upipaymentreceipt">
                      Payment Photo
                    </Label>
                    <FileUpload
                      id="upipaymentreceipt"
                      onChange={handleChange("upiPaymentReceipt", "upi")}
                      value={refundPayment?.upi?.upiPaymentReceipt || null}
                      accept=".png, .jpg, .jpeg"
                      isImage={true}
                    />
                  </div>
                </div>
              </div>
            </TabsContent>
          </Tabs>
          <div className="flex justify-start">
            <RButton onClick={handlePayment}>Pay</RButton>
          </div>
        </div>
      ) : (
        <div></div>
      )}
    </div>
  );
};

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(RefundDetails))
);
