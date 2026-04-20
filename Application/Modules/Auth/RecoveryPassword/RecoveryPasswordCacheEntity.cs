using Application.Interfaces;

namespace Application.Modules.Auth.RecoveryPassword;

public class RecoveryPasswordCacheEntity : ITokenCacheEntity
{
    public string TokenHash { get; init; } = string.Empty;
    public Guid UserId { get; init; } = Guid.Empty;
    public string Purpose { get; init; } = "password-recovery";
    public DateTime ExpiresAt { get; init; } = DateTime.UtcNow.AddHours(1);
}
