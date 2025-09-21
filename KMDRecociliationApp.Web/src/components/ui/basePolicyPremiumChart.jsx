/* eslint-disable react/prop-types */
import { Trash2Icon } from "@/lib/icons";
import PropTypes from "prop-types";
import { Button } from "./button";
import RInput from "./rInput";
import { isTopUpPolicy, isAgeBandPremiumPolicy } from "@/lib/helperFunctions";
import { Combobox } from "@/components/ui/comboBox";
import { Label } from "@/components/ui/label";
import { Card, CardContent } from "@/components/ui/card";
import { Trash2 } from "lucide-react";
const PremiumInput = ({ label, value, onChange, error }) => (
  <div className="flex flex-col space-y-1 mr-5">
    <label className="text-sm font-medium">{label}</label>
    <input
      type="number"
      value={value || ""}
      onChange={onChange}
      className={`px-3 py-2 rounded-md border ${
        error
          ? "border-red-500 focus:ring-red-500"
          : "border-gray-300 focus:ring-blue-500"
      } focus:outline-none focus:ring-2`}
    />
    {error && <span className="text-red-500 text-xs">{error}</span>}
  </div>
);

export default function BasePolicyPremiumChart({
  product,
  policyType,
  ageBandPremiumRate,
  onChange,
  errors,
  onRemovePolicyOption,
}) {
  const isAgeBand = isAgeBandPremiumPolicy(policyType);
  return (
    <div className="space-y-6">
      {product?.premiumChart.map((option, index) => (
        <Card key={option.productPremiumId} className="relative">
          <CardContent className="pt-6">
            <div className="absolute top-4 right-4">
              <Button
                variant="ghost"
                size="icon"
                onClick={() => onRemovePolicyOption(index)}
                className="h-8 w-8 text-gray-500 hover:text-red-500"
              >
                <Trash2 className="h-4 w-4" />
              </Button>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-6">
              {isAgeBand && (
                <div className="col-span-full md:col-span-1">
                  <Label>
                    Age Band
                    <span className="text-red-500 ml-1">*</span>
                  </Label>
                  <Combobox
                    options={ageBandPremiumRate}
                    valueProperty="id"
                    labelProperty="name"
                    id="ageBandPremiumRateValue"
                    onChange={onChange("ageBandPremiumRateValue", index)}
                    value={option.ageBandPremiumRateValue}
                    error={
                      errors?.[`premiumChart[${index}].ageBandPremiumRateValue`]
                    }
                  />
                </div>
              )}

              <PremiumInput
                label="Sum Insured"
                value={option.sumInsured}
                onChange={onChange("sumInsured", index)}
                error={errors?.[`premiumChart[${index}].sumInsured`]}
                required
              />

              <PremiumInput
                label="Self Only"
                value={option.selfOnlyPremium}
                onChange={onChange("selfOnlyPremium", index)}
                error={errors?.[`premiumChart[${index}].selfOnlyPremium`]}
              />

              <PremiumInput
                label="Self + Spouse"
                value={option.selfSpousePremium}
                onChange={onChange("selfSpousePremium", index)}
                error={errors?.[`premiumChart[${index}].selfSpousePremium`]}
              />

              {isAgeBand && (
                <>
                  <PremiumInput
                    label="Self + Spouse + 1 Child"
                    value={option.selfSpouse1ChildrenPremium}
                    onChange={onChange("selfSpouse1ChildrenPremium", index)}
                    error={
                      errors?.[
                        `premiumChart[${index}].selfSpouse1ChildrenPremium`
                      ]
                    }
                  />

                  <PremiumInput
                    label="Self + Spouse + 2 Children"
                    value={option.selfSpouse2ChildrenPremium}
                    onChange={onChange("selfSpouse2ChildrenPremium", index)}
                    error={
                      errors?.[
                        `premiumChart[${index}].selfSpouse2ChildrenPremium`
                      ]
                    }
                  />

                  <PremiumInput
                    label="Self + 1 Child"
                    value={option.self1ChildrenPremium}
                    onChange={onChange("self1ChildrenPremium", index)}
                    error={
                      errors?.[`premiumChart[${index}].self1ChildrenPremium`]
                    }
                  />

                  <PremiumInput
                    label="Self + 2 Children"
                    value={option.self2ChildrenPremium}
                    onChange={onChange("self2ChildrenPremium", index)}
                    error={
                      errors?.[`premiumChart[${index}].self2ChildrenPremium`]
                    }
                  />
                </>
              )}
            </div>

            {/* Additional Coverage Sections */}
            {product?.isHandicappedChildrenCoverage && (
              <div className="mt-6 border-t pt-6">
                <h3 className="text-sm font-medium mb-4">
                  Handicapped Children Coverage
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-6">
                  {product.numberOfHandicappedChildren >= 1 && (
                    <PremiumInput
                      label="Child 1"
                      value={option.child1Premium}
                      onChange={onChange("child1Premium", index)}
                      error={errors?.[`premiumChart[${index}].child1Premium`]}
                    />
                  )}
                  {product.numberOfHandicappedChildren >= 2 && (
                    <PremiumInput
                      label="Child 2"
                      value={option.child2Premium}
                      onChange={onChange("child2Premium", index)}
                      error={errors?.[`premiumChart[${index}].child2Premium`]}
                    />
                  )}
                </div>
              </div>
            )}

            {product?.isParentsCoverage && (
              <div className="mt-6 border-t pt-6">
                <h3 className="text-sm font-medium mb-4">Parents Coverage</h3>
                <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-6">
                  {product.numberOfParents >= 1 && (
                    <PremiumInput
                      label="Parent 1"
                      value={option.parent1Premium}
                      onChange={onChange("parent1Premium", index)}
                      error={errors?.[`premiumChart[${index}].parent1Premium`]}
                    />
                  )}
                  {product.numberOfParents >= 2 && (
                    <PremiumInput
                      label="Parent 2"
                      value={option.parent2Premium}
                      onChange={onChange("parent2Premium", index)}
                      error={errors?.[`premiumChart[${index}].parent2Premium`]}
                    />
                  )}
                </div>
              </div>
            )}

            {product?.isInLawsCoverage && (
              <div className="mt-6 border-t pt-6">
                <h3 className="text-sm font-medium mb-4">In-Laws Coverage</h3>
                <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-6">
                  {product.numberOfInLaws >= 1 && (
                    <PremiumInput
                      label="In-Law 1"
                      value={option.inLaw1Premium}
                      onChange={onChange("inLaw1Premium", index)}
                      error={errors?.[`premiumChart[${index}].inLaw1Premium`]}
                    />
                  )}
                  {product.numberOfInLaws >= 2 && (
                    <PremiumInput
                      label="In-Law 2"
                      value={option.inLaw2Premium}
                      onChange={onChange("inLaw2Premium", index)}
                      error={errors?.[`premiumChart[${index}].inLaw2Premium`]}
                    />
                  )}
                </div>
              </div>
            )}
          </CardContent>
        </Card>
      ))}
    </div>
  );
}

BasePolicyPremiumChart.defaultProps = {
  product: [],
  onChange: () => {},
  onRemovePolicyOption: () => {},
  errors: {},
};

BasePolicyPremiumChart.propTypes = {
  product: PropTypes.object,
  onChange: PropTypes.func,
  onRemovePolicyOption: PropTypes.func,
  errors: PropTypes.object,
};
