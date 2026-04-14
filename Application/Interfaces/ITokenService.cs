
namespace Application.Interfaces;

public interface ITokenService
{
    string CreateToken(Guid userId);
    string CreateRefreshToken();
}