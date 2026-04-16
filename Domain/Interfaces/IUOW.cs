
namespace Domain.Interfaces;

public interface IUOW
{
    IUserRepository UserRepository { get; }
    ITokenRepository TokenRepository { get; }
    Task SaveChangesAsync();
}