using FluentValidation;
using KMDRecociliationApp.Domain.Common;
using KMDRecociliationApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.DTO
{
    public class DTOAssociation
    {
        ////[JsonIgnore]
        //public IFormFile? MandateFile { get; set; }
        ////[JsonIgnore]
        //public IFormFile? QRCodeFile { get; set; }
        public int? Id { get; set; }
        public string? AssociationName { get; set; }
        public string? AssociationCode { get; set; }
        public CommonNameModel? Organisation { get; set; }
        //organisation
        public CommonNameModel? ParentAssociation { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public CommonNameModel? State { get; set; }
        public string? PINCode { get; set; }
        public CommonNameModel? Country { get; set; }
        public bool AcceptOnePayPayment { get; set; }
        public CommonFileModel? MandateFile { get; set; }
        public bool isMandateFileUpdated { get; set; }
        public CommonFileModel? QrCodeFile { get; set; }
        public bool isQrCodeFileUpdated { get; set; }

        public List<DTOAssociationContactDetails>? associationContactDetails { get; set; }
        public DTOAssociationBankDetails Bank { get; set; }
        public List<DTOAssociationMessage>? associationMessages { get; set; }
        public string? OnePayId { get; set; }

        public DTOAssociation Copy(Association association
            , AssociationBankDetails associationBank, Organisation organisation
            ,Association parentAssociation,AddressState addressState
            ,AddressCountry country,List<AssociationContactDetails> associationContacts
            ,List<AssociationMessageDetails> associationMessages,AssociationOnePayDetails associationOnePay)
        {
            DTOAssociation dTOAssociation = new DTOAssociation();
            dTOAssociation.Id = association.Id;
            dTOAssociation.AssociationName = association.AssociationName;
            if (organisation != null)
            {
                dTOAssociation.Organisation = new CommonNameModel()
                { Id = organisation.Id, Name = organisation.Name };
            }
            else
            {
                dTOAssociation.Organisation = new CommonNameModel() { Id = 0, Name = "" };
            }
            if (parentAssociation != null)
            {
                dTOAssociation.ParentAssociation = new CommonNameModel()
                { Id = parentAssociation.Id, Name = parentAssociation.AssociationName };
            }
            else
            {
                dTOAssociation.ParentAssociation = new CommonNameModel() 
                { Id = 0, Name = "" };
            }
            dTOAssociation.Address1 = association.Address1;
            dTOAssociation.Address2 = association.Address2;
            dTOAssociation.City = association.City;
            dTOAssociation.PINCode = association.PINCode;
            dTOAssociation.AssociationCode = association.AssociationCode;
            dTOAssociation.AcceptOnePayPayment = association.AcceptOnePayPayment;
            dTOAssociation.MandateFile = new CommonFileModel()
            {
                Id = association.Id,
                Name = associationBank.MendateName,
                Url = associationBank.MendateUrl
            };
            dTOAssociation.QrCodeFile = new CommonFileModel()
            {
                Id = association.Id,
                Name = associationBank.QRCodeName,
                Url = associationBank.QRCodeUrl
            };

            if (addressState != null)
            {
                dTOAssociation.State = new CommonNameModel()
                { Id = addressState.Id, Name = addressState.Name };
            }
            else
            {
                dTOAssociation.State = new CommonNameModel()
                { Id = 0, Name = "" };
            }
            if (country != null)
            {
                dTOAssociation.Country = new CommonNameModel()
                { Id = country.Id, Name = country.Name };
            }
            else
            {
                dTOAssociation.Country = new CommonNameModel()
                { Id = 0, Name = "" };
            }
            if (associationBank != null)
            {
                dTOAssociation.Bank = new DTOAssociationBankDetails();
                dTOAssociation.Bank.AccountName =associationBank.AccountName;
                dTOAssociation.Bank.BranchName = associationBank.BranchName;
                dTOAssociation.Bank.BankName = associationBank.BankName;
                dTOAssociation.Bank.AccountNumber = associationBank.AccountNumber;
                dTOAssociation.Bank.IFSCCode = associationBank.IFSCCode;
                dTOAssociation.Bank.MICRCode = associationBank.MICRCode;  
                dTOAssociation.Bank.Id=associationBank.Id;
                dTOAssociation.Bank.AssociationId = association.Id;
                
               // //dTOAssociation.Bank.Mendate.Id = associationBank.MendateId;
               // dTOAssociation.Bank.Mendate.Name= associationBank.MendateName;
               // dTOAssociation.Bank.Mendate.Url = associationBank.MendateUrl;
               // //dTOAssociation.Bank.Mendate.isDeleated = associationBank.MendateIsDeleated;
               //// dTOAssociation.Bank.QRCode.Id = associationBank.QRCodeId;
               // dTOAssociation.Bank.QRCode.Name= associationBank.QRCodeName;
               // dTOAssociation.Bank.QRCode.Url = associationBank.QRCodeUrl;
               // //dTOAssociation.Bank.QRCode.isDeleated= associationBank.QRCodeIsDeleated;
            }
            else
            {
                dTOAssociation.Bank = new DTOAssociationBankDetails();
            }

            if (associationContacts != null)
            {
                dTOAssociation.associationContactDetails = associationContacts?.Select(contact => new DTOAssociationContactDetails
                {
                    Id = contact.Id,
                    AssociationId = contact.AssociationId,
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Phone = contact.Phone,
                    Email = contact.Email,

                }).ToList();
            }
            else
            {
                dTOAssociation.associationContactDetails = new List<DTOAssociationContactDetails>();
            }

            if (associationMessages != null)
            {
                dTOAssociation.associationMessages = associationMessages?.Select(message => new DTOAssociationMessage
                {
                    Id=message.Id,
                    AssociationId=message.AssociationId,
                    Name=message.Name,
                    Template=message.Template,
                    IsApproved=message.IsApproved
                }).ToList();
                
            }
            else
            { 
                dTOAssociation.associationMessages = new List<DTOAssociationMessage>(); 
            }

            if (associationOnePay != null)
            {
                dTOAssociation.OnePayId = associationOnePay.OnePayId;
               

            }
            else
            {
                dTOAssociation.OnePayId = null;
            }

            return dTOAssociation;

        }
    }
    //public class DTOAssociationOnePayDetails
    //{
    //    public int? Id { get; set; }
    //    public int AssociationId { get; set; }
    //    public string? OnepayUrl { get; set; }
    //    public string? OnePayId { get; set; }

    //}
    public class DTOAssociationContactDetails
    {
        public int? Id { get; set; }
        public int AssociationId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

       


    }
    public class DTOAssociationBankDetails
    {
        public int? Id { get; set; }
        public int AssociationId { get; set; }
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? MICRCode { get; set; }
       
    }
    public class DTOAssociationMessage
    {
        public int? Id { get; set; }
        public int AssociationId { get; set; }
        public string? Name { get; set; }
        public string? Template { get; set; }
        public bool IsApproved { get; set; }
    }

    public class AssociationValidator : AbstractValidator<DTOAssociation>
    {
        public AssociationValidator()
        {
            //RuleFor(association => association.Name).NotEmpty().WithMessage("Association Name is required.");

        }
    }
}
