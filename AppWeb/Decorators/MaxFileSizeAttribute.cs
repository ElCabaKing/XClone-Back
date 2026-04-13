using System.ComponentModel.DataAnnotations;
using Shared.Constants;


namespace AppWeb.Decorators;
public class MaxFileSizeAttribute() : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var file = value as IFormFile;

        if (file != null && file.Length > MediaConstants.MAX_PROFILE_PICTURE_SIZE_BYTES)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}