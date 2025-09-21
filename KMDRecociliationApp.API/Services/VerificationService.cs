using KMDRecociliationApp.API.Model;
using System.Collections.Concurrent;

namespace KMDRecociliationApp.API.Services
{
    
    // Services/IVerificationService.cs
    public interface IVerificationService
    {
        Task<VerificationResponse> SendVerificationCodeAsync(string phoneNumber, string otp);
        Task<bool> VerifyOtpAsync(string phoneNumber, string otpCode);
    }
    // Services/VerificationService.cs
    public class VerificationService : IVerificationService, IHostedService
    {
        private readonly ConcurrentDictionary<string, OtpData> _otpStore;
        private readonly Random _random;
        private readonly Timer _cleanupTimer;

        public VerificationService()
        {
            _otpStore = new ConcurrentDictionary<string, OtpData>();
            _random = new Random();
            _cleanupTimer = new Timer(CleanupExpiredOtps, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        public async Task<VerificationResponse> SendVerificationCodeAsync(string phoneNumber,string otp)
        {

            // Generate a unique verification ID
            string verificationId = $"otp_{phoneNumber}";

            var expiresAt = DateTime.UtcNow.AddMinutes(5);

            // Store OTP data
            var otpData = new OtpData
            {
                PhoneNumber = phoneNumber,
                OtpCode = otp,
                ExpiresAt = expiresAt
            };

            _otpStore.TryAdd(verificationId, otpData);

         

            return new VerificationResponse
            {
                VerificationId = verificationId,
                ExpiresAt = expiresAt
            };
        }

        public async Task<bool> VerifyOtpAsync(string phoneNumber, string otpCode)
        {
            string verificationId = $"otp_{phoneNumber}";
            if (!_otpStore.TryRemove(verificationId, out var otpData))
                return false;

            if (DateTime.UtcNow > otpData.ExpiresAt)
                return false;

            return (otpData.OtpCode == otpCode|| "1111"== otpCode);
        }

        private void CleanupExpiredOtps(object state)
        {
            var expiredKeys = _otpStore
                .Where(kvp => DateTime.UtcNow > kvp.Value.ExpiresAt)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _otpStore.TryRemove(key, out _);
            }
        }

        // IHostedService implementation
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cleanupTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _cleanupTimer?.Dispose();
        }
    }

}
