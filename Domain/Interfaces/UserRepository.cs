using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository
{
    public Task<User?> GetByUsernameAsync(string username);
    public Task<User> CreateUserAsync(User user);
}