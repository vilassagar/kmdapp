using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class AddressState:ParentEntity
    { 
        public string ?Name { get; set; }
        public int? CountryId { get; set; }
    }
    public class AddressCountry : ParentEntity
    {
        public string? Name { get; set; }
        public string ? CountryCode { get; set; }
    }
}
