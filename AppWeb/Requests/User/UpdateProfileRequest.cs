
using System.ComponentModel.DataAnnotations;
using AppWeb.Decorators;
using Shared.Constants;

namespace AppWeb.Requests.User;

public class UpdateProfileRequest
{
    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    public Guid UserId { get; set; } = default!;
    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [MaxLength(150, ErrorMessage = ValidationConstants.MAX_LENGTH)]
    [MinLength(10, ErrorMessage = ValidationConstants.MIN_LENGTH)]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = ValidationConstants.USERNAME_ALPHANUMERIC)]
    public string Username { get; set; } = default!;

    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [EmailAddress(ErrorMessage = ValidationConstants.EMAIL_ADDRESS)]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [MaxLength(100, ErrorMessage = ValidationConstants.MAX_LENGTH)]
    public string FirstName { get; set; } = default!;

    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [MaxLength(100, ErrorMessage = ValidationConstants.MAX_LENGTH)]
    public string LastName { get; set; } = default!;
}