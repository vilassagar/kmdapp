// WithAuthentication.jsx
import { userStore } from "@/lib/store";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const WithAuthentication = (WrappedComponent) => {
  return function Hoc(props) {
    const navigate = useNavigate();
    const user = userStore((state) => state.user);

    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [isCheckingAuth, setIsCheckingAuth] = useState(true);

    useEffect(() => {
      if (user) {
        setIsAuthenticated(true);
      } else {
        // Using navigate instead of window.location for a cleaner SPA navigation
        navigate("/login");
      }

      setIsCheckingAuth(false);
    }, [user, navigate]);

    if (isCheckingAuth) {
      return null;
    }

    if (!isAuthenticated) {
      return null;
    }

    return <WrappedComponent {...props} />;
  };
};

export default WithAuthentication;
