import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import { RPagination } from "@/components/ui/RPagination";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
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
import { useCampaignsStore, usePermissionStore } from "@/lib/store";
import { closeCampaign, getCampaigns } from "@/services/campaigns";
import { format } from "date-fns";
import { Edit, PlusCircle, X } from "lucide-react";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

const PAGE_NAME = "campaigns";

function Campaigns() {
  const navigate = useNavigate();

  const permissions = usePermissionStore((state) => state.permissions);
  const campaigns = useCampaignsStore((state) => state.campaigns);
  const paginationModel = useCampaignsStore((state) => state.paginationModel);
  const setPaginationModel = useCampaignsStore(
    (state) => state.setPaginationModel
  );
  const searchTerm = useCampaignsStore((state) => state.searchTerm);
  const setSearchTerm = useCampaignsStore((state) => state.setSearchTerm);
  const addCampaigns = useCampaignsStore((state) => state.addCampaigns);
  const campaignIndex = useCampaignsStore((state) => state.campaignIndex);
  const setCampaignIndex = useCampaignsStore((state) => state.setCampaignIndex);
  const setCurrentCampaignId = useCampaignsStore(
    (state) => state.setCurrentCampaignId
  );
  const setMode = useCampaignsStore((state) => state.setMode);

  useEffect(() => {
    (async () => {
      await getPaginatedCampaigns(paginationModel, searchTerm);
    })();
  }, []);

  const getPaginatedCampaigns = async (paginationModel, searchTerm) => {
    let response = await getCampaigns(
      paginationModel.pageNumber,
      paginationModel.recordsPerPage,
      searchTerm
    );

    if (response.status === "success") {
      addCampaigns(response.data);
      setPaginationModel(response.data.paging);
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get campaigns.",
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
      await getPaginatedCampaigns(pageModel, "");
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
    await getPaginatedCampaigns(pageModel, searchTerm);
  };

  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedCampaigns(paging, searchTerm);
  };

  const handleCloseCampaign = async (event) => {
    let campaignId = campaigns?.contents[campaignIndex]["campaignId"];
    let response = await closeCampaign(campaignId);
    if (response.status === "success") {
      await getPaginatedCampaigns(paginationModel, searchTerm);
    } else {
      //show error message
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to close campaign.",
      });
    }
  };

  const handleCreateCampaign = () => {
    setCurrentCampaignId(0);
    setMode("new");
    navigate("/createcampaign");
  };

  const handleEditCampaign = (index) => (event) => {
    let campaignId = campaigns?.contents[index]["campaignId"];
    setCurrentCampaignId(campaignId);
    setMode("edit");
    navigate("/createcampaign");
  };

  const handleClearSearch = async () => {
    setSearchTerm("");
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };
    setPaginationModel(pageModel);
    await getPaginatedCampaigns(pageModel, "");
  };

  return (
    <div>
      <div>
        <h1 className="text-2xl font-bold mb-6">Manage Campaigns</h1>

        <div className="mb-6 flex justify-between items-center">
          <div className="flex flex-1 mr-4">
            <RInput
              type="search"
              placeholder="Search Campaigns..."
              value={searchTerm}
              onChange={handleSearchChange}
              onKeyPress={handleSearch}
              className="w-full max-w-md"
            />
            <RButton onClick={handleSearch} className="ml-2">
              Search
            </RButton>
            <RButton onClick={handleClearSearch} className="ml-2">
              Clear Search
            </RButton>
          </div>
          {permissions?.[PAGE_NAME]?.create ? (
            <Button onClick={handleCreateCampaign}>
              New
              <PlusCircle className="h-5 w-5 ml-2" />
            </Button>
          ) : null}
        </div>
        <div className="overflow-x-auto rounded-lg shadow-lg ">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="min-w-[20px]">Campaign Name</TableHead>
                <TableHead className="min-w-[20px]">Start Date</TableHead>
                <TableHead className="min-w-[20px]">End Date</TableHead>
                <TableHead className="min-w-[20px]">Remainig Days</TableHead>
                <TableHead className="min-w-[20px]">Status</TableHead>
                <TableHead className="min-w-[20px]">Action</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {campaigns?.contents?.length ? (
                campaigns?.contents?.map((campaign, index) => (
                  <TableRow key={campaign.id}>
                    <TableCell>{campaign.campaignName}</TableCell>
                    <TableCell>
                      {campaign?.startDate?.length
                        ? format(campaign?.startDate, "PPP")
                        : null}
                    </TableCell>
                    <TableCell>
                      {campaign?.endDate?.length
                        ? format(campaign?.endDate, "PPP")
                        : null}
                    </TableCell>
                    <TableCell>{campaign.remainingDays}</TableCell>
                    <TableCell>
                      <Badge
                        variant={campaign.isCampaignOpen ? "" : "destructive"}
                      >
                        {campaign.isCampaignOpen ? "Open" : "Closed"}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex">
                        {permissions?.[PAGE_NAME]?.update ? (
                          <RButton
                            variant="ghost"
                            onClick={handleEditCampaign(index)}
                          >
                            <Edit className="h-4 w-4" />
                          </RButton>
                        ) : null}
                        {permissions?.[PAGE_NAME]?.delete ? (
                          <ConfirmDialog
                            dialogTrigger={
                              <RButton
                                variant="ghost"
                                className="flex items-center gap-2"
                                onClick={(event) => {
                                  setCampaignIndex(index);
                                }}
                              >
                                <X className="h-4 w-4 text-red-500" />
                              </RButton>
                            }
                            onConfirm={handleCloseCampaign}
                            dialogTitle="Are you sure to close the campaign?"
                            dialogDescription="This action cannot be undone. This will permanently close your
            campaign."
                          />
                        ) : null}
                      </div>
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={9} className="h-24 text-center">
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
    </div>
  );
}
//export default Campaigns;
export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(Campaigns))
);
