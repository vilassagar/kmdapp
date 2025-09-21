import { download } from "@/services/files";
import { format } from "date-fns";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { toast } from "./use-toast";

export default function UpiDetails({ payment }) {
  const [upi, setUpi] = useState(null);
  const [upiPhoto, setUpiPhoto] = useState("");

  useEffect(() => {
    (async () => {
      if (payment) {
        setUpi(payment.upi);

        let { id, name, url } = payment.upi.upiPaymentReceipt;

        // get upi photo
        if (url?.length) {
          let response = await download(id, name, url);
          if (response.status === "success") {
            let url = window.URL.createObjectURL(new Blob([response.data]));
            setUpiPhoto(url);
          } else {
            toast({
              variant: "destructive",
              title: "Error.",
              description: "Unable to get cheque photo.",
            });
          }
        } else {
          setUpiPhoto(null);
        }
      }
    })();
  }, [payment]);

  return (
    <div>
      <div className="grid grid-cols-2 gap-4">
        <div>
          <div className="font-medium">Pensioner Name</div>
          <div>{payment.pensionerName}</div>
        </div>
        <div>
          <div className="font-medium">Transaction ID</div>
          <div>{upi?.transactionId || "-"}</div>
        </div>
        <div>
          <div className="font-medium">Amount</div>
          <div>{upi?.amount || "-"}</div>
        </div>
        <div>
          <div className="font-medium">Date</div>
          <div>{upi?.date?.length ? format(upi?.date, "PPP") : null}</div>
        </div>
      </div>
      {upi?.upiPaymentReceipt?.url?.length ? (
        <div className="mt-6">
          <img
            src={upiPhoto}
            alt="Payment Screenshot"
            width={200}
            height={200}
            className="rounded-md"
          />
        </div>
      ) : null}
    </div>
  );
}

UpiDetails.defaultProps = {
  payment: null,
};

UpiDetails.propTypes = {
  payment: PropTypes.object,
};
