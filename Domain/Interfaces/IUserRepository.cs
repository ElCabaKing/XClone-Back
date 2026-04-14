using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository
{
    public Task<User?> GetByUsernameorEmailAsync(string credential);
    public Task<User> CreateUserAsync(User user);
    public Task<bool> UsernameOrEmailExists(string username, string email);
    public Task<User> UpdateUserAsync(User user);
}