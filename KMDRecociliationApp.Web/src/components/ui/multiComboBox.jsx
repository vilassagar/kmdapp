import { Check, ChevronsUpDown, CircleX } from "lucide-react";
import * as React from "react";
import { Button } from "@/components/ui/button";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { cn } from "@/lib/utils";
import PropTypes from "prop-types";
import { Badge } from "./badge";

export function MultiCombobox({
  options,
  onChange,
  valueProperty,
  labelProperty,
  values,
  isDisabled,
  error,
  id,
  placeholder,
}) {
  const [open, setOpen] = React.useState(false);
  const [cOptions, setCOptions] = React.useState([]);
  const [selectedValues, setSelectedValues] = React.useState([]);
  const [valueSet, setValueSet] = React.useState(new Set());

  React.useEffect(() => {
    if (Array.isArray(options) && options.length) {
      setCOptions(options);
    }
  }, [options]);

  React.useEffect(() => {
    if (Array.isArray(values)) {
      setSelectedValues(values);

      let newSet = new Set();

      values.forEach((value) => {
        newSet.add(value.id);
      });

      setValueSet(newSet);
    }
  }, [values]);

  const handleChange = (value) => (e) => {
    //set value
    let nextState = [...selectedValues];

    if (valueSet.has(Number(value.id))) {
      let indexOfValue = nextState.findIndex(
        (selectedValue) => selectedValue.id == value.id
      );
      nextState.splice(indexOfValue, 1);
    } else {
      nextState.push(value);
    }

    setSelectedValues(nextState);
    setOpen(false);

    onChange(nextState);
  };

  const handleDeleteValue = (index) => (event) => {
    let nextState = [...selectedValues];
    nextState.splice(index, 1);
    setSelectedValues(nextState);

    onChange(nextState);
  };

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          role="combobox"
          aria-expanded={open}
          disabled={isDisabled}
          className={
            error.length
              ? " ring-red-600 focus-visible:ring-offset-0 focus-visible:ring-red-600 ring-1 flex justify-start h-auto min-h-10 py-2"
              : "focus-visible:ring-offset-0 focus-visible:ring-1 flex h-auto min-h-10 justify-start py-2"
          }
          id={id}
          size="sm"
        >
          <div className="flex items-center justify-between">
            <div className="flex flex-wrap">
              {selectedValues.length
                ? selectedValues.map((value, index) => (
                    <Badge className="mr-2 my-1" key={value[valueProperty]}>
                      {value["name"]}
                      <CircleX
                        className="h-4 w-4 ml-2 cursor-pointer"
                        onClick={handleDeleteValue(index)}
                      />
                    </Badge>
                  ))
                : placeholder}
            </div>
            <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
          </div>
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-[200px] p-0">
        <Command>
          <CommandInput placeholder="Search option..." />
          <CommandList>
            <CommandEmpty>No option found.</CommandEmpty>
            <CommandGroup>
              {cOptions.map((option) => (
                <CommandItem
                  key={option[valueProperty]}
                  value={option[valueProperty]}
                  onSelect={handleChange(option)}
                >
                  <Check
                    className={cn(
                      "mr-2 h-4 w-4",
                      valueSet.has(option[valueProperty])
                        ? "opacity-100"
                        : "opacity-0"
                    )}
                  />
                  {option[labelProperty]}
                </CommandItem>
              ))}
            </CommandGroup>
          </CommandList>
        </Command>
      </PopoverContent>
    </Popover>
  );
}

MultiCombobox.defaultProps = {
  options: [],
  onChange: () => {},
  valueProperty: "id",
  labelProperty: "name",
  values: [],
  isDisabled: false,
  error: "",
  id: "",
  placeholder: "Select option...",
};

MultiCombobox.propTypes = {
  options: PropTypes.array,
  onChange: PropTypes.func,
  valueProperty: PropTypes.string,
  labelProperty: PropTypes.string,
  values: PropTypes.array,
  isDisabled: PropTypes.bool,
  error: PropTypes.string,
  id: PropTypes.string,
  placeholder: PropTypes.string,
};
