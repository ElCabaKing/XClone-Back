

using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.Mappers;

namespace Infrastructure.Repositories;

public class TokenRepository (XDbContext context)
    : GenericRepository<Token>(context), ITokenRepository
{
    public async Task DeleteRefreshTokenAsync(Guid userId)
    {
        var tokenEntity = _context.Tokens.FirstOrDefault(t => t.UserId == userId);
        if (tokenEntity != null)        {
            _context.Tokens.Remove(tokenEntity);
        }
    }

    public async Task<string?> GetRefreshTokenAsync(Guid userId)
    {
        var tokenEntity = _context.Tokens.FirstOrDefault(t => t.UserId == userId);
        return tokenEntity?.RefreshToken;
    }

    public async Task StoreRefreshTokenAsync(Token token)
    {
        var tokenEntity = TokenMapper.MapToEntity(token);
        await _context.Tokens.AddAsync(tokenEntity);
    }
}