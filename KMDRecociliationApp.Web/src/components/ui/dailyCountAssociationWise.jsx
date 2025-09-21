import React, { useState } from "react";
import { CardDescription, CardHeader, CardTitle } from "./card";
import { DatePicker } from "./datePicker";
import RButton from "./rButton";
import { getDailyCountAssociationWise } from "@/services/reports";
import WithLayout from "../layout/WithLayout";
import { toast } from "./use-toast";
import { getCurrentTimeZoneIsoStringFromDate } from "@/lib/helperFunctions";

const DailyCountAssociationWise = () => {
  const [date, setDate] = useState("");

  const handleChange = (event) => {
    setDate(event);
  };

  const handleDownload = async () => {
    try {
      const newDate = getCurrentTimeZoneIsoStringFromDate(
        new Date(date)
      ).toISOString();
      const response = await getDailyCountAssociationWise(newDate);
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute(
        "download",
        `report_dailyCountAssociationWise_${String(
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
        description: "Unable to get daily count association wise.",
      });
    }
  };

  return (
    <div>
      <div>
        <CardHeader>
          <CardTitle>Daily Count Association-Wise</CardTitle>
          <CardDescription>
            Download a report of daily counts by association, including options
            chosen and premium.
          </CardDescription>
        </CardHeader>
        <div className="p-4 md:w-4/12">
          <DatePicker
            label="Date"
            id="date"
            type="date"
            placeholder="dd-mm-yyyy"
            onChange={handleChange}
            isRequired={true}
            date={date || ""}
            size="sm"
          />
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
    </div>
  );
};

export default WithLayout(DailyCountAssociationWise);
