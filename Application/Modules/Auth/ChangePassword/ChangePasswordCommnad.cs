

namespace Application.Modules.Auth.ChangePassword;

public class ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
)
{
    public Guid UserId { get; set; } = UserId;
    public string CurrentPassword { get; set; } = CurrentPassword!;
    public string NewPassword { get; set; } = NewPassword!;
}