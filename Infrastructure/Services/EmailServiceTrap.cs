using Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Infrastructure.Services;

public class EmailServiceMailTrap : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress("Test", "test@example.com"));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        email.Body = new TextPart("html")
        {
            Text = body
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            "sandbox.smtp.mailtrap.io",
            2525,
            SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            "a14267feae481d",
            "91bb3c6e8164fe"
        );

        await smtp.SendAsync(email);

        await smtp.DisconnectAsync(true);

        Console.WriteLine("Sent");
    }
}