

namespace Application.Modules.Auth.RecoveryPassword;

public class RecoveryPasswordCommand(
    string email
)
{
    public string Email { get; set; } = email!;
}