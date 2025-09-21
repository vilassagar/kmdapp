import DownloadIcon from "@/assets/DownloadIcon";
import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import { RPagination } from "@/components/ui/RPagination";
import AcknowledgementDialog from "@/components/ui/acknowledgementDialog";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { DatePicker } from "@/components/ui/datePicker";
import RButton from "@/components/ui/rButton";
import RInput from "@/components/ui/rInput";
import { ScrollArea, ScrollBar } from "@/components/ui/scroll-area";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { toast } from "@/components/ui/use-toast";
import { userStore } from "@/lib/store";
import { cn } from "@/lib/utils";
import {
  downloadopdacknowledgement,
  getPaymentHistory,
} from "@/services/customerProfile";
import { format } from "date-fns";
import { Search } from "lucide-react";
import { useEffect, useState } from "react";

const PAGE_NAME = "payments";

function PaymentHistory() {
  const user = userStore((state) => state.user);
  const search = window.location.search;
  const params = new URLSearchParams(search);
  const userId = params.get("userId");
  const [searchTerm, setSearchTerm] = useState("");
  const [date, setDate] = useState("");
  const [paginationModel, setPaginationModel] = useState({
    pageNumber: 1,
    recordsPerPage: 50,
  });
  const [paymentHistory, setPaymentHistory] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isAcknowledgementModalOpen, setIsAcknowledgementModalOpen] =
    useState(false);
  const [currentPaymentId, setCurrentPaymentId] = useState(null);
  const [currentOrderId, setCurrentOrderId] = useState(null);

  useEffect(() => {
    (async () => {
      await getPaginatedPaymentsHistory(paginationModel, searchTerm);
    })();
  }, []);

  const getPaginatedPaymentsHistory = async (paginationModel, searchTerm) => {
    setIsLoading(true);
    let response = await getPaymentHistory(
      paginationModel.pageNumber,
      paginationModel.recordsPerPage,
      searchTerm,
      userId > 0 ? userId : user?.userId || 0
    );

    if (response.status === "success") {
      setPaymentHistory(response.data);
      setPaginationModel(response.data.paging);
    } else {
      //show error
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get payment history.",
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
      await getPaginatedPaymentsHistory(pageModel, "");
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
    await getPaginatedPaymentsHistory(pageModel, searchTerm);
  };

  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedPaymentsHistory(paging, searchTerm);
  };

  const handleDownloadReceipt = async (orderId) => {
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

  return (
    <div>
      <h1 className="text-2xl font-bold mb-6">Payment History</h1>
      <div className="mb-6 flex justify-start">
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
            {/* <Search className="h-4 w-4" /> */}Search
          </RButton>
        </div>
        <div className="flex ml-10">
          <div className="flex-1  flex items-center flex-col ">
            <DatePicker
              id="payment-date"
              type="date"
              placeholder="dd-mm-yyyy"
              onChange={(event) => {
                setDate(event);
              }}
              date={date}
            />
          </div>
          <RButton
            onClick={handleSearch}
            className="ml-10"
            isLoading={isLoading}
          >
            <div className="flex items-center">
              Search
              <Search className="h-4 w-5 ml-2" />
            </div>
          </RButton>
        </div>
      </div>

      <div className="  rounded-lg shadow-lg  w-screen md:w-11/12 ">
        <ScrollArea>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-[150px]">Payment Mode</TableHead>
                <TableHead>Payable Amount</TableHead>
                <TableHead>Date</TableHead>
                <TableHead>Amount Paid</TableHead>
                <TableHead>Order No.</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Transaction ID</TableHead>
                <TableHead className="text-right">View Ack.</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {paymentHistory?.contents?.length ? (
                paymentHistory.contents.map((payment, index) => (
                  <TableRow key={payment.paymentId}>
                    <TableCell className="font-medium">
                      {payment.paymentMode}
                    </TableCell>
                    <TableCell>₹ {payment.payableAmount}</TableCell>
                    <TableCell>
                      {payment.date?.length
                        ? format(payment.date, "PPP")
                        : null}
                    </TableCell>
                    <TableCell>₹ {payment.amountPaid}</TableCell>
                    <TableCell>{payment.orderNumber}</TableCell>
                    <TableCell>
                      {" "}
                      <Badge
                        className={cn(
                          payment.paymentStatus.toLowerCase().trim() ===
                            "initiated" && "bg-yellow-500 text-black",
                          payment.paymentStatus.toLowerCase().trim() ===
                            "completed" && "bg-green-500 text-black",
                          payment.paymentStatus.toLowerCase().trim() ===
                            "rejected" && "bg-red-500 text-white",
                          payment.paymentStatus.toLowerCase().trim() ===
                            "failed" && "bg-red-500 text-white",
                          payment.paymentStatus.toLowerCase().trim() ===
                            "pending" && "bg-yellow-500 text-black",
                          "font-bold"
                        )}
                      >
                        {payment.paymentStatus}
                      </Badge>
                    </TableCell>
                    <TableCell>{payment.transactionId}</TableCell>
                    {/* <TableCell className="text-right">
                      {payment.paymentStatus.toLowerCase().trim() ===
                        "completed" && (
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() =>
                            handleDownloadReceipt(payment.orderNumber)
                          }
                        >
                          <DownloadIcon className="h-4 w-4" />
                        </Button>
                      )}
                    </TableCell> */}
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={7} className="h-24 text-center">
                    No results.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
          <ScrollBar orientation="horizontal" />
        </ScrollArea>
      </div>

      <div className="flex justify-end">
        <RPagination
          paginationModel={paginationModel}
          onPageChange={handlePageChange}
        />
      </div>
      <AcknowledgementDialog
        orderId={currentOrderId}
        paymentId={currentPaymentId}
        open={isAcknowledgementModalOpen}
        setOpen={setIsAcknowledgementModalOpen}
      />
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(PaymentHistory))
);
