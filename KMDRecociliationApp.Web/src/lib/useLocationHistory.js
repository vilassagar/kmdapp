import { useEffect } from "react";
import { useLocation } from "react-router-dom";
import { useLocationStore } from "./store";

const useLocationHistory = () => {
  const location = useLocation();
  const addLocation = useLocationStore((state) => state.addLocation);

  useEffect(() => {
    addLocation(location?.pathname);
  }, [location]);
};

export default useLocationHistory;
