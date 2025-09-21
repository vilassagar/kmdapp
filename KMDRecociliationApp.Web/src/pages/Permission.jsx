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
import deletePermission from "@/services/permissions/deletePermission";
import getPermissions from "@/services/permissions/getPermissions";
import { CirclePlus, FilePenIcon, Trash2Icon } from "lucide-react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const PAGE_NAME = "permissions";

function Permission() {
  const navigate = useNavigate();
  const userPermissions = usePermissionStore((state) => state.permissions);

  const [permissionIndex, setPermissionIndex] = useState(0);
  const [searchTerm, setSearchTerm] = useState("");
  const [paginationModel, setPaginationModel] = useState({
    pageNumber: 1,
    recordsPerPage: 50,
  });

  const [permissions, setPermissions] = useState();

  useEffect(() => {
    (async () => {
      await getPaginatedPermissions(paginationModel, searchTerm);
    })();
  }, []);

  const getPaginatedPermissions = async (paginationModel, searchTerm) => {
    let response = await getPermissions(
      paginationModel.pageNumber,
      paginationModel.recordsPerPage,
      searchTerm
    );

    if (response.status === "success") {
      setPermissions(response.data);
      setPaginationModel(response.data.paging);
    } else {
      //show error
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get permissions.",
      });
    }
  };

  const handleEdit = (id) => {
    navigate(`/createpermission/${id}`);
  };

  const handleNew = () => {
    navigate("/createpermission");
  };

  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedPermissions(paging, searchTerm);
  };

  const handleDeletePermission = async (permissionId) => {
    let response = await deletePermission(permissionId);
    if (response.status === "success") {
      // Show success message
      toast({
        variant: "success",
        title: "Permission deleted successfully",
        description: "Permission deleted successfully.",
      });
      let pageModel = {
        pageNumber: 1,
        recordsPerPage: 50,
      };

      setPaginationModel(pageModel);
      await getPaginatedPermissions(pageModel, searchTerm);
    } else {
      // Show error message
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to delete permission.",
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
      await getPaginatedPermissions(pageModel, "");
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
    await getPaginatedPermissions(pageModel, searchTerm);
  };

  const handleClearSearch = async () => {
    setSearchTerm("");
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };
    setPaginationModel(pageModel);
    await getPaginatedPermissions(pageModel, "");
  };

  return (
    <div>
      <div className="flex flex-col justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold ">Permissions</h1>
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
          {/* {permissions?.[PAGE_NAME]?.create ? (   // ) : null}*/}
            <RButton onClick={handleNew}>
              <span className="flex items-center">
                Create Permission
                <CirclePlus className="ml-2 h-4 w-4" />
              </span>
            </RButton>
         
        </div>
      </div>
      <div className=" rounded-lg shadow-lg">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Permission Type</TableHead>
              <TableHead>Permission Name</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {permissions?.contents?.length ? (
              permissions?.contents?.map((permission) => (
                <TableRow key={permission.name}>
                  <TableCell>{permission.type}</TableCell>
                  <TableCell>{permission.name}</TableCell>
                  <TableCell className="p-1 text-right">
                    <div className="flex justify-end">
                      {userPermissions?.[PAGE_NAME]?.update ? (
                        <RButton
                          variant="ghost"
                          className="flex items-center gap-2 "
                          onClick={() => {
                            handleEdit(permission.id);
                          }}
                        >
                          <FilePenIcon className="h-4 w-4" />
                        </RButton>
                      ) : null}
                      {userPermissions?.[PAGE_NAME]?.delete ? (
                        <ConfirmDialog
                          dialogTrigger={
                            <RButton
                              variant="ghost"
                              className="flex items-center gap-2"
                              onClick={(index) => {
                                setPermissionIndex(permission.id);
                              }}
                            >
                              <Trash2Icon className="h-4 w-4 text-red-500" />
                            </RButton>
                          }
                          onConfirm={() =>
                            handleDeletePermission(permissionIndex)
                          }
                          dialogTitle="Are you sure to delete the permission?"
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
  WithPermission(PAGE_NAME)(WithLayout(Permission))
);
