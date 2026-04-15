namespace Shared.Constants;

public class ResponseConstants
{
    public const string NOT_FOUND = "El recurso solicitado no fue encontrado.";
    public const string BAD_REQUEST = "La solicitud es inválida.";

    public const string INTERNAL_SERVER_ERROR = "Ocurrió un error interno en el servidor.";
    public static string ERROR_UNEXPECTED(string traceId)
    {
        return $"Ocurrió un error inesperado. TraceId: {traceId}";
    }
    public const string INVALID_CREDENTIALS = "Credenciales inválidas.";
    public const string TOO_MANY_REQUESTS = "Has excedido el número máximo de solicitudes. Por favor, inténtalo de nuevo más tarde.";

    public const string EMAIL_USERNAME_ALREADY_EXISTS = "El nombre de usuario o el correo electrónico ya existen.";
    public static string CLOUD_ERROR(string message)
    {
        return $"Error uploading media file.{message}";
    }
    public const string JWT_CONFIG_ERROR = "JWT configuration is missing or invalid.";
    public const string HASHING_ERROR = "Error en el proceso de hashing.";
    public const string USER_CREATION_ERROR = "Error al crear el usuario.";

}