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
import { getOrganisations, deleteOrganistion } from "@/services/organisations";
import { CirclePlus, FilePenIcon, Trash2Icon } from "lucide-react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const PAGE_NAME = "associations";

const Organisation = () => {
  const navigate = useNavigate();
  const permissions = usePermissionStore((state) => state.permissions);

  const [organisations, setOrganisations] = useState();
  const [organistaionIndex, setOrganisationIndex] = useState(0);
  const [searchTerm, setSearchTerm] = useState("");
  const [paginationModel, setPaginationModel] = useState({
    pageNumber: 1,
    recordsPerPage: 50,
  });

  useEffect(() => {
    (async () => {
      await getPaginatedOrganisations(paginationModel, searchTerm);
    })();
  }, []);

  const getPaginatedOrganisations = async (paginationModel, searchTerm) => {
    let response = await getOrganisations(
      paginationModel.pageNumber,
      paginationModel.recordsPerPage,
      searchTerm
    );

    if (response.status === "success") {
      setOrganisations(response.data);
      setPaginationModel(response.data.paging);
    } else {
      //show error
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get organisations.",
      });
    }
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
      await getPaginatedOrganisations(pageModel, "");
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
    await getPaginatedOrganisations(pageModel, searchTerm);
  };

  const handleDeleteOrganisation = async (organisationId) => {
    let response = await deleteOrganistion(organisationId);
    if (response.status === "success") {
      toast({
        variant: "success",
        title: "Organisation deleted successfully",
        description: "Organisation deleted successfully.",
      });
      let pageModel = {
        page: 1,
        pageSize: 50,
      };

      setPaginationModel(pageModel);
      await getPaginatedOrganisations(paginationModel, searchTerm);
    } else {
      //show error message
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to delete organisation.",
      });
    }
  };

  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedOrganisations(paging, searchTerm);
  };

  const handleEdit = (id) => {
    navigate(`/createorganisation?id=${id}&mode=edit`);
  };

  const handleNew = () => {
    navigate("/createorganisation");
  };

  const handleClearSearch = async () => {
    setSearchTerm("");
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };
    setPaginationModel(pageModel);
    await getPaginatedOrganisations(pageModel, "");
  };

  return (
    <div>
      <div className="flex flex-col justify-between">
        <div>
          <h1 className="text-2xl font-bold ">Organisations</h1>
        </div>
        <div className="flex items-center justify-between mb-6">
          <div className="flex flex-1 mr-4">
            <RInput
              type="search"
              placeholder="Search Organisations..."
              value={searchTerm}
              onChange={handleSearchChange}
              onKeyPress={handleSearch}
              className="w-full max-w-md"
            />
            <RButton onClick={handleSearch} className="ml-2">
              {/* <Search className="h-4 w-4" /> */}Search
            </RButton>
            <RButton onClick={handleClearSearch} className="ml-2">
              Clear Search
            </RButton>
          </div>
          {permissions?.[PAGE_NAME]?.create ? (
            <RButton onClick={handleNew}>
              <span className="flex items-center">
                Create Organisation
                <CirclePlus className="ml-2 h-4 w-4" />
              </span>
            </RButton>
          ) : null}
        </div>
      </div>
      <div className=" rounded-lg shadow-lg">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="p-2">Organisation Name</TableHead>
              <TableHead className="p-2">Orgnisation Description</TableHead>
              <TableHead className="p-2 text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {organisations?.contents?.length ? (
              organisations?.contents?.map((organisation) => (
                <TableRow key={organisation.name}>
                  <TableCell className="p-2">{organisation.name}</TableCell>
                  <TableCell className="p-2">
                    {organisation.description}
                  </TableCell>
                  <TableCell className="p-2 text-right">
                    <div className="flex justify-end">
                      {permissions?.[PAGE_NAME]?.update ? (
                        <RButton
                          variant="ghost"
                          className="flex items-center gap-2 "
                          onClick={() => {
                            handleEdit(organisation.id);
                          }}
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
                              onClick={(index) => {
                                setOrganisationIndex(organisation.id);
                              }}
                            >
                              <Trash2Icon className="h-4 w-4 text-red-500" />
                            </RButton>
                          }
                          onConfirm={() =>
                            handleDeleteOrganisation(organistaionIndex)
                          }
                          dialogTitle="Are you sure to delete the organisation?"
                          dialogDescription="This action cannot be undone. This will permanently delete your
                              organisation and remove your data from our servers."
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
            {}
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
};

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(Organisation))
);
