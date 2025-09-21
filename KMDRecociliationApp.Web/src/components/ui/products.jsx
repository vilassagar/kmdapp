import { useState } from "react";
import { Input } from "@/components/ui/input";
import { Checkbox } from "@/components/ui/checkbox";
import { Button } from "@/components/ui/button";

export default function Products() {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedProducts, setSelectedProducts] = useState([]);
  const insuranceProducts = [
    {
      id: 1,
      name: "Comprehensive Car Insurance",
      description: "Protect your vehicle from accidents and damages.",
      price: 99.99,
    },
    {
      id: 2,
      name: "Home Insurance",
      description: "Safeguard your home and belongings.",
      price: 149.99,
    },
    {
      id: 3,
      name: "Life Insurance",
      description: "Secure your family's financial future.",
      price: 29.99,
    },
    {
      id: 4,
      name: "Travel Insurance",
      description: "Enjoy your trips with peace of mind.",
      price: 59.99,
    },
    {
      id: 5,
      name: "Health Insurance",
      description: "Access quality healthcare with ease.",
      price: 199.99,
    },
    {
      id: 6,
      name: "Renters Insurance",
      description: "Protect your belongings in a rental property.",
      price: 39.99,
    },
    {
      id: 7,
      name: "Pet Insurance",
      description: "Protect your furry friends.",
      price: 29.99,
    },
    {
      id: 8,
      name: "Dental Insurance",
      description: "Keep your smile healthy.",
      price: 49.99,
    },
    {
      id: 9,
      name: "Disability Insurance",
      description: "Protect your income if you become disabled.",
      price: 79.99,
    },
    {
      id: 10,
      name: "Flood Insurance",
      description: "Protect your home from water damage.",
      price: 89.99,
    },
    {
      id: 11,
      name: "Earthquake Insurance",
      description: "Protect your home from natural disasters.",
      price: 99.99,
    },
    {
      id: 12,
      name: "Motorcycle Insurance",
      description: "Protect your bike on the open road.",
      price: 59.99,
    },
    {
      id: 13,
      name: "Boat Insurance",
      description: "Protect your watercraft from accidents.",
      price: 149.99,
    },
    {
      id: 14,
      name: "Recreational Vehicle Insurance",
      description: "Protect your RV on your adventures.",
      price: 89.99,
    },
    {
      id: 15,
      name: "Cyber Insurance",
      description: "Protect your digital assets from cyber threats.",
      price: 79.99,
    },
    {
      id: 16,
      name: "Umbrella Insurance",
      description: "Protect your assets from lawsuits.",
      price: 199.99,
    },
    {
      id: 17,
      name: "Identity Theft Insurance",
      description: "Protect your identity from fraud.",
      price: 39.99,
    },
    {
      id: 18,
      name: "Pet Liability Insurance",
      description: "Protect yourself from pet-related lawsuits.",
      price: 29.99,
    },
    {
      id: 19,
      name: "Business Insurance",
      description: "Protect your business from risks.",
      price: 299.99,
    },
    {
      id: 20,
      name: "Farm Insurance",
      description: "Protect your agricultural assets.",
      price: 199.99,
    },
    {
      id: 21,
      name: "Landlord Insurance",
      description: "Protect your rental property.",
      price: 149.99,
    },
    {
      id: 22,
      name: "Jewelry Insurance",
      description: "Protect your valuable jewelry.",
      price: 79.99,
    },
    {
      id: 23,
      name: "Art Insurance",
      description: "Protect your art collection.",
      price: 199.99,
    },
    {
      id: 24,
      name: "Wedding Insurance",
      description: "Protect your special day.",
      price: 99.99,
    },
    {
      id: 25,
      name: "Event Insurance",
      description: "Protect your events from unexpected issues.",
      price: 79.99,
    },
    {
      id: 26,
      name: "Drone Insurance",
      description: "Protect your drone from accidents.",
      price: 49.99,
    },
    {
      id: 27,
      name: "Cyber Liability Insurance",
      description: "Protect your business from cyber attacks.",
      price: 199.99,
    },
    {
      id: 28,
      name: "Professional Liability Insurance",
      description: "Protect your business from professional errors.",
      price: 299.99,
    },
    {
      id: 29,
      name: "Errors and Omissions Insurance",
      description: "Protect your business from professional mistakes.",
      price: 249.99,
    },
    {
      id: 30,
      name: "Pollution Liability Insurance",
      description: "Protect your business from environmental risks.",
      price: 199.99,
    },
    {
      id: 31,
      name: "Kidnap and Ransom Insurance",
      description: "Protect your business from kidnapping and ransom threats.",
      price: 499.99,
    },
  ];
  const filteredProducts = insuranceProducts.filter((product) =>
    product.name.toLowerCase().includes(searchTerm.toLowerCase())
  );
  const handleSearch = (e) => {
    setSearchTerm(e.target.value);
  };
  const handleProductSelect = (product) => {
    if (selectedProducts.includes(product.id)) {
      setSelectedProducts(selectedProducts.filter((id) => id !== product.id));
    } else {
      setSelectedProducts([...selectedProducts, product.id]);
    }
  };

  return (
    <div>
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 overflow-y-auto max-h-[500px] pb-10">
        {filteredProducts.map((product) => (
          <div
            key={product.id}
            className="bg-white rounded-lg shadow-md overflow-hidden"
          >
            <div className="p-4">
              <div className="flex items-center justify-between mb-2">
                <div className="flex items-center">
                  <Checkbox
                    checked={selectedProducts.includes(product.id)}
                    className="mr-2"
                    onCheckedChange={() => handleProductSelect(product)}
                  />
                  <h3 className="text-lg font-bold">{product.name}</h3>
                </div>
                <div className="text-primary-500 font-bold">
                  ${product.price}
                </div>
              </div>
              <p className="text-gray-500 mb-4">{product.description}</p>
            </div>
          </div>
        ))}
        {Array.from({ length: 30 }).map((_, index) => (
          <div
            key={index + 7}
            className="bg-white rounded-lg shadow-md overflow-hidden"
          >
            <div className="p-4">
              <div className="flex items-center justify-between mb-2">
                <div className="flex items-center">
                  <Checkbox
                    checked={selectedProducts.includes(index + 7)}
                    className="mr-2"
                    onCheckedChange={() =>
                      handleProductSelect({ id: index + 7 })
                    }
                  />
                  <h3 className="text-lg font-bold">
                    Additional Product {index + 1}
                  </h3>
                </div>
                <div className="text-primary-500 font-bold">$99.99</div>
              </div>
              <p className="text-gray-500 mb-4">
                This is an additional insurance product.
              </p>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
