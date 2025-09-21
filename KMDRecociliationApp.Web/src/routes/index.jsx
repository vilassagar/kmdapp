import Association from "@/pages/Association";
import BeneficiaryDetails from "@/pages/BeneficiaryDetails";
import Campaigns from "@/pages/Campaigns";
import Cart from "@/pages/Cart";
import CreateAssociation from "@/pages/CreateAssociation";
import CreateCampaign from "@/pages/CreateCampaign";
import CreatePermission from "@/pages/CreatePermission";
import CreateProduct from "@/pages/CreateProduct";
import CreateProductList from "@/pages/CreateProductList";
import CreateRole from "@/pages/CreateRole";
import CreateUser from "@/pages/CreateUser";
import ImportData from "@/pages/ImportData";
import Login from "@/pages/Login";
import OfflinePayments from "@/pages/OfflinePayments";
import PaymentHistory from "@/pages/PaymentHistory";
import PaymentInitiated from "@/pages/PaymentInitiated";
import PaymentSuccess from "@/pages/PaymentSuccess";
import Payments from "@/pages/Payments";
import Permission from "@/pages/Permission";
import Productlist from "@/pages/Productlist";
import Profile from "@/pages/Profile";
import MyPolicies from "@/pages/MyPolicies";
import RefundRequests from "@/pages/RefundRequests";
import Reports from "@/pages/Reports";
import Role from "@/pages/Role";
import SignUp from "@/pages/SignUp";
import UnauthorisedAccess from "@/pages/UnauthorisedAccess";
import Users from "@/pages/Users";
import Dashboard from "@/pages/Dashboard";
import Organisation from "@/pages/Organisation";
import CreateOrganisation from "@/pages/CreateOrganisation";
import RefundDetails from "@/pages/RefundDetails";
import DailyCountAssociationWise from "@/components/ui/dailyCountAssociationWise";
import AssociationWisePaymentDetails from "@/components/ui/associationWisePaymentDetails";
import RefundInitiated from "@/pages/RefundInitiated";
import OfflinePaymentsReports from "@/components/ui/offlinePaymentsReports";
import GetRefundReport from "@/components/ui/getRefundReport";
import CorrectionReport from "@/components/ui/correctionReport";
import BouncedPayments from "@/components/ui/bouncedPayments";
import ReconciledOnlinePayments from "@/components/ui/reconciledOnlinePayments";
import CompletedForms from "@/components/ui/completedForms";
import IncompleteTransactions from "@/components/ui/incompleteTransactions";
import InsuranceCompanyReport from "@/components/ui/insuranceCompanyReport";
import PaymentFailure from "@/pages/PaymentFailure";
import Maintenance from "@/pages/Maintenance";
import OnlinePaymentStatus from "@/pages/OnlinePaymentStatus";
import ExtractPensionersAndPayments from "@/pages/ExtractPensionersAndPayments";
import NewDashboard from "@/pages/NewDashboard";
import Reconcillation from "@/pages/Reconcillation";
import DashboardFilterData from "@/pages/DashboardFilterData";
import ResetPassword from "@/pages/ResetPassword";
import ApplicantInsurancePolicylist from "@/pages/ApplicantInsurancePolicylist";
import CreateApplicant from "@/pages/CreateApplicant";

