

using Application.Cache;
using Application.Interfaces;
using Domain.Interfaces;
using Shared.Helpers;

namespace Application.Modules.Auth.RecoveryPassword;

public class RecoveryPasswordHandler(
    IUserRepository userRepository,
    IEmailService emailService,
    ITokenCacheServiceGeneric<RecoveryPasswordCacheEntity> tokenCacheService,
    IPasswordService passwordService

)
{
    public async Task<RecoveryPasswordResponse> Handle(RecoveryPasswordCommand command)
    {
        var user = await userRepository.FirstOrDefaultAsync(u => u.Email == command.Email);

        if (user == null)
        {
            return new RecoveryPasswordResponse("Si el correo electrónico existe en nuestro sistema, se enviará un enlace de recuperación.");
        }
        var token = TokenHelper.GenerateRefreshToken(64);
        var tokenHash = passwordService.HashPassword(token);
        await tokenCacheService.SaveAsync(new RecoveryPasswordCacheEntity { TokenHash = tokenHash, UserId = user.Id });
        await emailService.SendEmailAsync(user.Email, token, $"ola aqui su token {token}"  );
        return new RecoveryPasswordResponse("Si el correo electrónico existe en nuestro sistema, se enviará un enlace de recuperación.");
    }
}