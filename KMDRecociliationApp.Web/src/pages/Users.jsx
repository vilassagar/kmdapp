import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import { RPagination } from "@/components/ui/RPagination";
import { Badge } from "@/components/ui/badge";
import { Checkbox } from "@/components/ui/checkbox";
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
import { isFilterPresent } from "@/lib/helperFunctions";
import { usePermissionStore, userStore } from "@/lib/store";

import {
  deleteUser,
  getAssociations,
  getUserList,
  getUserTypes,
} from "@/services/customerProfile";
import { updateUser } from "@/services/user";
import { produce } from "immer";
import {
  CirclePlus,
  CircleX,
  FilePenIcon,
  Trash2Icon,
  User,
  ChevronDown,
} from "lucide-react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import UserNavigationButton from "@/components/ui/user/UserNavigationButton";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

const PAGE_NAME = "users";

// Static user types data to ensure consistency
const STATIC_USER_TYPES = [
  { id: 1, name: "Pensioner" },
  { id: 2, name: "InternalUser" },
  { id: 3, name: "Association" },
  { id: 4, name: "Other" },
  { id: 5, name: "Community" },
  { id: 6, name: "DataCollection" },
];

function Users() {
  const navigate = useNavigate();

  const loggedInUser = userStore((state) => state.user);
  const userStoreData = userStore((state) => state.user);
  const associationId =
    userStoreData?.userType?.name?.toLowerCase()?.trim() === "association"
      ? userStoreData?.associationId
      : 0;

  // Function to check if user is an internal user (userType id = 2)
  const isInternalUser = () => {
    return (
      userStoreData?.userType?.id === 2 ||
      userStoreData?.userType?.name?.toLowerCase()?.trim() === "internaluser"
    );
  };

  const permissions = usePermissionStore((state) => state.permissions);

  const [searchTerm, setSearchTerm] = useState("");
  const [paginationModel, setPaginationModel] = useState({
    pageNumber: 1,
    recordsPerPage: 50,
  });
  const [userIndex, setUserIndex] = useState(0);
  const [users, setUsers] = useState();
  const [userToFreeze, setUserToFreeze] = useState(null);

  const [filters, setFilters] = useState({ userTypes: [], associations: [] });
  const [associations, setAssociations] = useState([]);
  const [currentUserType, setcurrentUserType] = useState(
    STATIC_USER_TYPES.find((ut) => ut.id === 1) || {
      id: userStoreData.userType.id,
      name: userStoreData.userType.name,
    }
  );
  const [userTypes, setUserTypes] = useState(STATIC_USER_TYPES);
  const [showUserTypeFilter, setShowUserTypeFilter] = useState(false);

  // Update the useEffect for initializing data
  useEffect(() => {
    const fetchUsersAndAssociations = async () => {
      const savedSearchTerm = localStorage.getItem("searchTerm");
      if (savedSearchTerm) {
        setSearchTerm(savedSearchTerm);
      }

      try {
        // Only fetch associations, use static user types data
        const associationsResponse = await getAssociations();
        setAssociations(associationsResponse.data);

        // Set default filter based on user type
        let defaultFilters = { userTypes: [], associations: [] };

        // Find the current user type in the static user types
        let userTypeToSet;

        if (!isInternalUser() && userStoreData?.userType) {
          // If not an internal user, set filter to current user type
          const currentUserTypeId = userStoreData.userType.id;
          userTypeToSet = STATIC_USER_TYPES.find(
            (ut) => ut.id === currentUserTypeId
          ) || {
            id: userStoreData.userType.id,
            name: userStoreData.userType.name,
          };

          defaultFilters.userTypes = [userTypeToSet];
        } else {
          // For internal users, set a default (Pensioner)
          userTypeToSet = STATIC_USER_TYPES.find((ut) => ut.id === 1) || {
            id: 1,
            name: "Pensioner",
          };
        }

        // Always set the current user type
        setcurrentUserType(userTypeToSet);

        // Add the userType to filter if it's not already there
        if (defaultFilters.userTypes.length === 0) {
          defaultFilters.userTypes = [userTypeToSet];
        }

        setFilters(defaultFilters);
        setShowUserTypeFilter(isInternalUser());

        await getPaginatedUsers(
          paginationModel,
          savedSearchTerm || "",
          defaultFilters
        );
      } catch (err) {
        toast({
          variant: "destructive",
          title: "Error fetching data",
          description: "Unable to load associations",
        });
      }
    };

    fetchUsersAndAssociations();
  }, []);

  // Update the select onChange handler to also filter the users
  const handleUserTypeChange = async (e) => {
    const userTypeId = parseInt(e.target.value);
    const userType = STATIC_USER_TYPES.find((ut) => ut.id === userTypeId);

    setcurrentUserType(userType);

    // Update the filters to include the selected user type
    const updatedFilters = produce(filters, (draft) => {
      // Remove any existing user types
      draft.userTypes = [];
      // Add the selected user type
      draft.userTypes.push(userType);
    });

    setFilters(updatedFilters);

    // Fetch users with the updated filter
    await getPaginatedUsers(paginationModel, searchTerm, updatedFilters);
  };
  const getPaginatedUsers = async (paginationModel, searchTerm, filters) => {
    let userTypes = filters.userTypes.map((userType) => {
      return userType.id;
    });

    let associations = filters.associations.map((association) => {
      return association.id;
    });

    filters = {
      userTypes: userTypes.toString(),
      associations: associations.toString(),
    };

    filters.associationId = associationId;

    let response = await getUserList(
      paginationModel.pageNumber,
      paginationModel.recordsPerPage,
      searchTerm,
      filters
    );

    if (response.status === "success") {
      setUsers(response.data);
      setPaginationModel(response.data.paging);
    } else {
      //show error
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get users.",
      });
    }
  };

  const handleSearchChange = (event) => {
    setSearchTerm(event.target.value);
  };

  const handleSearchSubmit = async () => {
    localStorage.setItem("searchTerm", searchTerm);
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };
    setPaginationModel(pageModel);
    await getPaginatedUsers(pageModel, searchTerm, filters);
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
      await getPaginatedUsers(pageModel, "", filters);
    }
  };

  const handleDeleteUser = async (event) => {
    let userId = users?.contents[userIndex]["userId"];
    try {
      let response = await deleteUser(userId);

      if (response.status === "success") {
        let pageModel = {
          page: 1,
          pageSize: 50,
        };

        setPaginationModel(pageModel);
        await getPaginatedUsers(paginationModel, searchTerm, filters);
      } else if (response.status === "invalid") {
        toast({
          variant: "destructive",
          title: "User can't be deleted.",
          description:
            "User has already initiated payment,user can't be deleted!",
        });
      } else {
        throw new Error(response.message);
      }
    } catch (error) {
      if (error.response) {
        const { status, data } = error.response;
        if (status === 400) {
          // BadRequest
          toast({
            variant: "destructive",
            title: "Bad Request",
            description:
              data.message ||
              "The user has completed the payment. User cannot be deleted.",
          });
        } else if (status === 404) {
          // NotFound
          toast({
            variant: "destructive",
            title: "User Not Found",
            description:
              data.message ||
              "The user you are trying to delete does not exist.",
          });
        } else {
          toast({
            variant: "destructive",
            title: "Something went wrong",
            description: data.message || "Unable to delete user.",
          });
        }
      } else {
        toast({
          variant: "destructive",
          title: "Something went wrong",
          description: "Unable to delete user due to an unexpected error.",
        });
      }
    }
  };

  const handleChange = (name) => async (event) => {
    let index = -1;
    let nextState = produce(filters, (draft) => {
      switch (name) {
        case "userTypes":
          index = isFilterPresent(draft[name], event.id);
          if (index >= 0) {
            draft[name].splice(index, 1);
          } else {
            draft[name].push(event);
          }
          break;

        case "associations":
          index = isFilterPresent(draft[name], event.id);
          if (index >= 0) {
            draft[name].splice(index, 1);
          } else {
            draft[name].push(event);
          }
          break;

        default:
          break;
      }
    });

    setFilters(nextState);
    await getPaginatedUsers(paginationModel, searchTerm, nextState);
  };

  const handleDeleteFilter = (name, index) => async (event) => {
    let nextState = produce(filters, (draft) => {
      switch (name) {
        case "userTypes":
          draft[name].splice(index, 1);
          break;

        case "associations":
          draft[name].splice(index, 1);
          break;

        default:
          break;
      }
    });

    setFilters(nextState);
    await getPaginatedUsers(paginationModel, searchTerm, nextState);
  };

  const handleClearSearch = async () => {
    localStorage.removeItem("searchTerm");
    setSearchTerm("");
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };
    setPaginationModel(pageModel);
    await getPaginatedUsers(pageModel, "", filters);
  };

  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedUsers(paging, searchTerm, filters);
  };

  const handleUserFreeze = async () => {
    try {
      const user = users.contents.find((u) => u.userId === userToFreeze);
      const updatedUser = {
        ...user,
        isUserFrozen: !user.isUserFrozen,
      };

      const response = await updateUser(userToFreeze, updatedUser);

      if (response.status === "success") {
        setUsers((prevUsers) =>
          produce(prevUsers, (draft) => {
            const index = draft.contents.findIndex(
              (u) => u.userId === userToFreeze
            );
            draft.contents[index] = updatedUser;
          })
        );

        setUserToFreeze(null);

        toast({
          variant: "success",
          title: `User ${updatedUser.isUserFrozen ? "Frozen" : "Unfrozen"}`,
        });
      } else {
        toast({
          variant: "destructive",
          title: "Error Updating User",
          description: response.message,
        });
      }
    } catch (error) {
      toast({
        variant: "destructive",
        title: "Error Updating User",
        description:
          "An unexpected error occurred while freezing/unfreezing the user.",
      });
    }
  };

  return (
    <div>
      <div>
        <h1 className="text-2xl font-bold mb-6">Users</h1>

        <div className="mb-6 flex justify-between items-center">
          <div className="flex flex-1 mr-4">
            {showUserTypeFilter && (
              <select
                className="w-48 rounded-md border border-gray-300 p-2"
                value={currentUserType?.id || 1}
                onChange={handleUserTypeChange}
              >
                {STATIC_USER_TYPES.map((usertype) => (
                  <option key={usertype.id} value={usertype.id}>
                    {usertype.name}
                  </option>
                ))}
              </select>
            )}
            <RInput
              type="search"
              placeholder="Search Users..."
              value={searchTerm}
              onChange={handleSearchChange}
              onKeyPress={handleSearch}
              className="w-full max-w-md"
            />
            <RButton
              onClick={handleSearch}
              className="ml-2 flex flex-col bg-red-900 hover:bg-red-800 text-white"
            >
              <span>Search</span>
            </RButton>
            <RButton onClick={handleClearSearch} className="ml-2">
              Clear Search
            </RButton>
          </div>

          <div className="flex items-center">
            {permissions?.[PAGE_NAME]?.create ? (
              <RButton
                onClick={() => {
                  navigate("/createuser?mode=new");
                }}
              >
                <span className="flex items-center">
                  Create User
                  <CirclePlus className="ml-2 h-4 w-4" />
                </span>
              </RButton>
            ) : null}
          </div>
        </div>

        <div className="my-3 flex flex-wrap">
          {filters.userTypes.map((userType, index) => (
            <Badge key={userType.id} className="rounded-sm mr-3 py-1 flex mb-2">
              {userType.name}
              {/* Only allow removing userType filter if it's an internal user */}
              {/* {showUserTypeFilter && (
                <CircleX
                  className="h-4 w-4 ml-2 cursor-pointer"
                  onClick={handleDeleteFilter("userTypes", index)}
                />
              )} */}
            </Badge>
          ))}
          {filters.associations.map((association, index) => (
            <Badge
              key={association.id}
              className="rounded-sm px-1 font-normal mr-3 p-1 mb-2"
            >
              {association.name}
              {/* <CircleX
                className="h-4 w-4 ml-2 cursor-pointer"
                onClick={handleDeleteFilter("associations", index)}
              /> */}
            </Badge>
          ))}
        </div>
      </div>

      <div className="rounded-lg shadow-lg">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="p-2">First Name</TableHead>
              <TableHead className="p-2">Last Name</TableHead>
              <TableHead className="p-2">User Type</TableHead>
              <TableHead className="p-2">Email</TableHead>
              <TableHead className="p-2">Mobile Number</TableHead>
              <TableHead className="p-2">Association Name</TableHead>
              <TableHead className="w-[150px] p-2 text-right">
                Actions
              </TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {users?.contents?.length ? (
              users?.contents?.map((user, index) => (
                <TableRow key={user.userId}>
                  <TableCell className="p-2">{user?.firstName}</TableCell>
                  <TableCell className="p-2">{user?.lastName}</TableCell>
                  <TableCell className="p-2">{user?.userType}</TableCell>
                  <TableCell className="p-2">{user?.email}</TableCell>
                  <TableCell className="p-2">{user?.mobileNumber}</TableCell>
                  <TableCell className="p-2">{user?.associationName}</TableCell>

                  <TableCell className="p-1 text-right">
                    <div className="flex justify-end">
                      <UserNavigationButton user={user} navigate={navigate} />

                      {permissions?.[PAGE_NAME]?.update ? (
                        <RButton
                          variant="ghost"
                          className="flex items-center gap-2 "
                          onClick={() => {
                            navigate(
                              `/createuser?mode=edit&userId=${user.userId}`
                            );
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
                              onClick={(event) => {
                                setUserIndex(index);
                              }}
                            >
                              <Trash2Icon className="h-4 w-4 text-red-500" />
                            </RButton>
                          }
                          onConfirm={handleDeleteUser}
                          dialogTitle="Are you sure to delete the product?"
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

export default WithAuthentication(WithPermission(PAGE_NAME)(WithLayout(Users)));
