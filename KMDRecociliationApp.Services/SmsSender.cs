using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace KMDRecociliationApp.Services
{
    public interface ISmsSender
    {
        public  Task<string> SendMessageAsync(string number, string message);
    }
    public class SmsSender : ISmsSender
    {
        private readonly IConfiguration _configuration;

        public SmsSender(IConfiguration configuration)
        {
            _configuration = configuration;
            //TwilioClient.Init(_configuration["Twilio:AccountSid"], _configuration["Twilio:AuthToken"]);
        }

        public async Task<string> SendMessageAsync(string number, string otp)
        {
            string sms = "";
            
            return await Task.FromResult(sms);
        }
    }
}
