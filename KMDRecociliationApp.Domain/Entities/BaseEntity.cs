using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace KMDRecociliationApp.Domain.Entities
{
    public class BaseEntity : TimeStamp
    {
       public int Id { get; set; }      
       public bool IsActive { get; set; }=true;
    }
    public class ParentEntity 
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
    public class KeyEntity
    {
        public int Id { get; set; }
    }
}