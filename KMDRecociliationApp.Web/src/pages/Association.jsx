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
import { deleteAssociation, getAssociationList } from "@/services/association";
import { CirclePlus, FilePenIcon, Search, Trash2Icon } from "lucide-react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { EyeIcon, Loader2 } from "lucide-react";
import { ErrorDisplay } from "@/components/ui/common/ErrorDisplay";
import { LoadingSpinner } from "@/components/ui/common/LoadingSpinner";
const PAGE_NAME = "associations";

function Association() {
  const navigate = useNavigate();

  const permissions = usePermissionStore((state) => state.permissions);
  const [searchTerm, setSearchTerm] = useState("");
  const [paginationModel, setPaginationModel] = useState({
    pageNumber: 1,
    recordsPerPage: 50,
  });
  const [associations, setAssociations] = useState({});
  const [associationIndex, setAssociationIndex] = useState(0);
  const [isLoadingDependencies, setIsLoadingDependencies] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    setIsLoadingDependencies(true);
    (async () => {
      await getPaginatedAssociations(paginationModel, searchTerm);
      setIsLoadingDependencies(false);
    })();
  }, []);

  const getPaginatedAssociations = async (paginationModel, searchTerm) => {
    let response = await getAssociationList(
      paginationModel.pageNumber,
      paginationModel.recordsPerPage,
      searchTerm
    );

    if (response.status === "success") {
      setAssociations(response.data);
      setPaginationModel(response.data.paging);
    } else {
      //show error
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get associations.",
      });
    }
  };

  const handleDeleteAssociation = async (event) => {
    let associationId = associations?.contents[associationIndex]["id"];
    let response = await deleteAssociation(associationId);
    if (response.status === "success") {
      //show success message
      let pageModel = {
        page: 1,
        pageSize: 50,
      };

      setPaginationModel(pageModel);
      await getPaginatedAssociations(paginationModel, searchTerm);
    } else {
      //show error message
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to delete association.",
      });
    }
  };

  const handleEdit = (id) => {
    navigate(`/createassociation?id=${id}&mode=edit`);
  };

  const handleNew = () => {
    navigate(`/createassociation`);
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
      await getPaginatedAssociations(pageModel, "");
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
    await getPaginatedAssociations(pageModel, searchTerm);
  };

  const handleClearSearch = async () => {
    setSearchTerm("");
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };
    setPaginationModel(pageModel);
    await getPaginatedAssociations(pageModel, "");
  };

  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedAssociations(paging, searchTerm);
  };
  if (isLoadingDependencies) {
    return <LoadingSpinner />;
  }

  if (error) {
    <ErrorDisplay />;
  }

  return (
    <div>
      <div className="flex flex-col justify-between">
        <div>
          <h1 className="text-2xl font-bold ">Associations</h1>
        </div>
        <div className="flex items-center justify-between mb-6">
          <div className="flex flex-1 mr-4">
            <RInput
              type="search"
              placeholder="Search Associations..."
              value={searchTerm}
              onChange={handleSearchChange}
              onKeyPress={handleSearch}
              className="w-full max-w-md"
            />
            <RButton onClick={handleSearch} className="ml-2">
              {/* <Search className="h-4 w-4" /> */}Search
            </RButton>
            <RButton onClick={handleClearSearch} className="ml-2">
              {/* <X className="h-4 w-4" /> */} Clear Search
            </RButton>
          </div>
          {permissions?.[PAGE_NAME]?.create ? (
            <RButton onClick={handleNew}>
              <span className="flex items-center">
                Create Association
                <CirclePlus className="ml-2 h-4 w-4" />
              </span>
            </RButton>
          ) : null}
        </div>
      </div>
      <div className="border rounded-lg shadow-lg border-0">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="p-2">Association Name</TableHead>
              <TableHead className="p-2">Organisation</TableHead>
              <TableHead className="p-2">Members</TableHead>
              <TableHead className="w-[150px] p-2">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {associations?.contents?.length ? (
              associations?.contents?.map((association, index) => (
                <TableRow key={association.id}>
                  <TableCell className="p-2">
                    {association?.associationName}
                  </TableCell>
                  <TableCell className="p-2">
                    {association?.organisationName}
                  </TableCell>
                  <TableCell className="p-2">{association?.members}</TableCell>

                  <TableCell className="p-1">
                    <div className="flex">
                      {permissions?.[PAGE_NAME]?.update ? (
                        <RButton
                          variant="ghost"
                          className="flex items-center gap-2 "
                          onClick={() => handleEdit(association.associationId)}
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
                              onClick={(event) => {
                                setAssociationIndex(index);
                              }}
                            >
                              <Trash2Icon className="h-4 w-4 text-red-500" />
                            </RButton>
                          }
                          onConfirm={handleDeleteAssociation}
                          dialogTitle="Are you sure to delete the association?"
                          dialogDescription="This action cannot be undone. This will permanently delete your association and remove your data from our servers."
                        />
                      ) : null}
                    </div>
                  </TableCell>
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell colSpan={7} className="h-24 text-center">
                  No results.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
      <div className="flex justify-end">
        <RPagination
          paginationModel={paginationModel}
          onPageChange={handlePageChange}
        />
      </div>
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(Association))
);
