using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Mapper
{
 
    public static class CommonMapper
    {
        public static CommonNameDTO ToOrganisationDto(Organisation organisation)
        {
            return new CommonNameDTO
            {
                Id = organisation.Id,
                Name = organisation.Name
            };
        }
        public static CommonNameDTO ToAddressStateDto(AddressState state)
        {
            return new CommonNameDTO
            {
                Id = state.Id,
                Name = state.Name
            };
        }
        public static CommonNameDTO ToApplicationRoleDto(ApplicationRole role)
        {
            return new CommonNameDTO
            {
                Id = role.Id,
                Name = role.RoleName
            };
        }
        public static CommonNameDTO ToAddressCountryDto(AddressCountry state)
        {
            return new CommonNameDTO
            {
                Id = state.Id,
                Name = state.Name
            };
        }
        public static CommonAssociationDTO ToAssociationDto(Association association)
        {
            return new CommonAssociationDTO
            {
                Id = association.Id,
                Name = association.AssociationName,
                AccountName=association.AssociationBankDetails.BankName
            };
        }
        public  static CommonNameDTO ToProductDto(Product product)
        {
            return new CommonNameDTO
            {
                Id = product.Id,
                Name = product.ProductName
            };
        }
    }
}
