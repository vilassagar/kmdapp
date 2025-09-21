using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using KMDRecociliationApp.Data.Common;
using KMDRecociliationApp.Data.Helpers;
using KMDRecociliationApp.Domain.Common;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using KMDRecociliationApp.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Serilog;


namespace KMDRecociliationApp.Data.Repositories
{
    public class UserRepository : MainHeaderRepo<ApplicationUser>
    {
        ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
       : base(context)
        {
            _context = context;
        }
        public DataReturn<UserlistResult> GetAll(DataFilter<UserlistResult> filter)
        {
            var ret = new DataReturn<UserlistResult>();
            UserType usertype = UserType.Pensioner;
            if (filter.userType != null)
            {
                // Make sure it's converted to int before casting to enum
                var userTypeValue = Convert.ToInt32(filter.userType);
                usertype = (UserType)userTypeValue;                
            }

            var objList = new List<ApplicationUser>();
            int numberOfRecords = 0;
            var query = from u in _context.Applicationuser.AsNoTracking()
                        join a in _context.Association.AsNoTracking()
                             on u.AssociationId equals a.Id into associationGroup
                        from a in associationGroup.DefaultIfEmpty()
                        where u.IsActive == true 
                        && u.UserType== usertype
                        // First group by user to get their roles
                        select new UserlistResult
                        {
                            UserId = u.Id,
                            FirstName = u.FirstName ?? "",
                            LastName = u.LastName ?? "",
                            Email = u.Email ?? "",
                            CountryCode = u.CountryCode ?? "",
                            MobileNumber = u.MobileNumber ?? "",
                            AssociationName = a != null ? a.AssociationName : "",
                            AssociationId = u.AssociationId ?? 0,
                            UserType = u.UserType.HasValue ? u.UserType.Value.ToString() : "",
                            IsProfilePreez = u.IsProfilePreez,
                            Roles = ""
                        };

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.Trim().ToLower();
                query = query.Where(x =>
                    (x.MobileNumber ?? "").ToLower().Contains(searchTerm) ||
                    (x.Email ?? "").ToLower().Contains(searchTerm) ||
                    (x.AssociationName ?? "").ToLower().Contains(searchTerm) ||
                    (x.FirstName ?? "").ToLower().Contains(searchTerm) ||
                    (x.LastName ?? "").ToLower().Contains(searchTerm)
                    //(x.Roles ?? "").ToLower().Contains(searchTerm)
                    );
            }
            // Apply association filter
            if (filter.AssociationId > 0)
            {
                query = query.Where(x => x.AssociationId == filter.AssociationId);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(filter.SortName))
            {
                switch (filter.SortName.ToLower())
                {
                    case "firstname":
                        query = filter.SortDirection.ToLower() == "desc"
                            ? query.OrderByDescending(x => x.FirstName)
                            : query.OrderBy(x => x.FirstName);
                        break;
                    case "lastname":
                        query = filter.SortDirection.ToLower() == "desc"
                            ? query.OrderByDescending(x => x.LastName)
                            : query.OrderBy(x => x.LastName);
                        break;
                    case "email":
                        query = filter.SortDirection.ToLower() == "desc"
                            ? query.OrderByDescending(x => x.Email)
                            : query.OrderBy(x => x.Email);
                        break;
                    case "associationname":
                        query = filter.SortDirection.ToLower() == "desc"
                            ? query.OrderByDescending(x => x.AssociationName)
                            : query.OrderBy(x => x.AssociationName);
                        break;
                    case "mobilenumber":
                        query = filter.SortDirection.ToLower() == "desc"
                            ? query.OrderByDescending(x => x.MobileNumber)
                            : query.OrderBy(x => x.MobileNumber);
                        break;
                    case "usertype":
                        query = filter.SortDirection.ToLower() == "desc"
                            ? query.OrderByDescending(x => x.UserType)
                            : query.OrderBy(x => x.UserType);
                        break;
                    default:
                        // Default sort by UserId descending if no valid sort field is specified
                        query = query.OrderByDescending(x => x.UserId);
                        break;
                }
            }
            else
            {
                // Default sort by UserId descending if no sort field is specified
                query = query.OrderByDescending(x => x.UserId);
            }

