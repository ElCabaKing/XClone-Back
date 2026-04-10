namespace Infrastructure.Constants;

public static class ServicesResponseConstants
{
    public static string CLOUD_ERROR(string message) { 
        return $"Error uploading media file.{message}"; 
        }
}