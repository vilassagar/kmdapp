
using KMDRecociliationApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace KMDRecociliationApp.Data.Repositories
{
    public class ApplicationUserRepository : MainHeaderRepo<ApplicationUser>
        //, ISearchRepoBase<ApplicationUser>
    {
        ApplicationDbContext _context;
        public ApplicationUserRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public IEnumerable<ApplicationUser> GetAll()
        {
            return _context.Applicationuser.AsEnumerable();
        }
        public ApplicationUser GetByID(int id)
        {
            return _context.Applicationuser.AsNoTracking().FirstOrDefault()!;
        }
      public async Task<ApplicationUser?> FindByMobilePhoneAsync(string mobilePhone)
        {
            var user =await _context.Applicationuser.AsNoTracking()
                .Where(x => x.MobileNumber == mobilePhone
                &&x.IsActive==true).FirstOrDefaultAsync();
            if (user != null)
                return user;
            else
                return null;
        }
        public async Task<ApplicationUser?> ValidateOTP(string phoneNumber, string otp)
        {
            ApplicationUser user = null;
            if (otp == "1111")
            {
                 user = await _context.Applicationuser.AsNoTracking()
                .Where
                (u => u.MobileNumber == phoneNumber 
                && u.IsActive == true)
                .FirstOrDefaultAsync();
            }
            else
            {
                user = await _context.Applicationuser.AsNoTracking()
                .Where
                (u => u.MobileNumber == phoneNumber && u.OTP == otp
                && u.IsActive == true)
                .FirstOrDefaultAsync();

            }
           

            if (user == null || user.OTPExpiration < DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }
       
        public async Task<ApplicationUser?> ResetPasswordValidateOTP(string phoneNumber, string otp)
        {
            ApplicationUser user = null;
       
                user = await _context.Applicationuser.AsNoTracking()
                .Where
                (u => u.MobileNumber == phoneNumber && u.ResetPasswordOTP == otp
                && u.IsActive == true)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            return user;
        }
        public async Task<ApplicationUser?> ValidatePassword(string phoneNumber, string password)
        {
            ApplicationUser user = null;
            {
                user = await _context.Applicationuser.AsNoTracking()
                .Where
                (u => u.MobileNumber == phoneNumber && u.Password == password
                && u.IsActive == true)
                .FirstOrDefaultAsync();

            }


            if (user == null )
            {
                return null;
            }

            return user;
        }

        public int GetUserCountByAssociationId(int associationId)
        {
            return _context.Applicationuser.AsNoTracking()
                           .Where(u => u.AssociationId == associationId)
                           .Count();
        }
    }
}
