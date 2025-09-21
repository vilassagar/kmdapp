using DocumentFormat.OpenXml.Wordprocessing;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMDRecociliationApp.Domain.DTO.InsurerData
{
    // DTO for CSV import
    public class ApplicantImportDto
    {
        [DisplayName("SR. No")]
        public int SerialNumber { get; set; }

        [DisplayName("First Name")]
        [Required]
        public string FirstName { get; set; }
    
        [DisplayName("Last Name")]
        [Required]
        public string LastName { get; set; }

        [DisplayName("DOB")]
        [Required]
        public string DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [DisplayName("Contact/Mobile No")]
        public string ContactNumber { get; set; }
       
        [Display(Name = "Email")]
        public string? Email { get; set; }
       
        public string? Salary { get; set; }

        [DisplayName("Associated Organization")]
        public string AssociatedOrganization { get; set; }

        [DisplayName("Location/Address")]
        [Required]
        public string Address { get; set; }

        [DisplayName("ID Card type")]
        [Required]
        public string IdCardType { get; set; }

        [DisplayName("ID Card no")]
        [Required]
        public string IdCardNumber { get; set; }
      
        [DisplayName("Product Name")]
        [Required]
        public string ProductName { get; set; }

        [DisplayName("Dependent First Name")]
        public string DependentFirstName { get; set; }

        [DisplayName("Dependent Last Name")]
        public string DependentLastName { get; set; }

        [DisplayName("Dependent Relationship")]
        public string DependentRelationship { get; set; }

        [DisplayName("Dependent DOB")]
        public string? DependentDateOfBirth { get; set; }

        [DisplayName("Bank Name")]
        public string BankName { get; set; }

        [DisplayName("Bank Account No")]
        public string BankAccountNumber { get; set; }

        [DisplayName("Bank IFSC Code")]
        public string BankIfscCode { get; set; }

        [DisplayName("MICR Code")]
        public string MicrCode { get; set; }

        [DisplayName("Bank Branch name & Address")]
        public string BankBranchDetails { get; set; }
    }

}
