import WithAuthentication from "@/components/hoc/withAuthentication";
import { CircleCheckIcon } from "lucide-react";
import { Button } from "@/components/ui/button";

const RefundInitiated = () => {
  return (
    <div className="flex min-h-screen items-center justify-center bg-background px-4 py-12 sm:px-6 lg:px-8">
      <div className="mx-auto w-full max-w-md space-y-8">
        <div>
          <CircleCheckIcon className="mx-auto h-12 w-12 text-green-500" />
          <h2 className="mt-4 text-3xl font-bold tracking-tight text-foreground sm:text-4xl text-center">
            Your refund request has been initiated
          </h2>
          <p className="mt-2 text-center text-sm text-muted-foreground">
            We have received your refund request and it is being processed. You
            will receive a confirmation once the refund is complete.
          </p>
        </div>
        <div>
          <Button
            onClick={() => {
              window.location.href = "/";
            }}
            className="inline-flex w-full items-center justify-center rounded-md bg-gray-900 px-4 py-2 text-sm font-medium text-gray-50 shadow transition-colors hover:bg-gray-900/90 focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-gray-950 disabled:pointer-events-none disabled:opacity-50 dark:bg-gray-50 dark:text-gray-900 dark:hover:bg-gray-50/90 dark:focus-visible:ring-gray-300"
            prefetch={false}
          >
            Go to Home
          </Button>
        </div>
      </div>
    </div>
  );
};

export default WithAuthentication(RefundInitiated);
