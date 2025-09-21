import React, { useEffect, useState } from "react";
import { Label } from "./label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./select";
import PropTypes from "prop-types";

export default function RSelect({
  id,
  label,
  value,
  onChange,
  options,
  isDisabled,
  placeholder,
  className,
  valueProperty,
  nameProperty,
  isRequired,
}) {
  const [selectedValue, setSelectedValue] = useState("");

  useEffect(() => {
    if (value) {
      setSelectedValue(value[valueProperty]);
    }
  }, [value]);

  const handleChange = (event) => {
    let selectedOption = options.find(
      (option) => option[valueProperty] == event
    );

    setSelectedValue(event);

    onChange(selectedOption);
  };

  return (
    <div className={className}>
      {label ? (
        <Label htmlFor={id}>
          <span>{label}</span>
          {isRequired ? <span className="text-red-600	ml-1">*</span> : null}
        </Label>
      ) : null}
      <Select
        id={id}
        onValueChange={handleChange}
        value={selectedValue || ""}
        disabled={isDisabled}
      >
        <SelectTrigger>
          <SelectValue placeholder={placeholder} />
        </SelectTrigger>
        <SelectContent>
          {options.map((option) => (
            <SelectItem
              key={option[valueProperty]}
              value={option[valueProperty]}
            >
              {option[nameProperty]}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}

RSelect.defaultProps = {
  id: "",
  label: "",
  value: null,
  onChange: () => {},
  options: [],
  isDisabled: false,
  placeholder: "",
  className: "",
  valueProperty: "id",
  nameProperty: "name",
  isRequired: false,
};

RSelect.propTypes = {
  id: PropTypes.string,
  label: PropTypes.string,
  value: PropTypes.object,
  onChange: PropTypes.func,
  options: PropTypes.array,
  isDisabled: PropTypes.bool,
  placeholder: PropTypes.string,
  className: PropTypes.string,
  valueProperty: PropTypes.string,
  nameProperty: PropTypes.string,
  isRequired: PropTypes.bool,
};
