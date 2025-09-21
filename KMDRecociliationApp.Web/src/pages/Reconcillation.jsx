import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import { toast } from "@/components/ui/use-toast";
import { userStore } from "@/lib/store";
import {
  downloadApplicationUserTemplate,
  downloadAssociationTemplate,
  uploadAssociations,
  uploadProducts,
  uploadRetiree,
} from "@/services/importData";
import { uploadCheque, uploadNEFT } from "@/services/reconcilation";
import { Download, UploadIcon } from "lucide-react";
import PropTypes from "prop-types";
import { useState, useEffect } from "react";
import { Combobox } from "@/components/ui/comboBox";
import { getCampaignsList } from "@/services/campaigns";

const PAGE_NAME = "importdata";

function ImportData() {
  const user = userStore((state) => state.user);
  const [uploadStatus, setUploadStatus] = useState({});
  const [campaigns, setCampaigns] = useState([]);
  const [error, setError] = useState(null);
  const [campaign, setCampaign] = useState();

  const associationId =
    user?.userType?.name?.toLowerCase()?.trim() === "association"
      ? user?.associationId
      : 0;
  useEffect(() => {
    const initializeData = async () => {
      try {
        await Promise.all([getCampaigns()]);
      } catch (error) {
        console.error("Error initializing data:", error);
        setError("Failed to initialize data");
      }
    };

    initializeData();
  }, []);

  const handleDownload = (fileUrl, fileName) => {
    fetch(fileUrl)
      .then((response) => response.blob())
      .then((blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.style.display = "none";
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
      })
      .catch(() => alert("An error occurred while downloading the file!"));
  };
  const getCampaigns = async () => {
    try {
      const response = await getCampaignsList(0, "all");
      if (
        response.status === "success" &&
        response.data &&
        response.data.length > 0
      ) {
        setCampaigns(response.data);
        const defaultCampaign = response.data[0];
        setCampaign(defaultCampaign);
      } else {
        setError("Failed to get campaigns");
      }
    } catch (error) {
      console.error("Error fetching campaigns:", error);
      toast({
        variant: "destructive",
        title: "Error",
        description: "Unable to fetch campaigns.",
      });
    }
  };

  const handleUpload = async (file, type) => {
    const formData = new FormData();
    formData.append("template", file);
    setUploadStatus((prev) => ({ ...prev, [type]: "uploading" }));

    try {
      let response;
      switch (type) {
        case "neft":
          response = await uploadNEFT(formData, campaign?.id);
          break;
        case "cheque":
          response = await uploadCheque(formData, campaign?.id);
          break;
        default:
          throw new Error("Invalid upload type");
      }

      if (response.status === "success") {
        setUploadStatus((prev) => ({ ...prev, [type]: "success" }));
        toast({
          variant: "success",
          title: "File uploaded successfully.",
          description: "File has been uploaded successfully.",
        });
      } else {
        setUploadStatus((prev) => ({ ...prev, [type]: "error" }));
        toast({
          variant: "destructive",
          title: "Something went wrong.",
          description: "Unable to upload file.",
        });
      }

      return response;
    } catch (error) {
      setUploadStatus((prev) => ({ ...prev, [type]: "error" }));
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to upload file.",
      });
      return error;
    }
  };
  const handleCampaignChange = (value) => {
    setCampaign(value);
  };

  return (
    <div className="container mx-auto px-4 py-12 md:py-16 lg:py-20">
      <div className="max-w-3xl mx-auto space-y-6">
        <div className="text-center">
          <h1 className="text-3xl font-bold tracking-tight sm:text-4xl">
            Upload Data
          </h1>
          <p className="mt-3 text-lg text-gray-500 dark:text-gray-400">
            Securely upload Excel files related to NEFT and Cheque Payments.
          </p>
        </div>
        <div className="space-y-2">
          <label className="text-sm font-medium">Campaign</label>
          <div className="w-48">
            <Combobox
              options={campaigns}
              placeholder="Select campaign type"
              valueProperty="id"
              labelProperty="name"
              onChange={handleCampaignChange}
              value={campaign}
              className="w-full"
            />
          </div>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <UploadCard
            title="NEFT"
            description="Upload Excel files containing information about NEFT Payments."
            inputId="neft-file"
            downloadFile={() =>
              handleDownload(
                "/src/assets/ReconcilationNEFTTemplate.xlsx",
                "ReconcilationNEFTTemplate.xlsx"
              )
            }
            onUpload={(file) => handleUpload(file, "neft")}
            uploadStatus={uploadStatus.neft}
          />
          <UploadCard
            title="Cheque"
            description="Upload Excel files containing information about Cheque Payments."
            inputId="cheque-file"
            downloadFile={() =>
              handleDownload(
                "/src/assets/ReconcilationChequeTemplate.xlsx",
                "ReconcilationChequeTemplate.xlsx"
              )
            }
            onUpload={(file) => handleUpload(file, "cheque")}
            uploadStatus={uploadStatus.cheque}
          />
        </div>
      </div>
    </div>
  );
}

