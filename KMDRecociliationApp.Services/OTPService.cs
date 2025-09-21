using KMDRecociliationApp.Data;
using KMDRecociliationApp.Domain.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Net.Http.Headers;
namespace KMDRecociliationApp.Services
{
    public class OTPService
    {
        private readonly ApplicationDbContext _context;

        public OTPService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GenerateOTP()
        {
            var random = new Random();
            return random.Next(1000, 10000).ToString();
        }

        //public async Task<bool> SendOTP(string phoneNumber, string otp, SMSRequestObject sMSRequest)
        //{
        //    var client = new RestClient("https://bulkpush.mytoday.com");
        //    var request = new RestRequest("BulkSms/JsonSingleApi", Method.Post);
        //    // Convert the object to JSON
        //    sMSRequest.message = string.Format(sMSRequest.message, otp);
        //    sMSRequest.mobile=phoneNumber;
        //    string jsonString = JsonSerializer.Serialize(sMSRequest);
        //    request.AddHeader("Content-Type", "application/json");

        //    request.AddJsonBody(jsonString);
        //    //request.AddJsonBody(sMSRequest);

        //    // Execute the request asynchronously
        //    var response = await client.ExecuteAsync(request);
        //    if (response.IsSuccessful)
        //    {
        //        return true;
        //    }
        //    else {

        //        // 
        //        // Use Twilio or any SMS service to send OTP
        //        // Twilio example:
        //        // var client = new TwilioRestClient(accountSid, authToken);
        //        // client.Messages.Create(to: phoneNumber, from: "Your_Twilio_Number", body: $"Your OTP is {otp}");
        //        Console.WriteLine($"Error: {response.ErrorMessage}, Status Code: {response.StatusCode}");
        //        return false;
        //    }
        //}

        public async Task<(HttpResponseMessage, string)> SendOTP(string phoneNumber, string otp, SMSRequestObject sMSRequest)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                // Set the base address of the client
                client.BaseAddress = new Uri("https://bulkpush.mytoday.com");

                // Set headers if needed, such as Content-Type
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Prepare the request data
                //sMSRequest.messages = string.Format(sMSRequest.messages, otp);
                sMSRequest.messages = "Hi, Your OTP to login is " + otp + ". For security reasons do not share the OTP  with anyone.-K M DASTUR REINSURANCE BROKERS PVT LTD";
                sMSRequest.mobile = long.Parse(phoneNumber);

                // Serialize the SMS request object to JSON
                var jsonRequest = JsonSerializer.Serialize(sMSRequest);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
               
                try
                {
                    // Make the POST request
                     response = await client.PostAsync("/BulkSms/JsonSingleApi", content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        return (response, jsonRequest);  // Success
                    }
                    else
                    {
                        // Log the error status code
                        Console.WriteLine($"Error: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                        return (response, jsonRequest);
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception details for debugging
                    Console.WriteLine($"Exception: {ex.Message}");
                    return (response, jsonRequest);
                }
            }
        }

        public async Task<(HttpResponseMessage, string)> SendEmail(string eMail, string otp)
        {
            //EmailRequestObject emailRequestObject=new EmailRequestObject();
            HttpResponseMessage response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                // Set the base address of the client
                client.BaseAddress = new Uri("https://emailapi.netcorecloud.net");

                // Set headers if needed, such as Content-Type
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("api_key", "cabbf5ad13bac20aa3d0b27c2f86f9b1");


                string emailId = eMail;
                // Prepare the request data
                //sMSRequest.messages = string.Format(sMSRequest.messages, otp);
                //From from=new From();
                //from.email = "noreply@kmdastur.com";
                //emailRequestObject.from = from;
                //emailRequestObject.subject = "KMD : Login OTP";
                //emailRequestObject.content[0].type = "html";
                //emailRequestObject.content[0].value = "Hi, Your OTP to login is " + otp + ". For security reasons do not share the OTP  with anyone.-K M DASTUR REINSURANCE BROKERS PVT LTD";
                //emailRequestObject.personalizations[0].to[0].email = emailId;
                //emailRequestObject.personalizations[0].to[0].name = GetNameFromEmail(emailId);

                EmailRequestObject emailRequestObject = new EmailRequestObject
                {
                    from = new From
                    {
                        email = "noreply@kmdastur.com" // Default email, can be changed if needed
                    },
                    subject = "KMD : Login OTP",
                    content = new List<Content>
                    {
                        new Content
                        {
                            type = "html", // Or "text/html" depending on the content type
                            value = "Hi, Your OTP to login is " + otp + ". For security reasons do not share the OTP  with anyone.-K M DASTUR REINSURANCE BROKERS PVT LTD.\n",
                        }
                    },
                    personalizations = new List<Personalization>
                    {
                        new Personalization
                        {
                            to = new List<To>
                            {
                                new To
                                {
                                    email = emailId,
                                    name = GetNameFromEmail(emailId)
                                }
                                // Add more recipients if needed
                            }
                        }
                    }
                };


                // Serialize the SMS request object to JSON
                var jsonRequest = JsonSerializer.Serialize(emailRequestObject);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                try
                {
                    // Make the POST request
                    response = await client.PostAsync("/v5.1/mail/send", content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        return (response, jsonRequest);  // Success
                    }
                    else
                    {
                        // Log the error status code
                        Console.WriteLine($"Error: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                        return (response, jsonRequest);
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception details for debugging
                    Console.WriteLine($"Exception: {ex.Message}");
                    return (response, jsonRequest);
                }
            }


        }

        public string GetNameFromEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                return string.Empty;

            // Get the part before the "@" symbol
            string username = email.Split('@')[0];

            // Replace dots or underscores with spaces and capitalize each word
            string[] nameParts = username.Split(new[] { '.', '_' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < nameParts.Length; i++)
            {
                nameParts[i] = char.ToUpper(nameParts[i][0]) + nameParts[i].Substring(1).ToLower();
            }

            return string.Join(" ", nameParts);
        }

    }

}
