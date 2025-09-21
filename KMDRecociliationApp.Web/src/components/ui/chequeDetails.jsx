import { download } from "@/services/files";
import { format } from "date-fns";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { toast } from "./use-toast";

export default function ChequeDetails({ payment }) {
  const [cheque, setCheque] = useState(null);
  const [chequePhoto, setChequePhoto] = useState("");

  useEffect(() => {
    (async () => {
      if (payment) {
        setCheque(payment.chequeDetails);
        let { id, name, url } = payment.chequeDetails.chequePhoto;

        // get cheque photo
        if (url?.length) {
          let response = await download(id, name, url);
          if (response.status === "success") {
            let url = window.URL.createObjectURL(new Blob([response.data]));
            setChequePhoto(url);
          } else {
            toast({
              variant: "destructive",
              title: "Error.",
              description: "Unable to get cheque photo.",
            });
          }
        } else {
          setChequePhoto(null);
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
          <div className="font-medium">Cheque Number</div>
          <div>{cheque?.chequeNumber || "-"}</div>
        </div>
        <div>
          <div className="font-medium">Bank Name</div>
          <div>{cheque?.bankName || "-"}</div>
        </div>
        <div>
          <div className="font-medium">IFSC Code</div>
          <div>{cheque?.ifsccode || "-"}</div>
        </div>
        <div>
          <div className="font-medium">MICR Code</div>
          <div>{cheque?.micrcode || "-"}</div>
        </div>
        <div>
          <div className="font-medium">Amount</div>
          <div>{cheque?.amount || "-"}</div>
        </div>
        <div>
          <div className="font-medium">Date</div>
          <div>{cheque?.date?.length ? format(cheque?.date, "PPP") : null}</div>
        </div>

        <div>
          <div className="font-medium">In Favour Of</div>
          <div>{cheque?.inFavourOf?.accountName || "-"}</div>
        </div>

        <div>
          <div className="font-medium">Cheque Deposit Location</div>
          <div>{cheque?.chequeDepositLocation || "-"}</div>
        </div>
      </div>
      {chequePhoto !== null ? (
        <div className="mt-6">
          <img
            src={chequePhoto}
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

ChequeDetails.defaultProps = {
  payment: null,
};

ChequeDetails.propTypes = {
  payment: PropTypes.object,
};