function UploadCard({
  title,
  description,
  inputId,
  downloadFile,
  onUpload,
  uploadStatus,
}) {
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [error, setError] = useState(null);
  const [campaigns, setCampaigns] = useState();
  const [selectedCampaign, setSelectedCampaign] = useState();
  const [isCampaignsLoading, setIsCampaignsLoading] = useState(true);

  const getCampaigns = async () => {
    setIsCampaignsLoading(true);
    try {
      const response = await getCampaignsList(0, "all");
      if (response.status === "success" && response.data) {
        setCampaigns(response.data);
        if (response.data.length > 0) {
          setSelectedCampaign(response.data[0]);
        }
      } else {
        setError("Failed to get campaigns");
      }
    } catch (error) {
      console.log("Error", error);
      setError("Failed to fetch campaigns");
    } finally {
      setIsCampaignsLoading(false);
    }
  };

  const handleFileChange = (event) => {
    const file = event.target.files[0];
    if (file) {
      onUpload(file);
    }
  };

  useEffect(() => {
    if (selectedCampaign?.id) {
      (async () => {
        await getPaginatedRefundRequests(
          paginationModel,
          search,
          selectedCampaign?.id
        );
      })();
    }
  }, [selectedCampaign]);

  return (
    <div className="bg-white dark:bg-gray-950 p-6 rounded-lg shadow-lg">
      <div className="flex items-center justify-between">
        <h2 className="text-xl font-semibold mb-2">{title}</h2>
        <Download onClick={downloadFile} className="cursor-pointer" />
      </div>

      <p className="text-gray-500 dark:text-gray-400 mb-4">{description}</p>
      <div className="flex items-center justify-center bg-gray-100 dark:bg-gray-800 rounded-md py-8">
        <label
          htmlFor={inputId}
          className="cursor-pointer flex flex-col items-center justify-center space-y-2"
        >
          <UploadIcon className="h-8 w-8 text-gray-500 dark:text-gray-400" />
          <span className="text-sm font-medium text-gray-500 dark:text-gray-400">
            {uploadStatus === "uploading"
              ? "Uploading..."
              : uploadStatus === "success"
              ? "Upload Successful"
              : uploadStatus === "error"
              ? "Upload Failed"
              : "Choose File"}
          </span>
          <input
            id={inputId}
            type="file"
            accept=".xlsx"
            className="hidden"
            onChange={handleFileChange}
          />
        </label>
      </div>
    </div>
  );
}

UploadCard.propTypes = {
  title: PropTypes.string,
  description: PropTypes.string,
  inputId: PropTypes.string,
  downloadFile: PropTypes.func,
  onUpload: PropTypes.func,
  uploadStatus: PropTypes.string,
};

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(ImportData))
);
