import { download } from "@/services/files";
import { format } from "date-fns";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { toast } from "./use-toast";

export default function GatewayDetails({ payment }) {
  const [gateway, setgateway] = useState(null);


  useEffect(() => {
    (async () => {
      if (payment) {
        setgateway(payment.gateway);
       
       

        
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
          <div className="font-medium">Transaction Number</div>
          <div>{gateway?.transactionNumber || "-"}</div>
        </div>
     
        <div>
          <div className="font-medium">Amount</div>
          <div>{gateway?.amount || "-"}</div>
        </div>
        <div>
          <div className="font-medium">Date</div>
          <div>{gateway?.date?.length ? format(gateway?.date, "PPP") : null}</div>
        </div>
      </div>
     
    </div>
  );
}

GatewayDetails.defaultProps = {
  payment: null,
};

GatewayDetails.propTypes = {
  payment: PropTypes.object,
};
