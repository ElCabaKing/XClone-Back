

using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Modules.Auth.Login;

public class LoginHandler(ITokenService tokenService,
IUserRepository userRepository,
IPasswordService passwordService)
{
    public async Task<LoginResponse> Handle(LoginCommand command)
    {
        var user = await userRepository.GetByUsernameAsync(command.Username);

        if (user == null || !passwordService.VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var token = tokenService.CreateToken(user.Id);
        return new LoginResponse
        {
            Token = token,
            RefreshToken = "sercies.CreateRefreshToken(user.Id)"
        };
    }
}