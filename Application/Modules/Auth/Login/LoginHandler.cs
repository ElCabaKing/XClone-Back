

using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Modules.Auth.Login;

public class LoginHandler(ITokenService tokenService,
IUserRepository userRepository,
IPasswordService passwordService)
{
    /// <summary>
    /// Maneja la autenticacion de un usuario existente
    /// </summary>
    public async Task<LoginResponse> Handle(LoginCommand command)
    {
        var user = await userRepository.GetByUsernameorEmailAsync(command.Credential);

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