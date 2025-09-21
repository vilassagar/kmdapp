import React, { useState, useEffect } from "react";
import { CardDescription, CardHeader, CardTitle } from "./card";
import RButton from "./rButton";
import WithLayout from "../layout/WithLayout";
import { DateRangePicker } from "./dateRangePicker";
import { Combobox } from "./comboBox";
import { getAssociations, getOrganisations } from "@/services/customerProfile";
import { produce } from "immer";
import { toast } from "./use-toast";
import { getRefundReports } from "@/services/reports";
import { getRefundReportsPostObject } from "@/lib/helperFunctions";

const GetRefundReport = () => {
  const [refundReport, setRefundReport] = useState({
    association: null,
    organisation: null,
    startDate: "",
    endDate: "",
  });
  const [associations, setAssociations] = useState([]);
  const [organisations, setOrganisations] = useState([]);

  useEffect(() => {
    (async () => {
      await getAllAssociations(0);
      await getAllOrganisations();
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

  const getAllOrganisations = async () => {
    let response = await getOrganisations();
    if (response.status === "success") {
      setOrganisations(response.data);
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get organisations.",
      });
    }
  };

  const handleChange = (name) => (event) => {
    let nextState = produce(refundReport, (draft) => {
      switch (name) {
        case "association":
        case "organisation":
          draft[name] = event;
          break;

        case "dates":
          draft["startDate"] = event?.from;
          draft["endDate"] = event?.to;
          break;
      }
    });

    setRefundReport(nextState);
  };

  const handleDownload = async () => {
    try {
      const postObject = getRefundReportsPostObject(refundReport);
      const response = await getRefundReports(postObject);
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;

      link.setAttribute(
        "download",
        `report_refundReport_${String(new Date().getDate()).padStart(
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
        <CardTitle>Refund Report</CardTitle>
        <CardDescription>
          {" "}
          Download a report of all refunds processed.
        </CardDescription>
      </CardHeader>
      <div className="p-4 w-4/12">
        <label className="text-sm">Association</label>
        <Combobox
          label="Association"
          options={associations}
          valueProperty="id"
          labelProperty="name"
          id="association-name"
          onChange={handleChange("association")}
          value={refundReport.association}
        />
      </div>
      <div className="p-4 w-4/12">
        <label className="text-sm">Organisation</label>
        <Combobox
          label="Organisation"
          options={organisations}
          valueProperty="id"
          labelProperty="name"
          id="organisation-name"
          onChange={handleChange("organisation")}
          value={refundReport.organisation}
        />
      </div>

      <div className="p-4">
        <div className="m2-5">
          <DateRangePicker
            label="Select Date Range"
            dates={{
              from: refundReport?.startDate,
              to: refundReport?.endDate,
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

export default WithLayout(GetRefundReport);
