using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using KMDRecociliationApp.Data.Common;
using KMDRecociliationApp.Data.Mapper;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using KMDRecociliationApp.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq;
using KMDRecociliationApp.Data.Helpers;
namespace KMDRecociliationApp.Data.Repositories
{
    public class ProductRepository : MainHeaderRepo<Product>
    {
        ApplicationDbContext _context;
        public ProductRepository(
            ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }
        public async Task<DataReturn<PolicyProductResult>> GetAll(DataFilter<PolicyProductResult> filter)
        {
            var ret = new DataReturn<PolicyProductResult>();
            ret.Contents = new List<PolicyProductResult>();
            var objList = new List<Product>();
            try
            {
                int numberOfRecords = 0;
                var productquery = _context.Product.AsNoTracking()
                    .Include(x => x.BasePolicy)
                     .Include(p => p.ProductPremiums)
                     .ThenInclude(a=>a.AgeBandPremiumRate)
                    .Where(x => x.IsActive == true)
                     .AsQueryable()
                    ;

                if (filter.Search != null)
                {
                    objList = productquery.Search(filter.Search, "ProductName", "ProviderName").ToList();
                }
                else
                {
                    objList = new ObjectQuery<Product>().GetAllByFilter(filter.PageNumber, filter.Limit
                   , filter.SortName, filter.SortDirection
                   , filter.Filter == null ? null : filter.Filter.GetDelta()
                   , productquery, "Product", out numberOfRecords).ToList();
                }
                foreach (var product in objList)
                {
                    //if (product.PolicyTypeId == (int)ProductPolicyType.BasePolicy
                    //    || product.PolicyTypeId == (int)ProductPolicyType.OPD
                    //    || product.PolicyTypeId == (int)ProductPolicyType.Other ||
                    //    product.PolicyTypeId == (int)ProductPolicyType.PaymentProtectionPolicy||
                    //     product.PolicyTypeId == (int)ProductPolicyType.AgeBandPremium)
                    if(CommonHelper.CheckPolicyExists(product.PolicyTypeId.Value))
                    {
                        var x = new PolicyProductResult();
                        ret.Contents.Add(x.CopyPolicyData(product));
                    }
                    else
                    {
                        var baseProduct = await _context.Product.AsNoTracking()
                        .Include(x => x.ProductPremiums)
                        .Where(x => x.Id == product.BasePolicyId)
                        .FirstOrDefaultAsync();
                        if (baseProduct != null)
                        {

                            var x = new PolicyProductResult();
                            ret.Contents.Add(x.CopyTopupPolicyData(product, baseProduct));
                        }
                    }
                }

                ret.Source = "Product";
                ret.ResultCount = numberOfRecords;
                ret.StatusCode = 200;
                //Paging information
                int numberOfPages = (numberOfRecords / filter.Limit) + ((numberOfRecords % filter.Limit > 0) ? 1 : 0);
                DataPaging paging = new DataPaging();
                paging.RecordsPerPage = filter.Limit;
                paging.PageNumber = filter.PageNumber;
                paging.NumberOfPages = numberOfPages;
                if (filter.PageNumber > 1)
                    paging.PreviousPageNumber = filter.PageNumber - 1;
                if (numberOfPages > filter.PageNumber)
                    paging.NextPageNumber = filter.PageNumber + 1;
                ret.Paging = paging;
                DataSorting sorting = new DataSorting();
                sorting.SortName = filter.SortName;
                sorting.SortDirection = filter.SortDirection;
                ret.Sorting = sorting;
            }
            catch (Exception ex)
            {
                Log.Error("error " + ex.Message);
            }
            return ret;
        }
        public async Task<DataReturn<PolicyProductResult>> GetAllBasePolicy(DataFilter<PolicyProductResult> filter)
        {
            var ret = new DataReturn<PolicyProductResult>();
            ret.Contents = new List<PolicyProductResult>();
            var objList = new List<Product>();
            try
            {
                int numberOfRecords = 0;
                var productquery = _context.Product.AsNoTracking()
                    .Where(x => x.IsActive == true
                    && x.PolicyTypeId == (int)ProductPolicyType.BasePolicy ||
                    x.PolicyTypeId == (int)ProductPolicyType.OPD ||
                    x.PolicyTypeId == (int)ProductPolicyType.Other ||
                    x.PolicyTypeId == (int)ProductPolicyType.PaymentProtectionPolicy
                    || x.PolicyTypeId == (int)ProductPolicyType.AgeBandPremium)
                     .AsQueryable();
                //.Include(x => x.BasePolicy)
                //.Include(p => p.ProductPremiums)
                //.Where(x => x.IsActive == true
                //&&
                //.Where(x =>
                //x.PolicyType==ProductPolicyType.BasePolicy)



                if (filter.Search != null)
                {
                    objList = productquery.Search(filter.Search, "PermissionType", "Description").ToList();
                }
                else
                {
                    objList = new ObjectQuery<Product>().GetAllByFilter(filter.PageNumber, filter.Limit
                   , filter.SortName, filter.SortDirection
                   , filter.Filter == null ? null : filter.Filter.GetDelta()
                   , productquery, "Product", out numberOfRecords).ToList();
                }
                foreach (var product in objList)
                {
                    //if (product.PolicyTypeId == (int)ProductPolicyType.BasePolicy ||
                    //    product.PolicyTypeId == (int)ProductPolicyType.OPD ||
                    //    product.PolicyTypeId == (int)ProductPolicyType.Other ||
                    //    product.PolicyTypeId == (int)ProductPolicyType.PaymentProtectionPolicy|| 
                    //    product.PolicyTypeId == (int)ProductPolicyType.AgeBandPremium)
                    if(CommonHelper.CheckPolicyExists(product.PolicyTypeId.Value))
                    {
                        var x = new PolicyProductResult();
                        ret.Contents.Add(x.CopyPolicyData(product));
                    }
                    //else
                    //{
                    //    var baseProduct = await _context.Product.AsNoTracking()
                    //    .Include(x => x.ProductPremiums)
                    //    .Where(x => x.Id == product.BasePolicyId)
                    //    .FirstOrDefaultAsync();
                    //    if (baseProduct != null)
                    //    {

                    //        var x = new PolicyProductResult();
                    //        ret.Contents.Add(x.CopyTopupPolicy(product, baseProduct));
                    //    }

                    //}
                }

                ret.Source = "Product";
                ret.ResultCount = numberOfRecords;
                ret.StatusCode = 200;
                //Paging information
                int numberOfPages = (numberOfRecords / filter.Limit) + ((numberOfRecords % filter.Limit > 0) ? 1 : 0);
                DataPaging paging = new DataPaging();
                paging.RecordsPerPage = filter.Limit;
                paging.PageNumber = filter.PageNumber;
                paging.NumberOfPages = numberOfPages;
                if (filter.PageNumber > 1)
                    paging.PreviousPageNumber = filter.PageNumber - 1;
                if (numberOfPages > filter.PageNumber)
                    paging.NextPageNumber = filter.PageNumber + 1;
                ret.Paging = paging;
                DataSorting sorting = new DataSorting();
                sorting.SortName = filter.SortName;
                sorting.SortDirection = filter.SortDirection;
                ret.Sorting = sorting;
            }
            catch (Exception ex)
            {
                Log.Error("error " + ex.Message);
            }
            return ret;
        }
        public static bool IsDateInRange(DateTime currentDate, DateTime startDate, DateTime endDate)
        {
            // Normalize all dates to start of day to ignore time component
            currentDate = currentDate.Date;
            startDate = startDate.Date;
            endDate = endDate.Date;

            // Check if current date is between start and end dates (inclusive)
            return !(currentDate >= startDate && currentDate <= endDate);
        }
        public List<AgeBandPremiumRate> GetAgeBandPremiumRateId(int age,int userId)
        {
            var ageBandPremium = (from p in _context.CampaignProducts
                                  join a in _context.CampaignAssociations
                                  on p.CampaignId equals a.CampaignId
                                  join pp in _context.ProductPremimumChart
                                  on p.ProductId equals pp.ProductId
                                  join ab in _context.AgeBandPremiumRate.AsNoTracking()
                                  on pp.AgeBandPremiumRateId equals ab.Id
                                  where ab.AgeBandStart <= age && ab.AgeBandEnd >= age
                                  select ab).ToList();

                   // .Where(x => x.AgeBandStart <=age && x.AgeBandEnd >= age).FirstOrDefault();
            
            return ageBandPremium;
        }
        public  DTOPolicy GetProductListByUser(int userId,int age)
        {
            var ret = new DTOPolicy();
            try
            {
                var currentDate = DateTime.Now.Date;
                var compaignProducts = (from c in _context.Campaigns.AsNoTracking()
                                        join cp in _context.CampaignProducts.AsNoTracking()
                                        on c.Id equals cp.CampaignId
                                        join p in _context.Product
                                        on cp.ProductId equals p.Id
                                        join ca in _context.CampaignAssociations.AsNoTracking()
                                        on c.Id equals ca.CampaignId
                                        join u in _context.Applicationuser.AsNoTracking()
                                        on ca.AssociationId equals u.AssociationId
                                        where u.Id == userId
                                        && (p.PolicyTypeId == (int)ProductPolicyType.BasePolicy ||
                                        p.PolicyTypeId == (int)ProductPolicyType.OPD ||
                                        p.PolicyTypeId == (int)ProductPolicyType.Other ||
                                         p.PolicyTypeId == (int)ProductPolicyType.AgeBandPremium ||
                                        p.PolicyTypeId == (int)ProductPolicyType.PaymentProtectionPolicy
                                        )

                                        && ((DateTime.Now.Date >= c.StartDate.Value.Date ||
                                         DateTime.Now.Date <= c.StartDate.Value.Date)
                                         && DateTime.Now.Date <= c.EndDate.Value.Date)
                                        select new
                                        {
                                            Campaign = new
                                            {
                                                c.Id,
                                                c.StartDate,
                                                c.EndDate
                                            },
                                            CampaignProduct = new
                                            {
                                                cp.ProductId,
                                                cp.CampaignId
                                            },
                                            Product = new
                                            {
                                                p.Id,
                                                p.PolicyTypeId,
                                                p.BasePolicyId
                                            },
                                            
                                        })
                .Distinct()
                .AsNoTracking()
                .ToList();

                if (compaignProducts != null && compaignProducts.Any())
                {
                 
                    
                    int ageBandPremiumRateId = 0;
                    string ageBandPremiumRateValue = null;
                    List<AgeBandPremiumRate> ageBandPremiumRate = null;
                    ret.Products = new List<DTOPolicyProduct>();
                    foreach (var p in compaignProducts)
                    {

                        if (!ret.Products.Any(x => x.ProductId == p.CampaignProduct.ProductId))
                        {
                            var policyproduct = (from p1 in _context.PolicyProductDetails.AsNoTracking()
                                                 join cp in _context.PolicyHeader.AsNoTracking()
                                                  on p1.PolicyHeaderId equals cp.Id
                                                 where cp.UserId == userId &&
                                                 p1.ProductId == p.CampaignProduct.ProductId
                                                 && p.CampaignProduct.CampaignId == cp.CampaignId
                                                 select p1).ToList();
                            if (p.Product.PolicyTypeId == (int)ProductPolicyType.AgeBandPremium)
                            {
                                 ageBandPremiumRate = GetAgeBandPremiumRateId(age, userId);
                                //if(ageBandPremiumRate!=null)
                                //{
                                //    ageBandPremiumRateValue = ageBandPremiumRate.AgeBandValue;
                                //    ageBandPremiumRateId = ageBandPremiumRate.Id;
                                //}
                                
                            }
                            if (policyproduct != null && policyproduct.Count() == 0)
                            {

                                var item = (_context.Product.AsNoTracking()
                                    .Include(x => x.BasePolicy)
                                     .Include(p => p.ProductPremiums)                                    
                                    .Where(x => x.Id == p.CampaignProduct.ProductId
                                    ))
                                    .FirstOrDefault();




                                if (item != null)
                                {
                                    DTOPolicy dTOPolicyProduct = new DTOPolicy();

                                    if (CommonHelper.CheckPolicyExists(item.PolicyTypeId.Value))
                                    {
                                        var topupProduct = _context.Product.AsNoTracking()
                                               .Include(x => x.ProductPremiums)
                                               .Where(x => x.BasePolicyId == item.Id)
                                               .FirstOrDefault();

                                        var policyProductdata = new DTOPolicyProduct().CopyBasePolicy(item, topupProduct, ageBandPremiumRate);
                                            //, ageBandPremiumRateId, ageBandPremiumRateValue);
                                        policyProductdata.CampaignId = p.CampaignProduct.CampaignId;
                                        policyProductdata.IsCampaignExpired = IsDateInRange(DateTime.Now, p.Campaign.StartDate.Value, p.Campaign.EndDate.Value);
                                        policyProductdata.AgeBandPremiumRateValue = ageBandPremiumRateValue;
                                        ret.Products.Add(policyProductdata);

                                    }
                                    else
                                    {
                                        var baseProduct = _context.Product.AsNoTracking()
                                                .Include(x => x.ProductPremiums)
                                                .Where(x => x.Id == item.BasePolicyId)
                                                .FirstOrDefault();
                                        if (baseProduct != null)
                                        {
                                            var policyProductdatabase = new DTOPolicyProduct().CopyTopupPolicy(item, baseProduct);
                                            policyProductdatabase.CampaignId = p.CampaignProduct.CampaignId;
                                            policyProductdatabase.IsCampaignExpired = IsDateInRange(DateTime.Now, p.Campaign.StartDate.Value, p.Campaign.EndDate.Value);
                                            ret.Products.Add(policyProductdatabase);
                                        }
                                    }


                                }

                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error("error " + ex.Message);
            }
            return ret;
        }

    
        public async Task<DTOProduct> GetByID(int id)
        {
            DTOProduct dTOProduct = new DTOProduct();
            var product = await _context.Product.AsNoTracking()
                .Include(x => x.ProductPremiums)               
                .Include(x => x.BasePolicy)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
                return dTOProduct;
            //if (product.PolicyTypeId == (int)ProductPolicyType.BasePolicy ||
            //    product.PolicyTypeId == (int)ProductPolicyType.OPD ||
            //    product.PolicyTypeId == (int)ProductPolicyType.Other ||
            //    product.PolicyTypeId == (int)ProductPolicyType.PaymentProtectionPolicy)
            if(CommonHelper.CheckPolicyExists(product.PolicyTypeId.Value))
            {
                var objAgeband = _context.AgeBandPremiumRate.ToList();
                return dTOProduct.CopyBasePolicy(product, objAgeband);
            }
            else
            {

                var baseProduct = await _context.Product.AsNoTracking()
                .Include(x => x.ProductPremiums)
                //.ThenInclude(x => x.AgeBandPremiumRate)
                .Where(x => x.Id == product.BasePolicyId)
                .FirstOrDefaultAsync();
                if (baseProduct != null)
                    return dTOProduct.CopyTopupPolicy(product, baseProduct);
                else
                { return dTOProduct; }
            }

            //return await _context.Product.AsNoTracking().Where(x => x.Id == id)
            //    .AsNoTracking().FirstOrDefaultAsync();
        }
        public List<CommonNameDTO> GetProductList()
        {


            var list = _context.Product
                .Where(p => p.PolicyTypeId == (int)ProductPolicyType.BasePolicy ||
                p.PolicyTypeId == (int)ProductPolicyType.OPD ||
                p.PolicyTypeId == (int)ProductPolicyType.Other ||
                p.PolicyTypeId == (int)ProductPolicyType.PaymentProtectionPolicy) // Filter base products
            .Where(p => !_context.Product
            .Any(topUp => topUp.BasePolicyId == p.Id)) // Ensure no top-up products are linked
                .ToList();
            //var list = _context.Product.AsNoTracking().ToList();
            return list.Select(CommonMapper.ToProductDto).ToList();
        }

        public async Task<bool> CheckPolicyName(DTOProduct policyProduct, bool update = false)
        {
            if (update)
            {
                return await _context.Product.AsNoTracking()
                    .Where(x => x.IsActive == true)
                .AnyAsync(x => x.ProductName == policyProduct.ProductName
                && x.Id != policyProduct.ProductId
                );
            }
            else
            {
                return await _context.Product.AsNoTracking()
                    .Where(x => x.IsActive == true)
                .AnyAsync(x => x.ProductName == policyProduct.ProductName);

            }
        }

        public bool CheckProductIsUsed(int productId)
        {
            var item = _context.CampaignProducts.AsNoTracking().Where(x => x.ProductId == productId);

            if (item.Any())
                return true;

            var item3 = _context.PolicyProductDetails.AsNoTracking().Where(x => x.ProductId == productId);
            if (item3.Any())
                return true;

            return false;
        }

    }
}
