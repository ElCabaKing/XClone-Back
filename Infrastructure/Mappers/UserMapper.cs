using UserDomain =  Domain.Entities.User;
using UserEntity =  Infrastructure.Persistence.User;
namespace Infrastructure.Mappers;

public class UserMapper
{
    public static UserDomain MaptToDomain(UserEntity user)
    {
        return new UserDomain
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            Role = user.Status.Name,
            CreatedAt = user.CreatedAt,
            IsVerified = user.IsVerified,
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
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            CreatedAt = user.CreatedAt,
            IsVerified = user.IsVerified,
            StatusId = (byte)user.Status,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl ?? string.Empty
        };
    }
}