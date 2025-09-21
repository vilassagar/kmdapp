import PropTypes from "prop-types";
import RInput from "./rInput";
import { useEffect, useState } from "react";
import { Checkbox } from "./checkbox";
import { getProductList } from "@/services/product";
import { toast } from "./use-toast";

export default function ProductSearch({
  products,
  onSelect,
  selectedCampaignProducts,
  onIdsChange, // New prop to pass comma-separated IDs back to parent
}) {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedProductIds, setSelectedProductIds] = useState([]);
  const [selectedProducts, setSelectedProducts] = useState([]);
  const [campaignProducts, setCampaignProducts] = useState([]);
  const [commaSeparatedIds, setCommaSeparatedIds] = useState("");

  useEffect(() => {
    (() => {
      setCampaignProducts(products);
    })();
  }, [products]);

  useEffect(() => {
    (() => {
      if (selectedCampaignProducts?.length) {
        let ids = [];
        selectedCampaignProducts.forEach((product) => {
          ids.push(product.productId);
        });

        setSelectedProductIds(ids);

        // Update comma-separated string when ids are set
        setCommaSeparatedIds(ids.join(","));

        setSelectedProducts(selectedCampaignProducts);
      } else {
        setSelectedProductIds([]);
        setCommaSeparatedIds("");
        setSelectedProducts([]);
      }
    })();
  }, [selectedCampaignProducts]);

  // Update comma-separated IDs whenever selectedProductIds changes
  useEffect(() => {
    const newCommaSeparatedIds = selectedProductIds.join(",");
    setCommaSeparatedIds(newCommaSeparatedIds);

    // Pass the comma-separated IDs back to parent if onIdsChange is provided
    if (onIdsChange) {
      onIdsChange(newCommaSeparatedIds);
    }
  }, [selectedProductIds, onIdsChange]);

  const handleProductSelect = (product) => (event) => {
    // update selected productIds
    setSelectedProductIds((prevSelected) => {
      const newSelectedIds = prevSelected.includes(product.productId)
        ? prevSelected.filter((productId) => productId !== product.productId)
        : [...prevSelected, product.productId];

      // Update comma-separated string directly here for immediate feedback
      const newCommaSeparatedIds = newSelectedIds.join(",");
      setCommaSeparatedIds(newCommaSeparatedIds);

      // Pass the comma-separated IDs back to parent if onIdsChange is provided
      if (onIdsChange) {
        onIdsChange(newCommaSeparatedIds);
      }

      return newSelectedIds;
    });

    //update selected products
    let nextProducts = [...selectedProducts];
    if (event) {
      nextProducts.push(product);
    } else {
      let productId = product.productId;
      let productIndex = nextProducts.findIndex(
        (p) => p.productId === productId // Using === instead of ==
      );
      if (productIndex !== -1) {
        // Check if product was found
        nextProducts.splice(productIndex, 1);
      }
    }

    setSelectedProducts(nextProducts);
    onSelect(nextProducts);
  };

  const handleSearchProduct = async (event) => {
    setSearchTerm(event.target.value);

    let response = await getProductList(1, 10000, event.target.value);
    if (response.status === "success") {
      setCampaignProducts(response?.data?.contents);
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get products.",
      });
    }
  };

  const handleSelectAllProducts = (event) => {
    if (event) {
      let selectedProductIds = [];
      campaignProducts.forEach((product) => {
        selectedProductIds.push(product.productId);
      });
      setSelectedProductIds(selectedProductIds);

      // Create comma-separated string of all product IDs
      const commaSeparated = selectedProductIds.join(",");
      setCommaSeparatedIds(commaSeparated);

      // Pass the comma-separated IDs back to parent if onIdsChange is provided
      if (onIdsChange) {
        onIdsChange(commaSeparated);
      }

      setSelectedProducts(campaignProducts);
      onSelect(campaignProducts);
    } else {
      setSelectedProductIds([]);
      setCommaSeparatedIds("");

      // Pass empty string to parent when deselecting all
      if (onIdsChange) {
        onIdsChange("");
      }

      setSelectedProducts([]);
      onSelect([]);
    }
  };

  return (
    <div className="space-y-2">
      <h1 className="text-2xl font-bold">Select Products</h1>
      <div className="border rounded-lg">
        <div className="p-4 border-b flex">
          <RInput
            placeholder="Search Products..."
            value={searchTerm}
            onChange={handleSearchProduct}
            type="text"
          />
        </div>
        <div className="p-4 text-sm text-muted-foreground flex justify-between">
          <div>
            {selectedProductIds.length} out of {campaignProducts.length}{" "}
            products selected.
          </div>

          <div className="px-4 py-2">
            <Checkbox
              className="mr-3"
              id="check-spouse-premium"
              onCheckedChange={handleSelectAllProducts}
            />
            Select All
          </div>
        </div>

        <div className="flex flex-wrap gap-2 pb-5 border-b px-4">
          {selectedProductIds.map((productId) => {
            const product = products.find((p) => p.productId === productId);
            return (
              <div
                key={product.productId}
                className="bg-muted/50 px-3 py-1 rounded-md flex items-center gap-2"
              >
                <span>{product.productName}</span>
              </div>
            );
          })}
        </div>
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 overflow-y-auto max-h-[300px] pb-10">
          {campaignProducts.length
            ? campaignProducts.map((product) => (
                <div
                  key={product.productId}
                  className="bg-white rounded-lg shadow-md overflow-hidden"
                >
                  <div className="p-4">
                    <div className="flex items-center justify-between mb-2">
                      <div className="flex items-center">
                        <Checkbox
                          checked={selectedProductIds.includes(
                            product.productId
                          )}
                          className="mr-2"
                          onCheckedChange={handleProductSelect(product)}
                        />
                        <h3 className="text-lg font-bold">
                          {product.productName}
                        </h3>
                      </div>
                    </div>
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-600 font-bold mt-2">
                          {product.providerName}
                        </p>
                        <p className="text-sm text-gray-500">
                          {product.policyType?.toLowerCase().trim() ===
                          "topuppolicy"
                            ? `Top Up Policy (Linked to ${product.basePolicy})`
                            : product.policyType}
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
              ))
            : null}
        </div>
      </div>
    </div>
  );
}

ProductSearch.defaultProps = {
  products: [],
  onSelect: () => {},
  onIdsChange: null,
  selectedCampaignProducts: [],
};

ProductSearch.propTypes = {
  products: PropTypes.array,
  onSelect: PropTypes.func,
  onIdsChange: PropTypes.func,
  selectedCampaignProducts: PropTypes.array,
};
