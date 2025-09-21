import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import { RPagination } from "@/components/ui/RPagination";
import NoData from "@/components/ui/noData";
import OnlinePaymentDialog from "@/components/ui/onlinePaymentDialog";
import RButton from "@/components/ui/rButton";
import RInput from "@/components/ui/rInput";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { toast } from "@/components/ui/use-toast";
import { getPolicyOrders } from "@/services/customerProfile";
import { format } from "date-fns";
import { userStore } from "@/lib/store";
import { EyeIcon, Loader2 } from "lucide-react";
import { useEffect, useState } from "react";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import SearchComponent from "@/components/ui/SearchComponent";
import { getCampaignsList } from "@/services/campaigns";
const PAGE_NAME = "offlinepayments";

function OnlinePaymentStatus() {
  const [isPaymentDialogOpen, setIsPaymentDialogOpen] = useState(false);
  const [campaigns, setCampaigns] = useState();
  const [selectedCampaign, setSelectedCampaign] = useState();
  const [error, setError] = useState(null);
  const user = userStore((state) => state.user);

  const associationId =
    user?.userType?.name?.toLowerCase()?.trim() === "association"
      ? user?.userId
      : 0;

  const [onlinepaymentstatuses, setOnlinePaymentStatuses] = useState();
  const [paginationModel, setPaginationModel] = useState({
    pageNumber: 1,
    recordsPerPage: 50,
  });

  const [isLoading, setIsLoading] = useState(true);

  const [paymentId, setPaymentId] = useState("");
  const [searchTerm, setSearchTerm] = useState("");
  // Add initial loading state for campaigns
  const [isCampaignsLoading, setIsCampaignsLoading] = useState(true);
  const getCampaigns = async () => {
    setIsCampaignsLoading(true);
    try {
      const response = await getCampaignsList(0, "all");
      if (response.status === "success" && response.data) {
        setCampaigns(response.data);
        if (response.data.length > 0) {
          setSelectedCampaign(response.data[0]);
        }
      } else {
        setError("Failed to get campaigns");
      }
    } catch (error) {
      console.log("Error", error);
      setError("Failed to fetch campaigns");
    } finally {
      setIsCampaignsLoading(false);
    }
  };

  // Initial data fetch
  useEffect(() => {
    getCampaigns();
  }, []);
  // Separate useEffect for payment statuses
  useEffect(() => {
    const fetchInitialData = async () => {
      if (!isCampaignsLoading && selectedCampaign?.id) {
        await getPaginatedPaymentStatuses(
          paginationModel,
          searchTerm,
          selectedCampaign.id
        );
      }
    };

    fetchInitialData();
  }, [isCampaignsLoading, selectedCampaign?.id]);

  const getPaginatedPaymentStatuses = async (
    paginationModel,
    searchTerm,
    campaignId
  ) => {
    setIsLoading(true);
    let response = await getPolicyOrders(
      paginationModel.pageNumber,
      paginationModel.recordsPerPage,
      searchTerm,
      user?.userId || 0,
      associationId,
      campaignId
    );

    if (response.status === "success") {
      setOnlinePaymentStatuses(response.data);
      setPaginationModel(response.data.paging);
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get payments.",
      });
    }
    setIsLoading(false);
  };

  const handleSearch = async (event) => {
    if (event.key === "Enter" || event.type === "click") {
      await handleSearchSubmit();
    } else if (searchTerm.length >= 3) {
      await handleSearchSubmit();
    } else if (searchTerm.length === 0) {
      let pageModel = {
        pageNumber: 1,
        recordsPerPage: 50,
      };

      setPaginationModel(pageModel);
      await getPaginatedPaymentStatuses(pageModel, "", selectedCampaign?.id);
    }
  };

  const handleSearchChange = (event) => {
    setSearchTerm(event.target.value);
  };

  const handleSearchSubmit = async () => {
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };

    setPaginationModel(pageModel);
    await getPaginatedPaymentStatuses(
      pageModel,
      searchTerm,
      selectedCampaign?.id
    );
  };

  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedPaymentStatuses(paging, searchTerm, selectedCampaign?.id);
  };

  const handlePaymentClick = (index) => (event) => {
    let paymentId = onlinepaymentstatuses["contents"][index]["orderId"];
    let paymentStatus = onlinepaymentstatuses["contents"][index]["status"];
    setPaymentId(paymentId);
    // setCurrentPaymentStatus(paymentStatus);
    setIsPaymentDialogOpen(true);
  };

  const handleSubmitPaymentAck = async () => {
    await getPaginatedPaymentStatuses(
      paginationModel,
      searchTerm,
      selectedCampaign?.id
    );
  };
  const handleClearSearch = async () => {
    setSearchTerm("");
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };
    setPaginationModel(pageModel);
    await getPaginatedPaymentStatuses(pageModel, "", selectedCampaign?.id);
  };
  const handleCampaignChange = (campaign) => {
    setSelectedCampaign(campaign);
  };

  if (isLoading || isCampaignsLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <Loader2 className="w-16 h-16 animate-spin" />
      </div>
    );
  }
  return (
    <div>
      <h1 className="text-2xl font-bold mb-6"></h1>

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
      {onlinepaymentstatuses?.contents?.length ? (
        <div className="overflow-x-auto rounded-lg shadow-lg ">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-[8%]">OrderId</TableHead>
                <TableHead className="w-[6%]">Amount</TableHead>
                <TableHead className="w-[6%]">Paid</TableHead>
                <TableHead className="w-[10%]">Name</TableHead>
                <TableHead className="w-[8%]">Mobile</TableHead>
                <TableHead className="w-[7%]">Status</TableHead>
                <TableHead className="w-[10%]">Org</TableHead>
                <TableHead className="w-[10%]">Assoc</TableHead>
                <TableHead className="w-[6%]">Trans#</TableHead>
                <TableHead className="w-[6%]">Date</TableHead>
                <TableHead className="w-[3%] text-right">Act</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {onlinepaymentstatuses?.contents?.map((order, index) => (
                <TableRow key={order.orderId}>
                  <TableCell className="truncate px-2">
                    {order.orderId}
                  </TableCell>
                  <TableCell className="truncate px-2">
                    {order.amount}
                  </TableCell>
                  <TableCell className="truncate px-2">
                    {order.paidAmount}
                  </TableCell>
                  <TableCell className="w-[10%] px-2">{order.name}</TableCell>
                  <TableCell className="w-[8%] px-2">
                    {order.mobileNumber}
                  </TableCell>
                  <TableCell className="w-[7%] px-2">
                    {order.status !== "" ? (
                      <Badge
                        variant="pending"
                        className={cn(
                          "truncate",
                          order?.status?.toLowerCase()?.trim() ===
                            "initiated" && "bg-yellow-400 text-black",
                          order?.status?.toLowerCase()?.trim() ===
                            "completed" && "bg-green-600 text-black",
                          order?.status?.toLowerCase()?.trim() === "rejected" &&
                            "bg-red-600 text-white"
                        )}
                      >
                        {order.status}
                      </Badge>
                    ) : (
                      <div></div>
                    )}
                  </TableCell>
                  <TableCell className="truncate px-2">
                    {order.organisationName}
                  </TableCell>
                  <TableCell className="w-[10%] px-2">
                    {order.associationName}
                  </TableCell>
                  <TableCell className="w-[6%] px-2">
                    {order.transactionNumber}
                  </TableCell>
                  <TableCell className="w-[6%] px-2">
                    {order.date?.length ? format(order.date, "PPP") : null}
                  </TableCell>
                  <TableCell className="text-right px-2">
                    <RButton
                      variant="outline"
                      size="icon"
                      onClick={handlePaymentClick(index)}
                    >
                      <EyeIcon className="w-4 h-4" />
                    </RButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      ) : (
        <NoData />
      )}

      <div className="flex justify-end">
        <RPagination
          paginationModel={paginationModel}
          onPageChange={handlePageChange}
        />
      </div>

      <OnlinePaymentDialog
        paymentId={paymentId}
        open={isPaymentDialogOpen}
        setOpen={setIsPaymentDialogOpen}
        onPaymentAck={handleSubmitPaymentAck}
      />
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(OnlinePaymentStatus))
);
