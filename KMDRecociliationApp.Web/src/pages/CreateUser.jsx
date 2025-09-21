import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import { Combobox } from "@/components/ui/comboBox";
import { Label } from "@/components/ui/label";
import { toast } from "@/components/ui/use-toast";
import UserProfile from "@/components/ui/userProfile";
import { usePermissionStore,userStore } from "@/lib/store";
import { getUserTypes } from "@/services/customerProfile";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const PAGE_NAME = "users";

function CreateUser() {
  let params = new URLSearchParams(window.location.search);
  let pageMode = params.get("mode");
  let userId = params.get("userId");
 const userStoreData = userStore((state) => state.user);
  const associationId =
  userStoreData?.userType?.name?.toLowerCase()?.trim() === "association"
    ? userStoreData?.userId
    : 0;

  const permissions = usePermissionStore((state) => state.permissions);

  const navigate = useNavigate();

  const [userType, setUserType] = useState(null);

  const [userTypes, setUserTypes] = useState([]);

  useEffect(() => {
    (async () => {
      let response = await getUserTypes(associationId);
      if (response.status === "success") {
        setUserTypes(response.data);
      } else {
        toast({
          variant: "destructive",
          title: "Something went wrong.",
          description: "Unable to get user types.",
        });
      }
    })();
  }, []);

  return (
    <div>
      <h1 className="mb-8 text-2xl font-bold">Create User</h1>
      {pageMode === "new" ? (
        <div className="grid grid-cols-2 mb-4">
          <div className="flex flex-col justify-between space-y-2">
            <Label htmlFor="user-type">User Type</Label>
            <Combobox
              options={userTypes}
              valueProperty="id"
              labelProperty="name"
              id="user-type"
              onChange={(event) => setUserType(event)}
              value={userType}
            />
          </div>
        </div>
      ) : null}

      {pageMode === "new" ? <hr className="mt-10 mb-5" /> : null}

      <UserProfile
        userType={userType || null}
        mode={pageMode}
        onSave={() => {
          navigate("/users");
        }}
        userId={userId}
        permissions={permissions}
        pageName={PAGE_NAME}
      />
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(CreateUser))
);
