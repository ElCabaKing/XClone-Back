using Application.Interfaces;
using Domain.Interfaces;
using Shared.Generics;
using Domain.Entities;
using UserDomain = Domain.Entities.User;
using Shared.Helpers;
using Domain.Exceptions;
using Shared.Constants;

namespace Application.Modules.User.UpdateUser;

public class UpdateUserHandler(
    IUOW uow
)
{
    /// <summary>
    /// Maneja la actualización de un usuario existente menos su contraseña y foto de perfil, para mantener la simplicidad del ejemplo. En un caso real, se podrían manejar estas actualizaciones en comandos separados.
    /// </summary>
    public async Task<GenericResponse<UpdateUserResponse>> Handle(UpdateUserCommand command)
    {
        var existingUser = await uow.UserRepository.GetByIdAsync(command.UserId) ??
            throw new NotFoundException(ResponseConstants.NOT_FOUND);
        var response = await uow.UserRepository.Update(MapToDomain(command));
        
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