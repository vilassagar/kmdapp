import React, { useEffect, useState } from "react";
import { CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import RButton from "@/components/ui/rButton";
import WithLayout from "@/components/layout/WithLayout";
import { produce } from "immer";
import { toast } from "@/components/ui/use-toast";
import { getExtractPensionerPaymentDetails } from "@/services/reports";
import { Loader2 } from "lucide-react";
import { Combobox } from "@/components/ui/comboBox";
import {
  getPaymentStatuses,
  getPaymentStatuss,
  getPaymentTypes,
} from "@/services/customerProfile";
import { getExtractPensionersAndPaymentsPostObject } from "@/lib/helperFunctions";
import { getCampaignsList } from "@/services/campaigns";
import { userStore } from "@/lib/store";
const ExtractPensionersAndPayments = () => {
  const [paymentstatuses, setPaymentStatuses] = useState([]);
  const [paymentTypes, setPaymentTypes] = useState([]);
  const [campaigns, setCampaigns] = useState([]);
  const [error, setError] = useState(null);
  const [report, setReport] = useState({
    paymentType: null,
    paymentStatus: null,
    campaignId: null,
  });
  const [isLoading, setIsLoading] = useState(false);
  const user = userStore((state) => state.user);
  const associationId =
    user?.userType?.name?.toLowerCase()?.trim() === "association"
      ? user?.associationId
      : 0;
  useEffect(() => {
    const initializeData = async () => {
      try {
        await Promise.all([
          fetchPaymentTypes(),
          fetchPaymentStatuses(),
          getCampaigns(),
        ]);
      } catch (error) {
        console.error("Error initializing data:", error);
        setError("Failed to initialize data");
      }
    };

    initializeData();
  }, []);

  const getCampaigns = async () => {
    try {
      const response = await getCampaignsList(0, "all");
      if (
        response.status === "success" &&
        response.data &&
        response.data.length > 0
      ) {
        setCampaigns(response.data);

        // Set the first campaign as default
        const defaultCampaign = response.data[0];
        setReport((prevReport) => ({
          ...prevReport,
          campaignId: defaultCampaign, // Set the entire campaign object as the value
        }));
      } else {
        setError("Failed to get campaigns");
      }
    } catch (error) {
      console.error("Error fetching campaigns:", error);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Unable to fetch campaigns.",
      });
    }
  };

  const fetchPaymentStatuses = async () => {
    try {
      const response = await getPaymentStatuss();
      if (response.status === "success") {
        setPaymentStatuses(response.data);
      } else {
        throw new Error("Failed to fetch payment statuses");
      }
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Something went wrong",
        description: "Unable to get payment statuses.",
      });
    }
  };

  const fetchPaymentTypes = async () => {
    try {
      const response = await getPaymentTypes();
      if (response.status === "success") {
        setPaymentTypes(response.data);
      } else {
        throw new Error("Failed to fetch payment types");
      }
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Something went wrong",
        description: "Unable to get payment types.",
      });
    }
  };

  const handleChange = (name) => (event) => {
    setReport(
      produce((draft) => {
        draft[name] = event;
      })
    );
  };

  const handleDownload = async () => {
    setIsLoading(true);
    try {
      const postObject = getExtractPensionersAndPaymentsPostObject(report);
      const response = await getExtractPensionerPaymentDetails(
        postObject.paymentTypeId,
        postObject.paymentStatusId,
        postObject.campaignId,
        associationId
      );

      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute(
        "download",
        `pensioner_and_payments_${String(new Date().getDate()).padStart(
          2,
          "0"
        )}${String(new Date().getMonth() + 1).padStart(
          2,
          "0"
        )}${new Date().getFullYear()}.xlsx`
      );
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Something went wrong",
        description: "Unable to get pensioner and payments details.",
      });
    }
    setIsLoading(false);
  };

  return (
    <div className="max-w-6xl mx-auto">
      <CardHeader className="space-y-2">
        <CardTitle>Extract Pensioners and Payments</CardTitle>
        <CardDescription>
          Download a report of all pensioner and payments
        </CardDescription>
      </CardHeader>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-1 p-4">
        <div className="space-y-2">
          <label className="text-sm font-medium">Campaign</label>
          <div className="w-48">
            <Combobox
              options={campaigns}
              placeholder="Select campaign type"
              valueProperty="id"
              labelProperty="name"
              onChange={handleChange("campaignId")}
              value={report.campaignId}
              className="w-full"
            />
          </div>
        </div>
        <div className="space-y-2">
          <label className="text-sm font-medium">Payment Type</label>
          <Combobox
            label="Payment Type"
            options={paymentTypes}
            valueProperty="id"
            labelProperty="name"
            id="paymentType"
            onChange={handleChange("paymentType")}
            value={report.paymentType}
            className="w-full"
          />
        </div>

        <div className="space-y-2">
          <label className="text-sm font-medium">Payment Status</label>
          <Combobox
            label="Payment Status"
            options={paymentstatuses}
            valueProperty="id"
            labelProperty="name"
            id="payment-status"
            onChange={handleChange("paymentStatus")}
            value={report.paymentStatus}
            className="w-full"
          />
        </div>
      </div>

      <div className="p-4">
        <RButton
          className="w-full md:w-auto"
          variant="outline"
          size="sm"
          onClick={handleDownload}
          disabled={isLoading}
        >
          {isLoading ? (
            <span className="flex items-center justify-center">
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              Please wait
            </span>
          ) : (
            <span>Download</span>
          )}
        </RButton>
      </div>

      {error && <div className="p-4 text-red-500 text-sm">{error}</div>}
    </div>
  );
};

export default WithLayout(ExtractPensionersAndPayments);
