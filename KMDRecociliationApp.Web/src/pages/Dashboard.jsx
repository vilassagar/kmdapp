import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import AssociationWisePaymentTable from "@/components/ui/associationWisePaymentTable";
import OfflinePaymentsChart from "@/components/ui/offlinePaymentsChart";
import PaymentsModeGraph from "@/components/ui/paymentsModeGraph";
import RButton from "@/components/ui/rButton";
import { toast } from "@/components/ui/use-toast";
import UserCountGraph from "@/components/ui/userCountGraph";
import { userStore } from "@/lib/store";
import { getAssociationExtract } from "@/services/reports";
import PaymentCompleted from "@/components/ui/paymentCompleted";

const PAGE_NAME = "dashboard";

const Dashboard = () => {
  const user = userStore((state) => state.user);

  const dashboarduser =
    user?.userType?.name?.toLowerCase()?.trim() === "internaluser" ||
    user?.userType?.name?.toLowerCase()?.trim() === "association";
  const notdashboarduser =
    user?.userType?.name?.toLowerCase()?.trim() === "pensioner" ||
    user?.userType?.name?.toLowerCase()?.trim() === "community";
  const associationId =
    user?.userType?.name?.toLowerCase()?.trim() === "association"
      ? user?.userId
      : 0;

  console.log(dashboarduser);

  const handleDownload = async () => {
    try {
      const response = await getAssociationExtract(associationId);
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;

      link.setAttribute(
        "download",
        `association_extract_${String(new Date().getDate()).padStart(
          2,
          "0"
        )}${String(new Date().getMonth() + 1).padStart(
          2,
          "0"
        )}${new Date().getFullYear()}.xlsx`
      );
      document.body.appendChild(link);
      link.click();
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get correction report details.",
      });
    }
  };

  return (
    <>
      {dashboarduser ? (
        <div>
          {associationId && (
            <div className="mb-4">
              <RButton onClick={handleDownload}>Download Extract</RButton>
            </div>
          )}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            <div className="lg:col-span-1 md:col-span-1">
              <UserCountGraph />
            </div>
            <div className="lg:col-span-1 md:col-span-1">
              <OfflinePaymentsChart />
            </div>
            <div className="lg:col-span-1 md:col-span-1">
              <PaymentsModeGraph />
            </div>
            <div className="lg:col-span-1 md:col-span-1">
              <PaymentCompleted />
            </div>
            <div className="col-span-1 md:col-span-2 lg:col-span-4">
              <AssociationWisePaymentTable />
            </div>
          </div>
        </div>
      ) : (
        <div>
          <h3>Welcome To KMD Enrollment Portal</h3>
        </div>
      )}
    </>
  );
};

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(Dashboard))
);
