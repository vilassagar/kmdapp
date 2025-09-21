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
import { Download, UploadIcon } from "lucide-react";
import PropTypes from "prop-types";
import { useState } from "react";

const PAGE_NAME = "importdata";

function ImportData() {
  const user = userStore((state) => state.user);
  const [uploadStatus, setUploadStatus] = useState({});

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

  const handleDownloadUserTemplate = async () => {
    let response = await downloadApplicationUserTemplate();
    if (response.status === "success") {
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `ApplicationUserTemplate.xlsx`);
      document.body.appendChild(link);
      link.click();
      window.URL.revokeObjectURL(url);
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to download template.",
      });
    }
  };

  const handleDownloadAssociationTemplate = async () => {
    let response = await downloadAssociationTemplate();
    if (response.status === "success") {
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `AssociationTemplate.xlsx`);
      document.body.appendChild(link);
      link.click();
      window.URL.revokeObjectURL(url);
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to download template.",
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
        case "retirees":
          response = await uploadRetiree(formData);
          break;
        case "associations":
          response = await uploadAssociations(formData);
          break;
        case "products":
          response = await uploadProducts(formData);
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

  return (
    <div className="container mx-auto px-4 py-12 md:py-16 lg:py-20">
      <div className="max-w-3xl mx-auto space-y-6">
        <div className="text-center">
          <h1 className="text-3xl font-bold tracking-tight sm:text-4xl">
            Upload Data
          </h1>
          <p className="mt-3 text-lg text-gray-500 dark:text-gray-400">
            Securely upload Excel files related to retirees, associations, and
            products.
          </p>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <UploadCard
            title="Retirees"
            description="Upload Excel files containing information about retirees."
            inputId="retiree-file"
            downloadFile={handleDownloadUserTemplate}
            onUpload={(file) => handleUpload(file, "retirees")}
            uploadStatus={uploadStatus.retirees}
          />
          {user?.userType?.name?.toLowerCase()?.trim() !== "association" && (
            <>
              <UploadCard
                title="Associations"
                description="Upload Excel files containing information about bank associations."
                inputId="bank-file"
                downloadFile={handleDownloadAssociationTemplate}
                onUpload={(file) => handleUpload(file, "associations")}
                uploadStatus={uploadStatus.associations}
              />
              <UploadCard
                title="Products"
                description="Upload Excel files containing information about products."
                inputId="product-file"
                downloadFile={() =>
                  handleDownload(
                    "/src/assets/ProductsTemplate.xlsx",
                    "ProductsTemplate.xlsx"
                  )
                }
                onUpload={(file) => handleUpload(file, "products")}
                uploadStatus={uploadStatus.products}
              />
            </>
          )}
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
  const handleFileChange = (event) => {
    const file = event.target.files[0];
    if (file) {
      onUpload(file);
    }
  };

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
