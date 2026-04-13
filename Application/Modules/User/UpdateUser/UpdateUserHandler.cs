using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Modules.User.UpdateUser;

public class UpdateUserHandler(
    IUserRepository userRepository,
    ICloudStorage   cloudStorage
)
{
    public Task Handle(UpdateUserCommand command)
    {

        return userRepository.UpdateUserAsync(new Domain.Entities.User
        {
            Id = command.UserId,
            Username = command.Username ?? string.Empty,
            Email = command.Email ?? string.Empty,
            FirstName = command.FirstName ?? string.Empty,
            LastName = command.LastName ?? string.Empty,
            // ProfilePicture handling would go here, but is omitted for brevity
        });
    }
}