            numberOfRecords = query.Count();
            var paginatedResults = filter.Limit > 0
                ? query.Skip((filter.PageNumber - 1) * filter.Limit)
                      .Take(filter.Limit)
                      .ToList()
                : query.ToList();
            ret.Contents = paginatedResults;

            ret.Source = "Users";
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
            return ret;
        }
        public async Task<IList<CommonNameModel>> GetOrganisations()
        {
            return await _context.Organisations.AsNoTracking()
                                              .Where(x => x.IsActive == true)
                                              .Select(a => new CommonNameModel
                                              {
                                                  Id = a.Id,
                                                  Name = a.Name
                                              })
                                               .ToListAsync();
        }

        public async Task<IList<UserPermissionResult>> GetUserPermissionsAsync(int userId)
        {


            List<UserPermissionResult> result = new List<UserPermissionResult>();
            List<UserPermissionResult> ret = new List<UserPermissionResult>();
            try
            {
                var userObj=_context.Applicationuser
                    .Where(x => x.Id == userId && x.Is_SystemAdmin == true).FirstOrDefault();
                if (userObj != null && userObj.Is_SystemAdmin == true)
                {
                    result = await (from userrole in _context.ApplicationUserRole.AsNoTracking()
                                    join rolepermission in _context.ApplicationRolePermission.AsNoTracking()
                                    on userrole.RoleId equals rolepermission.RoleId
                                    join permission in _context.ApplicationPermission.AsNoTracking()
                                    on rolepermission.PermissionId equals permission.Id
                                    where 
                                    //userrole.UserId == userId
                                    //&& 
                                    rolepermission.IsActive == true
                                    && userrole.IsActive == true &&
                                     (permission.PermissionType.ToLower() == "api"
                                    || permission.PermissionType.ToLower() == "ui")
                                    && userrole.IsActive == true
                                    select new UserPermissionResult
                                    {
                                        PageName = permission.Description,
                                        Read = rolepermission.Read,
                                        Create = rolepermission.Create,
                                        Delete = rolepermission.Delete,
                                        Update = rolepermission.Update
                                    }).ToListAsync();

                    ret = result.GroupBy(p => p.PageName)
                         .Select(g => new UserPermissionResult
                         {
                             PageName = g.Key,
                             Read = g.Any(x => x.Read == true),     // Use comparison instead of casting
                             Create = g.Any(x => x.Create == true), // Use comparison instead of casting
                             Delete = g.Any(x => x.Delete == true), // Use comparison instead of casting
                             Update = g.Any(x => x.Update == true)  // Use comparison instead of casting
                         })
                         .ToList();
                }
                else
                {


                    result = await (from userrole in _context.ApplicationUserRole.AsNoTracking()
                                    join rolepermission in _context.ApplicationRolePermission.AsNoTracking()
                                    on userrole.RoleId equals rolepermission.RoleId
                                    join permission in _context.ApplicationPermission.AsNoTracking()
                                    on rolepermission.PermissionId equals permission.Id
                                    where userrole.UserId == userId
                                    && rolepermission.IsActive == true
                                    && userrole.IsActive == true &&
                                     (permission.PermissionType.ToLower() == "api"
                                    || permission.PermissionType.ToLower() == "ui")
                                    && userrole.IsActive == true
                                    select new UserPermissionResult
                                    {
                                        PageName = permission.Description,
                                        Read = rolepermission.Read,
                                        Create = rolepermission.Create,
                                        Delete = rolepermission.Delete,
                                        Update = rolepermission.Update
                                    }).ToListAsync();

                    ret = result.GroupBy(p => p.PageName)
                         .Select(g => new UserPermissionResult
                         {
                             PageName = g.Key,
                             Read = g.Any(x => x.Read == true),     // Use comparison instead of casting
                             Create = g.Any(x => x.Create == true), // Use comparison instead of casting
                             Delete = g.Any(x => x.Delete == true), // Use comparison instead of casting
                             Update = g.Any(x => x.Update == true)  // Use comparison instead of casting
                         })
                         .ToList();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // Consider: logger.LogError(ex, "Error retrieving user permissions for userId: {UserId}", userId);

                // Optionally rethrow or handle the exception
                // throw;
            }

            return ret;



        }

        public async Task<IList<CommonNameModel>> GetAssociationsAsync(int Id)
        {
            if (Id == 0)
            {
                return await _context.Association
                                         .AsNoTracking()
                                         //.Where(x => x.OraganisationId == Id)
                                         .Select(a => new CommonNameModel
                                         {
                                             Id = a.Id,
                                             Name = a.AssociationName
                                         })
                                         .ToListAsync();
            }
            else
            {
                return await _context.Association
                                     .AsNoTracking()
                                     .Where(x => x.OraganisationId == Id)
                                     .Select(a => new CommonNameModel
                                     {
                                         Id = a.Id,
                                         Name = a.AssociationName
                                     })
                                     .ToListAsync();
            }
        }

        public async Task<IList<CommonNameModel>> getCampignAssociationAsync(int Id)
        {
            
                return await (from a in _context.Association.AsNoTracking()
                        join ca in _context.CampaignAssociations.AsNoTracking()
                         on a.Id equals ca.AssociationId
                         where ca.CampaignId==Id
                          select new CommonNameModel
                          {
                              Id = a.Id,
                              Name = a.AssociationName
                          }).ToListAsync();
                                
            
        }

        public async Task<ApplicationUser> IsUserExists(UserDTO user, bool isUpdate = false)
        {
            if (isUpdate)
            {
                var appuser = await _context.Applicationuser.AsNoTracking()
                     .AsNoTracking()
                    //  .Where
                    // (x => x.CountryCode.ToLower() == user.CountryCode.ToLower())
                    .Where(x => x.MobileNumber.ToLower().Trim() == user.MobileNumber.ToLower().Trim())
                    .Where(x => x.Id != user.UserId)
                    .FirstOrDefaultAsync();

                if (appuser == null)
                    return new ApplicationUser();
                return appuser;

            }
            else
            {
                var appuser = await _context.Applicationuser
                     .AsNoTracking()
                    //.Where
                    //(x => x.CountryCode.ToLower() == user.CountryCode.ToLower())
                    .Where(x => x.MobileNumber.ToLower().Trim() == user.MobileNumber.ToLower().Trim())
                    .FirstOrDefaultAsync();
                if (appuser == null)
                    return new ApplicationUser();
                return appuser;
            }
        }
        public async Task<UserDTO> GetByID(int Id)
        {
            var user = new UserDTO();
            var appuser = await _context.Applicationuser.AsNoTracking()
                .Where(x => x.Id == Id)
                .Include(x => x.Association)
                    .Include(x => x.Organisation)
                    .Include(x => x.State)
                    .Include(x => x.Country)
                .FirstOrDefaultAsync();

            if (appuser != null)
            {
                user = user.CopyUser(appuser);

                var roles = _context.ApplicationUserRole.AsNoTracking()
                     .Include(x => x.Role)
                     .Where(x => x.UserId == Id && x.IsActive == true);
                List<CommonNameDTO> commons = new List<CommonNameDTO>();
                foreach (var item in roles)
                {
                    if (item.IsActive == true)
                    {
                        commons.Add(new CommonNameDTO()
                        {
                            Id = item.Role.Id,
                            Name = item.Role.RoleName
                        });
                    }
                }
                user.Roles = commons;
            }
            return user;
        }

        public ApplicationUser CreateUserObj(UserDTO userDTO, ApplicationUser userObj = null)
        {
            if (userObj == null)
                userObj = new ApplicationUser();

            userObj.IsActive = true;
            userObj.UserType = (UserType)(userDTO.UserTypeId > 0 ? userDTO.UserTypeId.Value : (int)UserType.Pensioner);
            if (userDTO.UserType != null && userDTO.UserType.Id > 0)
            {
                userObj.UserType = (UserType)userDTO.UserType.Id;
            }
            userObj.CreatedAt = DateTime.Now;
            userObj.UpdatedAt = DateTime.Now;
            userObj.UpdatedBy = base.UserId > 0 ? base.UserId : 0;
            userObj.CreatedBy = base.UserId;

            userObj.FirstName = userDTO.FirstName;
            userObj.LastName = userDTO.LastName;
            userObj.Email = userDTO.Email;
            userObj.CountryCode = userDTO.CountryCode;
            userObj.MobileNumber = userDTO.MobileNumber;
            userObj.DOB = userDTO.DateOfBirth;
            userObj.AssociationId = userDTO.AssociationId > 0 ? userDTO.AssociationId : null;
            userObj.OrganisationId = userDTO.OrganisationId > 0 ? userDTO.OrganisationId : null;
            if (userDTO.GenderId > 0)
                userObj.Gender = (Gender)userDTO.GenderId.Value;
            else
            {
                userObj.Gender = null;
            }
            if (userDTO.PensionerIdType != null && userDTO.PensionerIdType.Id > 0)
            {
                userObj.PensionerIdType = (UserIdType)userDTO.PensionerIdType.Id;
            }
            else
            {
                userObj.PensionerIdType = (UserIdType)userDTO.pensioneridtypeId;
            }
            userObj.PensionerIdNumber = userDTO.Uniqueidnumber;
            userObj.Password = userDTO.Password;
            //(Gender)(userDTO.GenderId > 0 ? userDTO.GenderId.Value : null);
            userObj.Address = userDTO.Address;
            userObj.City = userDTO.City;
            userObj.StateId = userDTO.StateId > 0 ? userDTO.StateId : null;
            if (userDTO.CountryId == null || userDTO.CountryId == 0)
                userObj.CountryId = 1;
            else
                userObj.CountryId = userDTO.CountryId;
            userObj.Pincode = userDTO.Pincode;
            userObj.EMPIDPFNo = userDTO.EmpId;

            return userObj;
        }
        public async Task<ApplicationUser> RegisterUserAsync(UserDTO userDTO, int updatedBy)
        {
            var userObj = CreateUserObj(userDTO);
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    userObj.CreatedAt = DateTime.Now;
                    userObj.UpdatedAt = DateTime.Now;
                    userObj.UpdatedBy = updatedBy;
                    userObj.CreatedBy = updatedBy;

                    userObj.IsProfileComplete = false;
                    _context.Applicationuser.Add(userObj);
                    await _context.SaveChangesAsync();

                    // Ensure user.Id is not 0 and was inserted successfully
                    if (userObj.Id == 0)
                    {
                        throw new Exception("User insertion failed.");
                    }


                    if (userDTO.RoleIds != null && userDTO.RoleIds.Any())
                    {
                        foreach (var role in userDTO.RoleIds)
                        {

                            ApplicationUserRole userRole = new ApplicationUserRole();
                            userRole.UserId = userObj.Id;
                            userRole.RoleId = role;
                            userRole.UpdatedAt = DateTime.Now;
                            userRole.CreatedAt = DateTime.Now;
                            userRole.CreatedBy = updatedBy;
                            userRole.UpdatedBy = updatedBy;
                            _context.ApplicationUserRole.Add(userRole);
                            await _context.SaveChangesAsync();
                        }
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error("Error while register the user", ex);
                    throw;
                }
            }

            return userObj;
        }

