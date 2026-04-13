namespace Application.Modules.Auth.Login;

public class LoginResponse
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }

}