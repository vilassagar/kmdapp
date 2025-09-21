import WithAuthentication from "@/components/hoc/withAuthentication";
import { Button } from "@/components/ui/button";
import { toast } from "@/components/ui/use-toast";
import { updateFailedPayment } from "@/services/paymentGateway";
import { CircleX } from "lucide-react";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
function PaymentFailure() {
  const navigate = useNavigate();
  const search = window.location.search;
  const params = new URLSearchParams(search);
  const paymentSuccess = sessionStorage.getItem("payment_success");
  const parsedSuccess = JSON.parse(paymentSuccess);
  const postObject = {
    userId: parsedSuccess.userId,
    policyId: parsedSuccess?.policyId,
    transactionNumber: "",
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
    const response = await updateFailedPayment(paymentDetails);
    if (response.success === "success") {
      toast({
        variant: "",
        title: "Failure.",
        description: "",
      });
      sessionStorage.removeItem("payment_success");
    }
  };

  useEffect(() => {
    paymentUpdate(postObject);
  }, []);

  return (
    <div className="flex min-h-screen flex-col bg-gray-100 dark:bg-gray-950">
      <main className="container mx-auto flex flex-1 flex-col items-center justify-center px-4 py-12 md:px-6 lg:py-24">
        <div className="mx-auto flex max-w-md flex-col items-center justify-center gap-8">
          <div className="flex flex-col items-center gap-4">
            <CircleX className="h-16 w-16 text-red-500" />
            <h1 className="text-3xl font-bold tracking-tight text-gray-900 dark:text-gray-50">
              Payment Failed
            </h1>
            <p className="text-lg text-gray-500 dark:text-gray-400">
              We were unable to process your payment.
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

export default WithAuthentication(PaymentFailure);