const routes = [
  {
    exact: true,
    path: "/",
    component: <Profile />,
  },
  {
    exact: true,
    path: "profile",
    component: <Profile />,
  },
  {
    exact: true,
    path: "mypolicies",
    component: <MyPolicies />,
  },
  {
    exact: true,
    path: "/pensioner",
    component: <Profile />,
  },
  {
    exact: true,
    path: "/adminprofile",
    component: <Profile />,
  },
  {
    exact: true,
    path: "/productlist",
    component: <Productlist />,
  },
  {
    exact: true,
    path: "/payments",
    component: <PaymentHistory />,
  },
  {
    exact: true,
    path: "/signup",
    component: <SignUp />,
  },
  {
    exact: true,
    path: "/login",
    component: <Login />,
  },
  {
    exact: true,
    path: "/payment",
    component: <Payments />,
  },
  {
    exact: true,
    path: "/paymentsuccess",
    component: <PaymentSuccess />,
  },
  {
    exact: true,
    path: "/paymentinitiated",
    component: <PaymentInitiated />,
  },
  {
    exact: true,
    path: "/association",
    component: <Association />,
  },
  {
    exact: true,
    path: "/users",
    component: <Users />,
  },

  {
    exact: true,
    path: "/reports",
    component: <Reports />,
  },
  {
    exact: true,
    path: "/importdata",
    component: <ImportData />,
  },
  {
    exact: true,
    path: "/offlinepayments",
    component: <OfflinePayments />,
  },
  {
    exact: true,
    path: "/createproduct",
    component: <CreateProduct />,
  },

  {
    exact: true,
    path: "/createproductlist",
    component: <CreateProductList />,
  },
  {
    exact: true,
    path: "/cart",
    component: <Cart />,
  },
  {
    exact: true,
    path: "/campaigns",
    component: <Campaigns />,
  },
  {
    exact: true,
    path: "/beneficiarydetails",
    component: <BeneficiaryDetails />,
  },

  {
    exact: true,
    path: "/createuser",
    component: <CreateUser />,
  },
  {
    exact: true,
    path: "/createcampaign",
    component: <CreateCampaign />,
  },
  {
    exact: true,
    path: "/createassociation",
    component: <CreateAssociation />,
  },

  {
    exact: true,
    path: "/refundrequests",
    component: <RefundRequests />,
  },
  {
    exact: true,
    path: "/roles",
    component: <Role />,
  },
  {
    exact: true,
    path: "/createrole",
    component: <CreateRole />,
  },
  {
    exact: true,
    path: "/createrole/:id",
    component: <CreateRole />,
  },
  {
    exact: true,
    path: "/permissions",
    component: <Permission />,
  },

  {
    exact: true,
    path: "/createpermission",
    component: <CreatePermission />,
  },
  {
    exact: true,
    path: "/createpermission/:id",
    component: <CreatePermission />,
  },
  {
    exact: true,
    path: "/dashboard",
    component: <NewDashboard />,
  },

  {
    exact: true,
    path: "/organisations",
    component: <Organisation />,
  },

  {
    exact: true,
    path: "/createorganisation",
    component: <CreateOrganisation />,
  },

  {
    exact: true,
    path: "/refunddetails/:id",
    component: <RefundDetails />,
  },
  {
    exact: true,
    path: "/dailycountassociationwise",
    component: <DailyCountAssociationWise />,
  },

  {
    exact: true,
    path: "/associationwisepaymentdetails",
    component: <AssociationWisePaymentDetails />,
  },

  {
    exact: true,
    path: "/refundinitiated",
    component: <RefundInitiated />,
  },
  {
    exact: true,
    path: "/unauthorized",
    component: <UnauthorisedAccess />,
  },
  {
    exact: true,
    path: "/offlinepaymentsreports",
    component: <OfflinePaymentsReports />,
  },
  {
    exact: true,
    path: "/getrefundreports",
    component: <GetRefundReport />,
  },
  {
    exact: true,
    path: "/correctionreport",
    component: <CorrectionReport />,
  },
  {
    exact: true,
    path: "/bouncedpayments",
    component: <BouncedPayments />,
  },
  {
    exact: true,
    path: "/reconciledonlinepayments",
    component: <ReconciledOnlinePayments />,
  },
  {
    exact: true,
    path: "/completedforms",
    component: <CompletedForms />,
  },
  {
    exact: true,
    path: "/incompletetransactions",
    component: <IncompleteTransactions />,
  },
  {
    exact: true,
    path: "/insurancecompanyreport",
    component: <InsuranceCompanyReport />,
  },
  {
    exact: true,
    path: "/paymentfailure",
    component: <PaymentFailure />,
  },
  {
    exact: true,
    path: "/maintenance",
    component: <Maintenance />,
  },
  {
    exact: true,
    path: "/onlinepaymentstatus",
    component: <OnlinePaymentStatus />,
  },
  {
    exact: true,
    path: "/extractpensionersandpayments",
    component: <ExtractPensionersAndPayments />,
  },
  {
    exact: true,
    path: "/newdashboard",
    component: <NewDashboard />,
  },
  {
    exact: true,
    path: "/reconciliation",
    component: <Reconcillation />,
  },
  {
    exact: true,
    path: "/dashboardfilterdata",
    component: <DashboardFilterData />,
  },

  {
    exact: true,
    path: "/resetpassword",
    component: <ResetPassword />,
  },
  {
    exact: true,
    path: "/insurancepolicydata",
    component: <ApplicantInsurancePolicylist />,
  },
  {
    exact: true,
    path: "/createapplicant",
    component: <CreateApplicant />,
  },
];

export default routes;
