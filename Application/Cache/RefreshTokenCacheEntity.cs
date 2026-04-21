using Application.Interfaces;

namespace Application.Cache;

public class RefreshTokenCacheEntity : ITokenCacheEntity
{
    public string TokenHash { get; init; } = string.Empty;
    public Guid UserId { get; init; } = Guid.Empty;
    public string Purpose { get; init; } = "refresh-token";
    public DateTime ExpiresAt { get; init; } = DateTime.UtcNow.AddDays(10);
}
