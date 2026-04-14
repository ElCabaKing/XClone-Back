namespace Shared.Constants;

public static class MediaConstants
{
    public const string DEFAULT_PROFILE_PICTURE_URL = "https://res.cloudinary.com/dzj8q4q9u/image/upload/v1700000000/default-profile-picture.png";
    public const int MAX_PROFILE_PICTURE_SIZE_BYTES = 1 * 1024 * 1024; // 5 MB
    public static readonly string[] ALLOWED_PROFILE_PICTURE_TYPES = 
    ["image/jpeg", "image/png", "image/gif"];


    public const string PROFILE_TOO_LARGE = "La imagen de perfil excede el tamaño máximo permitido de 5 MB.";
    public const string PROFILE_INVALID_TYPE = "El tipo de archivo de la imagen de perfil no es válido. Solo se permiten JPEG, PNG y GIF.";
}