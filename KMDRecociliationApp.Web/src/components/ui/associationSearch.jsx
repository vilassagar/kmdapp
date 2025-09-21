import PropTypes from "prop-types";
import RInput from "./rInput";
import { useEffect, useState } from "react";
import { Checkbox } from "./checkbox";
import { getAssociationList } from "@/services/association";
import { toast } from "./use-toast";

export default function AssociationSearch({
  associations,
  onSelect,
  selectedCampaignAssociations,
  onIdsChange, // New prop to pass comma-separated IDs back to parent
}) {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedAssociationIds, setSelectedAssociationIds] = useState([]);
  const [selectedAssociations, setSelectedAssociations] = useState([]);
  const [campaignAssociations, setCampaignAssociations] = useState([]);
  const [commaSeparatedIds, setCommaSeparatedIds] = useState("");

  useEffect(() => {
    (async () => {
      setCampaignAssociations(associations);
    })();
  }, [associations]);

  useEffect(() => {
    (() => {
      if (selectedCampaignAssociations) {
        let ids = [];
        selectedCampaignAssociations.forEach((association) => {
          ids.push(association.associationId);
        });

        setSelectedAssociationIds(ids);
        // Update comma-separated string when selectedAssociationIds changes
        setCommaSeparatedIds(ids.join(","));

        setSelectedAssociations(selectedCampaignAssociations);
      } else {
        setSelectedAssociationIds([]);
        setCommaSeparatedIds("");
        setSelectedAssociations([]);
      }
    })();
  }, [selectedCampaignAssociations]);

  useEffect(() => {
    // Update comma-separated string whenever selectedAssociationIds changes
    const newCommaSeparatedIds = selectedAssociationIds.join(",");
    setCommaSeparatedIds(newCommaSeparatedIds);

    // Pass the comma-separated IDs back to parent if onIdsChange is provided
    if (onIdsChange) {
      onIdsChange(newCommaSeparatedIds);
    }
  }, [selectedAssociationIds, onIdsChange]);

  const handleAssociationSelect = (association) => (event) => {
    // update selected associationIds
    setSelectedAssociationIds((prevSelected) => {
      const newSelectedIds = prevSelected.includes(association.associationId)
        ? prevSelected.filter((id) => id !== association.associationId)
        : [...prevSelected, association.associationId];

      // Update comma-separated string directly here for immediate feedback
      setCommaSeparatedIds(newSelectedIds.join(","));

      return newSelectedIds;
    });

    //update selected products
    let nextAssociations = [...selectedAssociations];
    if (event) {
      nextAssociations.push({ ...association, message: "" });
    } else {
      let associationId = association.associationId; // Fixed typo: associoationId -> associationId
      let productIndex = nextAssociations.findIndex(
        (assoc) => assoc.associationId === associationId // Fixed comparison and typo
      );
      if (productIndex !== -1) {
        nextAssociations.splice(productIndex, 1);
      }
    }

    setSelectedAssociations(nextAssociations);

    // Pass both the association objects and the comma-separated IDs
    onSelect(nextAssociations);
  };

  const handleSearchAssociation = async (event) => {
    setSearchTerm(event.target.value);

    let response = await getAssociationList(1, 10000, event.target.value);
    if (response.status === "success") {
      setCampaignAssociations(response?.data?.contents);
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get associations.",
      });
    }
  };

  const handleSelectAllAssociations = (event) => {
    if (event) {
      let selectedAssociationIds = [];
      campaignAssociations.forEach((association) => {
        selectedAssociationIds.push(association.associationId);
      });
      setSelectedAssociationIds(selectedAssociationIds);
      // Create comma-separated string of all association IDs
      const commaSeparated = selectedAssociationIds.join(",");
      setCommaSeparatedIds(commaSeparated);

      setSelectedAssociations(campaignAssociations);
      onSelect(campaignAssociations);

      // Pass the comma-separated IDs back to parent if onIdsChange is provided
      if (onIdsChange) {
        onIdsChange(commaSeparated);
      }

      // Log the comma separated IDs
    } else {
      setSelectedAssociationIds([]);
      setCommaSeparatedIds("");
      setSelectedAssociations([]);
      onSelect([]);
    }
  };

  return (
    <div className="space-y-2 pt-5">
      <h1 className="text-2xl font-bold">Select Associations</h1>
      <div className="border rounded-lg">
        <div className="p-4 border-b flex">
          <RInput
            placeholder="Search Associations..."
            value={searchTerm}
            onChange={handleSearchAssociation}
            type="text"
          />
        </div>
        <div className="p-4 text-sm text-muted-foreground flex justify-between">
          <div>
            {selectedAssociationIds.length} out of {campaignAssociations.length}{" "}
            associations selected.
          </div>
          <div className="px-4 py-2">
            <Checkbox
              className="mr-3"
              id="check-spouse-premium"
              onCheckedChange={handleSelectAllAssociations}
            />
            Select All
          </div>
        </div>
        {/* {commaSeparatedIds && (
          <div className="px-4 py-2 border-t">
            <p className="text-sm font-medium">Selected Association IDs:</p>
            <div className="mt-1 p-2 bg-gray-50 rounded overflow-x-auto">
              <code className="text-xs">{commaSeparatedIds}</code>
            </div>
          </div>
        )} */}
        <div className="flex flex-wrap gap-2 pb-5 border-b px-4">
          {selectedAssociationIds.map((associationId) => {
            const association = associations.find(
              (a) => a.associationId === associationId
            );
            return (
              <div
                key={association.associationId}
                className="bg-muted/50 px-3 py-1 rounded-md flex items-center gap-2"
              >
                <span>{association.associationName}</span>
              </div>
            );
          })}
        </div>
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 overflow-y-auto max-h-[300px] pb-10">
          {campaignAssociations.map((association) => (
            <div
              key={association.associationId}
              className="bg-white rounded-lg shadow-md overflow-hidden"
            >
              <div className="p-4">
                <div className="flex items-center justify-between mb-2">
                  <div className="flex items-center">
                    <Checkbox
                      checked={selectedAssociationIds.includes(
                        association.associationId
                      )}
                      className="mr-2"
                      onCheckedChange={handleAssociationSelect(association)}
                    />
                    <h3 className="text-lg font-bold">
                      {association.associationName}
                    </h3>
                  </div>
                </div>
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-gray-600 font-bold mt-2">
                      {association.organisationName}
                    </p>
                    <p className="text-sm text-gray-500">
                      {association.members} Members
                    </p>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

AssociationSearch.defaultProps = {
  associations: [],
  onSelect: () => {},
  onIdsChange: null,
  selectedCampaignAssociations: [],
};

AssociationSearch.propTypes = {
  associations: PropTypes.array,
  onSelect: PropTypes.func,
  onIdsChange: PropTypes.func,
  selectedCampaignAssociations: PropTypes.array,
};
