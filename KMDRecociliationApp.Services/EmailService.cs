using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text.Json;
namespace KMDRecociliationApp.Services
{
    public interface IEmailService
    {
        public Task<bool> SendEmailAsync(string emails, string message,string subject);
    }
    public class EmailService: IEmailService
    {

        public async Task<bool> SendEmailAsync(string emails, string message, string subject)
        {
            //Email email = CreateEmail(emails, message, subject);
            //var content = ConvertEmailToStringContent(email);
            //var client = new HttpClient();
            //var request = new HttpRequestMessage
            //{
            //    Method = HttpMethod.Post,
            //    RequestUri = new Uri("https://emailapi.netcorecloud.net/v5.1/mail/send"),
            //    Headers =
            //        {
            //            { "api_key", "cabbf5ad13bac20aa3d0b27c2f86f9b1" },
            //            { "Accept", "application/json" },
            //        },
            //    //Content = content,
            //    Content = new StringContent(content)
            //    {
            //        Headers =
            //            {
            //                ContentType = new MediaTypeHeaderValue("application/json")
            //            }
            //    }
            //};
            //using (var response = await client.SendAsync(request))
            //{
            //    response.EnsureSuccessStatusCode();
            //    var body = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine(body);
            //}
            return true;
        }

        //public string ConvertEmailToStringContent(Email email)
        //{
        //    // Serialize the email object to JSON
        //    var jsonString = JsonSerializer.Serialize(email, new JsonSerializerOptions
        //    {
        //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Use camelCase for property names
        //        WriteIndented = false // Optional: pretty print disabled for smaller payload
        //    });




        //    return jsonString;
        //}

        //public Email CreateEmail(string email, string message, string subject)
        //{
        //    return new Email
        //    {
        //        //From = new FromInfo
        //        //{
        //        //    Email = "Retiree.mediclaim@kmdastur.com",
        //        //    Name = "Retiree mediclaim"
        //        //},
        //        //ReplyTo = email,
        //        //Subject = subject,
        //        //TemplateId = 0,
        //        ////Tags = new List<string> { "Pawnee" },
        //        //Content = new List<Content>
        //        //{
        //        //    new Content
        //        //    {
        //        //        Type = "html",
        //        //        Value = message,
        //        //    }
        //        //},
        //        //        Attachments = new List<Attachment>
        //        //{
        //        //    new Attachment
        //        //    {
        //        //        Name = "example.txt",
        //        //        Content = "base64 encoded file content"
        //        //    }
        //        //},
        //        //        Personalizations = new List<Personalization>
        //        //{
        //        //    new Personalization
        //        //    {
        //        //        Attributes = new Attributes
        //        //        {
        //        //            Lead = "Andy Dwyer",
        //        //            Band = "Mouse Rat"
        //        //        },
        //        //        To = new List<To>
        //        //        {
        //        //            new To { Email = "ann.perkins@parksnrec.com", Name = "Ann Perkins" }
        //        //        },
        //        //        Cc = new List<Cc>
        //        //        {
        //        //            new Cc { Email = "april.ludgate@parksnrec.com" },
        //        //            new Cc { Email = "ben.white@parksnrec.com" }
        //        //        },
        //        //        Bcc = new List<Bcc>
        //        //        {
        //        //            new Bcc { Email = "jerry.gergich@parksnrec.com" }
        //        //        },
        //        //        TokenTo = "noble-land-mermaid",
        //        //        TokenCc = "MSGID657243",
        //        //        TokenBcc = "MSGID657244",
        //        //        Attachments = new List<Attachment>
        //        //        {
        //        //            new Attachment
        //        //            {
        //        //                Name = "example.pdf",
        //        //                Content = "base64 encoded file content"
        //        //            }
        //        //        }
        //        //    }
        //        //},
        //        //Settings = new Settings
        //        //{
        //        //    OpenTrack = true,
        //        //    ClickTrack = true,
        //        //    UnsubscribeTrack = true,
        //        //    IpPool = "pool0"
        //        //},
        //        //        Bcc = new List<Bcc>
        //        //{
        //        //    new Bcc { Email = "donna.meagle@parksnrec.com" }
        //        //},
        //        // Schedule = 1589908859
        //    };
        //}
    }

    //public class Email
    //{

    //    public FromInfo From { get; set; }
    //    public string ReplyTo { get; set; }
    //    public string Subject { get; set; }
    //    public int TemplateId { get; set; }
    //    //public List<string> Tags { get; set; }
    //    public List<Content> Content { get; set; }
    //    //public List<Attachment> Attachments { get; set; }
    //    //public List<Personalization> Personalizations { get; set; }
    //    //public Settings Settings { get; set; }
    //    //public List<Bcc> Bcc { get; set; }
    //    //public long Schedule { get; set; }
    //}

    //public class FromInfo
    //{
    //    public string Email { get; set; }
    //    public string Name { get; set; }
    //}

    //public class Content
    //{
    //    public string Type { get; set; }
    //    public string Value { get; set; }
    //}

    //public class Attachment
    //{
    //    public string Name { get; set; }
    //    public string Content { get; set; }
    //}

    //public class Personalization
    //{
    //    public Attributes Attributes { get; set; }
    //    public List<To> To { get; set; }
    //    public List<Cc> Cc { get; set; }
    //    public List<Bcc> Bcc { get; set; }
    //    public string TokenTo { get; set; }
    //    public string TokenCc { get; set; }
    //    public string TokenBcc { get; set; }
    //    public List<Attachment> Attachments { get; set; }
    //}

    //public class Attributes
    //{
    //    public string Lead { get; set; }
    //    public string Band { get; set; }
    //}

    //public class To
    //{
    //    public string Email { get; set; }
    //    public string Name { get; set; }
    //}

    //public class Cc
    //{
    //    public string Email { get; set; }
    //}

    //public class Bcc
    //{
    //    public string Email { get; set; }
    //}

    //public class Settings
    //{
    //    public bool OpenTrack { get; set; }
    //    public bool ClickTrack { get; set; }
    //    public bool UnsubscribeTrack { get; set; }
    //    public string IpPool { get; set; }
    //}

}
