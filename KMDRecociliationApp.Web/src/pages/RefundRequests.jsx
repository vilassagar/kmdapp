import { useState, useEffect } from "react";
import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import { Badge } from "@/components/ui/badge";
import RInput from "@/components/ui/rInput";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { EyeIcon, Loader2 } from "lucide-react";
import { getRefundRequests } from "@/services/customerProfile";
import { useNavigate } from "react-router-dom";
import { RPagination } from "@/components/ui/RPagination";
import { toast } from "@/components/ui/use-toast";
import { cn } from "@/lib/utils";
import NoData from "@/components/ui/noData";
import RButton from "@/components/ui/rButton";
import { userStore } from "@/lib/store";
import SearchComponent from "@/components/ui/SearchComponent";
import { getCampaignsList } from "@/services/campaigns";

const PAGE_NAME = "refundrequests";

function RefundRequest() {
  const navigate = useNavigate();

  const user = userStore((state) => state.user);
  const [campaigns, setCampaigns] = useState();
  const [selectedCampaign, setSelectedCampaign] = useState();
  const associationId =
    user?.userType?.name?.toLowerCase()?.trim() === "association"
      ? user?.userId
      : 0;

  const [search, setSearch] = useState("");
  const [paginationModel, setPaginationModel] = useState({
    pageNumber: 1,
    recordsPerPage: 50,
  });
  const [refundRequests, setRefundRequests] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [error, setError] = useState(null);

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
        await getPaginatedRefundRequests(
          paginationModel,
          searchTerm,
          selectedCampaign.id
        );
      }
    };

    fetchInitialData();
  }, [isCampaignsLoading, selectedCampaign?.id]);

  const getPaginatedRefundRequests = async (
    paginationModel,
    searchTerm,
    campaignId
  ) => {
    let response = await getRefundRequests(
      paginationModel.pageNumber,
      paginationModel.recordsPerPage,
      searchTerm,
      associationId,
      campaignId
    );

    if (response.status === "success") {
      setRefundRequests(response.data.contents);
      setPaginationModel(response.data.paging);
      setIsLoading(false);
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get refund requests.",
      });
    }
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
      await getPaginatedRefundRequests(pageModel, "", selectedCampaign?.id);
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
    await getPaginatedRefundRequests(
      pageModel,
      searchTerm,
      selectedCampaign?.id
    );
  };

  const handleViewDetails = async (requestId) => {
    navigate(`/refunddetails/${requestId}`);
  };

  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedRefundRequests(paging, search, selectedCampaign?.id);
  };

  useEffect(() => {
    if (selectedCampaign?.id) {
      (async () => {
        await getPaginatedRefundRequests(
          paginationModel,
          search,
          selectedCampaign?.id
        );
      })();
    }
  }, [selectedCampaign]);

  const handleClearSearch = async () => {
    setSearchTerm("");
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };
    setPaginationModel(pageModel);
    await getPaginatedRefundRequests(paginationModel, "", selectedCampaign?.id);
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
  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <div>
      <div>
        <h1 className="text-2xl font-bold mb-6">Refund Requests</h1>
      </div>

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
      {refundRequests?.length ? (
        <div className="rounded-lg shadow-lg">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Refund Request Number</TableHead>
                <TableHead>Order Number</TableHead>
                <TableHead>Pensioner Name</TableHead>
                <TableHead>Refund Amount</TableHead>
                <TableHead>Refund Request Date</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Action</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {refundRequests?.map((request) => (
                <TableRow key={request.refundRequestNumber}>
                  <TableCell className="font-medium">
                    {request.refundRequestNumber}
                  </TableCell>
                  <TableCell>{request.orderNumber}</TableCell>
                  <TableCell>{request.retireeName}</TableCell>
                  <TableCell className="text-right">
                    ₹{request.refundAmount.toFixed(2)}
                  </TableCell>
                  <TableCell>
                    {new Date(request.refundRequestDate).toLocaleDateString()}
                  </TableCell>
                  <TableCell>
                    <Badge
                      className={cn(
                        request?.status?.toLowerCase()?.trim() ===
                          "initiated" && "bg-yellow-400 text-black",
                        request?.status?.toLowerCase()?.trim() ===
                          "completed" && "bg-green-600 text-black",
                        request?.status?.toLowerCase()?.trim() === "rejected" &&
                          "bg-red-600 text-white"
                      )}
                    >
                      {request.status}
                    </Badge>
                  </TableCell>

                  <TableCell>
                    <RButton
                      variant="ghost"
                      size="icon"
                      onClick={() =>
                        handleViewDetails(request.refundRequestNumber)
                      }
                    >
                      <EyeIcon className="h-5 w-5" />
                      <span className="sr-only">View details</span>
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
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(RefundRequest))
);
