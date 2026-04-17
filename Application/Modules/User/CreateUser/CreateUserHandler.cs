using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Shared.Constants;
using Shared.Generics;
using Shared.Helpers;

namespace Application.Modules.Users.CreateUser;

public class CreateUserHandler(
    IEmailService emailService,
IPasswordService passwordService,
IUOW uow,
ICloudStorage cloudStorage)
{

    /// <summary>
    /// Maneja la creación de un nuevo usuario
    /// </summary>
    public async Task<GenericResponse<CreateUserResponse>> Handle(CreateUserCommand command)
    {
        if (await uow.UserRepository.FirstOrDefaultAsync(
            u => u.Username == command.Username || 
            u.Email == command.Email) != null)
        {
            throw new AlreadyExistsException(ResponseConstants.EMAIL_USERNAME_ALREADY_EXISTS);
        }

        var HashedPassword = passwordService.HashPassword(command.Password) ??
            throw new ServiceErrorException(ResponseConstants.HASHING_ERROR);


        string? profilePictureUrl = null;

        if (command.ProfilePicture != null)
        {
            await using var stream = command.ProfilePicture;

            profilePictureUrl = await cloudStorage.UploadFileAsync(
               stream,
               command.ProfilePictureFileName!
           );
        }
        
        User newUser = new()
        {
            Id = Guid.NewGuid(),
            Username = command.Username,
            Email = command.Email,
            PasswordHash = HashedPassword,
            FirstName = command.FirstName,
            LastName = command.LastName,
            ProfilePictureUrl = profilePictureUrl ?? MediaConstants.DEFAULT_PROFILE_PICTURE_URL
        };
        var response = await uow.UserRepository.Create(newUser)
            ?? throw new ServiceErrorException(ResponseConstants.USER_CREATION_ERROR);

        await uow.SaveChangesAsync();

        EmailTemplate email = await  uow.EmailTemplateRepository.FirstOrDefaultAsync(email => email.Name == EmailTemplateConstants.WELCOME_EMAIL_TEMPLATE) ??
           throw new NotFoundException(ResponseConstants.EMAIL_TEMPLATE_NOT_FOUND);

        await emailService.SendEmailAsync(
           response.Email,
           email.Subject,
           email.Body.
           Replace("{{first_name}}", response.FirstName)
        );

        return ResponseHelper.Create(new CreateUserResponse(
            response.Id,
            response.Username,
            response.Email,
            response.FirstName,
            response.LastName,
            response.ProfilePictureUrl)

        );

    }

}