using KMDRecociliationApp.Data.Mapper;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Repositories
{
    public class CommonHelperRepository
    {
        ApplicationDbContext context;
        private readonly ILogger _logger;

        public CommonHelperRepository(ILoggerFactory logger, ApplicationDbContext appContext)
        {
            context = appContext;
            _logger = logger.CreateLogger("CommonHelperRepositoryRepo");
        }
        public List<CommonNameDTO> GetStates()
        {
            var states = context.AddressState.AsNoTracking().ToList();
            return states.Select(CommonMapper.ToAddressStateDto).ToList();
        }
        public List<CommonNameDTO> GetRoles()
        {
            var roles = context.ApplicationRole.AsNoTracking().ToList();
            return roles.Select(CommonMapper.ToApplicationRoleDto).ToList();
        }
        public List<CommonNameDTO> GetCountries()
        {
            var countries = context.AddressCountry.AsNoTracking().ToList();
            return countries.Select(CommonMapper.ToAddressCountryDto).ToList();
        }
        public List<CommonNameDTO> GetOrganisations()
        {
            var organisations = context.Organisations.AsNoTracking().ToList();
            return organisations
                .Select(CommonMapper.ToOrganisationDto).ToList();
        }
        public List<CommonNameDTO> GetAgeBand()
        {
            var ageBands = context.AgeBandPremiumRate.AsNoTracking()
                .Select(x=> new CommonNameDTO
                {
                    //AgeBandEnd = x.AgeBandEnd,
                    //AgeBandStart = x.AgeBandStart,
                    Name = x.AgeBandValue,
                    Id= x.Id

                }).ToList();

            return ageBands;
        }
        public List<CommonNameDTO> getApplicantOrganizations()
        {
            var ageBands = context.ApplicantInsurancePolicies.AsNoTracking()
                .Select(x => new CommonNameDTO
                {
                    //AgeBandEnd = x.AgeBandEnd,
                    //AgeBandStart = x.AgeBandStart,
                    Name = x.AssociatedOrganization,
                    Id = x.Id

                }).ToList();

            return ageBands;
        }
        public List<CommonAssociationDTO> GetAssociations(int associationId, string filter)
        {
            List<CommonAssociationDTO> lst = new List<CommonAssociationDTO>();
            if (filter.Trim().ToLower() == "all" && associationId == 0)
            {
                lst.Add(new CommonAssociationDTO() { Id = -1, Name = "All" });
            }
            var associations = context.Association.AsNoTracking()
                 .Include(x => x.AssociationBankDetails)
                 .Where(x => x.Id == (associationId > 0 ? associationId : x.Id))
                .ToList();

            var obj = associations
                .Select(CommonMapper.ToAssociationDto).ToList();
            lst.AddRange(obj);
            return lst;
        }
        public List<CommonNameDTO> GetGenders()
        {
            var genders = new List<CommonNameDTO>()
            {
                new CommonNameDTO() { Id = (int)Gender.Male, Name = nameof(Gender.Male) },
                new CommonNameDTO() { Id = (int)Gender.Female, Name = nameof(Gender.Female) }
            };


            return genders;
        }
        public List<CommonNameDTO> GetIdTypes()
        {
            var genders = new List<CommonNameDTO>()
            {
                new CommonNameDTO() { Id = (int)IdCardType.Aadhar, Name = nameof(IdCardType.Aadhar) },
                new CommonNameDTO() { Id = (int)IdCardType.PanCard, Name = nameof(IdCardType.PanCard) },
                new CommonNameDTO() { Id = (int)IdCardType.VoterId, Name = nameof(IdCardType.VoterId) },
                 new CommonNameDTO() { Id = (int)IdCardType.PassPort, Name = nameof(IdCardType.PassPort) }
            };


            return genders;
        }
        public List<NomineeRelationDTO> GetNomineeRelations()
        {
            var nomineeRelation = new List<NomineeRelationDTO>()
            {
               // new NomineeRelationDTO() { Id = (int)NomineeRelation.None, Name = nameof(NomineeRelation.None),Gender = (int)Gender.Male },
              new NomineeRelationDTO() { Id = (int)NomineeRelation.Son, Name = nameof(NomineeRelation.Son),Gender = (int)Gender.Male },
                 new NomineeRelationDTO() { Id = (int)NomineeRelation.Daughter, Name = nameof(NomineeRelation.Daughter),Gender = (int)Gender.Female },
                 new NomineeRelationDTO() { Id = (int)NomineeRelation.Sister, Name = nameof(NomineeRelation.Sister),Gender = (int)Gender.Female },

                  new NomineeRelationDTO() { Id = (int)NomineeRelation.Father, Name = nameof(NomineeRelation.Father),Gender = (int)Gender.Male },
                new NomineeRelationDTO() { Id = (int)NomineeRelation.Mother, Name = nameof(NomineeRelation.Mother),Gender = (int)Gender.Female },
                 new NomineeRelationDTO() { Id = (int)NomineeRelation.Spouse, Name = nameof(NomineeRelation.Spouse) , Gender =(int) Gender.Female},
                new NomineeRelationDTO() { Id = (int)NomineeRelation.Brother, Name = nameof(NomineeRelation.Brother) , Gender =(int) Gender.Male},
                new NomineeRelationDTO() { Id = (int)NomineeRelation.FatherInlaw, Name = nameof(NomineeRelation.FatherInlaw) , Gender =(int) Gender.Male},
                new NomineeRelationDTO() { Id = (int)NomineeRelation.MotherInLaw, Name = nameof(NomineeRelation.MotherInLaw) , Gender =(int) Gender.Female},

            };


            return nomineeRelation;
        }

        public List<CommonNameDTO> GetPolicyTypes()
        {
            var types = new List<CommonNameDTO>()
            {
                new CommonNameDTO() { Id = (int)ProductPolicyType.BasePolicy, Name = nameof(ProductPolicyType.BasePolicy) },
                new CommonNameDTO() { Id = (int)ProductPolicyType.TopUpPolicy, Name = nameof(ProductPolicyType.TopUpPolicy) },
                new CommonNameDTO() { Id = (int)ProductPolicyType.OPD, Name = nameof(ProductPolicyType.OPD) },
                new CommonNameDTO() { Id = (int)ProductPolicyType.PaymentProtectionPolicy, Name = nameof(ProductPolicyType.PaymentProtectionPolicy) },
                      new CommonNameDTO() { Id = (int)ProductPolicyType.AgeBandPremium, Name = nameof(ProductPolicyType.AgeBandPremium) },
                new CommonNameDTO() { Id = (int)ProductPolicyType.Other, Name = nameof(ProductPolicyType.Other) }
            };
            //.Add(new CommonNameDTO() { Id = int(Gender.Male), Name = nameof(Gender.Male) });

            return types;
        }
        public List<CommonNameDTO> GetPensionerIdType()
        {
            var types = new List<CommonNameDTO>()
            {
                //new CommonNameDTO() { Id = (int)PensionerIdType.AadharNumber, Name = nameof(PensionerIdType.AadharNumber) },
                new CommonNameDTO() { Id = (int)UserIdType.PANNumber, Name = nameof(UserIdType.PANNumber) },
            
            };
            //.Add(new CommonNameDTO() { Id = int(Gender.Male), Name = nameof(Gender.Male) });

            return types;
        }
        public List<CommonNameDTO> GetUserTypesuserscreen()
        {
            var userTypes = new List<CommonNameDTO>()
            {
                new CommonNameDTO() { Id = (int)UserType.Pensioner, Name = nameof(UserType.Pensioner) },
                new CommonNameDTO() { Id = (int)UserType.Community, Name = nameof(UserType.Community) },
                new CommonNameDTO() { Id = (int)UserType.Association, Name = nameof(UserType.Association) }
            };
            return userTypes;
        }

        public List<CommonNameDTO> GetUserTypes(int AssociationId)
        {
            var userTypes = new List<CommonNameDTO>();
            if (AssociationId > 0)
            {
                userTypes.Add(new CommonNameDTO() { Id = (int)UserType.Pensioner, Name = nameof(UserType.Pensioner) });

            }
            else
            {
                userTypes.Add(new CommonNameDTO() { Id = (int)UserType.Pensioner, Name = nameof(UserType.Pensioner) });
                userTypes.Add(new CommonNameDTO() { Id = (int)UserType.InternalUser, Name = nameof(UserType.InternalUser) });
                userTypes.Add(new CommonNameDTO() { Id = (int)UserType.Association, Name = nameof(UserType.Association) });
                userTypes.Add(new CommonNameDTO() { Id = (int)UserType.Community, Name = nameof(UserType.Community) });
                userTypes.Add(new CommonNameDTO() { Id = (int)UserType.DataCollection, Name = nameof(UserType.DataCollection) });

            }
            return userTypes;
        }
        public List<CommonNameDTO> GetPaymentStatus()
        {
            var userTypes = new List<CommonNameDTO>()
            {
                new CommonNameDTO() { Id = (int)PaymentStatus.Completed, Name = nameof(PaymentStatus.Completed) },
                 new CommonNameDTO() { Id = (int)PaymentStatus.Initiated, Name = nameof(PaymentStatus.Initiated) },
                new CommonNameDTO() { Id = (int)PaymentStatus.Failed, Name = nameof(PaymentStatus.Failed) },
                new CommonNameDTO() { Id = (int)PaymentStatus.Rejected, Name = nameof(PaymentStatus.Rejected) },
                 new CommonNameDTO() { Id = (int)PaymentStatus.Pending, Name = nameof(PaymentStatus.Pending) },
            };
            return userTypes;
        }
        public List<CommonNameDTO> GetPaymentModes()
        {
            var paymentModes = new List<CommonNameDTO>()
            {
               //new CommonNameDTO() { Id = (int)PaymentMode.Online, Name = nameof(PaymentMode.Online) },
                new CommonNameDTO() { Id = (int)PaymentMode.Offline, Name = nameof(PaymentMode.Offline) }
            };
            return paymentModes;
        }
        public List<CommonNameDTO> GetOfflinePaymentModes()
        {
            var paymentTypes = new List<CommonNameDTO>()
            {
                new CommonNameDTO() { Id = (int)PaymentTypes.Cheque, Name = nameof(PaymentTypes.Cheque) },
                new CommonNameDTO() { Id = (int)PaymentTypes.NEFT, Name = nameof(PaymentTypes.NEFT) },
                   new CommonNameDTO() { Id = (int)PaymentTypes.UPI, Name = nameof(PaymentTypes.UPI) },

            };
            return paymentTypes;
        }

        public List<CommonNameDTO> GetPaymentStatuss()
        {
            //Pending = 0, Completed = 1, Rejected = 2, Initiated = 3,Failed=4, Unknown=5
            var paymentStatus = new List<CommonNameDTO>()
            {
                new CommonNameDTO() { Id = (int)PaymentStatus.Pending, Name = nameof(PaymentStatus.Pending) },
                new CommonNameDTO() { Id = (int)PaymentStatus.Completed, Name = nameof(PaymentStatus.Completed) },
                new CommonNameDTO() { Id = (int)PaymentStatus.Rejected, Name = nameof(PaymentStatus.Rejected) },
                new CommonNameDTO() { Id = (int)PaymentStatus.Initiated, Name = nameof(PaymentStatus.Initiated) },
                new CommonNameDTO() { Id = (int)PaymentStatus.Failed, Name = nameof(PaymentStatus.Failed) },

            };
            return paymentStatus;
        }

        public List<CommonNameDTO> GetPaymentTypes()
        {
            var paymentTypes = new List<CommonNameDTO>()
            {
                new CommonNameDTO() { Id = (int)PaymentTypes.Cheque, Name = nameof(PaymentTypes.Cheque) },
                new CommonNameDTO() { Id = (int)PaymentTypes.NEFT, Name = nameof(PaymentTypes.NEFT) },
                   new CommonNameDTO() { Id = (int)PaymentTypes.UPI, Name = nameof(PaymentTypes.UPI) },
                   new CommonNameDTO() { Id = (int)PaymentTypes.Gateway, Name = nameof(PaymentTypes.Gateway) },

            };
            return paymentTypes;
        }


    }


}
