import React, { useState, useEffect } from "react";
import { CardDescription, CardHeader, CardTitle } from "./card";
import RButton from "./rButton";
import WithLayout from "../layout/WithLayout";
import { DateRangePicker } from "./dateRangePicker";
import { Combobox } from "./comboBox";
import {
  getAssociations,
  getPaymentStatuses,
} from "@/services/customerProfile";
import { produce } from "immer";
import { toast } from "./use-toast";
import { getAssociationWisePaymentDetails } from "@/services/reports";
import { getAssociationWisePaymentDetailsPostObject } from "@/lib/helperFunctions";

const AssociationWisePaymentDetails = () => {
  const [associationInfo, setAssociationInfo] = useState({
    association: null,
    paymentStatus: null,
    startDate: "",
    endDate: "",
  });
  const [associations, setAssociations] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [paymentStatuses, setPaymentStatuses] = useState([]);

  useEffect(() => {
    (async () => {
      await getAllAssociations(0);
      await fetchPaymentStatuses();
    })();
  }, []);

  const fetchPaymentStatuses = async () => {
    let response = await getPaymentStatuses();
    if (response.status === "success") {
      setPaymentStatuses(response.data);
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get payment statuses.",
      });
    }
  };
  const getAllAssociations = async (orgId) => {
    let response = await getAssociations(orgId);
    if (response.status === "success") {
      setAssociations(response.data);
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get associations.",
      });
    }
  };

  const handleChange = (name) => (event) => {
    let nextState = produce(associationInfo, (draft) => {
      switch (name) {
        case "association":
          draft[name] = event;
          break;

        case "paymentStatus":
          draft[name] = event;
          break;

        case "dates":
          draft["startDate"] = event?.from;
          draft["endDate"] = event?.to;
          break;
      }
    });

    setAssociationInfo(nextState);
  };

  const handleDownload = async () => {
    try {
      const postObject =
        getAssociationWisePaymentDetailsPostObject(associationInfo);
      const response = await getAssociationWisePaymentDetails(postObject);
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;

      link.setAttribute(
        "download",
        `report_associationWisePaymentDetails_${String(
          new Date().getDate()
        ).padStart(2, "0")}${String(new Date().getMonth() + 1).padStart(
          2,
          "0"
        )}${new Date().getFullYear()}.xlsx`
      );
      document.body.appendChild(link);
      link.click();
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get association wise payment details.",
      });
    }
  };
  return (
    <div>
      <CardHeader>
        <CardTitle>Association Wise Payment Details</CardTitle>
        <CardDescription>
          Download a report of payment details by association, including options
          chosen and premium.
        </CardDescription>
      </CardHeader>
      <div className="p-4 w-4/12">
        <label className="text-sm">Associations</label>
        <Combobox
          label="Association"
          options={associations}
          valueProperty="id"
          labelProperty="name"
          id="association-name"
          onChange={handleChange("association")}
          value={associationInfo.association}
        />
      </div>

      <div className="p-4 w-4/12">
        <label className="text-sm">
          <span>Payment Status</span>
        </label>
        <Combobox
          label="Payment Status"
          options={paymentStatuses}
          valueProperty="id"
          labelProperty="name"
          id="payment-status"
          onChange={handleChange("paymentStatus")}
          value={associationInfo.paymentStatus}
        />
      </div>

      <div className="p-4">
        <div className="m2-5">
          <DateRangePicker
            label="Select Date Range"
            dates={{
              from: associationInfo?.startDate,
              to: associationInfo?.endDate,
            }}
            onChange={handleChange("dates")}
            size=""
            isRequired={true}
          />
        </div>
        <RButton
          className="mt-4"
          variant="outline"
          size="sm"
          onClick={handleDownload}
        >
          <span>Download</span>
        </RButton>
      </div>
    </div>
  );
};

export default WithLayout(AssociationWisePaymentDetails);
