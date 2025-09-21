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
import { deleteRole, getRoles } from "@/services/roles";
import { CirclePlus, FilePenIcon, Trash2Icon } from "lucide-react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const PAGE_NAME = "roles";

const Role = () => {
  const navigate = useNavigate();
  const permissions = usePermissionStore((state) => state.permissions);

  const [roles, setRoles] = useState();
  const [roleIndex, setRoleIndex] = useState(0);
  const [searchTerm, setSearchTerm] = useState("");
  const [paginationModel, setPaginationModel] = useState({
    pageNumber: 1,
    recordsPerPage: 50,
  });

  useEffect(() => {
    (async () => {
      await getPaginatedRoles(paginationModel, searchTerm);
    })();
  }, []);

  const getPaginatedRoles = async (paginationModel, searchTerm) => {
    let response = await getRoles(
      paginationModel.pageNumber,
      paginationModel.recordsPerPage,
      searchTerm
    );

    if (response.status === "success") {
      setRoles(response.data);
      setPaginationModel(response.data.paging);
    } else {
      //show error
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get roles.",
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
      await getPaginatedRoles(pageModel, "");
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
    await getPaginatedRoles(pageModel, searchTerm);
  };

  const handleDeleteRole = async (roleId) => {
    let response = await deleteRole(roleId);
    if (response.status === "success") {
      toast({
        variant: "success",
        title: "Role deleted successfully",
        description: "Role deleted successfully.",
      });
      let pageModel = {
        page: 1,
        pageSize: 50,
      };

      setPaginationModel(pageModel);
      await getPaginatedRoles(paginationModel, searchTerm);
    } else {
      //show error message
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to delete role.",
      });
    }
  };

  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedRoles(paging, searchTerm);
  };

  const handleEdit = (id) => {
    navigate(`/createrole/${id}`);
  };

  const handleNew = () => {
    navigate("/createrole");
  };

  const handleClearSearch = async () => {
    setSearchTerm("");
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };
    setPaginationModel(pageModel);
    await getPaginatedRoles(pageModel, "");
  };

  return (
    <div>
      <div className="flex flex-col justify-between">
        <div>
          <h1 className="text-2xl font-bold ">Roles</h1>
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
                Create Role
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
              <TableHead className="p-2">Role Name</TableHead>
              <TableHead className="p-2">Role Description</TableHead>
              <TableHead className="p-2 text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {roles?.contents?.length ? (
              roles?.contents?.map((role) => (
                <TableRow key={role.name}>
                  <TableCell className="p-2">{role.name}</TableCell>
                  <TableCell className="p-2">{role.description}</TableCell>
                  <TableCell className="p-2 text-right">
                    <div className="flex justify-end">
                      {permissions?.[PAGE_NAME]?.update ? (
                        <RButton
                          variant="ghost"
                          className="flex items-center gap-2 "
                          onClick={() => {
                            handleEdit(role.id);
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
                                setRoleIndex(role.id);
                              }}
                            >
                              <Trash2Icon className="h-4 w-4 text-red-500" />
                            </RButton>
                          }
                          onConfirm={() => handleDeleteRole(roleIndex)}
                          dialogTitle="Are you sure to delete the role?"
                          dialogDescription="This action cannot be undone. This will permanently delete your
                              product and remove your data from our servers."
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

export default WithAuthentication(WithPermission(PAGE_NAME)(WithLayout(Role)));
