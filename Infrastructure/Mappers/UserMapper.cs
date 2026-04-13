using UserDomain =  Domain.Entities.User;
using UserEntity =  Infrastructure.Persistence.User;
namespace Infrastructure.Mappers;

public class UserMapper
{
    public static UserDomain MapToDomain(UserEntity user)
    {
        return new UserDomain
        {

            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            Role = (Domain.Enums.UserRoleEnum)user.RoleId,
            CreatedAt = user.CreatedAt,
            Status = (Domain.Enums.UserStatusEnum)user.StatusId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl ?? string.Empty  
        };
    }

    public static UserEntity MapToEntity(UserDomain user)
    {
        return new UserEntity
        {

                Id = Guid.NewGuid(),
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                RoleId = (byte)user.Role,
                CreatedAt = DateTime.UtcNow,
                StatusId = (byte)user.Status,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl
        };
    }
}