
namespace Application.Modules.Auth.Login;

public class LoginCommand(
    string username,
    string password
)
{
    public string Username { get; set; } = username;
    public string Password { get; set; } = password;
}