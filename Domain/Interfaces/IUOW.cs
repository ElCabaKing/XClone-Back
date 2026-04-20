
namespace Domain.Interfaces;

public interface IUOW
{
    IUserRepository UserRepository { get; }
    IEmailTemplateRepository EmailTemplateRepository { get; }
    Task SaveChangesAsync();
}