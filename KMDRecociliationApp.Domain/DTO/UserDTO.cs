using FluentValidation;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class UserDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? CountryCode { get; set; }
        public string? MobileNumber { get; set; }
        public string? EmpId { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public string? Pincode { get; set; }
        public string? Password { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public CommonNameDTO? Gender { get; set; }
        public int pensioneridtypeId {  get; set; }
        public int? OrganisationId { get; set; }
        public int? AssociationId { get; set; }
        public int? UserTypeId { get; set; }
        public int? GenderId { get; set; }
        public string? OTP { get; set; }
        public DateTime OTPExpiration { get; set; }
        public int UserId { get; set; }   
        public string? Uniqueidnumber { get; set; }
        public CommonNameDTO? Organisation { get; set; }
        public CommonNameDTO? Association { get; set; }
        public CommonNameDTO? UserType { get; set; }
        public CommonNameDTO? PensionerIdType { get; set; }
        public CommonNameDTO? State { get; set; }
        public CommonNameDTO? Country { get; set; }
        public List<int>? RoleIds { get; set; }
        public List<CommonNameDTO>? Roles { get; set; }
        public UserDTO CopyUser(ApplicationUser user)
        {
            UserDTO copy = new UserDTO();
            copy.FirstName = user.FirstName;
            copy.LastName = user.LastName;
            copy.Email = user.Email;
            copy.CountryCode = user.CountryCode;
            copy.MobileNumber = user.MobileNumber;
            copy.UserTypeId = user.UserType != null ? (int)user.UserType : 0;
            if (user.Gender != null && (int)user.Gender != 0)
            {
                copy.Gender = new CommonNameDTO() { Id = (int)user.Gender, Name = user.Gender.Value.ToString() };
            }
            else
                copy.Gender = null;
            copy.UserType = user.UserType != null ? new CommonNameDTO() { Id = (int)user.UserType, Name = user.UserType.Value.ToString() } : null;
            copy.PensionerIdType = user.PensionerIdType != null ? new CommonNameDTO() { Id = (int)user.PensionerIdType, Name = user.PensionerIdType.Value.ToString() } : null;
            copy.Uniqueidnumber = user.PensionerIdNumber;
            copy.DateOfBirth = user.DOB;
            copy.UserId = user.Id;
            copy.OrganisationId = user.OrganisationId > 0 ? user.OrganisationId : null;
            copy.AssociationId = user.AssociationId > 0 ? user.AssociationId : null;
            copy.EmpId = user.EMPIDPFNo;
            copy.Address = user.Address;
            copy.City = user.City;
            copy.StateId = copy.StateId;
            copy.CountryId = user.CountryId;
            copy.Pincode = user.Pincode;
            copy.Password=user.Password;
            copy.Organisation = user.Organisation != null ? new CommonNameDTO() { Id = user.Organisation.Id, Name = user.Organisation.Name } : null;
            copy.Association = user.Association != null ? new CommonNameDTO() { Id = user.Association.Id, Name = user.Association.AssociationName } : null;
            copy.State = user.State != null ? new CommonNameDTO() { Id = user.State.Id, Name = user.State.Name } : null;
            copy.Country = user.Country != null ? new CommonNameDTO() { Id = user.Country.Id, Name = user.Country.Name } : null;

            return copy;
        }
    }
    public class ApplicationUserValidator : AbstractValidator<UserDTO>
    {
        public ApplicationUserValidator()
        {
            RuleFor(applicationUser => applicationUser.FirstName).NotEmpty().WithMessage("First Name is required.");
            RuleFor(applicationUser => applicationUser.LastName).NotEmpty().WithMessage("Last Name is required.");
          //  RuleFor(applicationUser => applicationUser.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("A valid email is required.");
            RuleFor(applicationUser => applicationUser.CountryCode).NotEmpty().WithMessage("CountryCode is required.").Matches(@"^\+[0-9]+$").WithMessage("Country Code must start with '+' followed by digits only.");
            RuleFor(applicationUser => applicationUser.MobileNumber).NotEmpty().WithMessage("MobileNumber is required.");
            //RuleFor(applicationUser => applicationUser.OrganisationId).NotEmpty().WithMessage("Organisation is required.");
            //RuleFor(applicationUser => applicationUser.AssociationId).NotEmpty().WithMessage("Association is required.");

        }
    }
}
