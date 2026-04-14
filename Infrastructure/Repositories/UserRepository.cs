using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;


namespace Infrastructure.Repositories;

public class UserRepository(XDbContext context) : IUserRepository
{
    public async Task<User> CreateUserAsync(User user)
    {
        var userEntity = UserMapper.MapToEntity(user);
        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByUsernameorEmailAsync(string credential)
    {
        var userEntity = await context.Users.FirstOrDefaultAsync(u => u.Username == credential || u.Email == credential);
        if (userEntity == null) throw new NotFoundException(ResponseConstants.NOT_FOUND);
        return UserMapper.MapToDomain(userEntity);
    }

    public async Task<bool> UsernameOrEmailExists(string username, string email)
    {
        var exists = await context.Users.AnyAsync(u => u.Username == username || u.Email == email);
        return exists;
    }

    public async Task<User?> UpdateUserAsync(User user)
    {
        var userEntity = await context.Users.FindAsync(user.Id);
        if (userEntity == null) throw new NotFoundException(ResponseConstants.NOT_FOUND);

        userEntity.FirstName = user.FirstName ?? userEntity.FirstName;
        userEntity.LastName = user.LastName ?? userEntity.LastName;
        userEntity.Email = user.Email ?? userEntity.Email;
        userEntity.Username = user.Username ?? userEntity.Username;
        if (user.ProfilePictureUrl != null)
            userEntity.ProfilePictureUrl = user.ProfilePictureUrl;

        context.Users.Update(userEntity);
        await context.SaveChangesAsync();
        return UserMapper.MapToDomain(userEntity);
    }
}