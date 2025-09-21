import { PlusIcon, Trash2Icon } from "@/lib/icons";
import PropTypes from "prop-types";
import { Button } from "./button";
import RInput from "./rInput";

export default function TopUpPolicyPremiumChart({
  product,
  onChange,
  errors,
  onRemovePolicyOption,
  onTopupAdd,
}) {
  return (
    <div>
      {product?.premiumChart.map((option, index) => (
        <div key={option.productPremiumId} className="mb-5 flex items-end">
          {option.isBasePolicy && index !== 0 ? <hr className="my-10" /> : null}
          <div className="grid grid-cols-6 gap-5 items-end">
            <div className="mr-5">
              <RInput
                label="Sum Insured"
                value={option.sumInsured}
                onChange={onChange("sumInsured", index)}
                error={errors?.[`premiumChart[${index}].sumInsured`]}
                //  isDisabled={option.isBasePolicy}
              />
            </div>

            <div className="mr-5">
              <RInput
                label="Self Only Premium"
                value={option.selfOnlyPremium}
                onChange={onChange("selfOnlyPremium", index)}
                error={errors?.[`premiumChart[${index}].selfOnlyPremium`]}
                //isDisabled={option.isBasePolicy}
              />
            </div>
            <div className="mr-5">
              <RInput
                label="Self + Spouse Premium"
                value={option.selfSpousePremium}
                onChange={onChange("selfSpousePremium", index)}
                error={errors?.[`premiumChart[${index}].selfSpousePremium`]}
                // isDisabled={option.isBasePolicy}
              />
            </div>
            {option.isBasePolicy && product.isSpouseCoverage ? (
              <div>
                <RInput
                  label="Spouse Premium"
                  value={option.spousePremium}
                  onChange={onChange("spousePremium", index)}
                  error={errors?.[`premiumChart[${index}].spousePremium`]}
                  //isDisabled={option.isBasePolicy}
                />
              </div>
            ) : null}

            {product.numberOfHandicappedChildren >= 1 &&
            option.isBasePolicy &&
            option.child1Premium > 0 ? (
              <div className="mr-5">
                <RInput
                  label="Child 1 Premium"
                  value={option.child1Premium}
                  onChange={onChange("child1Premium", index)}
                  error={errors?.[`premiumChart[${index}].child1Premium`]}
                  isDisabled={option.isBasePolicy}
                />
              </div>
            ) : null}
            {product.numberOfHandicappedChildren >= 2 &&
            option.isBasePolicy &&
            option.child2Premium > 0 ? (
              <div>
                <RInput
                  label="Child 2 Premium"
                  value={option.child2Premium}
                  onChange={onChange("child2Premium", index)}
                  error={errors?.[`premiumChart[${index}].child2Premium`]}
                  isDisabled={option.isBasePolicy}
                />
              </div>
            ) : null}

            {product.numberOfParents >= 1 &&
            option.isBasePolicy &&
            option.parent1Premium > 0 ? (
              <div className="mr-5">
                <RInput
                  label="Parent 1 Premium"
                  value={option.parent1Premium}
                  onChange={onChange("parent1Premium", index)}
                  error={errors?.[`premiumChart[${index}].parent1Premium`]}
                  isDisabled={option.isBasePolicy}
                />
              </div>
            ) : null}
            {product.numberOfParents >= 2 &&
            option.isBasePolicy &&
            option.parent2Premium > 0 ? (
              <div>
                <RInput
                  label="Parent 2 Premium"
                  value={option.parent2Premium}
                  onChange={onChange("parent2Premium", index)}
                  error={errors?.[`premiumChart[${index}].parent2Premium`]}
                  isDisabled={option.isBasePolicy}
                />
              </div>
            ) : null}

            {product.numberOfInLaws >= 1 &&
            option.isBasePolicy &&
            option.inLaw1Premium > 0 ? (
              <div className="mr-5">
                <RInput
                  label="In-Law 1 Premium"
                  value={option.inLaw1Premium}
                  onChange={onChange("inLaw1Premium", index)}
                  error={errors?.[`premiumChart[${index}].inLaw1Premium`]}
                  isDisabled={option.isBasePolicy}
                />
              </div>
            ) : null}
            {product.numberOfInLaws >= 2 &&
            option.isBasePolicy &&
            option.inLaw2Premium > 0 ? (
              <div>
                <RInput
                  label="In-Law 2 Premium"
                  value={option.inLaw2Premium}
                  onChange={onChange("inLaw2Premium", index)}
                  error={errors?.[`premiumChart[${index}].inLaw2Premium`]}
                  isDisabled={option.isBasePolicy}
                />
              </div>
            ) : null}
          </div>

          {option.isBasePolicy ? (
            <Button
              variant="outline"
              onClick={onTopupAdd(index)}
              className="ml-5"
            >
              <PlusIcon className="h-4 w-4" />
            </Button>
          ) : (
            <Button variant="outline" onClick={onRemovePolicyOption(index)}>
              <Trash2Icon className="h-4 w-4" />
            </Button>
          )}
        </div>
      ))}
    </div>
  );
}

TopUpPolicyPremiumChart.defaultProps = {
  product: [],
  onChange: () => {},
  onRemovePolicyOption: () => {},
  errors: {},
  onTopupAdd: () => {},
};

TopUpPolicyPremiumChart.propTypes = {
  product: PropTypes.object,
  onChange: PropTypes.func,
  onRemovePolicyOption: PropTypes.func,
  errors: PropTypes.object,
  onTopupAdd: PropTypes.func,
};
