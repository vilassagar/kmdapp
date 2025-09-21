/* eslint-disable react/prop-types */
import React from "react";
import { cn } from "@/lib/utils";
const ProductSummary = ({ mode, userDetails, permissions, PAGE_NAME }) => {
  return (
    <div
      className={cn(
        mode === "edit"
          ? "grid grid-cols-1 md:grid-cols-3"
          : "flex justify-end",
        "text-right bg-background text-card-foreground rounded-lg shadow-lg p-6"
      )}
    >
      <div className="gap-1 mb-5 grid text-left md:mb-0">
        <div className="text-muted-foreground text-sm">Total Premium</div>
        <div className="text-2xl font-bold">₹ {userDetails.totalPremium}</div>
      </div>
      {mode === "edit" && (
        <>
          <div className="gap-1 mb-5 grid text-center md:mb-0">
            <div className="text-muted-foreground text-sm">Amount Paid</div>
            <div className="text-2xl font-bold">
              ₹ {userDetails.totalPaidPremium}
            </div>
          </div>
          <div className="gap-1 mb-5 grid md:mb-0">
            <div className="text-muted-foreground text-sm">Payable Amount</div>
            <div className="text-primary text-2xl font-bold">
              ₹ {userDetails.totalPremium - userDetails?.totalPaidPremium}
            </div>
          </div>
        </>
      )}
    </div>
  );
};

export default ProductSummary;
