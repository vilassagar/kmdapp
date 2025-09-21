using DocumentFormat.OpenXml.InkML;
using KMDRecociliationApp.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Services
{
    public class WhatsAppMessageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _bearerToken;
        private readonly string _recipientType;
        private readonly string _messageType;
        private readonly string _source;
        private readonly string _xApiHeader;
        private readonly string _typeMediaTemplateType;
        private readonly string _KMDPortalURL;


        public WhatsAppMessageService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiUrl = configuration["WhatsappRequestObject:url"];
            _bearerToken = configuration["WhatsappRequestObject:Authorization"];
            _recipientType = configuration["WhatsappRequestObject:recipient_type"];
            _messageType = configuration["WhatsappRequestObject:message_type"];
            _source = configuration["WhatsappRequestObject:source"];
            _xApiHeader = configuration["WhatsappRequestObject:x-apiheader"];
            _typeMediaTemplateType = configuration["WhatsappRequestObject:type_media_templatetype"];
            _KMDPortalURL= configuration["WhatsappRequestObject:KMDPortalURL"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", _bearerToken);
        }

        public async Task<bool> SendWhatsAppMessage(string recipient,string messageTemplateName
            ,string _typeMediaTemplateUrl
            ,string _typeMediaTemplateFilename)
        {
           
            int uploadsIndex = _typeMediaTemplateUrl.IndexOf("Uploads");
            string result = "";
            if (uploadsIndex != -1)
            {
                // Get everything from "Uploads" onwards (including "Uploads")
                 result = _typeMediaTemplateUrl.Substring(uploadsIndex);
              
            }
            var templatedocurl = _KMDPortalURL + result;
            var payload = new
            {
                message = new[]
                {
                new
                {
                    recipient_whatsapp = recipient,
                    recipient_type = _recipientType,
                    message_type = _messageType,
                    source = _source,
                    x_apiheader = _xApiHeader,
                    type_media_template = new
                    {
                        type = _typeMediaTemplateType,
                        url = templatedocurl,
                        filename = _typeMediaTemplateFilename
                    },
                    type_template = new[]
                    {
                        new
                        {
                            name = messageTemplateName,
                            language = new
                            {
                                locale = "en",
                                policy = "deterministic"
                            }
                        }
                    }
                }
            }
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiUrl, content);
            return response.IsSuccessStatusCode;
        }
    }

}
