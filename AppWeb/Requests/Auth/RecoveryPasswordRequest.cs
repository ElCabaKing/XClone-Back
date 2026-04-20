

using System.ComponentModel.DataAnnotations;

namespace AppWeb.Requests.Auth;

public class RecoveryPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}