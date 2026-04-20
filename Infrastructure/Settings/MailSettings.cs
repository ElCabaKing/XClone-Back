
namespace Infrastructure.Settings;
public class BrevoSettings
{
    public string SmtpHost { get; set; } = default!;
    public int SmtpPort { get; set; }
    public string ServiceMail { get; set; } = default!;
    public string SmtpKey { get; set; } = default!;
    public string EmailFrom { get; set; } = default!;           
}