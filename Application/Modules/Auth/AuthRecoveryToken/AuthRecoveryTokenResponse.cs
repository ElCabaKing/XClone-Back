

namespace Application.Modules.Auth.AuthRecoveryToken
{
    public class AuthRecoveryTokenResponse(
        Guid userId
    )
    {
        public Guid UserId { get; set; } = userId;
    }
}