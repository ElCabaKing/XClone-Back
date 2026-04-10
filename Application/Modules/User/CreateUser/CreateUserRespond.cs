namespace Application.Modules.User.CreateUser;

public class CreateUserResponse(
    Guid id, 
    string username, 
    string email, 
    string firstName, 
    string lastName, 
    string? profilePictureUrl)
{
    public Guid Id { get; set; } = id;
    public string Username { get; set; } = username;
    public string Email { get; set; } = email;
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public string? ProfilePictureUrl { get; set; } = profilePictureUrl;
}