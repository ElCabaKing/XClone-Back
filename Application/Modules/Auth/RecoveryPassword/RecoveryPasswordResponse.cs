

namespace Application.Modules.Auth.RecoveryPassword;

public class RecoveryPasswordResponse(string message)
{
    public string Message { get; set; } = message;
}
