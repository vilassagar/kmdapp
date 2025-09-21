namespace KMDRecociliationApp.API.Model
{
    public class VerificationResponse
    {
        public string VerificationId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
    public class OtpData
    {
        public string PhoneNumber { get; set; }
        public string OtpCode { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
