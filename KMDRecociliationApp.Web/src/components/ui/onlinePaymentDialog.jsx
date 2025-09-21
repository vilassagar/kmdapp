import {
  getPolicyOrderById,
  updatePolicyOrder,
} from "@/services/customerProfile";
import { Dialog } from "@radix-ui/react-dialog";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import {
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "./dialog";
import { Label } from "./label";
import RButton from "./rButton";
import { Textarea } from "./textarea";
import { toast } from "./use-toast";
import { format } from "date-fns";
import { ScrollArea } from "./scroll-area";

function OnlinePaymentDialog({ paymentId, open, setOpen, onPaymentAck }) {
  const [payment, setPayment] = useState({});
  const [paidAmount, setPaidAmount] = useState("");
  const [transactionNumber, setTransactionNumber] = useState("");
  const [comment, setComment] = useState("");
  const [isCommentError, setIsCommentError] = useState(false);
  const [transactionError, setTransactionError] = useState("");
  const [paidAmountError, setPaidAmountError] = useState("");

  useEffect(() => {
    (async () => {
      if (paymentId && open) {
        let response = await getPolicyOrderById(paymentId);
        if (response.status === "success") {
          setPayment(response.data);
          setPaidAmount(response.data.paidAmount?.toString() || "");
          setTransactionNumber(response.data.transactionNumber || "");
          setComment(response?.data?.comment || "");
          setTransactionError("");
          setPaidAmountError("");
        } else {
          toast({
            variant: "destructive",
            title: "Something went wrong.",
            description: "Unable to get payment",
          });
        }
      }

      if (!open) {
        setPaidAmount("");
        setTransactionNumber("");
        setComment("");
        setIsCommentError(false);
        setTransactionError("");
        setPaidAmountError("");
      }
    })();
  }, [paymentId, open]);

  const validateTransactionNumber = (value) => {
    if (!value && value !== "") {
      return "Transaction number is required";
    }
    return "";
  };

  const validatePaidAmount = (value) => {
    if (!value && value !== "0") {
      return "Paid amount is required";
    }
    if (isNaN(value)) {
      return "Please enter a valid number";
    }
    return "";
  };

  const handlePaidAmountChange = (e) => {
    const value = e.target.value;
    setPaidAmount(value);
    const error = validatePaidAmount(value);
    setPaidAmountError(error);
  };

  const handleTransactionNumberChange = (e) => {
    const value = e.target.value;
    setTransactionNumber(value);
    const error = validateTransactionNumber(value);
    setTransactionError(error);
  };

  const handleSubmit = (isAccepted) => async (event) => {
    if (!comment.length) {
      setIsCommentError(true);
      return;
    }

    const transactionError = validateTransactionNumber(transactionNumber);
    if (transactionError) {
      setTransactionError(transactionError);
      return;
    }

    const paidError = validatePaidAmount(paidAmount);
    if (paidError) {
      setPaidAmountError(paidError);
      return;
    }

    let payload = {
      orderId: payment.orderId,
      name: payment.name,
      mobileNumber: payment.mobileNumber,
      amount: payment.amount,
      paidAmount: parseFloat(paidAmount),
      status: payment.status,
      organisationName: payment.organisationName,
      associationName: payment.associationName,
      transactionNumber: transactionNumber,
      date: payment.date,
      comment: comment,
    };
    let response = await updatePolicyOrder(payload);
    if (response.status === "success") {
      setOpen(false);
      setComment("");
      onPaymentAck();
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to save payment.",
      });
    }
  };
  const isFieldDisabled = (field) => {
if(transactionNumber!=="")
  return false;
  }
  return (
    <Dialog open={open} onOpenChange={setOpen} className="fixed max-h-screen inset-0 flex items-center justify-center p-4">
    <div className="relative bg-white rounded-lg shadow-lg max-h-screen w-full max-w-lg overflow-hidden">
    <ScrollArea className="max-h-[calc(100vh-2rem)] overflow-auto">
      <DialogContent className="p-6 max-h-screen overflow-auto">
        <DialogHeader>
          <DialogTitle>Payment Details</DialogTitle>
        </DialogHeader>
        <div className="space-y-6">
          {/* Read-only information grid */}
          <div className="grid grid-cols-2 gap-4">
            <div>
              <div className="font-medium">Order Id</div>
              <div>{payment.orderId}</div>
            </div>
            <div>
              <div className="font-medium">Date</div>
              <div>
                {payment.date?.length ? format(payment.date, "PPP") : null}
              </div>
            </div>
            <div>
              <div className="font-medium">Amount</div>
              <div>{payment.amount}</div>
            </div>
            <div>
              <div className="font-medium">Name</div>
              <div>{payment.name}</div>
            </div>
            <div>
              <div className="font-medium">Mobile Number</div>
              <div>{payment.mobileNumber}</div>
            </div>
            <div>
              <div className="font-medium">Status</div>
              <div>{payment.status}</div>
            </div>
            <div>
              <div className="font-medium">Organisation Name</div>
              <div>{payment.organisationName}</div>
            </div>
            <div>
              <div className="font-medium">Association Name</div>
              <div>{payment.associationName}</div>
            </div>
          </div>

          <div className="space-y-4">
            <div>
              <Label className="mb-2 block">
                <span>Paid Amount</span>
                <span className="text-red-600 align-top ml-1">*</span>
              </Label>

              <input
                type="text"
                value={paidAmount}
                onChange={handlePaidAmountChange}
                className={`w-full p-2 border rounded-md ${
                  paidAmountError ? "border-red-500" : "border-gray-300"
                } focus:outline-none focus:ring-2 focus:ring-blue-500`}
              />
              {paidAmountError && (
                <div className="text-sm text-red-600 mt-1">
                  {paidAmountError}
                </div>
              )}
            </div>

            <div>
              <Label className="mb-2 block">
                <span>Transaction Number</span>
                <span className="text-red-600 align-top ml-1">*</span>
              </Label>
              <input
                type="text"
                value={transactionNumber}
                onChange={handleTransactionNumberChange}
                className={`w-full p-2 border rounded-md ${
                  transactionError ? "border-red-500" : "border-gray-300"
                } focus:outline-none focus:ring-2 focus:ring-blue-500`}
              />
              {transactionError && (
                <div className="text-sm text-red-600 mt-1">
                  {transactionError}
                </div>
              )}
            </div>

            <div>
              <Label className="mb-2 block">
                <span>Comment</span>
                <span className="text-red-600 align-top ml-1">*</span>
              </Label>
              <Textarea
                value={comment}
                onChange={(event) => {
                  if (event && event.target.value.length) {
                    setIsCommentError(false);
                  } else {
                    setIsCommentError(true);
                  }
                  setComment(event.target.value);
                }}
                placeholder="Enter Comments"
                className={`w-full ${
                  isCommentError
                    ? "ring-red-600 focus-visible:ring-offset-0 focus-visible:ring-red-600 ring-1"
                    : "focus-visible:ring-offset-0 focus-visible:ring-1"
                }`}
              />
            </div>
          </div>
        </div>

        <DialogFooter>
          <div>
            {!payment.transactionNumber && (
              <RButton
                isDisabled={isFieldDisabled(payment.transactionNumber)}
                onClick={handleSubmit(true)}
              >
                Accept
              </RButton>
            )}
            <RButton
              className="ml-3"
              onClick={() => {
                setOpen(false);
                setIsCommentError(false);
                setTransactionError("");
                setPaidAmountError("");
              }}
            >
              Close
            </RButton>
          </div>
        </DialogFooter>
      </DialogContent>
    </ScrollArea>
  </div>
</Dialog>


  );
}

OnlinePaymentDialog.defaultProps = {
  paymentId: "",
  open: false,
  setOpen: () => {},
  onPaymentAck: () => {},
};

OnlinePaymentDialog.propTypes = {
  paymentId: PropTypes.string,
  open: PropTypes.bool,
  setOpen: PropTypes.func,
  onPaymentAck: PropTypes.func,
};

export default OnlinePaymentDialog;
