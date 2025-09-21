import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import { RPagination } from "@/components/ui/RPagination";
import { Badge } from "@/components/ui/badge";
import NoData from "@/components/ui/noData";
import OfflinePaymentDialog from "@/components/ui/offlinePaymentDialog";
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
import { useOfflinePaymentsStore, userStore } from "@/lib/store";
import { cn } from "@/lib/utils";
import { getOfflinePayments } from "@/services/customerProfile";
import { format } from "date-fns";
import { EyeIcon, Loader2 } from "lucide-react";
import { useEffect, useState } from "react";
import SearchComponent from "@/components/ui/SearchComponent";
import { getCampaignsList } from "@/services/campaigns";
const PAGE_NAME = "offlinepayments";

function OfflinePayments() {
  const [isPaymentDialogOpen, setIsPaymentDialogOpen] = useState(false);

  const user = userStore((state) => state.user);
  const [campaigns, setCampaigns] = useState();
  const [selectedCampaign, setSelectedCampaign] = useState();
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  const associationId =
    user?.userType?.name?.toLowerCase()?.trim() === "association"
      ? user?.userId
      : 0;

  const offlinePayments = useOfflinePaymentsStore(
    (state) => state.offlinePayments
  );
  const paginationModel = useOfflinePaymentsStore(
    (state) => state.paginationModel
  );
  const setPaginationModel = useOfflinePaymentsStore(
    (state) => state.setPaginationModel
  );
  const searchTerm = useOfflinePaymentsStore((state) => state.searchTerm);
  const setSearchTerm = useOfflinePaymentsStore((state) => state.setSearchTerm);
  const addOfflinePayments = useOfflinePaymentsStore(
    (state) => state.addOfflinePayments
  );
  const setCurrentPaymentId = useOfflinePaymentsStore(
    (state) => state.setCurrentPaymentId
  );
  const setCurrentPaymentStatus = useOfflinePaymentsStore(
    (state) => state.setCurrentPaymentStatus
  );

  const getCampaigns = async () => {
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
    }
  };

  const getPaginatedPayments = async (
    paginationModel,
    searchTerm,
    CampaignId
  ) => {
    setIsLoading(true);
    let response = await getOfflinePayments(
      paginationModel.pageNumber,
      paginationModel.recordsPerPage,
      searchTerm,
      user?.userId || 0,
      associationId,
      CampaignId
    );

    if (response.status === "success") {
      addOfflinePayments(response.data);
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
      await getPaginatedPayments(pageModel, "", selectedCampaign?.id);
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
    await getPaginatedPayments(pageModel, searchTerm, selectedCampaign?.id);
  };

  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedPayments(paging, searchTerm, selectedCampaign?.id);
  };

  const handlePaymentClick = (index) => (event) => {
    let paymentId = offlinePayments["contents"][index]["paymentId"];
    let paymentStatus = offlinePayments["contents"][index]["status"];
    setCurrentPaymentId(paymentId);
    setCurrentPaymentStatus(paymentStatus);
    setIsPaymentDialogOpen(true);
  };

  const handleSubmitPaymentAck = async () => {
    await getPaginatedPayments(
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
    await getPaginatedPayments(pageModel, "", selectedCampaign?.id);
  };
  // Handler for campaign change
  const handleCampaignChange = (campaign) => {
    setSelectedCampaign(campaign);
  };
  useEffect(() => {
    getCampaigns();
  }, []);

  useEffect(() => {
    if (selectedCampaign?.id) {
      getPaginatedPayments(paginationModel, searchTerm, selectedCampaign.id);
    }
  }, [selectedCampaign]);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <Loader2 className="w-16 h-16 animate-spin" />
      </div>
    );
  }

  return (
    <div>
      <h1 className="text-2xl font-bold mb-6">Approve/Reject Payments</h1>

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
      {offlinePayments?.contents?.length ? (
        <div className="overflow-x-auto rounded-lg shadow-lg ">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Pensioner Name</TableHead>
                <TableHead>Mobile Number</TableHead>
                <TableHead>Premium Amount</TableHead>
                <TableHead>Date</TableHead>
                <TableHead>Association Name</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Payment Mode</TableHead>
                <TableHead className="text-right">Action</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {offlinePayments?.contents?.map((payment, index) => (
                <TableRow key={payment.paymentId}>
                  <TableCell>{payment.retireeName}</TableCell>
                  <TableCell>{payment.mobileNumber}</TableCell>
                  <TableCell>{payment.amount}</TableCell>
                  <TableCell>
                    {payment.date?.length ? format(payment.date, "PPP") : null}
                  </TableCell>
                  <TableCell>{payment.associationName}</TableCell>
                  <TableCell>
                    {payment.status !== "" ? (
                      <Badge
                        variant="pending"
                        className={cn(
                          payment?.status?.toLowerCase()?.trim() ===
                            "initiated" && "bg-yellow-400 text-black",
                          payment?.status?.toLowerCase()?.trim() ===
                            "completed" && "bg-green-600 text-black",
                          payment?.status?.toLowerCase()?.trim() ===
                            "rejected" && "bg-red-600 text-white"
                        )}
                      >
                        {payment.status}
                      </Badge>
                    ) : (
                      <div></div>
                    )}
                  </TableCell>
                  <TableCell>{payment.paymentMode}</TableCell>
                  <TableCell className="text-right">
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

      <OfflinePaymentDialog
        open={isPaymentDialogOpen}
        setOpen={setIsPaymentDialogOpen}
        onPaymentAck={handleSubmitPaymentAck}
      />
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(OfflinePayments))
);
