import React, { useState, useEffect } from "react";
import { CardDescription, CardHeader, CardTitle } from "./card";
import RButton from "./rButton";
import WithLayout from "../layout/WithLayout";
import { DateRangePicker } from "./dateRangePicker";
import { Combobox } from "./comboBox";
import { getAssociations } from "@/services/customerProfile";
import { produce } from "immer";
import { toast } from "./use-toast";
import { getOfflinePayments } from "@/services/reports";
import { getOfflinePaymentPostObject } from "@/lib/helperFunctions";

const OfflinePaymentsReport = () => {
  const [offlinePayments, setOfflinePayments] = useState({
    association: null,
    startDate: "",
    endDate: "",
  });
  const [associations, setAssociations] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    (async () => {
      await getAllAssociations(0);
    })();
  }, []);

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
    let nextState = produce(offlinePayments, (draft) => {
      switch (name) {
        case "association":
          draft[name] = event;
          break;

        case "dates":
          draft["startDate"] = event?.from;
          draft["endDate"] = event?.to;
          break;
      }
    });

    setOfflinePayments(nextState);
  };

  const handleDownload = async () => {
    try {
      const postObject = getOfflinePaymentPostObject(offlinePayments);
      const response = await getOfflinePayments(postObject);
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;

      link.setAttribute(
        "download",
        `report_offlinePaymentReport_${String(new Date().getDate()).padStart(
          2,
          "0"
        )}${String(new Date().getMonth() + 1).padStart(
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
        description: "Unable to get offline payment report details.",
      });
    }
  };
  return (
    <div>
      <CardHeader>
        <CardTitle>Offline Payment Report</CardTitle>
        <CardDescription>
          Download a report of offline payments, including reconciled and not
          reconciled.
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
          value={offlinePayments.association}
        />
      </div>

      <div className="p-4">
        <div className="m2-5">
          <DateRangePicker
            label="Select Date Range"
            dates={{
              from: offlinePayments?.startDate,
              to: offlinePayments?.endDate,
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

export default WithLayout(OfflinePaymentsReport);
