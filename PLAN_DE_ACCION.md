# 🔧 PLAN DE ACCIÓN - FIXES RECOMENDADOS

## Prioridad 1: CRÍTICO (Implementar Hoy)

### ✅ 1. Corregir Nombres de Clases

```bash
# Cambios de names:
1. CreateUSerHandler → CreateUserHandler
2. DependencyInyection → DependencyInjection (ambos archivos)
3. MaptToDomain → MapToDomain (UserMapper.cs)
```

**Archivos a actualizar:**
- `Application/Modules/User/CreateUser/CreateUserHandler.cs` (rename + contenido)
- `Application/Modules/User/CreateUser/` (references)
- `Application/DependencyInyection.cs` (rename + contenido)
- `AppWeb/DependencyInjection.cs` (references)
- `Infrastructure/DependencyInjection.cs` (references)
- `Infrastructure/Mappers/UserMapper.cs` (method rename)
- `Infrastructure/Repositories/UserRepository.cs` (references)
- `Program.cs` (references)

**Tiempo estimado:** 15 minutos

---

### ⚠️ 2. Validar Archivo de Perfil - CRÍTICO

**Archivo:** `AppWeb/Controllers/UserController.cs`

```csharp
using Application.Modules.User.CreateUser;
using AppWeb.Requests.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers
{
    [Route("api/v1/[controller]")]  // ✅ AGREGADO versionado
    [ApiController]
    public class UserController
    (
        CreateUserHandler createUserHandler  // ✅ CORREGIDO nombre
    ) : ControllerBase
    {
        private const long MAX_FILE_SIZE = 5 * 1024 * 1024; // 5MB
        private static readonly string[] AllowedMimeTypes = 
        { 
            "image/jpeg", 
            "image/png", 
            "image/webp" 
        };
        private static readonly string[] AllowedExtensions = 
        { 
            ".jpg", 
            ".jpeg", 
            ".png", 
            ".webp" 
        };

        /// <summary>
        /// Crea un nuevo usuario con su foto de perfil
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserRequest request)
        {
            // ✅ AGREGADO: Validar ModelState
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ AGREGADO: Validar archivo si existe
            if (request.ProfilePicture != null)
            {
                var validationResult = ValidateProfilePicture(request.ProfilePicture);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.ErrorMessage);
            }

            try
            {
                var command = new CreateUserCommand(
                   request.Username,
                   request.Email,
                   request.Password,
                   request.FirstName,
                   request.LastName,
                   request.ProfilePicture?.OpenReadStream(),
                   request.ProfilePicture?.FileName,
                   request.ProfilePicture?.ContentType
               );
                var result = await createUserHandler.Handle(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, "Error al crear usuario");
            }
        }

        /// <summary>
        /// Valida que la imagen cumpla con los requisitos
        /// </summary>
        private FileValidationResult ValidateProfilePicture(IFormFile file)
        {
            // 1. Validar tamaño
            if (file.Length > MAX_FILE_SIZE)
                return FileValidationResult.Fail($"El archivo no puede exceder {MAX_FILE_SIZE / (1024 * 1024)}MB");

            // 2. Validar MIME type
            if (!AllowedMimeTypes.Contains(file.ContentType))
                return FileValidationResult.Fail("El archivo debe ser una imagen (JPEG, PNG o WebP)");

            // 3. Validar extensión
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtensions.Contains(ext))
                return FileValidationResult.Fail("La extensión del archivo no es válida");

            // 4. Validar que filename no sea vacío
            if (string.IsNullOrWhiteSpace(file.FileName))
                return FileValidationResult.Fail("El nombre del archivo es requerido");

            return FileValidationResult.Success();
        }
    }

    /// <summary>
    /// Resultado de validación de archivo
    /// </summary>
    internal class FileValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }

        public static FileValidationResult Success() 
            => new() { IsValid = true };

        public static FileValidationResult Fail(string message) 
            => new() { IsValid = false, ErrorMessage = message };
    }
}
```

**Tiempo estimado:** 20 minutos

---

### 🔒 3. Cerrar Stream Explícitamente

**Archivo:** `Application/Modules/User/CreateUser/CreateUserHandler.cs`

