import WithAuthentication from "@/components/hoc/withAuthentication";
import { Button } from "@/components/ui/button";
import { userStore } from "@/lib/store";
import { CircleCheckIcon } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import WithLayout from "@/components/layout/WithLayout";

function PaymentInitiated() {
  const navigate = useNavigate();
  const user = userStore((state) => state.user);
  const [typeid, setTypeid] = useState(null);

  useEffect(() => {
    // Get the typeid from URL params
    const params = new URLSearchParams(window.location.search);
    setTypeid(params.get("typeid"));
  }, []);

  const handleNavigation = () => {
    navigate("/dashboard");
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-background px-4 py-12 sm:px-6 lg:px-8">
      <div className="mx-auto w-full max-w-md space-y-8">
        <div>
          <CircleCheckIcon className="mx-auto h-12 w-12 text-green-500" />
          <h2 className="mt-4 text-3xl font-bold tracking-tight text-foreground sm:text-4xl text-center">
            {typeid ? "Payment Successful" : "Payment Initiated"}
          </h2>
          <p className="mt-2 text-center text-sm text-muted-foreground">
            {typeid
              ? "Your offline Payment is Successful"
              : "Your offline payment has been initiated. You will be notified once the payment is confirmed."}
          </p>
        </div>
        <div>
          <Button
            onClick={handleNavigation}
            className="inline-flex w-full items-center justify-center rounded-md bg-gray-900 px-4 py-2 text-sm font-medium text-gray-50 shadow transition-colors hover:bg-gray-900/90 focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-gray-950 disabled:pointer-events-none disabled:opacity-50 dark:bg-gray-50 dark:text-gray-900 dark:hover:bg-gray-50/90 dark:focus-visible:ring-gray-300"
          >
            Go to Home
          </Button>
        </div>
      </div>
    </div>
  );
}

export default WithAuthentication(PaymentInitiated);
