using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class CommonNameDTO
    {
        public CommonNameDTO() { }
        public CommonNameDTO(int id, string name)
        {
            Name = name;
            Id = id;
            
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        
    }
    public class CommonAssociationDTO
    {
        public CommonAssociationDTO() { }
        public CommonAssociationDTO(int id, string name
            , string? accountName,string code)
        {
            Name = name;
            Id = id;
            AccountName = accountName;
            Code = code;
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? AccountName { get; set; }
        public string? Code { get; set; }

    }
    public class NomineeRelationDTO
    {
        public NomineeRelationDTO() { }
        public NomineeRelationDTO(int id, string name, int gender = 1)
        {
            Name = name;
            Id = id;
            Gender = gender;
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Gender { get; set; }
    }
}
