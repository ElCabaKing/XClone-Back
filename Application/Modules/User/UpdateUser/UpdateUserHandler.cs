using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Modules.User.UpdateUser;

public class UpdateUserHandler(
    IUserRepository userRepository
)
{
    /// <summary>
    /// Maneja la actualización de un usuario existente menos su contraseña y foto de perfil, para mantener la simplicidad del ejemplo. En un caso real, se podrían manejar estas actualizaciones en comandos separados.
    /// </summary>
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