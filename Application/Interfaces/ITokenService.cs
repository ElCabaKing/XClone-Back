
namespace Application.Interfaces;

public interface ITokenService
{
    string CreateToken(string userId);
    string CreateRefreshToken(string userId);
}