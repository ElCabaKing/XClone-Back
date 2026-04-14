
namespace Application.Modules.Auth.Login;

public class LoginCommand(
    string credential,
    string password
)
{
    public string Credential { get; set; } = credential;
    public string Password { get; set; } = password;
}