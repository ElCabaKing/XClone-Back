
namespace Application.Interfaces;


public interface ITokenCacheEntity
{
    string TokenHash { get; }
    Guid UserId { get; }
    string Purpose { get; }
    DateTime ExpiresAt { get; }
}
public interface ITokenCacheServiceGeneric<T> where T : ITokenCacheEntity
{
    Task SaveAsync(T entity); // guarda con TTL según entity.ExpiresAt
    Task<T?> VerifyAndConsumeAsync(Guid userId);
}