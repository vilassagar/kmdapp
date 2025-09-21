import { usePermissionStore } from "@/lib/store";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

const WithPermission = (pageName) => (WrappedComponent) => {
  return function Hoc(props) {
    let permissions = usePermissionStore((state) => state.permissions);
    const navigate = useNavigate();

    useEffect(() => {
      if (permissions !== null && !permissions?.[pageName]?.read) {
        navigate("/unauthorized");
      }
    }, [navigate, permissions]);

    return <WrappedComponent {...props} />;
  };
};

export default WithPermission;
