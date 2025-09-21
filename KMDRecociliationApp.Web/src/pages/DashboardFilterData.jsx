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
import { getDashboardFilterData } from "@/services/dashboard";
import { format } from "date-fns";
import { EyeIcon, Loader2 } from "lucide-react";
import { useEffect, useState } from "react";
import { cn } from "@/lib/utils";
import { userStore } from "@/lib/store";

const PAGE_NAME = "DashboardFilterData";

function DashboardFilterData() {
  const search = window.location.search;
  const params = new URLSearchParams(search);
  const baseFilter = params.get("filter");
  const associationId = params.get("associationId");
  const campaignId = params.get("campaignId");
  const user = userStore((state) => state.user);
  const [offlinePayments, setOfflinePayments] = useState({
    contents: [],
    paging: {
      pageNumber: 1,
      recordsPerPage: 50,
      totalPages: 0,
      totalRecords: 0,
    },
  });

  const [paginationModel, setPaginationModel] = useState({
    pageNumber: 1,
    recordsPerPage: 50,
  });

  const [searchTerm, setSearchTerm] = useState("");

  const [isPaymentDialogOpen, setIsPaymentDialogOpen] = useState(false);
  const [currentPaymentId, setCurrentPaymentId] = useState(null);
  const [currentPaymentStatus, setCurrentPaymentStatus] = useState(null);

  const [isLoading, setIsLoading] = useState(true);

  
  useEffect(() => {}, []);

  useEffect(() => {
    getPaginatedPayments(paginationModel, searchTerm);
  }, []);

  const getPaginatedPayments = async (paginationModel, searchTerm) => {
    setIsLoading(true);
    try {
      let response = await getDashboardFilterData(
        paginationModel.pageNumber,
        paginationModel.recordsPerPage,
        searchTerm,
        baseFilter,
        campaignId,associationId
      );

      if (response.status === "success") {
        setOfflinePayments(response.data);
        setPaginationModel(response.data.paging);
      } else {
        toast({
          variant: "destructive",
          title: "Something went wrong.",
          description: "Unable to get payments.",
        });
      }
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Error",
        description: "Failed to fetch payments.",
      });
    } finally {
      setIsLoading(false);
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
      await getPaginatedPayments(pageModel, "");
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
    await getPaginatedPayments(pageModel, searchTerm);
  };

  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedPayments(paging, searchTerm);
  };

  const handlePaymentClick = (index) => (event) => {
    let paymentId = offlinePayments["contents"][index]["paymentId"];
    let paymentStatus = offlinePayments["contents"][index]["status"];
    setCurrentPaymentId(paymentId);
    setCurrentPaymentStatus(paymentStatus);
    setIsPaymentDialogOpen(true);
  };

  const handleSubmitPaymentAck = async () => {
    await getPaginatedPayments(paginationModel, searchTerm);
  };

  const handleClearSearch = async () => {
    setSearchTerm("");
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };
    setPaginationModel(pageModel);
    await getPaginatedPayments(pageModel, "");
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <Loader2 className="w-16 h-16 animate-spin" />
      </div>
    );
  }

  return (
    <div>
      <h1 className="text-2xl font-bold mb-6">Dashboard Filter Data</h1>
      <div className="flex items-center justify-between mb-6">
        <div className="flex flex-1 mr-4">
          <RInput
            type="search"
            placeholder="Search offline payments..."
            value={searchTerm}
            onChange={handleSearchChange}
            onKeyPress={handleSearch}
            className="w-full max-w-md"
          />
          <RButton onClick={handleSearch} className="ml-2">
            Search
          </RButton>
          <RButton onClick={handleClearSearch} className="ml-2">
            Clear Search
          </RButton>
        </div>
      </div>
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
  WithPermission(PAGE_NAME)(WithLayout(DashboardFilterData))
);
