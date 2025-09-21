import { userStore } from "@/lib/store";
import { InboxIcon } from "lucide-react";
import RButton from "./rButton";

export default function NoPolicy() {
  const user = userStore((state) => state.user);
  const isRetiree =
    user?.userType?.name?.toLowerCase().trim() === "pensioner" ||
    user?.userType?.name?.toLowerCase().trim() === "community";

  // Get URL parameters using a custom hook or in useEffect to avoid window undefined issues
  const getUrlParam = (param) => {
    if (typeof window === "undefined") return null;
    const params = new URLSearchParams(window.location.search);
    return params.get(param);
  };

  const userId = getUrlParam("userId");

  // Get stored pensioner data safely
  const getStoredPensioner = () => {
    if (typeof window === "undefined") return null;
    const stored = sessionStorage.getItem("lastVisitedpensioner");
    try {
      return stored ? JSON.parse(stored) : null;
    } catch (error) {
      console.error("Error parsing stored pensioner data:", error);
      return null;
    }
  };

  const lastVisitedPensioner = getStoredPensioner();

  const handleBuyPolicy = () => {
    const baseUrl = "/productlist";
    const url =
      lastVisitedPensioner?.userId && userId
        ? `${baseUrl}?userId=${userId}`
        : baseUrl;

    window.location.href = url;
  };

  return (
    <div className="flex flex-col items-center justify-center bg-background px-4 py-12 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-md text-center">
        <InboxIcon className="mx-auto h-12 w-12 text-primary" />

        <h1 className="mt-4 text-3xl font-bold tracking-tight text-foreground sm:text-4xl">
          You haven&apos;t purchased any policy yet
        </h1>

        <div className="mt-6">
          <RButton
            className="inline-flex items-center rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground shadow-sm transition-colors hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2"
            onClick={handleBuyPolicy}
          >
            Buy Policy
          </RButton>
        </div>
      </div>
    </div>
  );
}
