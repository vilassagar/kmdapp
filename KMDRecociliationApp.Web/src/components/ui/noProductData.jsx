import { userStore } from "@/lib/store";
import { InboxIcon } from "lucide-react";
import RButton from "./rButton";

export default function NoProductData() {
  const user = userStore((state) => state.user);

  const isRetiree =
    user?.userType?.name?.toLowerCase().trim() === "pensioner" ||
    user?.userType?.name?.toLowerCase().trim() === "community";
  return (
    <div className="flex  flex-col items-center justify-center bg-background px-4 py-12 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-md text-center">
        <InboxIcon className="mx-auto h-12 w-12 text-primary" />
        <h1 className="mt-4 text-3xl font-bold tracking-tight text-foreground sm:text-4xl">
          No products are currently available. Please check back later.
        </h1>
        <p className="mt-4 text-muted-foreground">
          We apologize, but there are no products available for purchase at this
          time. Please check back later or contact support for assistance.
        </p>
        <div className="mt-6">
          <RButton
            className="inline-flex items-center rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground shadow-sm transition-colors hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2"
            onClick={() => {
              if (isRetiree) {
                window.location.href = "/mypolicies";
              } else {
                window.location.href = "/mypolicies";
              }
            }}
          >
            Go to My Policies
          </RButton>
        </div>
      </div>
    </div>
  );
}
