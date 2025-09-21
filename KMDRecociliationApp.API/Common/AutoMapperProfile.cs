using AutoMapper;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.DTO.InsurerData;
using KMDRecociliationApp.Domain.Entities;

namespace KMDRecociliationApp.API.Common
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //// Applicant mappings
            //CreateMap<ApplicantInsurancePolicy, ApplicantDto>();
            // Applicant mappings
            CreateMap<ApplicantInsurancePolicy, ApplicantDto>()
                   .ForMember(dest => dest.BankDetails, opt => opt.Ignore())
                .ForMember(dest => dest.Dependents, opt => opt.Ignore())
                .ForMember(dest => dest.GenderName, opt => opt.MapFrom(src => src.Gender.ToString()))
             .ForMember(dest => dest.IdCardTypeName, opt => opt.MapFrom(src => src.IdCardType.ToString()));



            CreateMap<ApplicantDto, ApplicantInsurancePolicy>()
                .ForMember(dest => dest.BankDetails, opt => opt.Ignore())
                .ForMember(dest => dest.Dependents, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Bank details mappings
            CreateMap<ApplicantBankDetails, BankDetailsDto>();

            CreateMap<BankDetailsDto, ApplicantBankDetails>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Applicant, opt => opt.Ignore());

            // Dependent mappings
            CreateMap<ApplicantDependent, DependentDto>();
            CreateMap<DependentDto, ApplicantDependent>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Applicant, opt => opt.Ignore());
        }
    }
}
