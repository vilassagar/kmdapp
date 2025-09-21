import { download } from "@/services/files";
import { format } from "date-fns";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { toast } from "./use-toast";

export default function NeftDetails({ payment }) {
  const [neft, setNeft] = useState(null);
  const [neftPhoto, setNeftPhoto] = useState("");

  useEffect(() => {
    (async () => {
      if (payment) {
        setNeft(payment.neft);
        let { id, name, url } = payment.neft.neftPaymentReceipt;

        // get cheque photo

        if (url?.length) {
          let response = await download(id, name, url);
          if (response.status === "success") {
            let url = window.URL.createObjectURL(new Blob([response.data]));
            setNeftPhoto(url);
          } else {
            toast({
              variant: "destructive",
              title: "Error.",
              description: "Unable to get cheque photo.",
            });
          }
        } else {
          setNeftPhoto(null);
        }
      }
    })();
  }, [payment]);

  return (
    <div>
      <div className="grid grid-cols-2 gap-4">
        <div>
          <div className="font-medium">Pensioner Name</div>
          <div>{payment.retireeName}</div>
        </div>
        <div>
          <div className="font-medium">Bank Name</div>
          <div>{neft?.bankName || "-"}</div>
        </div>
        <div>
          <div className="font-medium">Branch Name</div>
          <div>{neft?.branchName || "-"}</div>
        </div>

        <div>
          <div className="font-medium">Account Number</div>
          <div>{neft?.accountNumber || "-"}</div>
        </div>
        <div>
          <div className="font-medium">Account Name</div>
          <div>{neft?.accountName || "-"}</div>
        </div>

        <div>
          <div className="font-medium">IFSC Code</div>
          <div>{neft?.ifscCode || "-"}</div>
        </div>
        <div>
          <div className="font-medium">Transaction ID</div>
          <div>{neft?.transactionId || "-"}</div>
        </div>
        <div>
          <div className="font-medium">Amount</div>
          <div>{neft?.amount || "-"}</div>
        </div>
        <div>
          <div className="font-medium">Date</div>
          <div>{neft?.date?.length ? format(neft?.date, "PPP") : null}</div>
        </div>
      </div>
      {neft?.neftPaymentReceipt?.url?.length ? (
        <div className="mt-6">
          <img
            src={neftPhoto}
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

NeftDetails.defaultProps = {
  payment: null,
};

NeftDetails.propTypes = {
  payment: PropTypes.object,
};
