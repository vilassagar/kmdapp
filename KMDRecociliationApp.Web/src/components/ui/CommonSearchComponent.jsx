/* eslint-disable react/prop-types */
import React from "react";
import { X } from "lucide-react";

const SearchComponent = ({
  searchTerm,
  selectedCampaign,
  onSearchChange,
  onSearch,
  onClearSearch,
  onKeyPress,
  placeholder = "Search...",
}) => {
  return (
    <div className="w-full flex items-center space-x-4">
      {/* Search Input */}
      <div className="flex flex-1">
        <input
          type="search"
          placeholder={placeholder}
          value={searchTerm}
          onChange={onSearchChange}
          onKeyPress={onKeyPress}
          className="flex-1 px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:border-red-900"
        />
        <button
          onClick={onSearch}
          className="bg-red-900 hover:bg-red-800 text-white px-6 py-2 font-medium transition-colors flex items-center gap-2"
        >
          Search
        </button>
        <button
          onClick={onClearSearch}
          className="bg-gray-200 hover:bg-gray-300 text-gray-700 px-6 py-2 rounded-md font-medium transition-colors flex items-center gap-2"
        >
          <X size={18} />
          Clear
        </button>
      </div>
    </div>
  );
};

export default SearchComponent;
