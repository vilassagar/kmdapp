using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class Organisation : ParentEntity
    {       
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Association> Association { get; set; } = new List<Association>();

    }
}
