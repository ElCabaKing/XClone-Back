namespace Application.Modules.Users.UpdateUser;

public class UpdateUserCommand(
    Guid userId,
    string? username,
    string? email,
    string? firstName,
    string? lastName
)
{
    public Guid UserId { get; set; } = userId;
    public string Username { get; set; } = username!;
    public string Email { get; set; } = email!;
    public string FirstName { get; set; } = firstName!;
    public string LastName { get; set; } = lastName!;
}