```csharp
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using Shared.Constants;
using Shared.Generics;
using Shared.Helpers;
using UserDomain = Domain.Entities.User;

namespace Application.Modules.User.CreateUser;

/// <summary>
/// Manejador del comando para crear usuario
/// </summary>
public class CreateUserHandler
(
    IPasswordService passwordService, 
    IUserRepository userRepository,
    ICloudStorage cloudStorage
)
{
    /// <summary>
    /// Procesa la creación de un nuevo usuario
    /// </summary>
    public async Task<GenericResponse<CreateUserResponse>> Handle(CreateUserCommand command)
    {
        if (await userRepository.UsernameOrEmailExists(command.Username, command.Email))
        {
            throw new AlreadyExistsException(ResponseConstants.EMAIL_USERNAME_ALREADY_EXISTS);
        }

        var hashedPassword = passwordService.HashPassword(command.Password);

        // ✅ MEJORADO: Usar using para cerrar stream explícitamente
        string? profilePictureUrl = null;
        if (command.ProfilePicture != null)
        {
            using (command.ProfilePicture)
            {
                profilePictureUrl = await cloudStorage.UploadFileAsync(
                    command.ProfilePicture, 
                    command.ProfilePictureFileName!
                );
            }
        }

        var user = MapToDomain(command, hashedPassword, profilePictureUrl);
        var response = await userRepository.CreateUserAsync(user);

        return ResponseHelper.Create(new CreateUserResponse(
            response.Id, 
            response.Username, 
            response.Email, 
            response.FirstName, 
            response.LastName, 
            response.ProfilePictureUrl
        ));
    }

    /// <summary>
    /// Mapea comando a entidad de dominio
    /// </summary>
    private static UserDomain MapToDomain(CreateUserCommand command, string passwordHash, string? profilePictureUrl)
    {
        return new UserDomain
        {
            Id = Guid.NewGuid(),
            Username = command.Username,
            Email = command.Email,
            PasswordHash = passwordHash,
            FirstName = command.FirstName,
            LastName = command.LastName,
            ProfilePictureUrl = profilePictureUrl ?? null,
            Status = Domain.Enums.UserStatusEnum.Active
        };
    }
}
```

**Tiempo estimado:** 10 minutos

---

### 🛡️ 4. Implementar Rate Limiting

**Archivo:** `AppWeb/Program.cs`

```csharp
using Application;
using AppWeb;
using AppWeb.Middlewares;
using DotNetEnv;
using Infrastructure;
using Scalar.AspNetCore;
using Serilog;
using System.Globalization;

Env.Load("../.env");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog();

// ✅ AGREGADO: Rate Limiting
builder.Services.AddRateLimiter(limiterOptions =>
{
    limiterOptions.AddFixedWindowLimiter(policyName: "create-user", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    });

    limiterOptions.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { message = "Demasiadas solicitudes. Intente más tarde." },
            cancellationToken
        );
    };
});

builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddAppWeb(builder.Configuration);

builder.Services.AddControllers();

// ✅ AGREGADO: CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "https://yourdomain.com"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// ✅ AGREGADO: Usar middlewares en orden correcto
app.UseRateLimiter();  // Primero
app.UseCors("AllowSpecificOrigins");  // Segundo
app.UseMiddleware<ErrorHandlerMiddleware>();  // Tercero
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
```

**Tiempo estimado:** 15 minutos

---

### 🔐 5. Mejorar Validación de Contraseña

**Crear archivo:** `Shared/Helpers/PasswordValidationHelper.cs`

```csharp
using System.Text.RegularExpressions;

namespace Shared.Helpers;

/// <summary>
/// Helper para validar contraseñas según políticas de seguridad
/// </summary>
public static class PasswordValidationHelper
{
    private const int MinLength = 8;
    private const int MaxLength = 128;

    /// <summary>
    /// Valida que la contraseña cumpla con todos los requisitos
    /// </summary>
    public static PasswordValidationResult Validate(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
            errors.Add("La contraseña es requerida");
        else
        {
            if (password.Length < MinLength)
                errors.Add($"La contraseña debe tener al menos {MinLength} caracteres");

            if (password.Length > MaxLength)
                errors.Add($"La contraseña no puede exceder {MaxLength} caracteres");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                errors.Add("La contraseña debe contener al menos una mayúscula");

            if (!Regex.IsMatch(password, @"[a-z]"))
                errors.Add("La contraseña debe contener al menos una minúscula");

            if (!Regex.IsMatch(password, @"[0-9]"))
                errors.Add("La contraseña debe contener al menos un número");

            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\|,.<>\/?]"))
                errors.Add("La contraseña debe contener al menos un carácter especial");

            if (Regex.IsMatch(password, @"(.)\1{2,}"))
                errors.Add("La contraseña no puede contener más de 2 caracteres repetidos consecutivos");
        }

        return new PasswordValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }
}

/// <summary>
/// Resultado de validación de contraseña
/// </summary>
public class PasswordValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}
```

**Actualizar:** `AppWeb/Requests/User/CreateUserRequest.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Helpers;

namespace AppWeb.Requests.User;

public class CreateUserRequest
{
    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [MaxLength(150, ErrorMessage = ValidationConstants.MAX_LENGTH)]
    [MinLength(3, ErrorMessage = ValidationConstants.MIN_LENGTH)]
    public string Username { get; set; } = default!;

    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [EmailAddress(ErrorMessage = ValidationConstants.EMAIL_ADDRESS)]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [CustomPasswordValidation]  // ✅ AGREGADO validador customizado
    public string Password { get; set; } = default!;
    
    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [MaxLength(100, ErrorMessage = ValidationConstants.MAX_LENGTH)]
    public string FirstName { get; set; } = default!;

    [Required(ErrorMessage = ValidationConstants.REQUIRED)]
    [MaxLength(100, ErrorMessage = ValidationConstants.MAX_LENGTH)]
    public string LastName { get; set; } = default!;

    public IFormFile? ProfilePicture { get; set; }
}

/// <summary>
/// Validador custom de contraseña
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class CustomPasswordValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string password)
            return ValidationResult.Success;

        var result = PasswordValidationHelper.Validate(password);
        
        if (!result.IsValid)
            return new ValidationResult(string.Join("; ", result.Errors));

        return ValidationResult.Success;
    }
}
```

