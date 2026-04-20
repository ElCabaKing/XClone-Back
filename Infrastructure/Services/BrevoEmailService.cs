using Application.Interfaces;
using Infrastructure.Settings;
using MailKit.Net.Smtp;
using MimeKit;

namespace Infrastructure.Services;
public class BrevoEmailService (BrevoSettings brevoSettings): IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(brevoSettings.EmailFrom));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        email.Body = new TextPart("html")
        {
            Text = body
        };

        using var smtp = new SmtpClient();

        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

        await smtp.ConnectAsync(
        brevoSettings.SmtpHost, 
        brevoSettings.SmtpPort, 
        MailKit.Security.SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            brevoSettings.ServiceMail,
            brevoSettings.SmtpKey
        );

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}