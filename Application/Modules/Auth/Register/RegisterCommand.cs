namespace Application.Modules.Auth.Register;

public class CreateUserCommand(
    string username, 
    string email, 
    string password, 
    string firstName, 
    string lastName, 
    Stream? profilePicture = null, 
    string? profilePictureFileName = null, 
    string? profilePictureContentType = null)
{
    public string Username { get; set; } = username;
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public Stream? ProfilePicture { get; init; } = profilePicture;
    public string? ProfilePictureFileName { get; init; } = profilePictureFileName;
    public string? ProfilePictureContentType { get; init; } = profilePictureContentType;
}