/* eslint-disable react/prop-types */
import { useEffect, useState } from "react";
import {
  Route,
  BrowserRouter as Router,
  Routes,
  Navigate,
  useNavigate,
  useLocation,
} from "react-router-dom";
import "./App.css";
import { getYears, months } from "./lib/data";
import {
  formatMonths,
  formatPermissions,
  formatYears,
} from "./lib/helperFunctions";
import { useDateStore, usePermissionStore, userStore } from "./lib/store";
import UnauthorisedAccess from "./pages/UnauthorisedAccess";
import routes from "./routes";
import { getPermissions } from "./services/customerProfile";

// Define public routes - add your public route paths here
const publicRoutes = [
  "/login",
  "/signup",
  "/contact", // example public route
];

const isPublicRoute = (pathname) => {
  return publicRoutes.includes(pathname);
};

// const AuthWrapper = ({ user }) => {
//   const navigate = useNavigate();
//   const location = useLocation();
// const [hasRedirected, setHasRedirected] = useState(false);

// useEffect(() => {
// if (!hasRedirected && !isPublicRoute(location.pathname)) {
//   const role = user?.userType?.name.toLowerCase().trim();
// if (role === "pensioner" || role === "community") {
//   if (user?.isPolicy === false && location.pathname !== "/productlist") {
//     navigate("/productlist");
//   } else if (
//     !user?.isProfileComplete &&
//     location.pathname !== "/profile"
//   ) {
//     navigate("/profile");
//   } else if (location.pathname === "/login") {
//     navigate("/");
//   }
// } else if (role && location.pathname === "/login") {
// navigate("/dashboard");
//}
//   setHasRedirected(true);
// }
//   }, [user, navigate, location.pathname]);

//   return null;
// };

const AppContent = ({ status, user, isInitialized }) => {
  const location = useLocation();

  if (status === "loading") {
    return <div>Loading...</div>;
  }

  if (status === "unauthorized") {
    return <UnauthorisedAccess />;
  }

  // Allow access to public routes without authentication
  if (!user && !isPublicRoute(location.pathname)) {
    return <Navigate to="/login" replace />;
  }

  return (
    <>
      <Routes>
        {routes.map((route, index) => (
          <Route key={index} path={route.path} element={route.component} />
        ))}
      </Routes>
    </>
  );
};

function App() {
  const setPermissions = usePermissionStore((state) => state.setPermissions);
  const user = userStore((state) => state.user);
  const setMonths = useDateStore((state) => state.setMonths);
  const setYears = useDateStore((state) => state.setYears);
  const [status, setStatus] = useState("loading");
  const [isInitialized, setIsInitialized] = useState(false);

  useEffect(() => {
    const initializeApp = async () => {
      if (!user) {
        setStatus("success");
        return;
      }

      try {
        const response = await getPermissions();
        if (response.status === "success") {
          const newPermissions = formatPermissions(response.data);
          setPermissions(newPermissions);

          const fMonths = formatMonths(months);
          const fYears = formatYears(getYears());
          setMonths(fMonths);
          setYears(fYears);

          const role = user?.userType?.name.toLowerCase().trim();
          if (role !== "pensioner" && role !== "community") {
            user["isProfileComplete"] = true;
          }

          setStatus("success");
          setIsInitialized(true);
        } else {
          setStatus("unauthorized");
        }
      } catch (error) {
        console.error("Error initializing app:", error);
        setStatus("unauthorized");
      }
    };

    initializeApp();
  }, [user, setPermissions, setMonths, setYears]);

  return (
    <Router>
      <AppContent status={status} user={user} isInitialized={isInitialized} />
    </Router>
  );
}

export default App;
