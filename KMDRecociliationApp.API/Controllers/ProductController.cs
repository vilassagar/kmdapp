using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Data;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Results;
using Serilog;

using KMDRecociliationApp.Domain.Enum;
using FluentValidation;
using FluentValidation.Results;

using KMDRecociliationApp.Data.Helpers;
using KMDRecociliationApp.Domain.Common;
using Microsoft.EntityFrameworkCore;
using KMDRecociliationApp.Data.Common;

namespace KMDRecociliationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ApiBaseController
    {
        private readonly ProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductController> _logger;
        private readonly CommonHelperRepository _commonHelperRepository;
        private readonly IValidator<DTOProduct> _validator;
        public ProductController(ILogger<ProductController> logger, ApplicationDbContext context
            , ProductRepository productRepository, CommonHelperRepository commonHelperRepository
           , IValidator<DTOProduct> validator
            ) : base(context)
        {
            _logger = logger;
            _productRepository = productRepository;
            _context = context;
            _commonHelperRepository = commonHelperRepository;
            _validator = validator;
        }

        [HttpGet("")]
        public async Task<IActionResult> Getproductlist(
            [FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
        {

            if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
            {
                Sorting = new DataSorting() { SortName = "id", SortDirection = "desc" };
            }
            DataFilter<PolicyProductResult> filter = new DataFilter<PolicyProductResult>()
            {
                PageNumber = searchDTO.Page,
                Limit = searchDTO.pageSize,
                Filter = null,
                Search = searchDTO.Search,
                SortName = Sorting.SortName,
                SortDirection = Sorting.SortDirection
            };
            DataReturn<PolicyProductResult> nVReturn = new DataReturn<PolicyProductResult>();
            nVReturn = await _productRepository.GetAll(filter);
            return await Task.FromResult(Ok(nVReturn));

        }


        [HttpGet("GetBaseProductList")]
        public async Task<IActionResult> GetBaseProductList(
            [FromQuery] SearchDTO searchDTO, [FromQuery] DataSorting Sorting)
        {

            if (Sorting == null || string.IsNullOrWhiteSpace(Sorting.SortName))
            {
                Sorting = new DataSorting() { SortName = "id", SortDirection = "desc" };
            }
            DataFilter<PolicyProductResult> filter = new DataFilter<PolicyProductResult>()
            {
                PageNumber = searchDTO.Page,
                Limit = searchDTO.pageSize,
                Filter = null,
                Search = searchDTO.Search,
                SortName = Sorting.SortName,
                SortDirection = Sorting.SortDirection
            };
            DataReturn<PolicyProductResult> nVReturn = new DataReturn<PolicyProductResult>();
            nVReturn = await _productRepository.GetAllBasePolicy(filter);
            return await Task.FromResult(Ok(nVReturn));

        }

        private Product CreateProductObj(DTOProduct product)
        {
            var tempProduct = new Product();
            tempProduct.IsActive = true;
            tempProduct.ProductName = product.ProductName;
            tempProduct.Disclaimer = product.Disclaimer;
            tempProduct.ProviderName = product.ProviderName;
            tempProduct.PolicyTypeId = product.PolicyTypeId;
            tempProduct.BasePolicyId = product.BasePolicyId == 0 ? null : product.BasePolicyId;
            tempProduct.IsSpouseCoverage = product.IsSpouseCoverage;
            tempProduct.IsParentsCoverage = product.IsParentsCoverage;
            tempProduct.NumberOfParents = product.NumberOfParents;
            tempProduct.NumberOfHandicappedChildren = product.NumberOfHandicappedChildren;
            tempProduct.IsHandicappedChildrenCoverage = product.IsHandicappedChildrenCoverage;
            tempProduct.IsInLawsCoverage = product.IsInLawsCoverage;
            tempProduct.NumberOfInLaws = product.NumberOfInLaws;

            if (product.ProductId > 0)
                tempProduct.Id = product.ProductId;
            return tempProduct;
        }
        private ProductPremimum CreatePremimumChartObj(DTOProductPremimumChart premimum, int productId
            , int? ParentProductPremimumId, int productPremimumId, bool isupdate = false)
        {
            var premimumChart = new ProductPremimum();
            premimumChart.SumInsured = premimum.SumInsured;
            premimumChart.SpousePremium = premimum.SpousePremium;
            premimumChart.SelfSpousePremium = premimum.SelfSpousePremium;
            premimumChart.SelfSpouse2ChildrenPremium = premimum.SelfSpouse2ChildrenPremium;
            premimumChart.SelfSpouse1ChildrenPremium = premimum.SelfSpouse1ChildrenPremium;
            premimumChart.Self1ChildrenPremium = premimum.Self1ChildrenPremium;
            premimumChart.Self2ChildrenPremium = premimum.Self2ChildrenPremium;

            premimumChart.SelfOnlyPremium = premimum.SelfOnlyPremium;
            premimumChart.Child1Premium = premimum.Child1Premium;
            premimumChart.Child2Premium = premimum.Child2Premium;
            premimumChart.Parent1Premium = premimum.Parent1Premium;
            premimumChart.Parent2Premium = premimum.Parent2Premium;
            premimumChart.InLaw1Premium = premimum.InLaw1Premium;
            premimumChart.InLaw2Premium = premimum.InLaw2Premium;
            premimumChart.ProductId = productId;
            premimumChart.AgeBandPremiumRateId = premimum.AgeBandPremiumRateValue!=null?
                premimum.AgeBandPremiumRateValue.Id:0;
            premimumChart.ParentProductPremimumId =ParentProductPremimumId==0?null: ParentProductPremimumId;
            if (isupdate == false)

            {
                premimumChart.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                premimumChart.Id = productPremimumId;
            }
            premimumChart.UpdatedAt = DateTime.UtcNow;

            return premimumChart;
        }

        [HttpPost()]
        public async Task<IActionResult> PostCreateProduct([FromForm] DTOProduct dtoProduct)
        {
           
            var validationResult = _validator.Validate(dtoProduct);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            validationResult.Errors = new List<ValidationFailure>();

            if (dtoProduct.PolicyTypeId == (int)ProductPolicyType.TopUpPolicy
                && (dtoProduct.BasePolicyId == null || dtoProduct.BasePolicyId == 0))
            {
                validationResult.Errors.Add(new ValidationFailure("BasePolicyId", "Base Policy is requird"));
            }
            if (dtoProduct.PremiumChart == null || !dtoProduct.PremiumChart.Any())
            {
                validationResult.Errors.Add(new ValidationFailure("PremiumChart", "PremiumChart is requird"));
            }
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            _productRepository.CurrentUser = HttpContext.User;

            if (dtoProduct.ProductId == 0)
            {
                var temp = await _productRepository.CheckPolicyName(dtoProduct);

                if (temp == false)
                {

                    Product retproduct = CreateProductObj(dtoProduct);
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            retproduct.CreatedAt = DateTime.Now;
                            retproduct.UpdatedAt = DateTime.Now;
                            retproduct.CreatedBy = _productRepository.UserId;
                            retproduct.UpdatedBy = _productRepository.UserId;
                            retproduct = _productRepository.Add(retproduct);
                            // Ensure user.Id is not 0 and was inserted successfully
                            if (retproduct.Id == 0)
                            {
                                throw new Exception("product Insert failed.");
                            }

                            if (retproduct.Id > 0 && dtoProduct.PremiumChart != null &&
                                dtoProduct.PremiumChart.Count > 0)
                            {

                                foreach (var premimum in dtoProduct.PremiumChart)
                                {


                                     if(CommonHelper.CheckPolicyExists(dtoProduct.PolicyTypeId.Value))
                                    {
                                        var premimumChart = CreatePremimumChartObj(premimum, retproduct.Id, 0, premimum.ProductPremiumId, false);
                                        if (premimum.ProductPremiumId == 0)
                                        {
                                            premimumChart.CreatedAt = DateTime.Now;
                                            premimumChart.UpdatedAt = DateTime.Now;
                                            premimumChart.CreatedBy = _productRepository.UserId;
                                            premimumChart.UpdatedBy = _productRepository.UserId;
                                            await _context.ProductPremimumChart.AddAsync(premimumChart);
                                            await _context.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            premimumChart.UpdatedAt = DateTime.Now;
                                            premimumChart.UpdatedBy = _productRepository.UserId;
                                            _context.ProductPremimumChart.Update(premimumChart);
                                            _context.SaveChanges();
                                        }

                                    }
                                    else
                                    {

                                        var topups = premimum.TopUpOptions;
                                        if (topups != null && topups.Any())
                                        {
                                            foreach (var topupsitem in topups)
                                            {
                                                if (topupsitem.ProductPremiumId == 0)
                                                {
                                                    var topupspremimumChart = CreatePremimumChartObj(topupsitem, retproduct.Id
                                                        , ParentProductPremimumId: premimum.ParentProductPremiumId, 0, false);
                                                    topupspremimumChart.CreatedAt = DateTime.Now;
                                                    topupspremimumChart.UpdatedAt = DateTime.Now;
                                                    topupspremimumChart.CreatedBy =_productRepository.UserId;
                                                    topupspremimumChart.UpdatedBy = _productRepository.UserId;
                                                    await _context.ProductPremimumChart.AddAsync(topupspremimumChart);
                                                    await _context.SaveChangesAsync();
                                                }
                                                else
                                                {
                                                    var topupspremimumChart = CreatePremimumChartObj(topupsitem, retproduct.Id,
                                                        ParentProductPremimumId: premimum.ParentProductPremiumId, topupsitem.ProductPremiumId, false);
                                                    
                                                    topupspremimumChart.UpdatedAt = DateTime.Now;                                                  
                                                    topupspremimumChart.UpdatedBy = _productRepository.UserId;
                                                    _context.ProductPremimumChart.Update(topupspremimumChart);
                                                    await _context.SaveChangesAsync();
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Log.Error("Error" + ex.Message);
                        }
                    }

                    if (retproduct != null && retproduct.Id > 0)
                    {
                        if (dtoProduct.ProductDocument.File != null &&
                            dtoProduct.ProductDocument.File.Length > 0)
                        {
                            CommonFileModel commonFile = new CommonFileModel();
                            if (dtoProduct.ProductDocument != null && dtoProduct.ProductDocument.File.Length != 0)
                            {
                                commonFile =await new DataHelpers().UploadFile(dtoProduct.ProductDocument.File, DataHelpers.PRODUCTDOCUMENT, retproduct.Id);
                            }


                            if (retproduct != null && retproduct.Id > 0 && commonFile != null
                                && !string.IsNullOrWhiteSpace(commonFile.Name)
                                && !string.IsNullOrWhiteSpace(commonFile.Url))
                            {
                                retproduct.ProductDocumentName = commonFile.Name;
                                retproduct.ProductDocumentUrl = commonFile.Url;
                                _context.Product.Update(retproduct);
                                await _context.SaveChangesAsync();
                            }
                        }
                        return Ok(new { Message = "Product created successfully", ProductId = retproduct.Id });

                    }
                    else
                    {
                        return Problem("error while creating the product" );
                    }


                }
                else
                {
                    string error = $"Product Name: {dtoProduct.ProductName} already exists";
                    return Conflict(new { Message = error });
                }


            }
            return BadRequest();

        }

        [HttpPatch("{productid}")]
        public async Task<IActionResult> PatchUpdateProduct(int productid, [FromForm] DTOProduct dtoProduct)
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{

            var validationResult = _validator.Validate(dtoProduct);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            validationResult.Errors = new List<ValidationFailure>();

            if (dtoProduct.PolicyTypeId == (int)ProductPolicyType.TopUpPolicy
                && (dtoProduct.BasePolicyId == null || dtoProduct.BasePolicyId == 0))
            {
                validationResult.Errors.Add(new ValidationFailure("BasePolicyId", "Base Policy is requird"));
            }
            if (dtoProduct.PremiumChart == null || !dtoProduct.PremiumChart.Any())
            {
                validationResult.Errors.Add(new ValidationFailure("PremiumChart", "PremiumChart is requird"));
            }
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            List<string> messages = new List<string>();
            PolicyProductResult productResult = new PolicyProductResult();
            if (productid <= 0)
                return await Task.FromResult(BadRequest(productResult));

            _productRepository.CurrentUser = HttpContext.User;
   
            var existingData = await _productRepository.CheckPolicyName(dtoProduct, update: true);
            if (existingData == false)
            {
                Product retproduct = CreateProductObj(dtoProduct);
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
        
                        retproduct.UpdatedAt = DateTime.Now;                       
                        retproduct.UpdatedBy = _productRepository.UserId;
                        retproduct = _productRepository.Update(retproduct);

                        // Ensure user.Id is not 0 and was inserted successfully
                        if (retproduct.Id == 0)
                        {
                            throw new Exception("product updated failed.");
                        }

                        if (retproduct.Id > 0 && dtoProduct.PremiumChart != null &&
                            dtoProduct.PremiumChart.Count > 0)
                        {

                            foreach (var premimum in dtoProduct.PremiumChart)
                            {

                                if (CommonHelper.CheckPolicyExists(dtoProduct.PolicyTypeId.Value))
                                {
                                    var premimumChart = CreatePremimumChartObj(premimum, retproduct.Id, 0, premimum.ProductPremiumId, true);
                                    if (premimum.ProductPremiumId == 0)
                                    {
                                        premimumChart.UpdatedBy = _productRepository.UserId;
                                        premimumChart.UpdatedAt = DateTime.Now;
                                        premimumChart.CreatedBy = _productRepository.UserId;
                                        premimumChart.CreatedAt = DateTime.Now;

                                        await _context.ProductPremimumChart.AddAsync(premimumChart);
                                        await _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        premimumChart.UpdatedBy = _productRepository.UserId;
                                        premimumChart.UpdatedAt = DateTime.Now;
                                        _context.ProductPremimumChart.Update(premimumChart);
                                        _context.SaveChanges();
                                    }
                                }
                                else
                                {
                                    var topups = premimum.TopUpOptions;
                                    if (topups != null && topups.Any())
                                    {
                                        foreach (var topupsitem in topups)
                                        {
                                            if (topupsitem.ProductPremiumId == 0)
                                            {
                                                var topupspremimumChart = CreatePremimumChartObj(topupsitem, retproduct.Id
                                                    , ParentProductPremimumId: premimum.ParentProductPremiumId, 0, true);
                                                topupspremimumChart.UpdatedBy = _productRepository.UserId;
                                                topupspremimumChart.UpdatedAt=DateTime.Now;
                                                await _context.ProductPremimumChart.AddAsync(topupspremimumChart);
                                                await _context.SaveChangesAsync();
                                            }
                                            else
                                            {
                                                var topupspremimumChart = CreatePremimumChartObj(topupsitem, retproduct.Id,
                                                    ParentProductPremimumId: premimum.ParentProductPremiumId, topupsitem.ProductPremiumId, true);
                                                topupspremimumChart.UpdatedBy = _productRepository.UserId;
                                                topupspremimumChart.UpdatedAt = DateTime.Now;
                                                _context.ProductPremimumChart.Update(topupspremimumChart);
                                                await _context.SaveChangesAsync();
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Log.Error("Error" + ex.Message);
                    }
                }

                if (retproduct != null && retproduct.Id > 0)
                {
                    if (dtoProduct.IsProductDocumentUpdated == true && dtoProduct.ProductDocument.File != null &&
                            dtoProduct.ProductDocument.File.Length > 0)
                    {
                        CommonFileModel commonFile = new CommonFileModel();
                        if (dtoProduct.ProductDocument != null && dtoProduct.ProductDocument.File.Length != 0)
                        {
                            commonFile = await new DataHelpers().UploadFile(dtoProduct.ProductDocument.File, DataHelpers.PRODUCTDOCUMENT, retproduct.Id);
                        }


                        if (retproduct != null && retproduct.Id > 0 && commonFile != null
                            && !string.IsNullOrWhiteSpace(commonFile.Name)
                            && !string.IsNullOrWhiteSpace(commonFile.Url))
                        {
                            retproduct.ProductDocumentUrl = commonFile.Url;
                            retproduct.ProductDocumentName = commonFile.Name;
                            _context.Product.Update(retproduct);
                            await _context.SaveChangesAsync();
                        }
                    }

                    return Ok(new { Message = "Product updated successfully", ProductId = retproduct.Id });
                }
                else
                {
                    return Problem("error while creaing the product");
                }

            }
            else
            {
                string error = $"policy name {dtoProduct.ProductName}  already exists";
                messages.Add(error);
                return await Task.FromResult(Conflict(messages));
            }

        }

        [HttpGet("{productid:int}")]
        public async Task<IActionResult> Getbyid(int productid)
        {
           
            try
            {
                var roledetails = _productRepository.GetByID(productid).Result;
                return await Task.FromResult(Ok(roledetails));
            }   
            catch (Exception ex)
            {
                Log.Fatal($"Error in Getbyid {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
           
        }

        [HttpGet("GetBasePolicyList")]
        public async Task<IActionResult> GetPolicyList()
        {
            //if (Request.HttpContext.User.Identity.IsAuthenticated)
            //{
            try
            {
                return Ok(_productRepository.GetProductList());
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetProductList {ex.Message}");
                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }
            //}
            //return Unauthorized();
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
          
          
            try
            {
                var product = _context.Product.AsNoTracking().Where(x => x.Id == id).FirstOrDefault();
                if (product == null)
                    return NotFound(new { Message = "Product not found" });
                if(_productRepository.CheckProductIsUsed(id))
                    return Conflict(new { Message = "Product can not be deleted" });

                var productchart = _context.ProductPremimumChart.AsNoTracking()
                     .Where(x => x.ProductId == id).ToList();
                foreach (var item in productchart)
                {
                    item.IsActive = false;
                    item.UpdatedAt = DateTime.Now;                   
                    
                }
               // _context.ProductPremimumChart.Update(item);
                _context.SaveChanges();

                if (product != null)
                {
                    product.IsActive =false;
                    product.UpdatedAt = DateTime.Now;
                    _context.Product.Update(product);
                    _context.SaveChanges();
                }

                return Ok(new { Message="Product is deleted" });


            }
            catch (Exception ex)
            {

                return await Task.FromResult(Problem(detail: $"Something went wrong!"));
            }

           


        }

        [HttpGet("GetPolicyTypes")]
        public IActionResult GetPolicyTypes()
        {
            try
            {

                return Ok(_commonHelperRepository.GetPolicyTypes());
            }
            catch (Exception ex)
            {
                Log.Fatal($"Error in GetProductList {ex.Message}");
                return Problem(detail: $"Something went wrong!");
            }
        }
    }
}
