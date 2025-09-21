using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO.InsurerData
{
    public class ApplicantDto
    {
        public int Id { get; set; }

        public string UniqueIdentifier { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        [StringLength(100, ErrorMessage = "First Name cannot exceed 100 characters")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? GenderName { get; set; }
        public string? IdCardTypeName { get; set; }
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]    
        public Gender Gender { get; set; }

        [StringLength(15, ErrorMessage = "Contact Number cannot exceed 15 characters")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Contact Number must contain only digits")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Salary is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive value")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Associated Organization is required")]
        [StringLength(100, ErrorMessage = "Associated Organization cannot exceed 100 characters")]
        public string AssociatedOrganization { get; set; }

        [Required(ErrorMessage = "Location/Address is required")]
        [StringLength(200, ErrorMessage = "Location/Address cannot exceed 200 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "ID Card Type is required")]
       
        public IdCardType IdCardType { get; set; }

        [Required(ErrorMessage = "ID Card Number is required")]
        [StringLength(50, ErrorMessage = "ID Card Number cannot exceed 50 characters")]
        public string IdCardNumber { get; set; }


        public BankDetailsDto? BankDetails { get; set; }

        public List<DependentDto> Dependents { get; set; } = new List<DependentDto>();
        public string FullName => $"{FirstName} {LastName}";
    }
}
