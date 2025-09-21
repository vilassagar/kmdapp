using FluentValidation;
using KMDRecociliationApp.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KMDRecociliationApp.Domain.Entities
{
    public class ApplicationUser     
        : BaseEntity
    {
        public  string? FirstName { get; set; } 
        public  string? LastName { get; set; }
        public  string? Email { get; set; }
        public string? CountryCode { get; set; }
        public  string? MobileNumber { get; set; }
        public string? Password { get; set; }
        public DateTime? DOB { get; set; }
        public UserType? UserType { get; set; }
        public UserIdType? PensionerIdType { get; set; }
        public string? PensionerIdNumber { get; set; }
        public Gender? Gender { get; set; }
        public int? OrganisationId { get; set; }
        public int? AssociationId { get; set; }
        public string? EMPIDPFNo { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public string? Pincode { get; set; }      
        public AddressState? State { get; set; }
        public AddressCountry? Country { get; set; }
        public Organisation? Organisation { get; set; }
        public Association? Association { get; set; }
        public  ICollection<RefundRequest> ? RefundRequest { get; set; }=new List<RefundRequest>();
        public string? OTP { get; set; }
        public string? ResetPasswordOTP { get; set; }
        public DateTime? OTPExpiration { get; set; }=DateTime.Now;
        public bool? IsProfileComplete { get; set; }=false;
        public ICollection<ApplicationUserRole> ApplicationUserRole { get; set; } = new List<ApplicationUserRole>();
       
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        public bool ? IsProfilePreez { get; set; }
        public bool? Is_SystemAdmin { get; set; }

    }



}