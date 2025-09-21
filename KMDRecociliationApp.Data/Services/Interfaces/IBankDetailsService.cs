using KMDRecociliationApp.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Services.Interfaces
{
    public interface IBankDetailsService
    {
        Task<BankDetailsDto> GetBankDetailsByIdAsync(int id);
        Task<BankDetailsDto> GetBankDetailsByApplicantIdAsync(int applicantId);
        Task<BankDetailsDto> CreateBankDetailsAsync(BankDetailsDto bankDetailsDto);
        Task<BankDetailsDto> UpdateBankDetailsAsync(int id, BankDetailsDto bankDetailsDto);
        Task DeleteBankDetailsAsync(int id);
    }
}
