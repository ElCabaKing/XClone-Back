

using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Constants;

namespace Application.Modules.Auth.Login;

public class LoginHandler(ITokenService tokenService,
IUOW uow,
IPasswordService passwordService) 
{
    /// <summary>
    /// Maneja la autenticacion de un usuario existente
    /// </summary>
    public async Task<LoginResponse> Handle(LoginCommand command)
    {
        var user = await uow.UserRepository.GetByUsernameorEmailAsync(command.Credential);

        if (user == null || !passwordService.VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException(ResponseConstants.INVALID_CREDENTIALS);
        }

        var token = tokenService.CreateToken(user.Id);
        var refreshToken = tokenService.CreateRefreshToken();
        await uow.TokenRepository.StoreRefreshTokenAsync(new Token
        {
            UserId = user.Id,
            RefreshToken = refreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        });

        return new LoginResponse
        {
            Token = token,
            RefreshToken = refreshToken
        };
    }
}