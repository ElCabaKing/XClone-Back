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
    public const string TOO_MANY_REQUESTS = "Has excedido el número máximo de solicitudes. Por favor, inténtalo de nuevo más tarde.";

    public const string EMAIL_USERNAME_ALREADY_EXISTS = "El nombre de usuario o el correo electrónico ya existen.";
}