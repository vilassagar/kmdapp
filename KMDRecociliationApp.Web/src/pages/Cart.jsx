import { useState } from "react";
import { Checkbox } from "@/components/ui/checkbox";
import { Button } from "@/components/ui/button";
import WithLayout from "@/components/layout/WithLayout";
import { useNavigate } from "react-router-dom";

function Cart() {
  const navigate = useNavigate();
  const policies = [
    {
      id: 1,
      name: "Life Insurance",
      description:
        "Protect your loved ones with a comprehensive life insurance policy.",
      price: 4999,
      checked: true,
    },
    {
      id: 2,
      name: "Home Insurance",
      description:
        "Safeguard your home and belongings with our reliable home insurance.",
      price: 7989,
      checked: true,
    },
    {
      id: 3,
      name: "Auto Insurance",
      description:
        "Drive with confidence knowing you're covered with our auto insurance.",
      price: 5939,
      checked: true,
    },
    {
      id: 4,
      name: "Health Insurance",
      description:
        "Prioritize your well-being with our comprehensive health insurance plan.",
      price: 9399,
      checked: true,
    },
  ];
  const [cart, setCart] = useState(policies);
  const togglePolicy = (id) => {
    setCart(
      cart.map((policy) =>
        policy.id === id ? { ...policy, checked: !policy.checked } : policy
      )
    );
  };
  const totalPrice = cart.reduce(
    (total, policy) => total + (policy.checked ? policy.price : 0),
    0
  );
  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-6">Cart</h1>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
        {cart.map((policy) => (
          <div key={policy.id} className="bg-white rounded-lg shadow-md p-6">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-lg font-bold">{policy.name}</h2>
              <Checkbox
                id={`policy-${policy.id}`}
                checked={policy.checked}
                onCheckedChange={() => togglePolicy(policy.id)}
              />
            </div>
            <p className="text-gray-500 mb-4">{policy.description}</p>
            <div className="flex items-center justify-between">
              <span className="text-gray-500">Price:</span>
              <span className="font-bold">₹{policy.price.toFixed(2)}</span>
            </div>
          </div>
        ))}
      </div>
      <div className="mt-8 bg-white rounded-lg shadow-md p-6">
        <h2 className="text-lg font-bold mb-4">Total</h2>
        <div className="flex items-center justify-between">
          <span className="text-gray-500">Total Price:</span>
          <span className="font-bold">₹{totalPrice.toFixed(2)}</span>
        </div>
        <Button
          onClick={() => navigate("/payment?path=cart")}
          className="mt-4 w-full"
        >
          Checkout
        </Button>
      </div>
    </div>
  );
}

export default WithLayout(Cart);
