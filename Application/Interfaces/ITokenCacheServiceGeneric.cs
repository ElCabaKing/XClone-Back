
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
    Task<T?> GetAndDeleteAsync(T entity); // obtiene y elimina el token
    Task<bool> ExistsAsync( string purpose, Guid userId); // verifica si el token existe

}