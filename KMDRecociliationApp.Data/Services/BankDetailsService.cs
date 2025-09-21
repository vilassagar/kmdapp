using AutoMapper;
using KMDRecociliationApp.Data.Repositories.Interfaces.KMD.EnrolmentPortal.Repositories.Interfaces;
using KMDRecociliationApp.Data.Services.Interfaces;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.Entities;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Services
{
    public class BankDetailsService : IBankDetailsService
    {
        private readonly IBankDetailsRepository _bankDetailsRepository;
        private readonly IMapper _mapper;

        public BankDetailsService(IBankDetailsRepository bankDetailsRepository, IMapper mapper)
        {
            _bankDetailsRepository = bankDetailsRepository;
            _mapper = mapper;
        }

        public async Task<BankDetailsDto> GetBankDetailsByIdAsync(int id)
        {
            var bankDetails = await _bankDetailsRepository.GetByIdAsync(id);
            if (bankDetails == null)
                return null;

            return _mapper.Map<BankDetailsDto>(bankDetails);
        }

        public async Task<BankDetailsDto> GetBankDetailsByApplicantIdAsync(int applicantId)
        {
            var bankDetails = await _bankDetailsRepository.GetBankDetailsByApplicantIdAsync(applicantId);
            if (bankDetails == null)
                return null;

            return _mapper.Map<BankDetailsDto>(bankDetails);
        }

        public async Task<BankDetailsDto> CreateBankDetailsAsync(BankDetailsDto bankDetailsDto)
        {
            var bankDetails = _mapper.Map<ApplicantBankDetails>(bankDetailsDto);
            bankDetails.CreatedAt = DateTime.UtcNow;

            await _bankDetailsRepository.AddAsync(bankDetails);
            await _bankDetailsRepository.SaveChangesAsync();

            return _mapper.Map<BankDetailsDto>(bankDetails);
        }

        public async Task<BankDetailsDto> UpdateBankDetailsAsync(int id, BankDetailsDto bankDetailsDto)
        {
            var existingBankDetails = await _bankDetailsRepository.GetByIdAsync(id);
            if (existingBankDetails == null)
                return null;

            _mapper.Map(bankDetailsDto, existingBankDetails);
            existingBankDetails.UpdatedAt = DateTime.UtcNow;

            await _bankDetailsRepository.UpdateAsync(existingBankDetails);
            await _bankDetailsRepository.SaveChangesAsync();

            return _mapper.Map<BankDetailsDto>(existingBankDetails);
        }

        public async Task DeleteBankDetailsAsync(int id)
        {
            var bankDetails = await _bankDetailsRepository.GetByIdAsync(id);
            if (bankDetails == null)
                return;

            await _bankDetailsRepository.DeleteAsync(bankDetails);
            await _bankDetailsRepository.SaveChangesAsync();
        }
    }
}
