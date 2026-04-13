using System.ComponentModel.DataAnnotations;
using Shared.Constants;

namespace AppWeb.Requests.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    public string Username { get; set; } = default!;

    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    public string Password { get; set; } = default!;
}