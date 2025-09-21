using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class AgeBandDTO
    {
        public int Id { get; set; }
        public int AgeBandStart { get; set; }
        public int AgeBandEnd { get; set; }
        public string? Name { get; set; }
    }
}
