namespace Application.Modules.User.UpdateUser;

public class UpdateUserCommand(
    Guid userId,
    string? username,
    string? email,
    string? firstName,
    string? lastName,
    Stream? profilePicture = null,
    string? profilePictureFileName = null,
    string? profilePictureContentType = null
)
{
    public Guid UserId { get; set; } = userId;
    public string? Username { get; set; } = username;
    public string? Email { get; set; } = email;
    public string? FirstName { get; set; } = firstName;
    public string? LastName { get; set; } = lastName;
    public Stream? ProfilePicture { get; set; } = profilePicture;
    public string? ProfilePictureFileName { get; set; } = profilePictureFileName;
    public string? ProfilePictureContentType { get; set; } = profilePictureContentType;
}