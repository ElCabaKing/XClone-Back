namespace Infrastructure.Constants;

public static class ServicesResponseConstants
{
    public static string CLOUD_ERROR(string message) { 
        return $"Error uploading media file.{message}"; 
        }
    public const string JWT_CONFIG_ERROR = "JWT configuration is missing or invalid.";
}