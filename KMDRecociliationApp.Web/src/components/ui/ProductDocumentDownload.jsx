/* eslint-disable react/prop-types */
import React, { useState } from "react";
import { Download } from "lucide-react";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { download as FileDownload } from "@/services/files";

const ProductDocumentDownload = ({
  productDocumentUrl,
  documentName,
  name,
  className,
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const [showTooltip, setShowTooltip] = useState(false);

  const handleDownload = async () => {
    if (!productDocumentUrl || !documentName) {
      setError("Document information is missing");
      return;
    }

    try {
      setIsLoading(true);
      setError(null);

      const response = await FileDownload(0, documentName, productDocumentUrl);

      if (response?.status === "success" && response.data) {
        const blob = new Blob([response.data], { type: "application/pdf" });
        const url = window.URL.createObjectURL(blob);

        const a = document.createElement("a");
        a.href = url;
        a.download = `${documentName.replace(/[^a-zA-Z0-9-_]/g, "_")}.pdf`;

        document.body.appendChild(a);
        a.click();

        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
      } else {
        throw new Error(response?.message || "Download failed");
      }
    } catch (error) {
      console.error("Error downloading document:", error);
      setError(
        error.message || "Failed to download document. Please try again."
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="relative inline-block">
      <button
        onClick={handleDownload}
        disabled={isLoading || !productDocumentUrl}
        onMouseEnter={() => setShowTooltip(true)}
        onMouseLeave={() => setShowTooltip(false)}
        className={`
          relative
          inline-flex items-center justify-center
          p-2
          text-white
          bg-red-900
          rounded-full
          transition-all
          duration-200
          hover:bg-red-800
          focus:outline-none
          focus:ring-2
          focus:ring-offset-2
          focus:ring-red-700
          disabled:bg-red-300
          disabled:cursor-not-allowed
          ${className || ""}
        `}
      >
        <Download className={`h-5 w-5 ${isLoading ? "animate-spin" : ""}`} />

        {/* Tooltip */}
        {showTooltip && (
          <div className="absolute bottom-full left-1/2 transform -translate-x-1/2 mb-2 px-2 py-1 text-sm text-white bg-gray-900 rounded whitespace-nowrap">
            {isLoading ? "Downloading..." : name || "Download"}
          </div>
        )}
      </button>

      {error && (
        <Alert
          variant="destructive"
          className="absolute top-full left-1/2 transform -translate-x-1/2 mt-2 z-50 w-48"
        >
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      {isLoading && (
        <div className="absolute bottom-0 left-0 right-0 h-1 bg-gray-200 overflow-hidden rounded-full">
          <div className="h-full bg-red-800 rounded-full animate-progress" />
        </div>
      )}
    </div>
  );
};

export default ProductDocumentDownload;
