using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KMDRecociliationApp.Domain.Enum;
namespace KMDRecociliationApp.Domain.Entities
{
    /// <summary>
    /// Entity class for Insurance Policy applicant details
    /// </summary>
    [Table("ApplicantInsurancePolicies")]
    public class ApplicantInsurancePolicy
    {
        public ApplicantInsurancePolicy()
        {
            Dependents = new HashSet<ApplicantDependent>();
        }

        [Key]
        public int Id { get; set; }

        // Auto-generated unique identifier based on ID Card No and first 4 letters of name
        [Required]
        [StringLength(50)]
        public string UniqueIdentifier { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Column(TypeName = "Email")]
        public string? Email { get; set; }
        [Column(TypeName = "CountryCode")]
        public string? CountryCode { get; set; }

        [StringLength(15)]
        [Column(TypeName = "MobileNumber")]
        [Display(Name = "Contact/Mobile No")]
        public string? MobileNumber { get; set; }
       
     

        [Required]
        [Column(TypeName = "date")]
        [Display(Name = "Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Salary { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Associated Organization")]
        public string? AssociatedOrganization { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Location/Address")]
        public string ?Address { get; set; }

        [Required]
        [Display(Name = "ID Card Type")]
        public IdCardType? IdCardType { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "ID Card No")]
        public string ?IdCardNumber { get; set; }
      

        [StringLength(500)]
        [Display(Name = "Product Name")]
        public string? ProductName { get; set; }

        // Navigation property for bank details
        public virtual ApplicantBankDetails? BankDetails { get; set; }

        // Navigation property for dependents
        public virtual ICollection<ApplicantDependent>? Dependents { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        /// <summary>
        /// Generates a unique identifier based on ID Card Number and first 4 letters of name
        /// </summary>
        public void GenerateUniqueIdentifier()
        {
            if (!string.IsNullOrEmpty(IdCardNumber) && !string.IsNullOrEmpty(FullName) && FullName.Length >= 4)
            {
                string namePrefix = FullName.Substring(0, 4).ToUpper();
                UniqueIdentifier = $"{IdCardNumber}_{namePrefix}";
            }
            else
            {
                string namePrefix = FullName.ToUpper();
                UniqueIdentifier = $"{IdCardNumber}_{namePrefix}";
            }
        }
    }

    /// <summary>
    /// Entity class for Bank Details
    /// </summary>
    [Table("ApplicantBankDetails")]
    public class ApplicantBankDetails
    {
        [Key]
        public int Id { get; set; }


        public int? ApplicantId { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankIfscCode { get; set; }
        public string? BankBranchDetails { get; set; }
        public string ?BankMicrCode { get; set; }

        // Navigation property for applicant
        [ForeignKey("ApplicantId")]
        public virtual ApplicantInsurancePolicy Applicant { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Entity class for Dependent Details
    /// </summary>

    [Table("ApplicantDependents")]
    public class ApplicantDependent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ApplicantId { get; set; }



        public string? FirstName { get; set; }

        public string? LastName { get; set; }


        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }


        public string? Relationship { get; set; }

        // Navigation property for applicant
        [ForeignKey("ApplicantId")]
        public virtual ApplicantInsurancePolicy Applicant { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Enumeration for ID Card Types
    /// </summary>
    public enum IdCardType
    {
        Aadhar = 1,
        PanCard = 2,
        VoterId = 3,
        PassPort = 4
    }

}
