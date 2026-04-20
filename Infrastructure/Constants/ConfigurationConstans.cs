namespace Infrastructure.Constants;

public class ConfigurationConstants
{
    // Cloudinary configuration keys
    public const string CloudinaryCloudName = "Cloudinary:CloudName";
    public const string CloudinaryApiKey = "Cloudinary:ApiKey";
    public const string CloudinaryApiSecret = "Cloudinary:ApiSecret";
    // JWT configuration keys
    public const string JwtKey = "JWT:Key";
    public const string JwtIssuer = "JWT:Issuer";
    public const string JwtAudience = "JWT:Audience";
    public const string JwtExpireMinutes = "JWT:ExpireMinutes";
    // MongoDB configuration key
    public const string MongoConnectionString = "MongoDb:ConnectionString";
    public const string ConnectionString = "DefaultConnection";

    // Mail configuration keys
    public const string MailSmtpKey = "Mail:SmtpKey";
    public const string MailServiceMail = "Mail:ServiceMail";
    public const string MailSmtpHost = "Mail:SmtpHost";
    public const string MailSmtpPort = "Mail:SmtpPort";
    public const string MailEmailFrom = "Mail:EmailFrom";
    

}