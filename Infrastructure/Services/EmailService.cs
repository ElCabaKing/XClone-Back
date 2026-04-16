
using Application.Interfaces;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Services;

public class EmailService(MailjetClient mailjetClient) : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var request = new MailjetRequest
        {
            Resource = SendV31.Resource
        }
                .Property(Send.Messages, new JArray
                {
            new JObject
            {
                {
                    "From", new JObject
                    {
                        { "Email", "davi.cabanilla@gmail.com" },
                        { "Name", "Tu App" }
                    }
                },
                {
                    "To", new JArray
                    {
                        new JObject
                        {
                            { "Email", to }
                        }
                    }
                },
                { "Subject", subject },
                { "TextPart", body }
            }
                });

        var response = await mailjetClient.PostAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error sending email with Mailjet");
        }
    }
}