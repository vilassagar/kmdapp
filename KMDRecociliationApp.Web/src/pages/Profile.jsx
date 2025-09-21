import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithRole from "@/components/hoc/withRole";
import WithLayout from "@/components/layout/WithLayout";
import { RPagination } from "@/components/ui/RPagination";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import NoPolicy from "@/components/ui/noPolicy";
import RButton from "@/components/ui/rButton";
import RInput from "@/components/ui/rInput";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { toast } from "@/components/ui/use-toast";
import UserProfile from "@/components/ui/userProfile";
import { getPaymentReceiptDoc } from "@/lib/helperFunctions";
import { usePermissionStore, usePolicyStore, userStore } from "@/lib/store";
import { cn } from "@/lib/utils";
import {
  getMyPolicies,
  getPaymentReceipt,
  freezPolicyOrder,
} from "@/services/customerProfile";
import { Separator } from "@radix-ui/react-dropdown-menu";
import { format } from "date-fns";
import jsPDF from "jspdf";
import {
  CircleArrowLeft,
  CirclePlus,
  DownloadIcon,
  FileEdit,
  Snowflake,
} from "lucide-react";
import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

const PAGE_NAME = "profile";
const PAGE_NAME_mypolicies = "mypolicies";

function Profile() {
  const search = window.location.search;
  const params = new URLSearchParams(search);
  const userId = params.get("userId");
  // Get campaign ID from URL
  const campaignId = new URLSearchParams(window.location.search).get(
    "campaignId"
  );

  const navigate = useNavigate();
  const user = userStore((state) => state.user);

  const permissions = usePermissionStore((state) => state.permissions);

  const mode = "edit";
  const markUserProfileComplete = userStore(
    (state) => state.markUserProfileComplete
  );

  useEffect(() => {
    (async () => {
      if (user && !user.isProfileComplete) {
        toast({
          title: "User Profile.",
          description: "Please complete the user profile details.",
        });
      }
    })();
  }, []);

  const handleSave = () => {
    markUserProfileComplete();
    if (userId) {
      navigate(`/productlist?userId=${userId}`);
    } else {
      navigate("/productlist");
    }
  };

  return (
    <article className="overflow-x-hidden  prose prose-gray px-4 md:px-0 dark:prose-invert">
      <div>
        <div className="flex justify-between items-center">
          <h1 className="text-2xl font-bold mb-6">Profile</h1>
          {userId ? (
            <CircleArrowLeft onClick={() => navigate(`/users`)} />
          ) : null}
        </div>

        <div className="space-y-4 p-3">
          <UserProfile
            userType={user && user.userType}
            mode={mode}
            onSave={handleSave}
            userId={user && user.userId}
            permissions={permissions}
            pageName={PAGE_NAME}
          />
        </div>
      </div>
    </article>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(Profile))
);
