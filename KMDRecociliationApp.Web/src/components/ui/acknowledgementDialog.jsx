import { getPaymentById } from "@/services/customerProfile";
import { Dialog } from "@radix-ui/react-dialog";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { Button } from "./button";
import ChequeDetails from "./chequeDetails";
import {
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "./dialog";
import NeftDetails from "./neftDetails";
import UpiDetails from "./upiDetails";
import { toast } from "./use-toast";

export default function AcknowledgementDialog({ paymentId, open, setOpen }) {
  const [paymentMode, setPaymentMode] = useState("");
  const [payment, setPayment] = useState(null);

  useEffect(() => {
    (async () => {
      if (paymentId && open) {
        let response = await getPaymentById(paymentId);
        if (response.status === "success") {
          setPayment(response.data);
          setPaymentMode(response.data?.paymentMode.name);
        } else {
          toast({
            variant: "destructive",
            title: "Something went wrong.",
            description: "Unable to get payment",
          });
        }
      }
    })();
  }, [paymentId, open]);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Payment Acknowledgement</DialogTitle>
        </DialogHeader>
        <div>
          <div>
            {paymentMode.toLowerCase().trim() === "cheque" ? (
              <ChequeDetails payment={payment} />
            ) : null}
            {paymentMode.toLowerCase().trim() === "neft" ? (
              <NeftDetails payment={payment} />
            ) : null}
            {paymentMode.toLowerCase().trim() === "upi" ? (
              <UpiDetails payment={payment} />
            ) : null}
          </div>
        </div>
        <DialogFooter>
          <div>
            <Button
              className="ml-3"
              onClick={() => {
                setOpen(false);
              }}
            >
              Close
            </Button>
          </div>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

AcknowledgementDialog.defaultProps = {
  paymentId: "",
  open: false,
  setOpen: () => {},
};

AcknowledgementDialog.propTypes = {
  paymentId: PropTypes.string,
  open: PropTypes.bool,
  setOpen: PropTypes.func,
};
