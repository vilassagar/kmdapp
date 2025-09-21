import React, { useState, useEffect } from "react";
import { CardDescription, CardHeader, CardTitle } from "./card";
import RButton from "./rButton";
import WithLayout from "../layout/WithLayout";
import { Combobox } from "./comboBox";
import { getAssociations } from "@/services/customerProfile";
import { produce } from "immer";
import { toast } from "./use-toast";
import { getCompletedFormsPostObject } from "@/lib/helperFunctions";
import { getCompletedForms } from "@/services/reports";
import { DatePicker } from "./datePicker";

const CompletedForms = () => {
  const [completedForm, setCompletedForm] = useState({
    association: null,
    date: "",
  });
  const [associations, setAssociations] = useState([]);

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
    let nextState = produce(completedForm, (draft) => {
      switch (name) {
        case "association":
          draft[name] = event;
          break;
        case "date":
          draft[name] = event;
          break;
      }
    });

    setCompletedForm(nextState);
  };

  const handleDownload = async () => {
    try {
      const postObject = getCompletedFormsPostObject(completedForm);
      const response = await getCompletedForms(postObject);
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;

      link.setAttribute(
        "download",
        `report_completedForm_${String(new Date().getDate()).padStart(
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
        description: "Unable to get completed forms.",
      });
    }
  };
  return (
    <div>
      <CardHeader>
        <CardTitle>Completed Forms</CardTitle>
        <CardDescription>
          Download a daily dump of all completed forms.{" "}
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
          value={completedForm.association}
        />
      </div>

      <div className="p-4">
        <div className="m2-5 md:w-4/12">
          <DatePicker
            label="Date"
            id="date"
            type="date"
            placeholder="dd-mm-yyyy"
            onChange={handleChange("date")}
            isRequired={true}
            date={completedForm.date}
            size="sm"
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

export default WithLayout(CompletedForms);