**Tiempo estimado:** 25 minutos

---

## Prioridad 2: ALTA (Esta Semana)

### 📊 6. Optimizar Queries de Base de Datos

**Archivo:** `Infrastructure/Repositories/UserRepository.cs`

```csharp
// ❌ ACTUAL - 2 queries
public async Task<bool> UsernameOrEmailExists(string username, string email)
{
    var userByUsername = await context.Users.AnyAsync(u => u.Username == username);
    var userByEmail = await context.Users.AnyAsync(u => u.Email == email);
    return userByUsername || userByEmail;
}

// ✅ MEJORADO - 1 query
public async Task<bool> UsernameOrEmailExists(string username, string email)
{
    return await context.Users.AnyAsync(u => 
        u.Username == username || u.Email == email
    );
}
```

---

### 🧪 7. Crear Suite de Tests Básica

**Crear archivo:** `Tests/Unit/CreateUserHandlerTests.cs`

```csharp
using Moq;
using NUnit.Framework;
using Application.Modules.User.CreateUser;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Exceptions;
using Shared.Constants;

namespace Tests.Unit;

[TestFixture]
public class CreateUserHandlerTests
{
    private Mock<IPasswordService> _mockPasswordService;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<ICloudStorage> _mockCloudStorage;
    private CreateUserHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockPasswordService = new Mock<IPasswordService>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockCloudStorage = new Mock<ICloudStorage>();

        _handler = new CreateUserHandler(
            _mockPasswordService.Object,
            _mockUserRepository.Object,
            _mockCloudStorage.Object
        );
    }

    [Test]
    public async Task Handle_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateUserCommand(
            "validuser123",
            "user@example.com",
            "SecurePass123!",
            "John",
            "Doe"
        );

        _mockUserRepository
            .Setup(r => r.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockPasswordService
            .Setup(p => p.HashPassword(It.IsAny<string>()))
            .Returns("hashed_password");

        _mockUserRepository
            .Setup(r => r.CreateUserAsync(It.IsAny<Domain.Entities.User>()))
            .ReturnsAsync(new Domain.Entities.User 
            { 
                Id = Guid.NewGuid(),
                Username = "validuser123",
                Email = "user@example.com"
            });

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual("validuser123", result.Data.Username);
    }

    [Test]
    public async Task Handle_WithExistingUsername_ThrowsAlreadyExistsException()
    {
        // Arrange
        var command = new CreateUserCommand(
            "existinguser",
            "new@example.com",
            "SecurePass123!",
            "John",
            "Doe"
        );

        _mockUserRepository
            .Setup(r => r.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act & Assert
        var ex = Assert.ThrowsAsync<AlreadyExistsException>(
            async () => await _handler.Handle(command)
        );
        Assert.AreEqual(ResponseConstants.EMAIL_USERNAME_ALREADY_EXISTS, ex?.Message);
    }
}
```

---

## Prioridad 3: MEDIA (Próximo Sprint)

### 📖 8. Agregar Documentación XML

```csharp
/// <summary>
/// Representa el comando para crear un usuario
/// </summary>
public class CreateUserCommand(...)
{
    /// <summary>
    /// Nombre de usuario único
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    /// Email del usuario
    /// </summary>
    public string Email { get; set; }
    
    // ... resto de propiedades documentadas
}
```

---

## 📋 CHECKLIST DE IMPLEMENTACIÓN

```
✅ = Completado
⬜ = Pendiente
```

**Crítica:**
- ⬜ Corregir nombres de clases (5 archivos)
- ⬜ Validar archivo (UserController)
- ⬜ Cerrar Stream (CreateUserHandler)
- ⬜ Rate Limiting (Program.cs)
- ⬜ Validación de contraseña (PasswordValidationHelper)

**Alta:**
- ⬜ Optimizar queries (UserRepository)
- ⬜ Escribir tests unitarios
- ⬜ CORS configuration (ya está en Program.cs arriba)
- ⬜ API versionado (ya está en Controller arriba)

**Media:**
- ⬜ Documentación XML en todas las clases públicas
- ⬜ Audit fields (CreatedAt automático en BD)
- ⬜ Logging mejorado
- ⬜ Documentación OpenAPI

---

**Tiempo total estimado: 90 minutos para Prioridad 1 + 2**
