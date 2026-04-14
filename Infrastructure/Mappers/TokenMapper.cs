

using TokenDomain =  Domain.Entities.Token;
using TokenEntity =  Infrastructure.Persistence.Token;
namespace Infrastructure.Mappers;

public class TokenMapper
{
    public static TokenDomain MapToDomain(TokenEntity token)
    {
        return new TokenDomain
        {

            Id = token.Id,
            UserId = token.UserId,
            RefreshToken = token.RefreshToken,
            ExpiryDate = token.ExpiresAt,
            CreatedAt = token.CreatedAt
        };
    }

    public static TokenEntity MapToEntity(TokenDomain token)
    {
        return new TokenEntity
        {

                Id = Guid.NewGuid(),
                UserId = token.UserId,
                RefreshToken = token.RefreshToken,
                ExpiresAt = token.ExpiryDate,
                CreatedAt = DateTime.UtcNow
        };
    }
}   
   
