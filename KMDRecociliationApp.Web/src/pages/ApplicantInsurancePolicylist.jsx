// Complete implementation of the ApplicantInsurancePolicylist component
// with fixed pagination and properly working Next button

import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import { RPagination } from "@/components/ui/RPagination";
import { ConfirmDialog } from "@/components/ui/confirmDialog";
import RButton from "@/components/ui/rButton";
import RInput from "@/components/ui/rInput";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { toast } from "@/components/ui/use-toast";
import { usePermissionStore } from "@/lib/store";
import {
  deleteApplicant,
  getApplicantList,
  getexceldata,
  downloadTemplate,
  uploadTemplate,
} from "@/services/applicantInsurancePolicy";
import {
  CirclePlus,
  FilePenIcon,
  Search,
  Trash2Icon,
  Upload,
  Download,
} from "lucide-react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ErrorDisplay } from "@/components/ui/common/ErrorDisplay";
import { LoadingSpinner } from "@/components/ui/common/LoadingSpinner";

const PAGE_NAME = "applicantinsurancepolicy";

function ApplicantInsurancePolicylist() {
  const navigate = useNavigate();
  const [fileInput, setFileInput] = useState(null);
  const [isImporting, setIsImporting] = useState(false);

  const permissions = usePermissionStore((state) => state.permissions);
  const [searchTerm, setSearchTerm] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [paginationModel, setPaginationModel] = useState({
    pageNumber: 1,
    recordsPerPage: 50,
  });
  const [applicants, setApplicants] = useState([]);
  const [applicantIndex, setApplicantIndex] = useState(0);
  const [isLoadingDependencies, setIsLoadingDependencies] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    setIsLoadingDependencies(true);
    (async () => {
      await getPaginatedApplicants(paginationModel, searchTerm);
      setIsLoadingDependencies(false);
    })();
  }, []);

  const handleImport = async () => {
    const fileInput = document.createElement("input");
    fileInput.type = "file";
    fileInput.accept = ".xlsx, .xls, .csv";
    fileInput.style.display = "none";

    fileInput.addEventListener("change", async (event) => {
      const file = event.target.files[0];
      if (file) {
        try {
          setIsImporting(true);
          const response = await uploadTemplate(file);

          if (response.status === "success") {
            toast({
              title: "Success",
              description: "Applicants imported successfully.",
            });

            let pageModel = {
              pageNumber: 1,
              recordsPerPage: 50,
            };
            setPaginationModel(pageModel);
            await getPaginatedApplicants(pageModel, searchTerm);
          } else {
            console.error("Import error:", response.validationErrors);
            toast({
              variant: "destructive",
              title: "Import Failed",
              description:
                response.validationErrors || "Unable to import applicants.",
            });
          }
        } catch (error) {
          console.error("Import error:", error);
          toast({
            variant: "destructive",
            title: "Import Failed",
            description: error.message || "Unable to import applicants.",
          });
        } finally {
          setIsImporting(false);
          document.body.removeChild(fileInput);
        }
      } else {
        document.body.removeChild(fileInput);
      }
    });

    document.body.appendChild(fileInput);
    fileInput.click();
  };

  const handleExport = async (event) => {
    setIsLoading(true);
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 99999999,
    };

    setPaginationModel(pageModel);
    await exportApplicants(pageModel, searchTerm);
    setIsLoading(false);
  };

  const exportApplicants = async (paginationModel, searchTerm) => {
    try {
      const response = await getexceldata(
        paginationModel.pageNumber,
        paginationModel.recordsPerPage,
        searchTerm
      );

      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute(
        "download",
        `Applicant_${String(new Date().getDate()).padStart(2, "0")}${String(
          new Date().getMonth() + 1
        ).padStart(2, "0")}${new Date().getFullYear()}.xlsx`
      );
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Something went wrong",
        description: "Unable to get applicant details.",
      });
    }
  };

  const handleDownload = async (event) => {
    setIsLoading(true);
    await downloadApplicantsTemplate();
    setIsLoading(false);
  };

  const downloadApplicantsTemplate = async () => {
    try {
      const response = await downloadTemplate();

      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute(
        "download",
        `ApplicantTemplate_${String(new Date().getDate()).padStart(
          2,
          "0"
        )}${String(new Date().getMonth() + 1).padStart(
          2,
          "0"
        )}${new Date().getFullYear()}.xlsx`
      );
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Something went wrong",
        description: "Unable to download applicant template.",
      });
    }
  };

  // In the getPaginatedApplicants function
  const getPaginatedApplicants = async (paginationModel, searchTerm) => {
    try {
      setIsLoading(true);

      let response = await getApplicantList(
        paginationModel.pageNumber,
        paginationModel.recordsPerPage,
        searchTerm
      );

      if (response.status === "success") {
        setApplicants(response.data.contents || []);

        // Important: Make sure you're correctly updating both pagination models
        // with ALL required properties from the API response
        setPaginationModel(response.data.paging);
      } else {
        toast({
          variant: "destructive",
          title: "Something went wrong.",
          description: "Unable to get applicants.",
        });
      }
    } catch (error) {
      console.error("Error fetching applicants:", error);
      toast({
        variant: "destructive",
        title: "Error",
        description: error.message || "Unable to get applicant data",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteApplicant = async (event) => {
    let applicantId = applicants[applicantIndex]?.id;
    if (!applicantId) {
      toast({
        variant: "destructive",
        title: "Error",
        description: "No applicant selected for deletion.",
      });
      return;
    }

    let response = await deleteApplicant(applicantId);
    if (response.status === "success") {
      toast({
        title: "Success",
        description: "Applicant deleted successfully.",
      });

      let pageModel = {
        pageNumber: 1,
        recordsPerPage: 50,
      };

      setPaginationModel(pageModel);
      await getPaginatedApplicants(pageModel, searchTerm);
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to delete applicant.",
      });
    }
  };

  const handleEdit = (id) => {
    navigate(`/createapplicant?id=${id}&mode=edit`);
  };

  const handleNew = () => {
    navigate(`/createapplicant`);
  };

  const handleSearch = async (event) => {
    if (event.key === "Enter" || event.type === "click") {
      await handleSearchSubmit();
    } else if (searchTerm.length >= 3) {
      await handleSearchSubmit();
    } else if (searchTerm.length === 0) {
      let pageModel = {
        pageNumber: 1,
        recordsPerPage: 50,
      };

      setPaginationModel(pageModel);
      await getPaginatedApplicants(pageModel, "");
    }
  };

  const handleSearchChange = (event) => {
    setSearchTerm(event.target.value);
  };

  const handleSearchSubmit = async () => {
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };

    setPaginationModel(pageModel);
    await getPaginatedApplicants(pageModel, searchTerm);
  };

  const handleClearSearch = async () => {
    setSearchTerm("");
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };
    setPaginationModel(pageModel);
    await getPaginatedApplicants(pageModel, "");
  };

  // FIXED: handlePageChange to properly handle the paging object from RPagination
  // const handlePageChange = async (pagingFromRPagination) => {
  //   console.log(
  //     "Page changed, received from RPagination:",
  //     pagingFromRPagination
  //   );

  //   // Convert the paging object from RPagination to the format expected by your API
  //   const updatedPaginationModel = {
  //     pageNumber: pagingFromRPagination.pageNumber,
  //     recordsPerPage: pagingFromRPagination.recordsPerPage,
  //   };

  //   console.log("Updated pagination model for API:", updatedPaginationModel);

  //   // Update state and fetch data
  //   setPaginationModel(updatedPaginationModel);
  //   await getPaginatedApplicants(updatedPaginationModel, searchTerm);
  // };
  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedApplicants(paging, searchTerm);
  };
  if (isLoadingDependencies) {
    return <LoadingSpinner />;
  }

  if (error) {
    return <ErrorDisplay error={error} />;
  }

  return (
    <div>
      <div className="flex flex-col justify-between">
        <div className="w-full px-4 py-3">
          <div className="flex items-center justify-between">
            <h1 className="text-2xl font-bold">Applicants</h1>

            <div className="flex items-center mr-2">
              <RButton
                onClick={handleDownload}
                className="bg-gray-500 hover:bg-gray-600 text-white mr-2"
              >
                <div className="flex items-center">
                  <Download
                    className="cursor-pointer mr-2"
                    size={24}
                    title="Download Template"
                  />
                  <span>Download Template</span>
                </div>
              </RButton>

              <RButton
                onClick={handleExport}
                className="bg-gray-500 hover:bg-gray-600 text-white mr-2"
              >
                <div className="flex items-center">
                  <Download className="mr-2 h-4 w-4" />
                  <span>Export</span>
                </div>
              </RButton>

              {permissions?.[PAGE_NAME]?.create ? (
                <div>
                  <RButton
                    onClick={handleImport}
                    className="mr-2 bg-gray-500 hover:bg-gray-600 text-white"
                    disabled={isImporting}
                  >
                    <div className="flex items-center">
                      {isImporting ? (
                        <>
                          <div className="animate-spin mr-2 h-4 w-4 border-t-2 border-b-2 border-current rounded-full"></div>
                          <span>Importing...</span>
                        </>
                      ) : (
                        <>
                          <Upload className="mr-2 h-4 w-4" />
                          <span>Import</span>
                        </>
                      )}
                    </div>
                  </RButton>
                </div>
              ) : null}
            </div>
          </div>
        </div>
        <div className="flex items-center justify-between mb-6">
          <div className="flex flex-1 mr-4">
            <RInput
              type="search"
              placeholder="Search Applicants..."
              value={searchTerm}
              onChange={handleSearchChange}
              onKeyDown={(e) => e.key === "Enter" && handleSearch(e)}
              className="w-1/2"
            />
            <RButton
              onClick={handleSearch}
              className="ml-2 w-24 mr-2 bg-red-900 hover:bg-red-800 text-white"
            >
              <span>Search</span>
            </RButton>
            <RButton onClick={handleClearSearch} className="ml-2 mr-2">
              <span>Clear Search</span>
            </RButton>
            {permissions?.[PAGE_NAME]?.create ? (
              <div>
                <RButton onClick={handleNew} className="ml-2">
                  <div className="flex items-center">
                    <span>Create Applicant</span>
                    <CirclePlus className="ml-2 h-4 w-4" />
                  </div>
                </RButton>
              </div>
            ) : null}
          </div>
        </div>
      </div>
      <div className="border rounded-lg shadow-lg border-0">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="p-2">Name</TableHead>
              <TableHead className="p-2">Gender</TableHead>
              <TableHead className="p-2">Organisation Name</TableHead>
              <TableHead className="p-2">Contact Number</TableHead>
              <TableHead className="p-2">Id Card</TableHead>
              <TableHead className="w-[150px] p-2">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {applicants.length > 0 ? (
              applicants.map((applicant, index) => (
                <TableRow key={applicant.id}>
                  <TableCell className="p-2">{applicant.fullName}</TableCell>
                  <TableCell className="p-2">{applicant.genderName}</TableCell>
                  <TableCell className="p-2">
                    {applicant.associatedOrganization || ""}
                  </TableCell>
                  <TableCell className="p-2">
                    {applicant.mobileNumber || ""}
                  </TableCell>
                  <TableCell className="p-2">
                    {applicant.idCardTypeName + " - " + applicant.idCardNumber}
                  </TableCell>
                  <TableCell className="p-1">
                    <div className="flex">
                      {permissions?.[PAGE_NAME]?.update ? (
                        <RButton
                          variant="ghost"
                          className="flex items-center gap-2"
                          onClick={() => handleEdit(applicant.id)}
                        >
                          <FilePenIcon className="h-4 w-4" />
                        </RButton>
                      ) : null}
                      {permissions?.[PAGE_NAME]?.delete ? (
                        <ConfirmDialog
                          dialogTrigger={
                            <RButton
                              variant="ghost"
                              className="flex items-center gap-2"
                              onClick={() => setApplicantIndex(index)}
                            >
                              <Trash2Icon className="h-4 w-4 text-red-500" />
                            </RButton>
                          }
                          onConfirm={handleDeleteApplicant}
                          dialogTitle="Are you sure to delete the applicant?"
                          dialogDescription="This action cannot be undone. This will permanently delete your applicant and remove the data from our servers."
                        />
                      ) : null}
                    </div>
                  </TableCell>
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell colSpan={6} className="h-24 text-center">
                  No results.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
      <div className="flex justify-end">
        {/* FIXED: Pass correct props to RPagination */}
        <RPagination
          paginationModel={paginationModel}
          onPageChange={handlePageChange}
        />
      </div>
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(ApplicantInsurancePolicylist))
);
