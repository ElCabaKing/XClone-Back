using System.ComponentModel.DataAnnotations;

namespace AppWeb.Decorators;

public class AllowedFileTypesAttribute : ValidationAttribute
{
    private readonly Type _type;
    private readonly string _fieldName;

    public AllowedFileTypesAttribute(Type type, string fieldName)
    {
        _type = type;
        _fieldName = fieldName;
    }
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var file = value as IFormFile;

        if (file is null)
            return ValidationResult.Success;

        var field = _type.GetField(_fieldName);
        var allowedTypes = field?.GetValue(null) as string[];

        if (allowedTypes is null || !allowedTypes.Contains(file.ContentType))
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}