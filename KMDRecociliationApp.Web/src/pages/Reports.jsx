import WithLayout from "@/components/layout/WithLayout";
import {
  Card,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { useNavigate } from "react-router-dom";

function Reports() {
  const navigate = useNavigate();

  return (
    <div>
      <h1 className="text-2xl font-bold">Reports</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 p-4 md:p-6 w-full">
        <Card onClick={() => navigate("/dailycountassociationwise")}>
          <CardHeader>
            <CardTitle>Daily Count Association-Wise</CardTitle>
            <CardDescription>
              Download a report of daily counts by association, including
              options chosen and premium.
            </CardDescription>
          </CardHeader>
        </Card>
        <Card onClick={() => navigate("/associationwisepaymentdetails")}>
          <CardHeader>
            <CardTitle>Association Wise Payment Details</CardTitle>
            <CardDescription>
              Download a report of payment details by association, including
              options chosen and premium.
            </CardDescription>
          </CardHeader>
        </Card>
        <Card onClick={() => navigate("/completedforms")}>
          <CardHeader>
            <CardTitle>Completed Forms</CardTitle>
            <CardDescription>
              Download a daily dump of all completed forms.
            </CardDescription>
          </CardHeader>
        </Card>
        <Card onClick={() => navigate("/incompletetransactions")}>
          <CardHeader>
            <CardTitle>Incomplete Transactions</CardTitle>
            <CardDescription>
              Download a daily dump of all incomplete transactions.
            </CardDescription>
          </CardHeader>
        </Card>
        <Card onClick={() => navigate("/bouncedpayments")}>
          <CardHeader>
            <CardTitle>Bounced Payments</CardTitle>
            <CardDescription>
              Download a report of all bounced payments.
            </CardDescription>
          </CardHeader>
        </Card>
        <Card onClick={() => navigate("/reconciledonlinepayments")}>
          <CardHeader>
            <CardTitle>Reconciled Online Payments</CardTitle>
            <CardDescription>
              Download a report of offline payments, including reconciled and
              not reconciled.
            </CardDescription>
          </CardHeader>
        </Card>
        <Card onClick={() => navigate("/correctionreport")}>
          <CardHeader>
            <CardTitle>Correction Report</CardTitle>
            <CardDescription>
              Download a report of all corrections made.
            </CardDescription>
          </CardHeader>
        </Card>
        <Card onClick={() => navigate("/getrefundreports")}>
          <CardHeader>
            <CardTitle>Refund Report</CardTitle>
            <CardDescription>
              Download a report of all refunds processed.
            </CardDescription>
          </CardHeader>
        </Card>
        <Card onClick={() => navigate("/insurancecompanyreport")}>
          <CardHeader>
            <CardTitle>Insurance Company Report</CardTitle>
            <CardDescription>
              Download a report in the format required by insurance companies.
            </CardDescription>
          </CardHeader>
        </Card>
        <Card onClick={() => navigate("/offlinepaymentsreports")}>
          <CardHeader>
            <CardTitle>Offline Payments</CardTitle>
            <CardDescription>
              Download a report of offline payments, including reconciled and
              not reconciled.
            </CardDescription>
          </CardHeader>
        </Card>
      </div>
    </div>
  );
}

export default WithLayout(Reports);
