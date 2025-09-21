import { getAssociations, getUserTypes } from "@/services/customerProfile";
import { FilterIcon } from "lucide-react";
import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import { Combobox } from "./comboBox";
import { Dialog, DialogContent } from "./dialog";
import { Label } from "./label";
import RButton from "./rButton";
import { Checkbox } from "./checkbox";
import RInput from "./rInput";

export default function UsersFilter({ open, setOpen, onFilterApply }) {
  const [associations, setAssociations] = useState([]);
  const [filteredAssociations, setFilteredAssociations] = useState([]);
  const [userTypes, setUserTypes] = useState([]);
  const [associationSearchTerm, setAssociationSearchTerm] = useState("");
  const [selectedValues, setSelectedValues] = useState();

  useEffect(() => {
    (async () => {
      try {
        const [response1, response2] = await Promise.all([
          getAssociations(),
          getUserTypes(),
        ]);

        setAssociations(response1.data);
        setFilteredAssociations(response1.data);
        setUserTypes(response2.data);
      } catch (err) {
        // show error
      }
    })();
  }, []);

  const handleAssociationSearch = (event) => {
    let searchString = event.target.value;
    setAssociationSearchTerm(searchString);
    let filteredAssociations = associations.filter((association) =>
      association?.name
        .toLowerCase()
        .trim()
        .includes(searchString.toLowerCase().trim())
    );

    setFilteredAssociations(filteredAssociations);
  };

  const handleUserTypeChange = (id) => (event) => {
    let nextState;
  };

  return (
    <div>
      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="sm:max-w-[500px]">
          <form className="space-y-2">
            <div className="grid grid-cols-1 gap-4 my-5">
              <div className="space-y-2">
                <Label htmlFor="association-filter">Filter by User Type </Label>
                <div className="grid grid-cols-2 md:grid-cols-3 gap-2">
                  {userTypes?.map((type) => (
                    <Label
                      key={type.id}
                      className="flex items-center gap-2 font-normal "
                    >
                      <Checkbox
                        onCheckedChange={handleUserTypeChange(type.id)}
                        checked={type?.isChecked}
                      />
                      {type?.name}
                    </Label>
                  ))}
                </div>
              </div>
            </div>
            <div className="grid grid-cols-1 gap-4 pt-5">
              <div className="space-y-2 ">
                <RInput
                  placeholder="Search associations"
                  label="Filter by Association"
                  id="association-filter"
                  type="text"
                  onChange={handleAssociationSearch}
                  value={associationSearchTerm}
                />
                <div className="grid grid-cols-2 md:grid-cols-3 gap-2 pt-3">
                  {filteredAssociations?.map((association) => (
                    <Label
                      key={association.id}
                      className="flex items-center gap-2 font-normal mb-3"
                    >
                      <Checkbox id="category-1" />
                      {association?.name}
                    </Label>
                  ))}
                </div>
              </div>
            </div>

            <div className="flex justify-end gap-2 pt-5 ">
              <RButton
                variant="outline"
                onClick={() => setOpen(false)}
                size="sm"
              >
                Cancel
              </RButton>
              <RButton size="sm">
                <span className="flex justify-between">
                  <FilterIcon className="w-5 h-5 mr-2" /> Apply Filters
                </span>
              </RButton>
            </div>
          </form>
        </DialogContent>
      </Dialog>

      <div></div>
    </div>
  );
}

UsersFilter.defaultProps = {
  open: false,
  setOpen: () => {},
  onFilterApply: () => {},
};

UsersFilter.propTypes = {
  open: PropTypes.bool,
  setOpen: PropTypes.func,
  onFilterApply: PropTypes.func,
};
