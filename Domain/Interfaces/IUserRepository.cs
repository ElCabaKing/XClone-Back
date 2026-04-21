using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    public Task<bool> UsernameOrEmailExists(string username, string email);
    public Task<User?> GetByIdAsync(Guid id);
    public Task<User?> Register(User user);
}