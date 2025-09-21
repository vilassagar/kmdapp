import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import AssociationSearch from "@/components/ui/associationSearch";
import { Button } from "@/components/ui/button";
import { DateRangePicker } from "@/components/ui/dateRangePicker";
import ProductSearch from "@/components/ui/productSearch";
import RInput from "@/components/ui/rInput";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Label } from "@/components/ui/label";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Textarea } from "@/components/ui/textarea";
import { toast } from "@/components/ui/use-toast";
import {
  getAssociationWiseTemapltes,
  getCampaignPostObject,
} from "@/lib/helperFunctions";
import { useCampaignsStore, usePermissionStore } from "@/lib/store";
import {
  getAssociationList,
  getMessageTemplates,
} from "@/services/association";
import {
  getCampaignById,
  saveCampaign,
  sendCampaignMessage,
  uploadCampaignTemplate,
} from "@/services/campaigns";
import { getAllProductList } from "@/services/product";
import { produce } from "immer";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import FileUpload from "@/components/ui/fileUpload";
const PAGE_NAME = "campaigns";

function CreateCampaign() {
  const navigate = useNavigate();
  const mode = useCampaignsStore((state) => state.mode);
  const currentCampaignId = useCampaignsStore(
    (state) => state.currentCampaignId
  );

  const campaign = useCampaignsStore((state) => state.campaign);
  const initializeCampaign = useCampaignsStore(
    (state) => state.initializeCampaign
  );

  const permissions = usePermissionStore((state) => state.permissions);
  const updateCampaign = useCampaignsStore((state) => state.updateCampaign);
  const products = useCampaignsStore((state) => state.products);
  const addProducts = useCampaignsStore((state) => state.addProducts);
  const associations = useCampaignsStore((state) => state.associations);
  const addAssociations = useCampaignsStore((state) => state.addAssociations);

  const [activeTab, setActiveTab] = useState("create");
  const [isNameEmpty, setIsNameEmpty] = useState(false);
  const [isDateError, setIsDateError] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  // useEffect(() => {
  //   (async () => {
  //     try {
  //       const [response1, response2] = await Promise.all([
  //         getAllProductList(1, 1000, ""),
  //         getAssociationList(1, 1000, ""),
  //       ]);
  //       addProducts(response1.data?.contents || []);
  //       addAssociations(response2.data?.contents || []);
  //     } catch (err) {
  //       // show error
  //     }

  //     if (mode === "new") {
  //       initializeCampaign();
  //     }

  //     if (mode === "edit") {
  //       // get campaign by id
  //       let response = await getCampaignById(currentCampaignId);
  //       if (response.status === "success") {
  //         updateCampaign(response.data);
  //       } else {
  //         toast({
  //           variant: "destructive",
  //           title: "Something went wrong.",
  //           description: "Unable to get campaign",
  //         });
  //       }
  //     }
  //   })();
  // }, []);
  // Modify your useEffect to include proper loading states
  useEffect(() => {
    (async () => {
      setIsLoading(true);
      try {
        const [response1, response2] = await Promise.all([
          getAllProductList(1, 1000, ""),
          getAssociationList(1, 1000, ""),
        ]);
        addProducts(response1.data?.contents || []);
        addAssociations(response2.data?.contents || []);

        if (mode === "new") {
          initializeCampaign();
        } else if (mode === "edit") {
          // get campaign by id
          let response = await getCampaignById(currentCampaignId);
          if (response.status === "success") {
            // Make sure IDs are properly set from the fetched data
            const campaignData = response.data;
            if (campaignData.associations) {
              campaignData.associationIds = campaignData.associations
                .map((assoc) => assoc.associationId)
                .toString();
            }
            if (campaignData.products) {
              campaignData.productIds = campaignData.products
                .map((prod) => prod.productId)
                .toString();
            }
            updateCampaign(campaignData);
          } else {
            toast({
              variant: "destructive",
              title: "Something went wrong.",
              description: "Unable to get campaign",
            });
          }
        }
      } catch (err) {
        toast({
          variant: "destructive",
          title: "Error loading data",
          description: "Failed to load necessary data",
        });
      } finally {
        setIsLoading(false);
      }
    })();
  }, []);

  const handleTabChange = (tabName) => {
    if (tabName === "execute") {
      if (permissions?.[PAGE_NAME]?.create) {
        //  handleCreateCampaign();
        setActiveTab("execute");
      } else {
        toast({
          variant: "destructive",
          title: "Something went wrong.",
          description: "You do not have permission to create campaign.",
        });
      }
    } else {
      setActiveTab(tabName);
    }
  };

  const handleChange = (name) => (event) => {
    let nextState = produce(campaign, (draft) => {
      switch (name) {
        case "campaignName":
          draft[name] = event.target.value;

          if (event.target.value.length) {
            setIsNameEmpty(false);
          }
          break;
        case "templateName":
          draft[name] = event.target.value;
          break;
        case "templateDocument":
          draft[name] = event;
          draft["istemplateDocumentUpdated"] = true;
          break;

        case "dates":
          draft["startDate"] = event?.from;
          draft["endDate"] = event?.to;

          if (event?.from === "" || event?.to === "") {
            setIsDateError(true);
          } else {
            setIsDateError(false);
          }
          break;

        case "products":
          draft[name] = event;
          break;

        case "associations":
          draft[name] = event;
          break;
      }
    });

    updateCampaign(nextState);
  };

  // New function to handle the comma-separated association IDs
  const handleAssociationIdsChange = (commaSeparatedIds) => {
    let nextState = produce(campaign, (draft) => {
      draft.associationIds = commaSeparatedIds;
    });

    updateCampaign(nextState);
  };

  // New function to handle the comma-separated product IDs
  const handleProductIdsChange = (commaSeparatedIds) => {
    let nextState = produce(campaign, (draft) => {
      draft.productIds = commaSeparatedIds;
    });

    updateCampaign(nextState);
  };

  const initialiseAssociationTemplates = async () => {
    //  get the templates of all associations and add it in object
    let templates = {};

    // Use the comma-separated association IDs if available, otherwise create it from the associations array
    let associationIds =
      campaign.associationIds ||
      campaign.associations
        .map((association) => association.associationId)
        .toString();

    let response = await getMessageTemplates(associationIds);
    if (response.status === "success") {
      templates = getAssociationWiseTemapltes(response.data);
      // set templates for all selected associations
      let nextState = produce(campaign, (draft) => {
        draft["associations"].forEach((association) => {
          association["message"] = templates[association.associationId];
        });
      });
      updateCampaign(nextState);
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get message templates",
      });
    }
  };

  // Update your handleCreateCampaign function to handle both create and update
  const handleCreateCampaign = async () => {
    // Validation checks remain the same
    if (!campaign.campaignName.length) {
      setIsNameEmpty(true);
      return;
    }

    if (!campaign.startDate?.length || !campaign.endDate?.length) {
      setIsDateError(true);
      return;
    }

    if (!campaign.productIds?.length) {
      toast({
        variant: "destructive",
        title: "Product not selected",
        description: "Please select at least one product.",
      });
      return;
    }

    if (!campaign.associationIds?.length) {
      toast({
        variant: "destructive",
        title: "Association not selected",
        description: "Please select at least one association.",
      });
      return;
    }

    // Get the campaign post object
    const postObject = getCampaignPostObject({
      ...campaign,
      associationIdsString: campaign.associationIds,
      productIdsString: campaign.productIds,
    });

    // Send the postObject to saveCampaign
    let response = await saveCampaign(postObject);

    if (response.status === "success") {
      // Handle template document upload if needed
      if (campaign.templateDocument && campaign.istemplateDocumentUpdated) {
        const submitData = new FormData();
        submitData.append("file", campaign.templateDocument);
        submitData.append(
          "campaignId",
          response.data.campaignId || campaign.campaignId
        );

        const uploadResponse = await uploadCampaignTemplate(submitData);
        if (uploadResponse.status !== "success") {
          toast({
            variant: "warning",
            title: "Campaign saved but template upload failed",
            description: "Template document couldn't be uploaded.",
          });
        }
      }

      toast({
        title: `Campaign ${
          mode === "new" ? "created" : "updated"
        } successfully`,
        description: "You can now proceed to execute the campaign.",
      });
      navigate("/campaigns");
    } else {
      if (response.status === "conflict") {
        toast({
          variant: "destructive",
          title: "Something went wrong.",
          description: response.errors.message,
        });
      } else {
        toast({
          variant: "destructive",
          title: "Something went wrong.",
          description: `Unable to ${
            mode === "new" ? "save" : "update"
          } campaign.`,
        });
      }
    }
  };

  const sendCampaign = async (associationId, messageTemplateName) => {
    let postObject = {
      campaignId: campaign?.campaignId || 0,
      associationId: associationId || 0,
      messageTemplateName: messageTemplateName || "",
    };
    let response = await sendCampaignMessage(postObject);
    if (response.status === "success") {
      setActiveTab("execute");
    } else {
      // 500
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to send campaign.",
      });
    }
  };

  // Add this new function to handle individual association messages
  const handleAssociationMessageChange = (associationId) => (event) => {
    let nextState = produce(campaign, (draft) => {
      // Find the specific association and update its message
      const associationIndex = draft.associations.findIndex(
        (assoc) => assoc.associationId === associationId
      );

      if (associationIndex !== -1) {
        draft.associations[associationIndex].message = event.target.value;
      }
    });

    updateCampaign(nextState);
  };

  return (
    <div>
      <div className="mb-6">
        <h1 className="text-2xl font-bold ">
          {mode === "new" ? "Create" : "Edit"} Campaign
        </h1>
      </div>

      {isLoading ? (
        <div className="flex justify-center items-center h-64">
          <p>Loading campaign data...</p>
        </div>
      ) : (
        <div>
          <div className="mb-6">
            <h1 className="text-2xl font-bold ">Create Campaign</h1>
          </div>

          <div className="space-y-6 my-5">
            <div className="flex-1">
              <div className="flex gap-32">
                <div className="mb-5 w-1/4 mr-5">
                  <RInput
                    label="Campaign Name"
                    placeholder="Enter Campaign Name"
                    onChange={handleChange("campaignName")}
                    value={campaign?.campaignName || ""}
                    type="text"
                    isRequired={true}
                    error={isNameEmpty ? "empty name" : ""}
                  />
                </div>

                <div className="m2-5">
                  <DateRangePicker
                    label="Select Date Range"
                    dates={{ from: campaign?.startDate, to: campaign?.endDate }}
                    onChange={handleChange("dates")}
                    size=""
                    isRequired={true}
                    error={isDateError ? "error" : ""}
                  />
                </div>
              </div>

              {/* <div className="flex gap-32">
                <div className="mb-5 w-1/4 mr-5">
                  <RInput
                    label="WhatsApp Template Name"
                    placeholder="Enter WhatsApp Template Name"
                    onChange={handleChange("templateName")}
                    value={campaign?.templateName || ""}
                    type="text"
                    isRequired={true}
                    error={isNameEmpty ? "empty name" : ""}
                  />
                </div>

                <div className="flex flex-col justify-between space-y-2">
                  <Label
                    htmlFor="templateDocument"
                    className="whitespace-nowrap"
                  >
                    Template Document
                  </Label>

                  <FileUpload
                    id="templateDocument"
                    onChange={handleChange("templateDocument")}
                    value={campaign?.templateDocument || null}
                    accept=".pdf"
                  />
                </div>
              </div> */}
            </div>
            {/* product selection */}
            <ProductSearch
              products={products}
              onSelect={handleChange("products")}
              onIdsChange={handleProductIdsChange}
              selectedCampaignProducts={campaign?.products || []}
            />
            {/* asscociation selection */}
            <AssociationSearch
              associations={associations}
              onSelect={handleChange("associations")}
              onIdsChange={handleAssociationIdsChange}
              selectedCampaignAssociations={campaign?.associations || []}
            />
          </div>

          {permissions?.[PAGE_NAME]?.create ? (
            <div className="flex justify-end">
              <Button onClick={handleCreateCampaign}>Save</Button>
            </div>
          ) : null}
        </div>
      )}
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(CreateCampaign))
);
