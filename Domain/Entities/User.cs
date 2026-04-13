using Domain.Enums;

namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public UserRoleEnum Role { get; set; }
    public UserStatusEnum Status { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; } = null!;
}