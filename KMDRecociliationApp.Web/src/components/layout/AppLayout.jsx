/* eslint-disable react-hooks/rules-of-hooks */
/* eslint-disable react/prop-types */
import { CircleUser, LogOut, Menu, Package } from "lucide-react";

import kmd from "@/assets/kmd.png";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";
import { usePermissionStore, usePolicyStore, userStore } from "@/lib/store";
import useLocationHistory from "@/lib/useLocationHistory";
import { cn } from "@/lib/utils";
import PropTypes from "prop-types";
import { useNavigate } from "react-router-dom";
import RButton from "../ui/rButton";
import {
  User,
  ShoppingBag,
  CreditCard,
  LayoutDashboard,
  Users,
  FileCheck,
  Download,
  ThumbsUp,
  RefreshCw,
  RotateCcw,
  FilePlus,
  Megaphone,
  Link,
  Shield,
  FileInput,
  Building2,
} from "lucide-react";
import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";

export default function AppLayout({ children }) {
  const navigate = useNavigate();
  const user = userStore((state) => state.user);
  const removeUser = userStore((state) => state.removeUser);
  const permissions = usePermissionStore((state) => state.permissions);
  const setMode = usePolicyStore((state) => state.setMode);
  const updatePolicyId = usePolicyStore((state) => state.updatePolicyId);
  const [showPensionerMenu, setShowPensionerMenu] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const isPensioner =
    user?.userType?.name?.toLowerCase().trim() === "pensioner" ||
    user?.userType?.name?.toLowerCase().trim() === "community";

  const storedValue = JSON.parse(localStorage.getItem("user"));
  const isProfileComplete = storedValue.state.user.isProfileComplete;

  // Add this at the top level of your AppLayout component
  const location = useLocation();
  useLocationHistory();

  useEffect(() => {
    const lastVisitedPensioner = JSON.parse(
      sessionStorage.getItem("lastVisitedpensioner")
    );
    setShowPensionerMenu(!isPensioner && !!lastVisitedPensioner?.userId);
  }, [isPensioner]);

  const handleLogout = () => {
    sessionStorage.clear();
    localStorage.clear();
    removeUser();
    navigate("/login");
  };

  const MenuItem = ({
    icon: Icon,
    text,
    onClick,
    permission,
    path,
    disabled,
  }) => {
    if (!permission) return null;
    const location = useLocation();
    const isActive = location.pathname === path;

    return (
      <div
        className={cn(
          "flex items-center gap-3 rounded-lg px-3 py-2 transition-all",
          isActive
            ? "bg-[#8B0000] text-white"
            : "text-gray-700 hover:bg-[#8B0000] hover:text-white",
          disabled
            ? "opacity-50 pointer-events-none cursor-not-allowed"
            : "cursor-pointer"
        )}
        onClick={() => !disabled && onClick()}
      >
        <Icon className="h-4 w-4" />
        <span className="text-sm">{text}</span>
      </div>
    );
  };

  const AdminMenuItems = () => (
    <div className="space-y-3">
      <p className="text-gray-700 uppercase font-extrabold mb-4 text-sm">
        {/* Admin */}
      </p>
      <div className="space-y-1">
        <MenuItem
          icon={LayoutDashboard}
          text="Dashboard"
          onClick={() => navigate("/dashboard")}
          path="/dashboard"
          permission={permissions?.dashboard?.read}
        />
        <MenuItem
          icon={Users}
          text="Users"
          onClick={() => navigate("/users")}
          path="/users"
          permission={permissions?.users?.read}
        />
        <MenuItem
          icon={FileCheck}
          text="Reconciliation"
          onClick={() => navigate("/reconciliation")}
          path="/reconciliation"
          permission={permissions?.importdata?.read}
        />
        <MenuItem
          icon={Download}
          text="Extract Pensioner and Payments"
          onClick={() => navigate("/extractpensionersandpayments")}
          path="/extractpensionersandpayments"
          permission={permissions?.reports?.read}
        />
        <MenuItem
          icon={ThumbsUp}
          text="Approve/Reject Payments"
          onClick={() => navigate("/offlinePayments")}
          path="/offlinePayments"
          permission={permissions?.offlinepayments?.read}
        />
        <MenuItem
          icon={RefreshCw}
          text="Update Payments"
          onClick={() => navigate("/onlinepaymentstatus")}
          path="/onlinepaymentstatus"
          permission={permissions?.offlinepayments?.read}
        />
        <MenuItem
          icon={RotateCcw}
          text="Refund Requests"
          onClick={() => navigate("/refundrequests")}
          path="/refundrequests"
          permission={permissions?.refundrequests?.read}
        />
        <MenuItem
          icon={FilePlus}
          text="Create Product"
          onClick={() => navigate("/createproductlist")}
          path="/createproductlist"
          permission={permissions?.createproduct?.read}
        />
        <MenuItem
          icon={Megaphone}
          text="Campaigns"
          onClick={() => navigate("/campaigns")}
          path="/campaigns"
          permission={permissions?.campaigns?.read}
        />
        <MenuItem
          icon={Link}
          text="Association"
          onClick={() => navigate("/association")}
          path="/association"
          permission={permissions?.associations?.read}
        />
        <MenuItem
          icon={Building2}
          text="Organisations"
          onClick={() => navigate("/organisations")}
          path="/organisations"
          permission={permissions?.organisations?.read}
        />
        <MenuItem
          icon={FileInput}
          text="Import Data"
          onClick={() => navigate("/importdata")}
          path="/importdata"
          permission={permissions?.importdata?.read}
        />
        <MenuItem
          icon={Shield}
          text="Roles"
          onClick={() => navigate("/roles")}
          path="/roles"
          permission={permissions?.roles?.read}
        />
        <MenuItem
          icon={Shield}
          text="Insurance Policy"
          onClick={() => navigate("/insurancepolicydata")}
          path="/insurancepolicydata"
          permission={permissions?.applicantinsurancepolicy?.read}
        />
      </div>
    </div>
  );

  const PensionerMenuItems = () => {
    const handleNavigateProfile = () => {
      if (!isPensioner) {
        const lastVisitedPensioner = JSON.parse(
          sessionStorage.getItem("lastVisitedpensioner")
        );
        if (lastVisitedPensioner?.userId) {
          navigate(`/?userId=${lastVisitedPensioner.userId}`);
        } else {
          navigate("/");
        }
      } else {
        navigate("/");
      }
    };

    const handleNavigatePolicies = () => {
      if (!isPensioner) {
        const lastVisitedPensioner = JSON.parse(
          sessionStorage.getItem("lastVisitedpensioner")
        );
        if (lastVisitedPensioner?.userId) {
          navigate(`/mypolicies?userId=${lastVisitedPensioner.userId}`);
        } else {
          navigate("/mypolicies");
        }
      } else {
        navigate("/mypolicies");
      }
    };

    const handleNavigatePayments = () => {
      if (!isPensioner) {
        const lastVisitedPensioner = JSON.parse(
          sessionStorage.getItem("lastVisitedpensioner")
        );
        if (lastVisitedPensioner?.userId) {
          navigate(`/payments?userId=${lastVisitedPensioner.userId}`);
        } else {
          navigate("/payments");
        }
      } else {
        navigate("/payments");
      }
    };

    const handleNavigateProductList = () => {
      if (!isPensioner) {
        const lastVisitedPensioner = JSON.parse(
          sessionStorage.getItem("lastVisitedpensioner")
        );
        setMode("new");
        updatePolicyId(0);
        if (lastVisitedPensioner?.userId) {
          navigate(`/productlist?userId=${lastVisitedPensioner.userId}`);
        } else {
          navigate("/productlist");
        }
      } else {
        navigate("/productlist");
      }
    };

    return (
      <div className="space-y-3">
        <p className="text-gray-700 uppercase font-extrabold mb-4 text-sm">
          User
        </p>
        <div className="space-y-1">
          <MenuItem
            icon={User}
            text="Profile"
            path="/Profile"
            onClick={handleNavigateProfile}
            permission={permissions?.profile?.read}
            disabled={false} // Profile is always enabled
          />
          <MenuItem
            icon={User}
            text="My Policies"
            path="/mypolicies"
            onClick={handleNavigatePolicies}
            permission={permissions?.mypolicies?.read}
            disabled={!isProfileComplete} // Profile is always enabled
          />
          <MenuItem
            icon={ShoppingBag}
            text="Products"
            path="/productlist"
            onClick={handleNavigateProductList}
            permission={permissions?.products?.read}
            disabled={!isProfileComplete}
          />
          <MenuItem
            icon={CreditCard}
            text="Payments"
            path="/payments"
            onClick={handleNavigatePayments}
            permission={permissions?.payments?.read}
            disabled={!isProfileComplete}
          />
        </div>
      </div>
    );
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Mobile Header */}
      <header className="sticky top-0 z-50 w-full border-b bg-white md:hidden">
        <div className="flex h-14 items-center justify-between px-4">
          <div
            onClick={() => navigate("/dashboard")}
            className="cursor-pointer"
          >
            <img src={kmd} alt="KMD logo" className="h-16 w-16" />
          </div>

          <div className="flex items-center gap-2">
            {user?.userName && (
              <span className="text-sm font-medium hidden sm:inline-block">
                Hello, {user.userName}
              </span>
            )}
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="icon" className="rounded-full">
                  <CircleUser className="h-6 w-6 text-[#8B0000]" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-48">
                <DropdownMenuLabel>My Account</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuItem
                  className="cursor-pointer"
                  onClick={() => navigate(isPensioner ? "/" : "/adminprofile")}
                >
                  <User className="h-4 w-4 mr-2" />
                  Profile
                </DropdownMenuItem>
                <DropdownMenuItem
                  className="cursor-pointer"
                  onClick={handleLogout}
                >
                  <LogOut className="h-4 w-4 mr-2" />
                  Logout
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
            <Sheet open={isMobileMenuOpen} onOpenChange={setIsMobileMenuOpen}>
              <SheetTrigger asChild>
                <RButton variant="outline" size="icon" className="h-9 w-9">
                  <Menu className="h-5 w-5" />
                </RButton>
              </SheetTrigger>
              <SheetContent side="left" className="w-72 p-0">
                <div className="flex h-14 items-center border-b px-4">
                  <div onClick={() => navigate("/")} className="cursor-pointer">
                    <img src={kmd} alt="KMD logo" className="h-16 w-16" />
                  </div>
                </div>
                <nav className="flex-1 overflow-y-auto">
                  <div className="flex flex-col p-4 space-y-6">
                    {permissions && (
                      <div>
                        {" "}
                        {/* Removed the conditional class here */}
                        {!isPensioner && <AdminMenuItems />}
                        {(isPensioner || showPensionerMenu) && (
                          <PensionerMenuItems />
                        )}
                      </div>
                    )}
                  </div>
                </nav>
              </SheetContent>
            </Sheet>
          </div>
        </div>
      </header>
      <div className="flex">
        {/* Desktop Sidebar */}
        <aside className="hidden md:flex md:w-64 lg:w-72 flex-col fixed h-screen border-r bg-white">
          <div className="flex h-14 items-center border-b px-4">
            <div
              onClick={() => navigate("/dashboard")}
              className="cursor-pointer"
            >
              <img src={kmd} alt="KMD logo" className="h-16 w-16" />
            </div>
          </div>
          <nav className="flex-1 overflow-y-auto p-4">
            {permissions && (
              <div>
                {" "}
                {/* Removed the conditional class here */}
                {!isPensioner && <AdminMenuItems />}
                {(isPensioner || showPensionerMenu) && <PensionerMenuItems />}
              </div>
            )}
          </nav>
        </aside>

        {/* Main Content */}
        <main className="flex-1 md:ml-64 lg:ml-72">
          {/* Desktop Header */}
          <header className="hidden md:flex h-14 items-center justify-end border-b bg-white px-4">
            <div className="flex items-center gap-4">
              {user?.userName && (
                <span className="font-medium">Hello, {user.userName}</span>
              )}
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button variant="ghost" size="icon" className="rounded-full">
                    <CircleUser className="h-6 w-6 text-[#8B0000]" />
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end" className="w-48">
                  <DropdownMenuLabel>My Account</DropdownMenuLabel>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem
                    className="cursor-pointer"
                    onClick={() =>
                      navigate(isPensioner ? "/" : "/adminprofile")
                    }
                  >
                    Profile
                  </DropdownMenuItem>
                  <DropdownMenuItem onClick={handleLogout}>
                    <LogOut className="h-4 w-4 mr-2" />
                    Logout
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            </div>
          </header>

          {/* Page Content */}
          <div className="p-4 md:p-6">{children}</div>
        </main>
      </div>
    </div>
  );
}

AppLayout.propTypes = {
  children: PropTypes.element,
};

AppLayout.defaultProps = {
  children: <></>,
};
