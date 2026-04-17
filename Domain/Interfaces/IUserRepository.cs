using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    public Task<User?> GetByUsernameorEmailAsync(string credential);
    public Task<bool> UsernameOrEmailExists(string username, string email);
}