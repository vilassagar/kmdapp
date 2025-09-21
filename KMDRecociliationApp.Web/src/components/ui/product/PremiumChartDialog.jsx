/* eslint-disable react/prop-types */
import React from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Button } from "@/components/ui/button";

const PremiumChartDialog = ({
  showDialog,
  selectedProduct,
  handleCloseDialog,
}) => {
  // Helper function to check if child premiums exist
  const hasChildPremiums =
    selectedProduct?.premiumChart?.[0]?.child1Premium > 0 ||
    selectedProduct?.premiumChart?.[0]?.child2Premium > 0;

  return (
    <Dialog open={showDialog} onOpenChange={handleCloseDialog}>
      <DialogContent className="w-dvw max-w-[900px] max-h-[600px] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{selectedProduct?.productName}</DialogTitle>
          <DialogDescription>Premium Chart</DialogDescription>
        </DialogHeader>

        <div className="rounded-lg shadow-lg">
          <Table size="sm" className="p-1">
            <TableHeader>
              <TableRow className="py-1 px-2">
                <TableHead className="p-1 text-xs">Sum Insured</TableHead>
                <TableHead className="p-1 text-xs">Self Only Premium</TableHead>
                <TableHead className="p-1 text-xs">
                  Self + Spouse Premium
                </TableHead>
                {selectedProduct?.premiumChart[0]?.selfSpouse1ChildrenPremium >
                0 ? (
                  <TableHead className="p-1 text-xs">
                    Self + Spouse + 1 Child
                  </TableHead>
                ) : null}
                {selectedProduct?.premiumChart[0]?.selfSpouse2ChildrenPremium >
                0 ? (
                  <TableHead className="p-1 text-xs">
                    Self + Spouse + 2 Children
                  </TableHead>
                ) : null}
                {selectedProduct?.premiumChart[0]?.self1ChildrenPremium > 0 ? (
                  <TableHead className="p-1 text-xs">Self + 1 Child</TableHead>
                ) : null}
                {selectedProduct?.premiumChart[0]?.self2ChildrenPremium > 0 ? (
                  <TableHead className="p-1 text-xs">
                    Self + 2 Children
                  </TableHead>
                ) : null}

                {selectedProduct?.premiumChart?.[0]?.child1Premium > 0 && (
                  <TableHead className="p-1 text-xs">Child 1 Premium</TableHead>
                )}
                {selectedProduct?.premiumChart?.[0]?.child2Premium > 0 && (
                  <TableHead className="p-1 text-xs">Child 2 Premium</TableHead>
                )}
              </TableRow>
            </TableHeader>
            <TableBody>
              {selectedProduct?.premiumChart?.map((premium) => (
                <React.Fragment key={premium.productPremiumId}>
                  {/* Base Premium Row */}
                  <TableRow className="py-1 px-2">
                    <TableCell className="p-1 text-sm">
                      {premium.sumInsured}
                    </TableCell>
                    <TableCell className="p-1 text-sm">
                      {premium.selfOnlyPremium}
                    </TableCell>
                    <TableCell className="p-1 text-sm">
                      {premium.selfSpousePremium}
                    </TableCell>
                    <TableCell className="p-1 text-sm">
                      {premium.selfSpouse1ChildrenPremium}
                    </TableCell>
                    <TableCell className="p-1 text-sm">
                      {premium.selfSpouse2ChildrenPremium}
                    </TableCell>
                    <TableCell className="p-1 text-sm">
                      {premium.self1ChildrenPremium}
                    </TableCell>
                    <TableCell className="p-1 text-sm">
                      {premium.self2ChildrenPremium}
                    </TableCell>
                    {premium?.child1Premium > 0 && (
                      <TableCell className="p-1 text-sm">
                        {premium.child1Premium}
                      </TableCell>
                    )}
                    {premium?.child2Premium > 0 && (
                      <TableCell className="p-1 text-sm">
                        {premium.child2Premium}
                      </TableCell>
                    )}
                  </TableRow>

                  {/* Top Up Section */}
                  {premium?.topUpOptions?.length > 0 && (
                    <>
                      <TableRow>
                        <TableCell
                          colSpan={hasChildPremiums ? 5 : 3}
                          className="p-1 text-sm font-bold"
                        >
                          Top up
                        </TableCell>
                      </TableRow>
                      {premium.topUpOptions.map((topUp) => (
                        <TableRow
                          key={topUp.productPremiumId}
                          className="bg-muted py-1 px-2"
                        >
                          <TableCell className="p-1 text-sm">
                            {topUp.sumInsured}
                          </TableCell>
                          <TableCell className="p-1 text-sm">
                            {topUp.selfOnlyPremium}
                          </TableCell>
                          <TableCell className="p-1 text-sm">
                            {topUp.selfSpousePremium}
                          </TableCell>
                          {hasChildPremiums && (
                            <>
                              <TableCell className="p-1 text-sm">-</TableCell>
                              <TableCell className="p-1 text-sm">-</TableCell>
                            </>
                          )}
                        </TableRow>
                      ))}
                    </>
                  )}
                </React.Fragment>
              ))}
            </TableBody>
          </Table>
        </div>

        <DialogFooter>
          <Button onClick={handleCloseDialog}>Close</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default PremiumChartDialog;
