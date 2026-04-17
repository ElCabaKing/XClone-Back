using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;


namespace Infrastructure.Repositories;

public class UserRepository(XDbContext context) 
    : GenericRepository<User>(context), IUserRepository
{
    public async Task<bool> UsernameOrEmailExists(string username, string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username || u.Email == email);

        return user != null;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id) ??
            throw new NotFoundException(ResponseConstants.NOT_FOUND);
        return UserMapper.MapToDomain(user);
    }
}