import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithRole from "@/components/hoc/withRole";
import WithLayout from "@/components/layout/WithLayout";
import { RPagination } from "@/components/ui/RPagination";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import NoPolicy from "@/components/ui/noPolicy";
import RButton from "@/components/ui/rButton";
import { toast } from "@/components/ui/use-toast";
import { getPaymentReceiptDoc } from "@/lib/helperFunctions";
import { usePermissionStore, usePolicyStore, userStore } from "@/lib/store";
import { cn } from "@/lib/utils";
import {
  getMyPolicies,
  getPaymentReceipt,
  downloadopdacknowledgement,
  freezPolicyOrder,
} from "@/services/customerProfile";

import { Separator } from "@radix-ui/react-dropdown-menu";
import { format } from "date-fns";
import jsPDF from "jspdf";
import {
  CircleArrowLeft,
  CirclePlus,
  DownloadIcon,
  FileEdit,
} from "lucide-react";
import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import SearchComponent from "@/components/ui/SearchComponent";
import { getcampaignlistbyuserid } from "@/services/campaigns";

const PAGE_NAME = "mypolicies";

function MyPolicies() {
  const search = window.location.search;
  const params = new URLSearchParams(search);
  const userId = params.get("userId");
  const navigate = useNavigate();
  const user = userStore((state) => state.user);
  console.log(user?.is_SystemAdmin);
  const [isLoading, setIsLoading] = useState(true);
  const [campaigns, setCampaigns] = useState([]);
  const [selectedCampaign, setSelectedCampaign] = useState(null);
  const [error, setError] = useState(null);
  const permissions = usePermissionStore((state) => state.permissions);

  const mode = "edit";
  const setMode = usePolicyStore((state) => state.setMode);
  const updatePolicyId = usePolicyStore((state) => state.updatePolicyId);

  const [paginationModel, setPaginationModel] = useState({
    pageNumber: 1,
    recordsPerPage: 50,
  });
  const [searchTerm, setSearchTerm] = useState("");
  const [policies, setPolicies] = useState(null);

  const getCampaigns = async () => {
    setIsLoading(true);
    try {
      const response = await getcampaignlistbyuserid(
        userId > 0 ? userId : user?.userId,
        user.associationId
      );
      if (response.status === "success" && response.data) {
        setCampaigns(response.data);
        if (response.data.length > 0) {
          const firstCampaign = response.data[0];
          setSelectedCampaign(firstCampaign);
          // Fetch initial policies with the first campaign
          await getPaginatedPolicies(
            paginationModel,
            searchTerm,
            firstCampaign.id
          );
        }
      } else {
        setError("Failed to get campaigns");
      }
    } catch (error) {
      console.error("Error fetching campaigns:", error);
      setError("Failed to fetch campaigns");
    } finally {
      setIsLoading(false);
    }
  };

  const getPaginatedPolicies = async (
    paginationModel,
    searchTerm,
    campaignId
  ) => {
    if (!campaignId) return;

    try {
      let response = await getMyPolicies(
        paginationModel.pageNumber,
        paginationModel.recordsPerPage,
        searchTerm,
        userId ? userId : userId > 0 ? userId : user?.userId,
        campaignId
      );

      if (response.status === "success") {
        setPolicies(response.data);
        setPaginationModel(response.data.paging);
      } else {
        toast({
          variant: "destructive",
          title: "Something went wrong.",
          description: "Unable to get policies.",
        });
      }
    } catch (error) {
      console.error("Error fetching policies:", error);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to fetch policies. Please try again.",
      });
    }
  };

  const handleSearch = async (event) => {
    const pageModel = {
      ...paginationModel,
      pageNumber: 1,
      recordsPerPage: 50,
    };
    setPaginationModel(pageModel);
    setSearchTerm(event.target.value);
    await getPaginatedPolicies(
      pageModel,
      event.target.value,
      selectedCampaign?.id
    );
  };

  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedPolicies(paging, searchTerm, selectedCampaign?.id);
  };

  const handleEditOrder = (index) => (event) => {
    let orderId = policies["contents"][index]["orderId"];
    setMode("edit");
    updatePolicyId(orderId);
    if (userId) {
      navigate(
        `/productlist?campaignId=${selectedCampaign?.id}&userId=${userId}`
      );
    } else {
      navigate(`/productlist?campaignId=${selectedCampaign?.id}`);
    }
  };

  const handleDownloadReceipt = (orderId) => async (event) => {
    try {
      const response = await downloadopdacknowledgement(orderId);

      // Create a Blob from the response
      const blob = new Blob([response.data], { type: "application/pdf" });

      // Create a link element and trigger the download
      const link = document.createElement("a");
      link.href = window.URL.createObjectURL(blob);
      link.download = "OPDAcknowledgement.pdf";
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    } catch (error) {
      console.error("Error downloading the PDF", error);
    }
  };

  const handleSearchChange = (event) => {
    setSearchTerm(event.target.value);
  };

  const handleClearSearch = async () => {
    setSearchTerm("");
    const pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };
    setPaginationModel(pageModel);
    await getPaginatedPolicies(pageModel, "", selectedCampaign?.id);
  };

  // Handler for campaign change
  const handleCampaignChange = async (campaign) => {
    // Call getPaginatedPolicies directly with the new campaign id
    await getPaginatedPolicies(paginationModel, searchTerm, campaign.id);
    // Update the selected campaign state after fetching policies
    setSelectedCampaign(campaign);
  };

  useEffect(() => {
    getCampaigns();
  }, []);

  return (
    <article className="overflow-x-hidden prose prose-gray px-4 md:px-0 dark:prose-invert">
      <div>
        <div className="flex justify-between items-center">
          <h1 className="text-2xl font-bold mb-6">My Policies</h1>
          {userId ? (
            <CircleArrowLeft onClick={() => navigate(`/users`)} />
          ) : null}
        </div>

        <div className="space-y-4">
          <div className="flex items-center justify-between mb-10">
            {!isLoading && campaigns.length > 0 ? (
              <SearchComponent
                campaigns={campaigns}
                searchTerm={searchTerm}
                selectedCampaign={selectedCampaign}
                onCampaignChange={handleCampaignChange}
                onSearchChange={handleSearchChange}
                onSearch={handleSearch}
                onClearSearch={handleClearSearch}
                placeholder="Search offline payments..."
              />
            ) : null}

            {permissions?.[PAGE_NAME]?.create && !userId ? (
              <RButton
                onClick={() => {
                  setMode("new");
                  updatePolicyId(0);
                  navigate("/productlist");
                }}
                className="ml-10"
              >
                <span className="flex items-center">
                  Buy Policy
                  <CirclePlus className="ml-2 h-4 w-4" />
                </span>
              </RButton>
            ) : null}
          </div>

          {isLoading ? (
            <div className="text-center py-4">Loading...</div>
          ) : error ? (
            <div className="text-red-500 text-center py-4">{error}</div>
          ) : policies?.contents?.length ? (
            policies.contents.map((order, orderIndex) => (
              <Card
                className="overflow-hidden shadow-lg border-0 mt-5"
                key={order.orderId}
              >
                <CardHeader className="flex flex-row items-start bg-gray-100">
                  <div className="grid gap-0.5">
                    <CardTitle className="group flex items-center gap-2 text-lg">
                      Order : {order.orderId}
                      <Button
                        size="icon"
                        variant="outline"
                        className="h-6 w-6 opacity-0 transition-opacity group-hover:opacity-100"
                      >
                        <div className="h-3 w-3" />
                        <span className="sr-only">Copy Order ID</span>
                      </Button>
                      {order?.paymentStatus?.toLowerCase()?.trim() ===
                      "completed" ? (
                        <div className="ml-10">
                          Payment Mode:{" "}
                          <span className="font-light">
                            {order?.paymentMode}/{order.paymentType}
                          </span>
                        </div>
                      ) : null}
                      <div className="ml-5">
                        Date:{" "}
                        <span className="font-light">
                          {order?.orderDate?.length
                            ? format(order?.orderDate, "PPP")
                            : null}
                        </span>
                      </div>
                    </CardTitle>
                  </div>
                  <div className="ml-auto flex items-center gap-1">
                    {/* {order?.paymentStatus?.toLowerCase()?.trim() ===
                    "completed" ? (
                      <Button
                        size="sm"
                        className="text-xs ml-5 mr-5"
                        onClick={handleDownloadReceipt(order?.orderId)}
                      >
                        <DownloadIcon className="h-4 w-4 cursor-pointer" />
                      </Button>
                    ) : null} */}

                    <Badge
                      className={cn(
                        order.paymentStatus.toLowerCase().trim() ===
                          "initiated" && "bg-yellow-500 text-black",
                        order.paymentStatus.toLowerCase().trim() ===
                          "completed" && "bg-green-500 text-black",
                        order.paymentStatus.toLowerCase().trim() ===
                          "rejected" && "bg-red-500 text-white",
                        order.paymentStatus.toLowerCase().trim() === "failed" &&
                          "bg-red-500 text-white",
                        order.paymentStatus.toLowerCase().trim() ===
                          "pending" && "bg-yellow-500 text-black",
                        "font-bold"
                      )}
                    >
                      {order.paymentStatus}
                    </Badge>

                    {(permissions?.[PAGE_NAME]?.update &&
                      ["rejected", "pending", "failed"].includes(
                        order?.paymentStatus?.toLowerCase()?.trim()
                      )) ||
                    user?.is_SystemAdmin ? (
                      <RButton
                        size="icon"
                        variant="outline"
                        className="h-8 w-16 ml-3 rounded border border-red-900 hover:bg-red-900 hover:text-white transition-colors duration-200 flex items-center justify-center text-red-900 text-sm font-bold"
                        onClick={handleEditOrder(orderIndex)}
                      >
                        {" "}
                        Edit
                        {/* <FileEdit className="h-4 w-4" /> */}
                      </RButton>
                    ) : // <RButton
                    //   onClick={() => handleEditOrder(orderIndex)}
                    //   className="h-8 w-16 ml-3 rounded border border-red-900 hover:bg-red-900 hover:text-white transition-colors duration-200 flex items-center justify-center text-red-900 text-sm font-bold"
                    // >
                    //   Edit
                    // </RButton>
                    null}
                  </div>
                </CardHeader>
                <CardContent className="p-6 text-sm">
                  <div className="grid gap-3">
                    <div className="font-semibold">Order Details</div>
                    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                      {order.policies.map((policy) => (
                        <div
                          key={policy.productId}
                          className="rounded-lg border bg-background p-4 shadow-sm"
                        >
                          <div className="flex items-center justify-between">
                            <div className="flex items-center gap-2">
                              <span className="font-medium">
                                {policy.productName}
                              </span>
                            </div>
                            <div className="text-muted-foreground">
                              Sum Insured: ₹ {policy.sumInsured}
                            </div>
                          </div>
                          <div className="mt-2 flex items-center justify-between">
                            <div className="text-muted-foreground">
                              Premium: ₹ {policy.premium}
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                    <Separator className="my-2" />
                    <div className="flex items-center justify-start font-semibold">
                      <span className="text-muted-foreground">
                        Total Premium:
                      </span>
                      <span className="font-bold">
                        ₹ {order.totalPremium.toFixed(2)}
                      </span>
                    </div>
                    <div className="flex items-center justify-start font-semibold">
                      <span className="text-muted-foreground">
                        Amount Paid:
                      </span>
                      <span className="font-bold">
                        ₹ {order.amountPaid.toFixed(2)}
                      </span>
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))
          ) : (
            <NoPolicy />
          )}
        </div>
      </div>
      <div className="flex justify-end">
        {policies?.contents?.length ? (
          <RPagination
            paginationModel={paginationModel}
            onPageChange={handlePageChange}
          />
        ) : null}
      </div>
    </article>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(MyPolicies))
);
