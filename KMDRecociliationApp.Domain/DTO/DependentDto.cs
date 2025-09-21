using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DependentDto
    {
        public int Id { get; set; }

        public int ApplicantId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
      
        public string? Relationship { get; set; }

    }
}
