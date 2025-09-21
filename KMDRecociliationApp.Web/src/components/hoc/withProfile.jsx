import { userStore } from "@/lib/store";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const WithProfile = (WrappedComponent) => {
  return function Hoc(props) {
    const navigate = useNavigate();
    const user = userStore((state) => state.user);
    const [isProfileComplete, setIsProfileComplete] = useState(false);
    const [checkingProfile, setCheckingProfile] = useState(true);
    // Get campaign ID from URL
    const campaignId = new URLSearchParams(window.location.search).get(
      "campaignId"
    );

    useEffect(() => {
      (() => {
        if (user.isProfileComplete) {
          setIsProfileComplete(true);
        } else {
          navigate(`/login?campaignId=${campaignId}`);
        }

        setCheckingProfile(false);
      })();
    }, []);

    if (checkingProfile) {
      return null;
    }

    if (!isProfileComplete) {
      return null;
    }

    if (isProfileComplete) {
      return <WrappedComponent {...props} />;
    }
  };
};

export default WithProfile;
