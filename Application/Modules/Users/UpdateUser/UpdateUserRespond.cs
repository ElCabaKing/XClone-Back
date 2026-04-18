namespace Application.Modules.Users.UpdateUser;

public class UpdateUserResponse(
    Guid id, 
    string username, 
    string email, 
    string firstName, 
    string lastName)
{
    public Guid Id { get; set; } = id;
    public string Username { get; set; } = username;
    public string Email { get; set; } = email;
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
}