        public async Task<ApplicationUser> UpdateUserAsync(int userId, UserDTO userDTO, int updatedBy)
        {
            var user = await _context.Applicationuser.AsNoTracking()
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync();
            if (user != null)
            {
                var userObj = new ApplicationUser();
                userObj = user;
                userObj.IsActive = true;
                userObj.UserType = (UserType)(userDTO.UserTypeId > 0 ? userDTO.UserTypeId.Value : (int)UserType.Pensioner);
                if (userDTO.UserType != null && userDTO.UserType.Id > 0)
                {
                    userObj.UserType = (UserType)userDTO.UserType.Id;
                }

                userObj.UpdatedAt = DateTime.Now;
                userObj.UpdatedBy = base.UserId > 0 ? base.UserId : 0;


                userObj.FirstName = userDTO.FirstName;
                userObj.LastName = userDTO.LastName;
                userObj.Email = userDTO.Email;
                userObj.CountryCode = userDTO.CountryCode;
                userObj.MobileNumber = userDTO.MobileNumber;
                userObj.DOB = userDTO.DateOfBirth;
                userObj.AssociationId = userDTO.AssociationId > 0 ? userDTO.AssociationId : null;
                userObj.OrganisationId = userDTO.OrganisationId > 0 ? userDTO.OrganisationId : null;
                if (userDTO.GenderId > 0)
                    userObj.Gender = (Gender)userDTO.GenderId.Value;
                else
                {
                    userObj.Gender = null;
                }
                //(Gender)(userDTO.GenderId > 0 ? userDTO.GenderId.Value : null);
                userObj.Address = userDTO.Address;
                userObj.City = userDTO.City;
                userObj.StateId = userDTO.StateId > 0 ? userDTO.StateId : null;
                if (userDTO.CountryId == null || userDTO.CountryId == 0)
                    userObj.CountryId = 1;
                else
                    userObj.CountryId = userDTO.CountryId;
                userObj.Pincode = userDTO.Pincode;
                userObj.EMPIDPFNo = userDTO.EmpId;

                if (userDTO.PensionerIdType != null && userDTO.PensionerIdType.Id > 0)
                {
                    userObj.PensionerIdType = (UserIdType)userDTO.PensionerIdType.Id;
                }
                else
                {
                    userObj.PensionerIdType = (UserIdType)userDTO.pensioneridtypeId;
                }
                userObj.PensionerIdNumber = userDTO.Uniqueidnumber;
                userObj.Password = userDTO.Password;
                // var userObj = CreateUserObj(userDTO, user);

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // userObj.Id = userId;
                        userObj.IsProfileComplete = true;
                        userObj.OTP = "";
                        userObj.OTPExpiration = null;
                        userObj.UpdatedBy = updatedBy;
                        userObj.UpdatedAt = DateTime.Now;
                        _context.Applicationuser.Update(userObj);
                        await _context.SaveChangesAsync();

                        // Ensure user.Id is not 0 and was inserted successfully
                        if (userObj.Id == 0)
                        {
                            throw new Exception("User insertion failed.");
                        }

                        if (userDTO.RoleIds != null && userDTO.RoleIds.Any())
                        {
                            foreach (var role in userDTO.RoleIds)
                            {
                                var userrole = await _context.ApplicationUserRole.AsNoTracking().
                                    Where(x => x.UserId == userId
                                    && x.RoleId == role
                                    && x.IsActive == true).FirstOrDefaultAsync();
                                if (userrole == null)
                                {
                                    ApplicationUserRole userRole = new ApplicationUserRole();
                                    userRole.UserId = userObj.Id;
                                    userRole.RoleId = role;
                                    userRole.UpdatedAt = DateTime.Now;
                                    userRole.CreatedAt = DateTime.Now;
                                    userRole.CreatedBy = updatedBy;
                                    userRole.UpdatedBy = updatedBy;
                                    _context.ApplicationUserRole.Add(userRole);
                                    await _context.SaveChangesAsync();
                                }

                            }
                            var userroles = await _context.ApplicationUserRole.AsNoTracking()
                                .Where(x => x.UserId == userId).ToListAsync();
                            if (userroles.Any())
                            {
                                var objroles = userroles
                                .Where(item => !userDTO.RoleIds.Contains(item.RoleId)).ToList();
                                foreach (var role in objroles)
                                {
                                    ApplicationUserRole userRole = new ApplicationUserRole();
                                    userRole.Id = role.Id;
                                    userRole.UserId = userObj.Id;
                                    userRole.RoleId = role.RoleId;
                                    userRole.UpdatedAt = DateTime.Now;
                                    userRole.UpdatedBy = updatedBy;
                                    userRole.IsActive = false;
                                    _context.ApplicationUserRole.Update(userRole);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }



                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Log.Error("Error while register the user", ex.Message);
                        throw;
                    }
                }
                return userObj;
            }
            return user;
        }



        public Task GetCountries()
        {
            throw new NotImplementedException();
        }

        public Task GetStates()
        {
            throw new NotImplementedException();
        }
    }
}
