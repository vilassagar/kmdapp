import { useOfflinePaymentsStore } from "@/lib/store";
import {
  getOfflinePaymentById,
  saveOfflinePayment,
} from "@/services/customerProfile";
import { Dialog } from "@radix-ui/react-dialog";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import ChequeDetails from "./chequeDetails";
import {
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "./dialog";
import { Label } from "./label";
import NeftDetails from "./neftDetails";
import GatewayDetails from "./gatewayDetails";
import RButton from "./rButton";
import { Textarea } from "./textarea";
import UpiDetails from "./upiDetails";
import { toast } from "./use-toast";

export default function OfflinePaymentDialog({ open, setOpen, onPaymentAck }) {
  const paymentId = useOfflinePaymentsStore((state) => state.currentPaymentId);
  const payment = useOfflinePaymentsStore((state) => state.payment);
  const setPayment = useOfflinePaymentsStore((state) => state.setPayment);
  const currentPaymentStatus = useOfflinePaymentsStore(
    (state) => state.currentPaymentStatus
  );

  const [paymentMode, setPaymentMode] = useState("");
  const [comment, setComment] = useState("");
  const [isCommentError, setIsCommentError] = useState(false);

  useEffect(() => {
    (async () => {
      if (paymentId && open) {
        let response = await getOfflinePaymentById(paymentId);
        if (response.status === "success") {
          setPayment(response.data);
          setComment(response?.data?.comment || "");
          setPaymentMode(response.data?.paymentMode);
        } else {
          toast({
            variant: "destructive",
            title: "Something went wrong.",
            description: "Unable to get payment",
          });
        }
      }

      if (!open) {
        setComment("");
        setIsCommentError(false);
      }
    })();
  }, [paymentId, open]);

  const handleSubmit = (isAccepted) => async (event) => {
    if (!comment.length) {
      setIsCommentError(true);
    } else {
      let payload = {
        paymentId: paymentId,
        isAccepted: isAccepted,
        comment: comment,
      };
      let response = await saveOfflinePayment(payload);
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
    }
  };
const isDisable = (status ) => {
  if(status === "initiated"  ||
    status === "completed")
    {
    return false;
    }
    else
    {return true;
    }
}

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Payment Details</DialogTitle>
        </DialogHeader>
        <div>
          <div>
            {paymentMode.toLowerCase().trim() === "cheque" ? (
              <ChequeDetails payment={payment} />
            ) : null}
            {paymentMode.toLowerCase().trim() === "neft" ? (
              <NeftDetails payment={payment} />
            ) : null}
             {paymentMode.toLowerCase().trim() === "gateway" ? (
              <GatewayDetails payment={payment} />
            ) : null}
            {paymentMode.toLowerCase().trim() === "upi" ? (
              <UpiDetails payment={payment} />
            ) : null}
          </div>
          {
            <div className="mt-8">
              <Label className="mb-3">Comments</Label>
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
                className={
                  isCommentError
                    ? " ring-red-600 focus-visible:ring-offset-0 focus-visible:ring-red-600 ring-1 "
                    : "focus-visible:ring-offset-0 focus-visible:ring-1"
                }
                disabled={ isDisable(currentPaymentStatus.toLowerCase().trim())}
              />
            </div>
          }
        </div>
        <DialogFooter>
          <div>
            <RButton
              onClick={handleSubmit(true)}
              isDisabled={ isDisable(currentPaymentStatus.toLowerCase().trim())}

              //   (currentPaymentStatus.toLowerCase().trim() != "initiated"  ||
              //       currentPaymentStatus.toLowerCase().trim() != "completed")
              // }
            >
              Accept
            </RButton>
            <RButton
              className="ml-3"
              onClick={handleSubmit(false)}
              isDisabled={ isDisable(currentPaymentStatus.toLowerCase().trim())}
              // isDisabled={
              //   currentPaymentStatus.toLowerCase().trim() !== "initiated"
              //   || currentPaymentStatus.toLowerCase().trim() !== "completed"
              // }
            >
              Reject
            </RButton>
            <RButton
              className="ml-3"
              onClick={() => {
                setOpen(false);
                setIsCommentError(false);
              }}
            >
              Close
            </RButton>
          </div>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

OfflinePaymentDialog.defaultProps = {
  open: false,
  setOpen: () => {},
  onPaymentAck: () => {},
};

OfflinePaymentDialog.propTypes = {
  open: PropTypes.bool,
  setOpen: PropTypes.func,
  onPaymentAck: PropTypes.func,
};
