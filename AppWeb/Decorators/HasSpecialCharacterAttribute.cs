using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AppWeb.Decorators
{
    public class HasSpecialCharacterAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var input = value.ToString();

            var regex = new Regex(@"[!@#$%^&*(),.?""{}|<>_\-+=\/\\\[\]]");

            if (!regex.IsMatch(input!))
            {
                return new ValidationResult(
                    ErrorMessage 
                );
            }

            return ValidationResult.Success;
        }
    }
}