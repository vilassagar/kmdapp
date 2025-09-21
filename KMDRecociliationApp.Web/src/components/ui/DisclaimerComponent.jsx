/* eslint-disable react/prop-types */
import React, { useState } from "react";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Checkbox } from "@/components/ui/checkbox";

const DisclaimerComponent = ({
  product,
  productIndex,
  handleChange,
  disclaimerText = ``,
  title = "Disclaimer",
  checkboxLabel = "I confirm that I have read, consent and agree to Policy's",
  linkText = "Disclaimer",
}) => {
  const [isOpen, setIsOpen] = useState(false);

  // Convert newline-separated text into paragraphs
  const disclaimerParagraphs = disclaimerText
    .split("\n\n")
    .filter((text) => text.trim());

  const handleConfirm = () => {
    handleChange("isDisclaimerAccepted", productIndex)(true);
    setIsOpen(false);
  };

  return (
    <div className="p-4">
      <div className="max-w-lg flex items-center">
        <Checkbox
          className="mr-3"
          checked={product.isDisclaimerAccepted}
          value={product.isDisclaimerAccepted}
          id={`check-Disclaimer-${productIndex}`}
          onCheckedChange={handleChange("isDisclaimerAccepted", productIndex)}
          disabled={product.isProductSelected}
        />
        <label htmlFor={`check-Disclaimer-${productIndex}`} className="text-sm">
          {checkboxLabel}{" "}
          <button
            onClick={() => setIsOpen(true)}
            className="text-blue-600 hover:underline"
            type="button"
          >
            {linkText}
          </button>
        </label>
      </div>

      <AlertDialog open={isOpen} onOpenChange={setIsOpen}>
        <AlertDialogContent className="max-w-2xl">
          <AlertDialogHeader>
            <AlertDialogTitle className="text-xl font-bold">
              {title}
            </AlertDialogTitle>
            <AlertDialogDescription className="text-base space-y-4">
              {disclaimerParagraphs.map((paragraph, index) => (
                <p key={index}>{paragraph}</p>
              ))}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            {/* <AlertDialogCancel className="bg-gray-100 hover:bg-gray-200">
              Close
            </AlertDialogCancel> */}
            <AlertDialogAction
              onClick={handleConfirm}
              className="bg-blue-600 text-white hover:bg-blue-700"
            >
              I Agree
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
};

export default DisclaimerComponent;
