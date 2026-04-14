using Application.Interfaces;
using Domain.Interfaces;
using Shared.Generics;
using Domain.Entities;
using UserDomain = Domain.Entities.User;
using Shared.Helpers;

namespace Application.Modules.User.UpdateUser;

public class UpdateUserHandler(
    IUserRepository userRepository
)
{
    /// <summary>
    /// Maneja la actualización de un usuario existente menos su contraseña y foto de perfil, para mantener la simplicidad del ejemplo. En un caso real, se podrían manejar estas actualizaciones en comandos separados.
    /// </summary>
    public async Task<GenericResponse<UpdateUserResponse>> Handle(UpdateUserCommand command)
    {
        var response = await userRepository.UpdateUserAsync(MapToDomain(command));
        return ResponseHelper.Create(new UpdateUserResponse(
            response.Id,
            response.Username,
            response.Email,
            response.FirstName,
            response.LastName
        ));
    }

    public static UserDomain MapToDomain(UpdateUserCommand command)
    {
        return new UserDomain
        {
            Id = command.UserId,
            Username = command.Username,
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName
        };
    }
}