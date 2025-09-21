import DownloadIcon from "@/assets/DownloadIcon";
import WithAuthentication from "@/components/hoc/withAuthentication";
import WithPermission from "@/components/hoc/withPermissions";
import WithLayout from "@/components/layout/WithLayout";
import { RPagination } from "@/components/ui/RPagination";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { ConfirmDialog } from "@/components/ui/confirmDialog";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import NoData from "@/components/ui/noData";
import RInput from "@/components/ui/rInput";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { toast } from "@/components/ui/use-toast";
import { downloadFile } from "@/lib/helperFunctions";
import { EyeIcon } from "@/lib/icons";
import { usePermissionStore } from "@/lib/store";
import { download } from "@/services/files";
import { deleteProduct, getProductList } from "@/services/product";
import { CirclePlus, FilePenIcon, Trash2Icon } from "lucide-react";
import { Fragment, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const PAGE_NAME = "createproduct";

function CreateProductList() {
  const permissions = usePermissionStore((state) => state.permissions);

  const [searchTerm, setSearchTerm] = useState("");
  const [paginationModel, setPaginationModel] = useState({
    pageNumber: 1,
    recordsPerPage: 50,
  });
  const [showDialog, setShowDialog] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [products, setProducts] = useState([]);
  const [productIndex, setProductIndex] = useState(0);

  const navigate = useNavigate();

  useEffect(() => {
    (async () => {
      await getPaginatedProducts(paginationModel, searchTerm);
    })();
  }, []);

  const getPaginatedProducts = async (paginationModel, searchTerm) => {
    let response = await getProductList(
      paginationModel.pageNumber,
      paginationModel.recordsPerPage,
      searchTerm
    );

    if (response.status === "success") {
      setProducts(response.data);
      setPaginationModel(response.data.paging);
    } else {
      //show error
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to get products.",
      });
    }
  };

  const handleSearch = async (event) => {
    let pageModel = {
      pageNumber: 1,
      recordsPerPage: 50,
    };

    setPaginationModel(pageModel);
    setSearchTerm(event.target.value);
    await getPaginatedProducts(pageModel, event.target.value);
  };

  const handleEdit = (index) => (event) => {
    let productId = products?.contents[index]["productId"];
    navigate(`/createproduct?mode=edit&productId=${productId}`);
  };

  const handleViewClick = (index) => {
    setSelectedProduct(products?.contents[index]);
    setShowDialog(true);
  };

  const handleDeleteProduct = async (event) => {
    let productId = products?.contents[productIndex]["productId"];
    let response = await deleteProduct(productId);
    if (response.status === "success") {
      //show success message
      let pageModel = {
        page: 1,
        pageSize: 50,
      };

      setPaginationModel(pageModel);
      await getPaginatedProducts(paginationModel, searchTerm);
    } else {
      //show error message
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to delete product.",
      });
    }
  };

  const handleDownloadProduct = (index) => async (event) => {
    let productDocument = products?.contents[index]["productDocument"];
    let { id, name, url } = productDocument;
    let response = await download(id, name, url);
    if (Response.status === "success") {
      url = URL.createObjectURL(new Blob([response.data]));
      downloadFile(url, name);
    } else {
      toast({
        variant: "destructive",
        title: "Something went wrong.",
        description: "Unable to download file.",
      });
    }
  };

  const handleCloseDialog = () => {
    setShowDialog(false);
    setSelectedProduct(null);
  };

  const handlePageChange = async (paging) => {
    setPaginationModel(paging);
    await getPaginatedProducts(paging, searchTerm);
  };

  return (
    <div>
      <div className="w-full max-w-6xl ">
        <h1 className="text-2xl font-bold mb-6">Products</h1>
        <div className="mb-6 flex justify-between">
          <RInput
            type="search"
            placeholder="Search Products..."
            value={searchTerm}
            onChange={handleSearch}
          />
          {permissions?.[PAGE_NAME]?.create ? (
            <Button
              onClick={() => {
                navigate("/createproduct?mode=new");
              }}
              className="ml-10"
            >
              Create Product
              <CirclePlus className="ml-2 h-4 w-4" />
            </Button>
          ) : null}
        </div>

        {products?.contents?.length ? (
          <div className="grid grid-cols-1 sm:grid-cols-2  lg:grid-cols-2 gap-6">
            {products?.contents?.map((product, index) => (
              <Card
                key={product.id}
                className="h-full relative pt-5 shadow-xl border-0"
              >
                <CardContent className="flex flex-col justify-between h-full">
                  <div>
                    <h3 className="text-lg font-semibold mr-3">
                      {product.productName}
                    </h3>

                    <TooltipProvider>
                      <Tooltip>
                        <TooltipTrigger asChild>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="absolute top-2 right-2 rounded-full border border-gray-200 w-8 h-8 dark:border-gray-800 mt-3"
                            onClick={() => handleViewClick(index)}
                          >
                            <EyeIcon className="w-4 h-4" />
                            <span className="sr-only">
                              View {product.productName}
                            </span>
                          </Button>
                        </TooltipTrigger>
                        <TooltipContent>
                          <p>View Premium Chart</p>
                        </TooltipContent>
                      </Tooltip>
                    </TooltipProvider>

                    <div className="flex items-center justify-between">
                      <div>
                        <p className="text-sm text-gray-600 font-bold mt-2">
                          {product.providerName}
                        </p>
                        <p className="text-sm text-gray-500 mt-4">
                          {product.policyType.toLowerCase().trim() ===
                            "topuppolicy" && product.basePolicy
                            ? `Top Up Policy (Linked to ${product.basePolicy})`
                            : `Policy Type: ${product.policyType}`}
                        </p>
                      </div>
                    </div>
                  </div>
                  <div className="flex justify-end">
                    {product?.productDocument?.url?.length ? (
                      <Button
                        variant="ghost"
                        className="flex items-center gap-2"
                        onClick={handleDownloadProduct(index)}
                        size="sm"
                      >
                        <DownloadIcon className="h-4 w-4" />
                      </Button>
                    ) : null}

                    {permissions?.[PAGE_NAME]?.update ? (
                      <Button
                        variant="ghost"
                        className="flex items-center gap-2"
                        onClick={handleEdit(index)}
                        size="sm"
                      >
                        <FilePenIcon className="h-4 w-4" />
                      </Button>
                    ) : null}
                    {permissions?.[PAGE_NAME]?.delete ? (
                      <ConfirmDialog
                        dialogTrigger={
                          <Button
                            variant="ghost"
                            className="flex items-center gap-2 text-red-600"
                            onClick={(event) => {
                              setProductIndex(index);
                            }}
                            size="sm"
                          >
                            <Trash2Icon className="h-4 w-4" />
                          </Button>
                        }
                        onConfirm={handleDeleteProduct}
                        dialogTitle="Are you sure to delete the product?"
                        dialogDescription="This action cannot be undone. This will permanently delete your
            product and remove your data from our servers."
                      />
                    ) : null}
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        ) : (
          <NoData />
        )}
      </div>

      <div className="flex justify-end">
        <RPagination
          paginationModel={paginationModel}
          onPageChange={handlePageChange}
        />
      </div>

      <Dialog open={showDialog} onOpenChange={handleCloseDialog}>
        <DialogContent className="w-dvw max-w-[900px] max-h-[600px] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>{selectedProduct?.productName}</DialogTitle>
            <DialogDescription>Premium Chart</DialogDescription>
          </DialogHeader>
          <div className="rounded-lg shadow-lg">
            <Table size="sm" className="p-1">
              {" "}
              {/* Added minimal padding to table */}
              <TableHeader>
                <TableRow className="py-1 px-2">
                  {" "}
                  {selectedProduct?.premiumChart[0]
                    ?.ageBandPremiumRateValue && (
                    <TableHead className="p-1 text-xs">Age Band</TableHead>
                  )}
                  {/* Reduced row padding */}
                  <TableHead className="p-1 text-xs">
                    Sum Insured
                  </TableHead>{" "}
                  {/* Reduced padding, smaller text */}
                  <TableHead className="p-1 text-xs">
                    Self Only Premium
                  </TableHead>
                  <TableHead className="p-1 text-xs">
                    Self + Spouse Premium
                  </TableHead>
                  {selectedProduct?.premiumChart[0]
                    ?.selfSpouse1ChildrenPremium > 0 ? (
                    <TableHead className="p-1 text-xs">
                      Self + Spouse + 1 Child
                    </TableHead>
                  ) : null}
                  {selectedProduct?.premiumChart[0]
                    ?.selfSpouse2ChildrenPremium > 0 ? (
                    <TableHead className="p-1 text-xs">
                      Self + Spouse + 2 Children
                    </TableHead>
                  ) : null}
                  {selectedProduct?.premiumChart[0]?.self1ChildrenPremium >
                  0 ? (
                    <TableHead className="p-1 text-xs">
                      Self + 1 Child
                    </TableHead>
                  ) : null}
                  {selectedProduct?.premiumChart[0]?.self2ChildrenPremium >
                  0 ? (
                    <TableHead className="p-1 text-xs">
                      Self + 2 Children
                    </TableHead>
                  ) : null}
                  {selectedProduct?.premiumChart[0]?.child1Premium > 0 ? (
                    <TableHead className="p-1 text-xs">
                      Child 1 Premium
                    </TableHead>
                  ) : null}
                  {selectedProduct?.premiumChart[0]?.child2Premium > 0 ? (
                    <TableHead className="p-1 text-xs">
                      Child 2 Premium
                    </TableHead>
                  ) : null}
                </TableRow>
              </TableHeader>
              <TableBody>
                {selectedProduct?.premiumChart?.map((premium) => (
                  <Fragment key={premium.productPremiumId}>
                    <TableRow className="py-1 px-2">
                      {" "}
                      {/* Reduced row padding */}
                      {premium?.ageBandPremiumRateValue && (
                        <TableHead className="p-1 text-xs">
                          {premium?.ageBandPremiumRateValue}
                        </TableHead>
                      )}
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
                      {premium?.self1ChildrenPremium > 0 ? (
                        <TableCell className="p-1 text-sm">
                          {premium.self1ChildrenPremium}
                        </TableCell>
                      ) : null}
                      {premium?.self2ChildrenPremium > 0 ? (
                        <TableCell className="p-1 text-sm">
                          {premium.self2ChildrenPremium}
                        </TableCell>
                      ) : null}
                      {premium?.child1Premium > 0 ? (
                        <TableCell className="p-1 text-sm">
                          {premium.child1Premium}
                        </TableCell>
                      ) : null}
                      {premium?.child1Premium > 0 ? (
                        <TableCell className="p-1 text-sm">
                          {premium.child2Premium}
                        </TableCell>
                      ) : null}
                    </TableRow>
                    {premium?.topUpOptions?.length ? (
                      <TableRow>
                        <TableCell className="font-bold p-1 text-sm">
                          Top up
                        </TableCell>
                      </TableRow>
                    ) : null}
                    {premium.topUpOptions?.map((topUp) => (
                      <TableRow
                        key={topUp.productPremiumId}
                        className="bg-muted py-1 px-2" // Reduced padding
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
                      </TableRow>
                    ))}
                  </Fragment>
                ))}
              </TableBody>
            </Table>
          </div>
          <DialogFooter>
            <Button onClick={handleCloseDialog}>Close</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}

export default WithAuthentication(
  WithPermission(PAGE_NAME)(WithLayout(CreateProductList))
);
