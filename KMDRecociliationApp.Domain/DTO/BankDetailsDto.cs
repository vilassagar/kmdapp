using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class BankDetailsDto
    {
        public int ?Id { get; set; }

        public string? BankName { get; set; }
        public string? BankBranchDetails { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankIfscCode { get; set; }      
        public string? BankMicrCode { get; set; }



    }
}
