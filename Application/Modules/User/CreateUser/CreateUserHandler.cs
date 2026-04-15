using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using Shared.Constants;
using Shared.Generics;
using Shared.Helpers;
using UserDomain = Domain.Entities.User;
namespace Application.Modules.User.CreateUser;

public class CreateUserHandler(
    IEmailService emailService,
IPasswordService passwordService,
IUserRepository userRepository,
ICloudStorage cloudStorage)
{

    /// <summary>
    /// Maneja la creación de un nuevo usuario
    /// </summary>
    public async Task<GenericResponse<CreateUserResponse>> Handle(CreateUserCommand command)
    {
        await emailService.SendEmailAsync(command.Email, "Welcome to XClone!", "Thank you for registering at XClone. We're excited to have you on board!");
        if (await userRepository.UsernameOrEmailExists(command.Username, command.Email))
        {
            throw new AlreadyExistsException(ResponseConstants.EMAIL_USERNAME_ALREADY_EXISTS);
        }

        var HashedPassword = passwordService.HashPassword(command.Password) ??
            throw new ServiceErrorException(ResponseConstants.HASHING_ERROR);

        await using var profilePictureStream = command.ProfilePicture;

        string? profilePictureUrl = null;

        if (command.ProfilePicture != null)
        {
            await using var stream = command.ProfilePicture;

            profilePictureUrl = await cloudStorage.UploadFileAsync(
               stream,
               command.ProfilePictureFileName!
           );
        }

        var response = await userRepository.CreateUserAsync(MapToDomain(
            command, HashedPassword,
            command.ProfilePicture != null ?
          profilePictureUrl : MediaConstants.DEFAULT_PROFILE_PICTURE_URL))
            ?? throw new ServiceErrorException(ResponseConstants.USER_CREATION_ERROR);

        return ResponseHelper.Create(new CreateUserResponse(
            response.Id,
            response.Username,
            response.Email,
            response.FirstName,
            response.LastName,
            response.ProfilePictureUrl)

        );

    }

    private static UserDomain MapToDomain(CreateUserCommand command, string passwordHash, string? profilePictureUrl)
    {
        return new UserDomain
        {
            Id = Guid.NewGuid(),
            Username = command.Username,
            Email = command.Email,
            PasswordHash = passwordHash,
            FirstName = command.FirstName,
            LastName = command.LastName,
            ProfilePictureUrl = profilePictureUrl ?? null,
            Status = Domain.Enums.UserStatusEnum.Active
        };
    }
}