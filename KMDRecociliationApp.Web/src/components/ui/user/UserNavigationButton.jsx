/* eslint-disable react/prop-types */
import React from "react";
import { User } from "lucide-react";

const UserNavigationButton = ({ user, navigate }) => {
  const handleClick = () => {
    // Store user data in session storage
    sessionStorage.setItem(
      "lastVisitedpensioner",
      JSON.stringify({
        userId: user.userId,
        userType: user.userType,
        lastVisit: new Date().toISOString(),
      })
    );
    if (user?.isProfileComplete) {
      navigate(`/mypolicies?userId=${user.userId}`);
    } else {
      navigate(`/pensioner?userId=${user.userId}`);
    }
  };

  // Only render for pensioner user type
  if (
    user?.userType.toLowerCase() === "pensioner" ||
    user?.userType.toLowerCase() === "association" ||
    user?.userType.toLowerCase() === "community"
  ) {
    return (
      <button
        className="inline-flex items-center justify-center p-2 rounded-md hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
        onClick={handleClick}
        aria-label="View user profile"
      >
        <User className="w-5 h-5 text-gray-600" />
      </button>
    );
  } else {
    return null;
  }
};

export default UserNavigationButton;
