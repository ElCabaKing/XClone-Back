using System.ComponentModel.DataAnnotations;

namespace AppWeb.Requests.User;
public class CreateUserRequest
{
    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [MaxLength(150, ErrorMessage = ValidationConstants.MAX_LENGTH)]
    [MinLength(10, ErrorMessage = ValidationConstants.MIN_LENGTH)]
    public string Username { get; set; } = default!;

    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [EmailAddress(ErrorMessage = ValidationConstants.EMAIL_ADDRESS)]
    public string Email { get; set; } = default!;
    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [MinLength(6, ErrorMessage = ValidationConstants.MIN_LENGTH)]
    public string Password { get; set; } = default!;

    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [MaxLength(100, ErrorMessage = ValidationConstants.MAX_LENGTH)]
    public string FirstName { get; set; } = default!;

    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [MaxLength(100, ErrorMessage = ValidationConstants.MAX_LENGTH)]
    public string LastName { get; set; } = default!;

    public IFormFile? ProfilePicture { get; set; }
}