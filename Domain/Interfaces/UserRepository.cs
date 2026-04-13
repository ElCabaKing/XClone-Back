using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository
{
    public Task<User?> GetByUsernameAsync(string username);
    public Task<User> CreateUserAsync(User user);
    public Task<User?> GetUserByEmailAsync(string email);
    public Task<bool> UsernameOrEmailExists(string username, string email);
    public Task<User?> UpdateUserAsync(User user);
}