import WithAuthentication from "@/components/hoc/withAuthentication";
import { Button } from "@/components/ui/button";
import { toast } from "@/components/ui/use-toast";
import { updatePayment } from "@/services/paymentGateway";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
function PaymentSuccess() {
  const navigate = useNavigate();
  const search = window.location.search;
  const params = new URLSearchParams(search);
  const transactionNumber = params.get("transactionNumber");
  const paymentSuccess = sessionStorage.getItem("payment_success");
  const parsedSuccess = JSON.parse(paymentSuccess);
  const postObject = {
    userId: parsedSuccess.userId,
    policyId: parsedSuccess?.policyId,
    transactionNumber: transactionNumber,
    premiumAmount: parsedSuccess?.totalPremium,
  };

  const handleNavigatePayments = () => {
    const lastVisitedPensioner = JSON.parse(
      sessionStorage.getItem("lastVisitedpensioner")
    );
    if (lastVisitedPensioner?.userId) {
      navigate(`/mypolicies?userId=${lastVisitedPensioner.userId}`);
    } else {
      navigate("/mypolicies");
    }
  };

  const paymentUpdate = async (paymentDetails) => {
    const response = await updatePayment(paymentDetails);
    if (response.success === "success") {
      toast({
        variant: "",
        title: "Success.",
        description: "",
      });
      sessionStorage.removeItem("payment_success");
    }
  };

  useEffect(() => {
    const lastVisitedPensioner = JSON.parse(
      sessionStorage.getItem("lastVisitedpensioner")
    );
    paymentUpdate(postObject);
  }, []);

  return (
    <div className="flex min-h-screen flex-col bg-gray-100 dark:bg-gray-950">
      <main className="container mx-auto flex flex-1 flex-col items-center justify-center px-4 py-12 md:px-6 lg:py-24">
        <div className="mx-auto flex max-w-md flex-col items-center justify-center gap-8">
          <div className="flex flex-col items-center gap-4">
            <CircleCheckIcon className="h-16 w-16 text-green-500" />
            <h1 className="text-3xl font-bold tracking-tight text-gray-900 dark:text-gray-50">
              Payment Successful
            </h1>
            <p className="text-lg text-gray-500 dark:text-gray-400">
              Thank you for your payment. Your order is now being processed.
            </p>
          </div>
          <Button
            onClick={() => {
              handleNavigatePayments();
            }}
            className="inline-flex w-full items-center justify-center rounded-md bg-gray-900 px-4 py-2 text-sm font-medium text-gray-50 shadow transition-colors hover:bg-gray-900/90 focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-gray-950 disabled:pointer-events-none disabled:opacity-50 dark:bg-gray-50 dark:text-gray-900 dark:hover:bg-gray-50/90 dark:focus-visible:ring-gray-300"
            prefetch={false}
          >
            Go to Home
          </Button>
        </div>
      </main>
    </div>
  );
}

function CircleCheckIcon(props) {
  return (
    <svg
      {...props}
      xmlns="http://www.w3.org/2000/svg"
      width="24"
      height="24"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <circle cx="12" cy="12" r="10" />
      <path d="m9 12 2 2 4-4" />
    </svg>
  );
}

export default WithAuthentication(PaymentSuccess);
