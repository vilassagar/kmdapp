import React, { useEffect, useState } from "react";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "./table";
import { getAssociationWisePayment } from "@/services/dashboard";
import { RPagination } from "./RPagination";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "./card";
import { userStore } from "@/lib/store";

const AssociationWisePaymentTable = () => {
  const user = userStore((state) => state.user);

  const associationId =
    user?.userType?.name?.toLowerCase()?.trim() === "association"
      ? user?.userId
      : 0;

  const [associations, setAssociations] = useState([]);
  const [paginationModel, setPaginationModel] = useState({
    numberOfPages: 10,
    pageNumber: 1,
    recordsPerPage: 50,
    nextPageNumber: 0,
    previousPageNumber: 0,
  });
  useEffect(() => {
    getPaginatedAssociationWisePaymentData(paginationModel);
  }, []);

  const getPaginatedAssociationWisePaymentData = async (paginationModel) => {
    const result = await getAssociationWisePayment(
      paginationModel.pageNumber,
      paginationModel.recordsPerPage,
      associationId
    );
    setAssociations(result.data);
    setPaginationModel(result.data.paging);
  };

  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedAssociationWisePaymentData();
  };

  return (
    <Card className="h-fit ">
      <CardHeader className="pb-0">
        <CardTitle>Association Wise Payment Status</CardTitle>
      </CardHeader>
      <CardContent className="overflow-y-auto h-72 rounded-lg shadow-lg p-4  ">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="min-w-[20px]">Association</TableHead>
              <TableHead className="min-w-[20px]">Initiated </TableHead>
              <TableHead className="min-w-[20px]">Completed </TableHead>
              <TableHead className="min-w-[20px]">Failed </TableHead>
              <TableHead className="min-w-[20px]">Rejected </TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {associations?.contents?.length ? (
              associations?.contents?.map((association, index) => (
                <TableRow key={association.id}>
                  <TableCell>{association.associationName}</TableCell>
                  <TableCell>{association.initiatedPayment}</TableCell>
                  <TableCell>{association.completedPayment}</TableCell>
                  <TableCell>{association.failedPayment}</TableCell>
                  <TableCell>{association.rejectedPayment}</TableCell>
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell colSpan={9} className="h-24 text-center">
                  No results.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </CardContent>
      <CardFooter className="flex justify-start">
        <RPagination
          paginationModel={paginationModel}
          onPageChange={handlePageChange}
        />
      </CardFooter>
    </Card>
  );
};

export default AssociationWisePaymentTable;
