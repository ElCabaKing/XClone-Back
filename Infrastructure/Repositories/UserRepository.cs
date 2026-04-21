using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.Constants;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;


namespace Infrastructure.Repositories;

public class UserRepository(XDbContext context)
        : GenericRepository<User>(context), IUserRepository
{
    public async Task<bool> UsernameOrEmailExists(string username, string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                u => u.Username == username ||
                u.Email == email) != null;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }


    public async Task<User?> Register(User user)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == RoleConstants.COMMON_ROLE) ??
            throw new NotFoundException(ResponseConstants.NOT_FOUND);

        user.Roles.Add(role);

        _context.Users.Add(user);

        return user;
    }

}