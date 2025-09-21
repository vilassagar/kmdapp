/* eslint-disable react/prop-types */
// File: ApplicantManagement.jsx
import React, { useState, useEffect, useRef } from "react";
import {
  Paperclip,
  FileDown,
  FileUp,
  Pencil,
  Eye,
  UserPlus,
  Loader2,
  XCircle,
  CheckCircle,
  AlertCircle,
} from "lucide-react";

const ApplicantManagement = () => {
  // State management
  const [activeTab, setActiveTab] = useState("list");
  const [applicants, setApplicants] = useState([]);
  const [selectedApplicant, setSelectedApplicant] = useState(null);
  const [isEditMode, setIsEditMode] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState({ text: "", type: "" });
  const [searchTerm, setSearchTerm] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [sortBy, setSortBy] = useState("fullName");
  const [sortDescending, setSortDescending] = useState(false);
  const [filterOptions, setFilterOptions] = useState({
    organization: "",
    idCardType: "",
    gender: "",
  });

  // File upload reference
  const fileInputRef = useRef(null);
  const [fileUploadProgress, setFileUploadProgress] = useState(0);
  const [importStatus, setImportStatus] = useState("idle"); // idle, loading, success, error
  const [exportFormat, setExportFormat] = useState("xlsx");
  const [exportLoading, setExportLoading] = useState(false);

  // Form state
  const [formData, setFormData] = useState({
    fullName: "",
    guardianName: "",
    dateOfBirth: "",
    gender: "",
    contactNumber: "",
    salary: "",
    associatedOrganization: "",
    address: "",
    idCardType: "",
    idCardNumber: "",
    bankDetails: {
      bankName: "",
      bankAccountNumber: "",
      bankIfscCode: "",
      bankBranchDetails: "",
      accountHolderName: "",
      accountType: "",
    },
    dependents: [],
  });

  // Fetch applicants when page, sort, or filters change
  useEffect(() => {
    fetchApplicants();
  }, [currentPage, pageSize, sortBy, sortDescending, filterOptions]);

  // Fetch applicants from API with pagination, sorting and filtering
  const fetchApplicants = async () => {
    setIsLoading(true);
    try {
      // Build query parameters
      const params = new URLSearchParams({
        pageNumber: currentPage,
        pageSize: pageSize,
        sortBy: sortBy,
        sortDescending: sortDescending,
        searchTerm: searchTerm,
        ...filterOptions,
      });

      const response = await fetch(`/api/applicants/filter?${params}`);
      if (response.ok) {
        const data = await response.json();
        setApplicants(data.items);
        setTotalPages(data.totalPages);
      } else {
        setMessage({ text: "Failed to fetch applicants", type: "error" });
      }
    } catch (error) {
      setMessage({ text: `Error: ${error.message}`, type: "error" });
    } finally {
      setIsLoading(false);
    }
  };

  // Handle search submission
  const handleSearch = (e) => {
    e.preventDefault();
    setCurrentPage(1); // Reset to first page
    fetchApplicants();
  };

  // Handle filter changes
  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    setFilterOptions((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  // Handle sort change
  const handleSortChange = (field) => {
    if (sortBy === field) {
      setSortDescending(!sortDescending);
    } else {
      setSortBy(field);
      setSortDescending(false);
    }
  };

  // Handle form input change
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    if (name.includes(".")) {
      const [parent, child] = name.split(".");
      setFormData({
        ...formData,
        [parent]: {
          ...formData[parent],
          [child]: value,
        },
      });
    } else {
      setFormData({
        ...formData,
        [name]: value,
      });
    }
  };

  // Handle dependent input change
  const handleDependentChange = (index, e) => {
    const { name, value } = e.target;
    const updatedDependents = [...formData.dependents];
    updatedDependents[index] = {
      ...updatedDependents[index],
      [name]: value,
    };
    setFormData({
      ...formData,
      dependents: updatedDependents,
    });
  };

  // Add new dependent
  const addDependent = () => {
    setFormData({
      ...formData,
      dependents: [
        ...formData.dependents,
        {
          fullName: "",
          dateOfBirth: "",
          gender: "",
          relationship: "",
          idCardType: "",
          idCardNumber: "",
          contactNumber: "",
        },
      ],
    });
  };

  // Remove dependent
  const removeDependent = (index) => {
    const updatedDependents = [...formData.dependents];
    updatedDependents.splice(index, 1);
    setFormData({
      ...formData,
      dependents: updatedDependents,
    });
  };

  // Form submission
  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      const url = isEditMode
        ? `/api/applicants/${selectedApplicant.id}`
        : "/api/applicants";
      const method = isEditMode ? "PUT" : "POST";
      const response = await fetch(url, {
        method,
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(formData),
      });

      if (response.ok) {
        const data = await response.json();
        setMessage({
          text: isEditMode
            ? "Applicant updated successfully"
            : "Applicant created successfully",
          type: "success",
        });
        fetchApplicants();
        if (!isEditMode) {
          resetForm();
        }
        if (isEditMode) {
          setSelectedApplicant(data);
        }
        setActiveTab("list");
      } else {
        const errorData = await response.json();
        setMessage({
          text: errorData.message || "Operation failed",
          type: "error",
        });
      }
    } catch (error) {
      setMessage({ text: `Error: ${error.message}`, type: "error" });
    } finally {
      setIsLoading(false);
    }
  };

  // Reset form to initial state
  const resetForm = () => {
    setFormData({
      fullName: "",
      guardianName: "",
      dateOfBirth: "",
      gender: "",
      contactNumber: "",
      salary: "",
      associatedOrganization: "",
      address: "",
      idCardType: "",
      idCardNumber: "",
      bankDetails: {
        bankName: "",
        bankAccountNumber: "",
        bankIfscCode: "",
        bankBranchDetails: "",
        accountHolderName: "",
        accountType: "",
      },
      dependents: [],
    });
  };

  // Handle view applicant
  const handleViewApplicant = async (id) => {
    setIsLoading(true);
    try {
      const response = await fetch(`/api/applicants/${id}`);
      if (response.ok) {
        const data = await response.json();
        setSelectedApplicant(data);
        setActiveTab("view");
      } else {
        setMessage({
          text: "Failed to fetch applicant details",
          type: "error",
        });
      }
    } catch (error) {
      setMessage({ text: `Error: ${error.message}`, type: "error" });
    } finally {
      setIsLoading(false);
    }
  };

  // Handle edit applicant
  const handleEditApplicant = async (id) => {
    setIsLoading(true);
    try {
      const response = await fetch(`/api/applicants/${id}`);
      if (response.ok) {
        const data = await response.json();
        setFormData(data);
        setSelectedApplicant(data);
        setIsEditMode(true);
        setActiveTab("form");
      } else {
        setMessage({
          text: "Failed to fetch applicant details",
          type: "error",
        });
      }
    } catch (error) {
      setMessage({ text: `Error: ${error.message}`, type: "error" });
    } finally {
      setIsLoading(false);
    }
  };

  // Handle create new applicant
  const handleCreateApplicant = () => {
    resetForm();
    setIsEditMode(false);
    setActiveTab("form");
  };

  // Handle file selection
  const handleFileSelect = () => {
    fileInputRef.current.click();
  };

  // Handle file import through API
  const handleFileImport = async (e) => {
    const file = e.target.files[0];
    if (!file) return;

    const formDataUpload = new FormData();
    formDataUpload.append("file", file);

    setImportStatus("loading");
    setFileUploadProgress(0);

    try {
      const xhr = new XMLHttpRequest();

      // Setup progress event
      xhr.upload.addEventListener("progress", (event) => {
        if (event.lengthComputable) {
          const percentComplete = Math.round(
            (event.loaded / event.total) * 100
          );
          setFileUploadProgress(percentComplete);
        }
      });

      // Setup completion event
      xhr.addEventListener("load", () => {
        if (xhr.status >= 200 && xhr.status < 300) {
          setImportStatus("success");
          setMessage({ text: "File imported successfully", type: "success" });
          fetchApplicants(); // Refresh the list
        } else {
          setImportStatus("error");
          setMessage({
            text: `Import failed: ${xhr.statusText}`,
            type: "error",
          });
        }
      });

      // Setup error event
      xhr.addEventListener("error", () => {
        setImportStatus("error");
        setMessage({
          text: "Import failed due to network error",
          type: "error",
        });
      });

      // Send the request
      xhr.open("POST", "/api/applicants/import");
      xhr.send(formDataUpload);
    } catch (error) {
      setImportStatus("error");
      setMessage({ text: `Error: ${error.message}`, type: "error" });
    }
  };

  // Handle export applicants
  const handleExport = async () => {
    setExportLoading(true);
    try {
      // Build filter parameters to match current view
      const params = new URLSearchParams({
        format: exportFormat,
        sortBy: sortBy,
        sortDescending: sortDescending,
        searchTerm: searchTerm,
        ...filterOptions,
      });

      const response = await fetch(`/api/applicants/export?${params}`);

      if (!response.ok) {
        throw new Error(`Export failed: ${response.statusText}`);
      }

      // Get filename from response headers or use default
      const contentDisposition = response.headers.get("content-disposition");
      let filename = "applicants." + exportFormat;

      if (contentDisposition) {
        const filenameMatch = contentDisposition.match(/filename="(.+)"/);
        if (filenameMatch) {
          filename = filenameMatch[1];
        }
      }

      // Create a blob and download it
      const blob = await response.blob();
      const url = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = filename;
      a.click();
      URL.revokeObjectURL(url);

      setMessage({ text: "Export completed successfully", type: "success" });
    } catch (error) {
      setMessage({ text: `Export failed: ${error.message}`, type: "error" });
    } finally {
      setExportLoading(false);
    }
  };

  // Message component
  // eslint-disable-next-line react/prop-types
  const MessageDisplay = ({ message }) => {
    if (!message.text) return null;

    const bgColor = message.type === "success" ? "bg-green-100" : "bg-red-100";
    const textColor =
      message.type === "success" ? "text-green-800" : "text-red-800";
    const Icon = message.type === "success" ? CheckCircle : AlertCircle;

    return (
      <div
        className={`${bgColor} ${textColor} p-3 rounded-md flex items-center mb-4`}
      >
        <Icon className="w-5 h-5 mr-2" />
        <span>{message.text}</span>
      </div>
    );
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-2xl font-bold mb-6">
        Insurance Policy Applicant Management
      </h1>

      {/* Message display */}
      <MessageDisplay message={message} />

      {/* Tabs */}
      <div className="flex flex-wrap border-b border-gray-200 mb-6">
        <button
          className={`px-4 py-2 font-medium text-sm mr-2 ${
            activeTab === "list"
              ? "border-b-2 border-blue-500 text-blue-600"
              : "text-gray-500 hover:text-gray-700"
          }`}
          onClick={() => setActiveTab("list")}
        >
          Applicant List
        </button>
        <button
          className={`px-4 py-2 font-medium text-sm mr-2 ${
            activeTab === "form"
              ? "border-b-2 border-blue-500 text-blue-600"
              : "text-gray-500 hover:text-gray-700"
          }`}
          onClick={handleCreateApplicant}
        >
          {isEditMode ? "Edit Applicant" : "Create Applicant"}
        </button>
        {selectedApplicant && (
          <button
            className={`px-4 py-2 font-medium text-sm mr-2 ${
              activeTab === "view"
                ? "border-b-2 border-blue-500 text-blue-600"
                : "text-gray-500 hover:text-gray-700"
            }`}
            onClick={() => setActiveTab("view")}
          >
            View Applicant
          </button>
        )}
        <button
          className={`px-4 py-2 font-medium text-sm mr-2 ${
            activeTab === "import"
              ? "border-b-2 border-blue-500 text-blue-600"
              : "text-gray-500 hover:text-gray-700"
          }`}
          onClick={() => setActiveTab("import")}
        >
          Import/Export
        </button>
      </div>

      {/* List View */}
      {activeTab === "list" && (
        <div>
          <div className="bg-white rounded-lg shadow overflow-hidden">
            {/* Search and Filter Controls */}
            <div className="p-4 border-b border-gray-200">
              <div className="flex flex-col md:flex-row gap-4 mb-4">
                {/* Search */}
                <div className="flex-1">
                  <div className="flex">
                    <input
                      type="text"
                      className="w-full p-2 border border-gray-300 rounded-l-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      placeholder="Search by name, ID or organization..."
                      value={searchTerm}
                      onChange={(e) => setSearchTerm(e.target.value)}
                      onKeyPress={(e) => e.key === "Enter" && handleSearch(e)}
                    />
                    <button
                      onClick={handleSearch}
                      className="bg-blue-500 text-white px-4 py-2 rounded-r-md hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-500"
                    >
                      Search
                    </button>
                  </div>
                </div>

                {/* Add New Button */}
                <div>
                  <button
                    onClick={handleCreateApplicant}
                    className="bg-green-500 text-white px-4 py-2 rounded-md hover:bg-green-600 flex items-center"
                  >
                    <UserPlus className="w-5 h-5 mr-1" />
                    <span>Add New</span>
                  </button>
                </div>
              </div>

              {/* Filters */}
              <div className="flex flex-wrap gap-4">
                <div className="w-full sm:w-auto">
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Organization
                  </label>
                  <select
                    name="organization"
                    className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    value={filterOptions.organization}
                    onChange={handleFilterChange}
                  >
                    <option value="">All Organizations</option>
                    <option value="Org1">Organization 1</option>
                    <option value="Org2">Organization 2</option>
                    <option value="Org3">Organization 3</option>
                  </select>
                </div>

                <div className="w-full sm:w-auto">
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    ID Card Type
                  </label>
                  <select
                    name="idCardType"
                    className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    value={filterOptions.idCardType}
                    onChange={handleFilterChange}
                  >
                    <option value="">All Types</option>
                    <option value="Aadhar">Aadhar</option>
                    <option value="Pan">Pan</option>
                    <option value="VoterId">Voter ID</option>
                    <option value="EShram">E-Shram</option>
                    <option value="BOCW">BOCW</option>
                  </select>
                </div>

                <div className="w-full sm:w-auto">
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Gender
                  </label>
                  <select
                    name="gender"
                    className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    value={filterOptions.gender}
                    onChange={handleFilterChange}
                  >
                    <option value="">All Genders</option>
                    <option value="Male">Male</option>
                    <option value="Female">Female</option>
                    <option value="Other">Other</option>
                  </select>
                </div>
              </div>
            </div>

            {/* Table */}
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th
                      scope="col"
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer"
                      onClick={() => handleSortChange("fullName")}
                    >
                      <div className="flex items-center">
                        <span>Full Name</span>
                        {sortBy === "fullName" && (
                          <span className="ml-1">
                            {sortDescending ? "↓" : "↑"}
                          </span>
                        )}
                      </div>
                    </th>
                    <th
                      scope="col"
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer"
                      onClick={() => handleSortChange("dateOfBirth")}
                    >
                      <div className="flex items-center">
                        <span>Date of Birth</span>
                        {sortBy === "dateOfBirth" && (
                          <span className="ml-1">
                            {sortDescending ? "↓" : "↑"}
                          </span>
                        )}
                      </div>
                    </th>
                    <th
                      scope="col"
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer"
                      onClick={() => handleSortChange("gender")}
                    >
                      <div className="flex items-center">
                        <span>Gender</span>
                        {sortBy === "gender" && (
                          <span className="ml-1">
                            {sortDescending ? "↓" : "↑"}
                          </span>
                        )}
                      </div>
                    </th>
                    <th
                      scope="col"
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer"
                      onClick={() => handleSortChange("associatedOrganization")}
                    >
                      <div className="flex items-center">
                        <span>Organization</span>
                        {sortBy === "associatedOrganization" && (
                          <span className="ml-1">
                            {sortDescending ? "↓" : "↑"}
                          </span>
                        )}
                      </div>
                    </th>
                    <th
                      scope="col"
                      className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                    >
                      ID Card
                    </th>
                    <th
                      scope="col"
                      className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider"
                    >
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {isLoading ? (
                    <tr>
                      <td colSpan="6" className="px-6 py-4 text-center">
                        <div className="flex justify-center items-center">
                          <Loader2 className="w-6 h-6 animate-spin text-blue-500 mr-2" />
                          <span>Loading...</span>
                        </div>
                      </td>
                    </tr>
                  ) : applicants.length === 0 ? (
                    <tr>
                      <td
                        colSpan="6"
                        className="px-6 py-4 text-center text-gray-500"
                      >
                        No applicants found
                      </td>
                    </tr>
                  ) : (
                    applicants.map((applicant) => (
                      <tr key={applicant.id} className="hover:bg-gray-50">
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="text-sm font-medium text-gray-900">
                            {applicant.fullName}
                          </div>
                          <div className="text-sm text-gray-500">
                            {applicant.contactNumber}
                          </div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="text-sm text-gray-900">
                            {new Date(
                              applicant.dateOfBirth
                            ).toLocaleDateString()}
                          </div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="text-sm text-gray-900">
                            {applicant.gender}
                          </div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="text-sm text-gray-900">
                            {applicant.associatedOrganization}
                          </div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="text-sm text-gray-900">
                            {applicant.idCardType}
                          </div>
                          <div className="text-sm text-gray-500">
                            {applicant.idCardNumber}
                          </div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                          <button
                            onClick={() => handleViewApplicant(applicant.id)}
                            className="text-blue-600 hover:text-blue-900 mr-3"
                          >
                            <Eye className="w-5 h-5" />
                          </button>
                          <button
                            onClick={() => handleEditApplicant(applicant.id)}
                            className="text-yellow-600 hover:text-yellow-900"
                          >
                            <Pencil className="w-5 h-5" />
                          </button>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>

            {/* Pagination */}
            <div className="px-6 py-4 bg-gray-50 border-t border-gray-200 flex items-center justify-between">
              <div className="flex items-center">
                <span className="text-sm text-gray-700">
                  Showing page {currentPage} of {totalPages}
                </span>
              </div>
              <div className="flex items-center space-x-2">
                <button
                  onClick={() =>
                    setCurrentPage((prev) => Math.max(prev - 1, 1))
                  }
                  disabled={currentPage <= 1}
                  className={`px-3 py-1 border rounded-md ${
                    currentPage <= 1
                      ? "bg-gray-200 text-gray-500 cursor-not-allowed"
                      : "bg-white text-gray-700 hover:bg-gray-100"
                  }`}
                >
                  Previous
                </button>
                <button
                  onClick={() =>
                    setCurrentPage((prev) => Math.min(prev + 1, totalPages))
                  }
                  disabled={currentPage >= totalPages}
                  className={`px-3 py-1 border rounded-md ${
                    currentPage >= totalPages
                      ? "bg-gray-200 text-gray-500 cursor-not-allowed"
                      : "bg-white text-gray-700 hover:bg-gray-100"
                  }`}
                >
                  Next
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Form View */}
      {activeTab === "form" && (
        <div className="bg-white rounded-lg shadow p-6">
          <h2 className="text-xl font-semibold mb-6">
            {isEditMode ? "Edit Applicant" : "Create New Applicant"}
          </h2>

          <div onSubmit={handleSubmit}>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {/* Personal Information Section */}
              <div className="md:col-span-2">
                <h3 className="text-lg font-medium text-gray-900 mb-3">
                  Personal Information
                </h3>
                <div className="border-t border-gray-200 pt-4">
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Full Name *
                      </label>
                      <input
                        type="text"
                        name="fullName"
                        required
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.fullName}
                        onChange={handleInputChange}
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Son/Daughter of
                      </label>
                      <input
                        type="text"
                        name="guardianName"
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.guardianName}
                        onChange={handleInputChange}
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Date of Birth *
                      </label>
                      <input
                        type="date"
                        name="dateOfBirth"
                        required
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.dateOfBirth.split("T")[0]} // Format date for input
                        onChange={handleInputChange}
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Gender *
                      </label>
                      <select
                        name="gender"
                        required
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.gender}
                        onChange={handleInputChange}
                      >
                        <option value="">Select Gender</option>
                        <option value="Male">Male</option>
                        <option value="Female">Female</option>
                        <option value="Other">Other</option>
                      </select>
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Contact/Mobile No
                      </label>
                      <input
                        type="tel"
                        name="contactNumber"
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.contactNumber}
                        onChange={handleInputChange}
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Salary *
                      </label>
                      <input
                        type="number"
                        name="salary"
                        required
                        min="0"
                        step="0.01"
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.salary}
                        onChange={handleInputChange}
                      />
                    </div>
                  </div>
                </div>
              </div>

              {/* Organization & ID Information */}
              <div className="md:col-span-2">
                <h3 className="text-lg font-medium text-gray-900 mb-3">
                  Organization & ID Information
                </h3>
                <div className="border-t border-gray-200 pt-4">
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Associated Organization *
                      </label>
                      <input
                        type="text"
                        name="associatedOrganization"
                        required
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.associatedOrganization}
                        onChange={handleInputChange}
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Location/Address *
                      </label>
                      <input
                        type="text"
                        name="address"
                        required
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.address}
                        onChange={handleInputChange}
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        ID Card Type *
                      </label>
                      <select
                        name="idCardType"
                        required
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.idCardType}
                        onChange={handleInputChange}
                      >
                        <option value="">Select ID Card Type</option>
                        <option value="Aadhar">Aadhar</option>
                        <option value="Pan">Pan</option>
                        <option value="VoterId">Voter ID</option>
                        <option value="EShram">E-Shram</option>
                        <option value="BOCW">BOCW</option>
                      </select>
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        ID Card No *
                      </label>
                      <input
                        type="text"
                        name="idCardNumber"
                        required
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.idCardNumber}
                        onChange={handleInputChange}
                      />
                    </div>
                  </div>
                </div>
              </div>

              {/* Bank Details */}
              <div className="md:col-span-2">
                <h3 className="text-lg font-medium text-gray-900 mb-3">
                  Bank Details
                </h3>
                <div className="border-t border-gray-200 pt-4">
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Bank Name *
                      </label>
                      <input
                        type="text"
                        name="bankDetails.bankName"
                        required
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.bankDetails.bankName}
                        onChange={handleInputChange}
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Bank Account No *
                      </label>
                      <input
                        type="text"
                        name="bankDetails.bankAccountNumber"
                        required
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.bankDetails.bankAccountNumber}
                        onChange={handleInputChange}
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Bank IFSC Code *
                      </label>
                      <input
                        type="text"
                        name="bankDetails.bankIfscCode"
                        required
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.bankDetails.bankIfscCode}
                        onChange={handleInputChange}
                      />
                    </div>

                    <div className="md:col-span-3">
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Bank Branch Name & Address *
                      </label>
                      <input
                        type="text"
                        name="bankDetails.bankBranchDetails"
                        required
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.bankDetails.bankBranchDetails}
                        onChange={handleInputChange}
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Account Holder Name
                      </label>
                      <input
                        type="text"
                        name="bankDetails.accountHolderName"
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.bankDetails.accountHolderName}
                        onChange={handleInputChange}
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Account Type
                      </label>
                      <select
                        name="bankDetails.accountType"
                        className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                        value={formData.bankDetails.accountType}
                        onChange={handleInputChange}
                      >
                        <option value="">Select Account Type</option>
                        <option value="Savings">Savings</option>
                        <option value="Current">Current</option>
                        <option value="Salary">Salary</option>
                      </select>
                    </div>
                  </div>
                </div>
              </div>

              {/* Dependents */}
              <div className="md:col-span-2">
                <div className="flex justify-between items-center mb-3">
                  <h3 className="text-lg font-medium text-gray-900">
                    Dependents
                  </h3>
                  <button
                    type="button"
                    onClick={addDependent}
                    className="bg-blue-100 text-blue-700 px-3 py-1 rounded-md hover:bg-blue-200 flex items-center text-sm"
                  >
                    <span className="mr-1">+</span> Add Dependent
                  </button>
                </div>
                <div className="border-t border-gray-200 pt-4">
                  {formData.dependents.length === 0 ? (
                    <p className="text-gray-500 text-center py-4">
                      No dependents added
                    </p>
                  ) : (
                    formData.dependents.map((dependent, index) => (
                      <div
                        key={index}
                        className="mb-6 p-4 border border-gray-200 rounded-md bg-gray-50"
                      >
                        <div className="flex justify-between items-center mb-3">
                          <h4 className="font-medium">
                            Dependent #{index + 1}
                          </h4>
                          <button
                            type="button"
                            onClick={() => removeDependent(index)}
                            className="text-red-600 hover:text-red-800 text-sm"
                          >
                            Remove
                          </button>
                        </div>
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                          <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                              Full Name *
                            </label>
                            <input
                              type="text"
                              name="fullName"
                              required
                              className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                              value={dependent.fullName}
                              onChange={(e) => handleDependentChange(index, e)}
                            />
                          </div>

                          <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                              Date of Birth *
                            </label>
                            <input
                              type="date"
                              name="dateOfBirth"
                              required
                              className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                              value={
                                dependent.dateOfBirth
                                  ? dependent.dateOfBirth.split("T")[0]
                                  : ""
                              }
                              onChange={(e) => handleDependentChange(index, e)}
                            />
                          </div>

                          <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                              Gender *
                            </label>
                            <select
                              name="gender"
                              required
                              className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                              value={dependent.gender}
                              onChange={(e) => handleDependentChange(index, e)}
                            >
                              <option value="">Select Gender</option>
                              <option value="Male">Male</option>
                              <option value="Female">Female</option>
                              <option value="Other">Other</option>
                            </select>
                          </div>

                          <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                              Relationship *
                            </label>
                            <input
                              type="text"
                              name="relationship"
                              required
                              className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                              value={dependent.relationship}
                              onChange={(e) => handleDependentChange(index, e)}
                            />
                          </div>

                          <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                              ID Card Type
                            </label>
                            <select
                              name="idCardType"
                              className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                              value={dependent.idCardType}
                              onChange={(e) => handleDependentChange(index, e)}
                            >
                              <option value="">Select ID Card Type</option>
                              <option value="Aadhar">Aadhar</option>
                              <option value="Pan">Pan</option>
                              <option value="VoterId">Voter ID</option>
                              <option value="EShram">E-Shram</option>
                              <option value="BOCW">BOCW</option>
                            </select>
                          </div>

                          <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                              ID Card No
                            </label>
                            <input
                              type="text"
                              name="idCardNumber"
                              className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                              value={dependent.idCardNumber}
                              onChange={(e) => handleDependentChange(index, e)}
                            />
                          </div>

                          <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                              Contact Number
                            </label>
                            <input
                              type="tel"
                              name="contactNumber"
                              className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                              value={dependent.contactNumber}
                              onChange={(e) => handleDependentChange(index, e)}
                            />
                          </div>
                        </div>
                      </div>
                    ))
                  )}
                </div>
              </div>
            </div>

            <div className="mt-8 flex justify-end space-x-4">
              <button
                type="button"
                onClick={() => {
                  resetForm();
                  setActiveTab("list");
                }}
                className="px-4 py-2 border border-gray-300 rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                Cancel
              </button>
              <button
                type="button"
                onClick={handleSubmit}
                disabled={isLoading}
                className="px-4 py-2 border border-transparent rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 flex items-center"
              >
                {isLoading && <Loader2 className="w-4 h-4 mr-2 animate-spin" />}
                {isEditMode ? "Update Applicant" : "Create Applicant"}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* View Details */}
      {activeTab === "view" && selectedApplicant && (
        <div className="bg-white rounded-lg shadow">
          <div className="px-6 py-4 border-b border-gray-200 flex justify-between items-center">
            <h2 className="text-xl font-semibold">Applicant Details</h2>
            <button
              onClick={() => handleEditApplicant(selectedApplicant.id)}
              className="bg-yellow-100 text-yellow-800 px-3 py-1 rounded-md hover:bg-yellow-200 flex items-center"
            >
              <Pencil className="w-4 h-4 mr-1" />
              Edit
            </button>
          </div>

          <div className="p-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {/* Personal Information */}
              <div className="col-span-2">
                <h3 className="text-lg font-medium text-gray-900 mb-3">
                  Personal Information
                </h3>
                <div className="bg-gray-50 p-4 rounded-md">
                  <dl className="grid grid-cols-1 md:grid-cols-2 gap-x-4 gap-y-2">
                    <div className="sm:col-span-1">
                      <dt className="text-sm font-medium text-gray-500">
                        Full Name
                      </dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {selectedApplicant.fullName}
                      </dd>
                    </div>

                    <div className="sm:col-span-1">
                      <dt className="text-sm font-medium text-gray-500">
                        Son/Daughter of
                      </dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {selectedApplicant.guardianName || "-"}
                      </dd>
                    </div>

                    <div className="sm:col-span-1">
                      <dt className="text-sm font-medium text-gray-500">
                        Date of Birth
                      </dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {new Date(
                          selectedApplicant.dateOfBirth
                        ).toLocaleDateString()}
                      </dd>
                    </div>

                    <div className="sm:col-span-1">
                      <dt className="text-sm font-medium text-gray-500">
                        Gender
                      </dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {selectedApplicant.gender}
                      </dd>
                    </div>

                    <div className="sm:col-span-1">
                      <dt className="text-sm font-medium text-gray-500">
                        Contact/Mobile No
                      </dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {selectedApplicant.contactNumber || "-"}
                      </dd>
                    </div>

                    <div className="sm:col-span-1">
                      <dt className="text-sm font-medium text-gray-500">
                        Salary
                      </dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {selectedApplicant.salary}
                      </dd>
                    </div>
                  </dl>
                </div>
              </div>

              {/* Organization & ID Information */}
              <div className="col-span-2">
                <h3 className="text-lg font-medium text-gray-900 mb-3">
                  Organization & ID Information
                </h3>
                <div className="bg-gray-50 p-4 rounded-md">
                  <dl className="grid grid-cols-1 md:grid-cols-2 gap-x-4 gap-y-2">
                    <div className="sm:col-span-1">
                      <dt className="text-sm font-medium text-gray-500">
                        Associated Organization
                      </dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {selectedApplicant.associatedOrganization}
                      </dd>
                    </div>

                    <div className="sm:col-span-1">
                      <dt className="text-sm font-medium text-gray-500">
                        Location/Address
                      </dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {selectedApplicant.address}
                      </dd>
                    </div>

                    <div className="sm:col-span-1">
                      <dt className="text-sm font-medium text-gray-500">
                        ID Card Type
                      </dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {selectedApplicant.idCardType}
                      </dd>
                    </div>

                    <div className="sm:col-span-1">
                      <dt className="text-sm font-medium text-gray-500">
                        ID Card No
                      </dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {selectedApplicant.idCardNumber}
                      </dd>
                    </div>
                  </dl>
                </div>
              </div>

              {/* Bank Details */}
              <div className="col-span-2">
                <h3 className="text-lg font-medium text-gray-900 mb-3">
                  Bank Details
                </h3>
                <div className="bg-gray-50 p-4 rounded-md">
                  <dl className="grid grid-cols-1 md:grid-cols-2 gap-x-4 gap-y-2">
                    <div className="sm:col-span-1">
                      <dt className="text-sm font-medium text-gray-500">
                        Bank Name
                      </dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {selectedApplicant.bankDetails?.bankName}
                      </dd>
                    </div>

                    <div className="sm:col-span-1">
                      <dt className="text-sm font-medium text-gray-500">
                        Bank Account No
                      </dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {selectedApplicant.bankDetails?.bankAccountNumber}
                      </dd>
                    </div>

                    <div className="sm:col-span-1">
                      <dt className="text-sm font-medium text-gray-500">
                        Bank IFSC Code
                      </dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {selectedApplicant.bankDetails?.bankIfscCode}
                      </dd>
                    </div>

                    <div className="sm:col-span-2">
                      <dt className="text-sm font-medium text-gray-500">
                        Bank Branch Name & Address
                      </dt>
                      <dd className="mt-1 text-sm text-gray-900">
                        {selectedApplicant.bankDetails?.bankBranchDetails}
                      </dd>
                    </div>

                    {selectedApplicant.bankDetails?.accountHolderName && (
                      <div className="sm:col-span-1">
                        <dt className="text-sm font-medium text-gray-500">
                          Account Holder Name
                        </dt>
                        <dd className="mt-1 text-sm text-gray-900">
                          {selectedApplicant.bankDetails?.accountHolderName}
                        </dd>
                      </div>
                    )}

                    {selectedApplicant.bankDetails?.accountType && (
                      <div className="sm:col-span-1">
                        <dt className="text-sm font-medium text-gray-500">
                          Account Type
                        </dt>
                        <dd className="mt-1 text-sm text-gray-900">
                          {selectedApplicant.bankDetails?.accountType}
                        </dd>
                      </div>
                    )}
                  </dl>
                </div>
              </div>

              {/* Dependents */}
              <div className="col-span-2">
                <h3 className="text-lg font-medium text-gray-900 mb-3">
                  Dependents
                </h3>
                {!selectedApplicant.dependents ||
                selectedApplicant.dependents.length === 0 ? (
                  <div className="bg-gray-50 p-4 rounded-md">
                    <p className="text-sm text-gray-500 text-center">
                      No dependents
                    </p>
                  </div>
                ) : (
                  <div className="space-y-4">
                    {selectedApplicant.dependents.map((dependent, index) => (
                      <div key={index} className="bg-gray-50 p-4 rounded-md">
                        <h4 className="font-medium text-gray-900 mb-3">
                          Dependent #{index + 1}
                        </h4>
                        <dl className="grid grid-cols-1 md:grid-cols-2 gap-x-4 gap-y-2">
                          <div className="sm:col-span-1">
                            <dt className="text-sm font-medium text-gray-500">
                              Full Name
                            </dt>
                            <dd className="mt-1 text-sm text-gray-900">
                              {dependent.fullName}
                            </dd>
                          </div>

                          <div className="sm:col-span-1">
                            <dt className="text-sm font-medium text-gray-500">
                              Date of Birth
                            </dt>
                            <dd className="mt-1 text-sm text-gray-900">
                              {new Date(
                                dependent.dateOfBirth
                              ).toLocaleDateString()}
                            </dd>
                          </div>

                          <div className="sm:col-span-1">
                            <dt className="text-sm font-medium text-gray-500">
                              Gender
                            </dt>
                            <dd className="mt-1 text-sm text-gray-900">
                              {dependent.gender}
                            </dd>
                          </div>

                          <div className="sm:col-span-1">
                            <dt className="text-sm font-medium text-gray-500">
                              Relationship
                            </dt>
                            <dd className="mt-1 text-sm text-gray-900">
                              {dependent.relationship}
                            </dd>
                          </div>

                          {dependent.idCardType && (
                            <div className="sm:col-span-1">
                              <dt className="text-sm font-medium text-gray-500">
                                ID Card Type
                              </dt>
                              <dd className="mt-1 text-sm text-gray-900">
                                {dependent.idCardType}
                              </dd>
                            </div>
                          )}

                          {dependent.idCardNumber && (
                            <div className="sm:col-span-1">
                              <dt className="text-sm font-medium text-gray-500">
                                ID Card No
                              </dt>
                              <dd className="mt-1 text-sm text-gray-900">
                                {dependent.idCardNumber}
                              </dd>
                            </div>
                          )}

                          {dependent.contactNumber && (
                            <div className="sm:col-span-1">
                              <dt className="text-sm font-medium text-gray-500">
                                Contact Number
                              </dt>
                              <dd className="mt-1 text-sm text-gray-900">
                                {dependent.contactNumber}
                              </dd>
                            </div>
                          )}
                        </dl>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Import/Export View */}
      {activeTab === "import" && (
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <div className="px-6 py-4 border-b border-gray-200">
            <h2 className="text-xl font-semibold">Import/Export Applicants</h2>
          </div>

          <div className="p-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
              {/* Import Section */}
              <div>
                <h3 className="text-lg font-medium text-gray-900 mb-4">
                  Import Applicants
                </h3>
                <div className="bg-gray-50 p-6 rounded-md">
                  <p className="text-sm text-gray-600 mb-4">
                    Upload CSV or Excel file containing applicant data. The file
                    should contain all required fields.
                  </p>

                  <input
                    type="file"
                    ref={fileInputRef}
                    className="hidden"
                    accept=".csv,.xlsx,.xls"
                    onChange={handleFileImport}
                  />

                  <div
                    onClick={handleFileSelect}
                    className="border-2 border-dashed border-gray-300 rounded-md py-8 px-4 text-center cursor-pointer hover:border-blue-500 transition-colors"
                  >
                    <Paperclip className="w-8 h-8 mx-auto text-gray-400" />
                    <p className="mt-2 text-sm text-gray-600">
                      Click to select a file
                    </p>
                    <p className="text-xs text-gray-500 mt-1">
                      Supported formats: CSV, XLSX, XLS
                    </p>
                  </div>

                  {importStatus === "loading" && (
                    <div className="mt-4">
                      <p className="text-sm text-gray-600 mb-2">
                        Uploading: {fileUploadProgress}%
                      </p>
                      <div className="w-full bg-gray-200 rounded-full h-2">
                        <div
                          className="bg-blue-600 h-2 rounded-full"
                          style={{ width: `${fileUploadProgress}%` }}
                        ></div>
                      </div>
                    </div>
                  )}

                  {importStatus === "success" && (
                    <div className="mt-4 flex items-center text-green-600">
                      <CheckCircle className="w-5 h-5 mr-2" />
                      <span>Import completed successfully</span>
                    </div>
                  )}

                  {importStatus === "error" && (
                    <div className="mt-4 flex items-center text-red-600">
                      <XCircle className="w-5 h-5 mr-2" />
                      <span>Import failed. Please check the file format.</span>
                    </div>
                  )}
                </div>
              </div>

              {/* Export Section */}
              <div>
                <h3 className="text-lg font-medium text-gray-900 mb-4">
                  Export Applicants
                </h3>
                <div className="bg-gray-50 p-6 rounded-md">
                  <p className="text-sm text-gray-600 mb-4">
                    Export applicant data in your preferred format. The export
                    will include all current filter settings.
                  </p>

                  <div className="mb-4">
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Export Format
                    </label>
                    <div className="flex space-x-4">
                      <label className="inline-flex items-center">
                        <input
                          type="radio"
                          className="text-blue-600"
                          name="exportFormat"
                          value="xlsx"
                          checked={exportFormat === "xlsx"}
                          onChange={() => setExportFormat("xlsx")}
                        />
                        <span className="ml-2 text-sm text-gray-700">
                          Excel (XLSX)
                        </span>
                      </label>
                      <label className="inline-flex items-center">
                        <input
                          type="radio"
                          className="text-blue-600"
                          name="exportFormat"
                          value="csv"
                          checked={exportFormat === "csv"}
                          onChange={() => setExportFormat("csv")}
                        />
                        <span className="ml-2 text-sm text-gray-700">CSV</span>
                      </label>
                      <label className="inline-flex items-center">
                        <input
                          type="radio"
                          className="text-blue-600"
                          name="exportFormat"
                          value="pdf"
                          checked={exportFormat === "pdf"}
                          onChange={() => setExportFormat("pdf")}
                        />
                        <span className="ml-2 text-sm text-gray-700">PDF</span>
                      </label>
                    </div>
                  </div>

                  <button
                    onClick={handleExport}
                    disabled={exportLoading}
                    className="w-full flex justify-center items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                  >
                    {exportLoading ? (
                      <>
                        <Loader2 className="w-5 h-5 mr-2 animate-spin" />
                        <span>Exporting...</span>
                      </>
                    ) : (
                      <>
                        <FileDown className="w-5 h-5 mr-2" />
                        <span>Export Applicants</span>
                      </>
                    )}
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ApplicantManagement;
