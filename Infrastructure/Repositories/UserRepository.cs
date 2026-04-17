using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;


namespace Infrastructure.Repositories;

public class UserRepository(XDbContext context) : GenericRepository<User>(context), IUserRepository
{
    public Task<User?> GetByUsernameorEmailAsync(string credential)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UsernameOrEmailExists(string username, string email)
    {
        throw new NotImplementedException();
    }
}