
using Domain.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.Repositories;

namespace Infrastructure.Unity;

public class UOW : IUOW
{
    private readonly XDbContext _context;
    private IUserRepository? _userRepository;
    private ITokenRepository? _tokenRepository;
    private IEmailTemplateRepository? _emailTemplateRepository;

    public UOW(XDbContext context)
    {
        _context = context;
    }
    public ITokenRepository TokenRepository => _tokenRepository ??= new TokenRepository(_context);

    public IEmailTemplateRepository EmailTemplateRepository => _emailTemplateRepository ??= new EmailTemplateRepository(_context);
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }


}