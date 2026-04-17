
using Domain.Entities;

namespace Domain.Interfaces;

public interface ITokenRepository : IGenericRepository<Token>
{
    public Task StoreRefreshTokenAsync(Token token);
    public Task<string?> GetRefreshTokenAsync(Guid userId);
    public Task DeleteRefreshTokenAsync(Guid userId);
}