# 📋 ISSUES PENDIENTES - Plan de Correción Inmediata
**Generado:** 14 de Abril de 2026  
**Prioridad:** Critical → High → Medium  

---

## 🔴 CRITICAL (Resolver esta semana)

### Issue #1: Nombres de Archivos con Typos
**Severidad:** MEDIA - Impacto en profesionalismo del código  
**Tiempo Estimado:** 30 minutos

#### Archivos a Renombrar:
1. `/Application/DependencyInyection.cs` → `DependencyInjection.cs`
2. `/Shared/Constants/ValidationContants.cs` → `ValidationConstants.cs`
3. `/Shared/Constants/ResponseConstans.cs` → `ResponseConstants.cs`
4. `/Shared/Constants/MediaConstans.cs` → `MediaConstants.cs`

#### Pasos:
```bash
# 1. Renombrar archivos
mv Application/DependencyInyection.cs Application/DependencyInjection.cs
mv Shared/Constants/ValidationContants.cs Shared/Constants/ValidationConstants.cs
mv Shared/Constants/ResponseConstans.cs Shared/Constants/ResponseConstants.cs
mv Shared/Constants/MediaConstans.cs Shared/Constants/MediaConstants.cs

# 2. Verificar imports en los archivos (VS debería hacer refactoring automático)
# 3. Compilar para validar que no hay errores
dotnet build
```

**Archivos Afectados por la Compilación:**
- AppWeb/requests/User/CreateUserRequest.cs
- AppWeb/Decorators/*.cs
- AppWeb/Controllers/*.cs
- Application/Modules/User/CreateUser/CreateUserHandler.cs
- AppWeb/Middlewares/ErrorHandlerMiddleware.cs
- AppWeb/DependencyInjection.cs
- Infrastructure/DependencyInjection.cs
- Todos los controladores

---

### Issue #2: Mejorar Validación de Contraseña
**Severidad:** ALTA - Seguridad, políticas débiles  
**Tiempo Estimado:** 15 minutos

#### Cambio Requerido:
```csharp
// ARCHIVO: /AppWeb/Requests/User/CreateUserRequest.cs

// ANTES:
[Required(ErrorMessage = ValidationConstants.REQUIRED)]
[MinLength(6, ErrorMessage = ValidationConstants.MIN_LENGTH)]
[MaxLength(100, ErrorMessage = ValidationConstants.MAX_LENGTH)]
[HasSpecialCharacter(ErrorMessage = ValidationConstants.PASSWORD_SPECIAL_CHAR)]
public string Password { get; set; } = default!;

// DESPUÉS:
[Required(ErrorMessage = ValidationConstants.REQUIRED)]
[MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
[MaxLength(100, ErrorMessage = ValidationConstants.MAX_LENGTH)]
[RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?""{}|<>_\-+=\/\\\[\]]).*$",
    ErrorMessage = "La contraseña debe contener: mayúscula, minúscula, número y carácter especial."
)]
public string Password { get; set; } = default!;
```

**Cambios:**
- ✅ Min length: 6 → 8 caracteres
- ✅ Agrega validación de mayúscula: `(?=.*[A-Z])`
- ✅ Agrega validación de minúscula: `(?=.*[a-z])`
- ✅ Agrega validación de número: `(?=.*\d)`
- ✅ Mantiene carácter especial: `(?=.*[!@#$...])`

**Pasos:**
1. Editar CreateUserRequest.cs
2. Actualizar el Regex pattern
3. Compilar y probar

---

### Issue #3: SameSite en JWT Cookies
**Severidad:** MEDIA - Security best practice  
**Tiempo Estimado:** 5 minutos

#### Cambio Requerido:
```csharp
// ARCHIVO: /AppWeb/Controllers/AuthController.cs

// ANTES:
Response.Cookies.Append("accessToken", result.Token, new CookieOptions
{
    HttpOnly = true,
    Secure = true,
    SameSite = SameSiteMode.None,  // ❌ Too permissive
    Expires = DateTime.UtcNow.AddMinutes(5)
});

// DESPUÉS (Para desarrollo con CORS):
Response.Cookies.Append("accessToken", result.Token, new CookieOptions
{
    HttpOnly = true,
    Secure = true,
    SameSite = SameSiteMode.Lax,  // ✅ Balance entre seguridad y funcionalidad
    Expires = DateTime.UtcNow.AddMinutes(5)
});

// ALTERNATIVA (Para producción estricta):
// SameSite = SameSiteMode.Strict  // Más seguro pero limitado
```

**Explicación:**
- `None`: Permite requests desde cualquier origin (CSRF vulnerable)
- `Lax`: Permite GET desde otros sites, pero no POST (balance)
- `Strict`: CSRF máximamente protegido, pero puede romper flujos legítimos

**Recomendación:** Usar `Lax` en desarrollo/staging, `Strict` en producción

---

## 🟠 HIGH (Resolver antes de siguiente sprint)

### Issue #4: Agregar Versionado de API
**Severidad:** MEDIA - Mantenibilidad futura  
**Tiempo Estimado:** 20 minutos

#### Cambios Requeridos:

```csharp
// ARCHIVO: /AppWeb/Controllers/UserController.cs

// ANTES:
[Route("api/[controller]")]

// DESPUÉS:
[Route("api/v1/[controller]")]

---

// ARCHIVO: /AppWeb/Controllers/AuthController.cs

// ANTES:
[Route("api/[controller]")]

// DESPUÉS:
[Route("api/v1/[controller]")]
```

**Endpoints Afectados:**
- POST /api/user → POST /api/v1/user
- POST /api/auth/login → POST /api/v1/auth/login

**Nota para Frontend:** Actualizar URLs en cliente (XClone-Front)

---

### Issue #5: Optimizar Query Duplicada en Repository
**Severidad:** BAJA - Performance  
**Tiempo Estimado:** 5 minutos

#### Cambio Requerido:
```csharp
// ARCHIVO: /Infrastructure/Repositories/UserRepository.cs

// ANTES (2 queries):
public async Task<bool> UsernameOrEmailExists(string username, string email)
{
    var userByUsername = await context.Users.AnyAsync(u => u.Username == username);
    var userByEmail = await context.Users.AnyAsync(u => u.Email == email);
    return userByUsername || userByEmail;
}

// DESPUÉS (1 query):
public async Task<bool> UsernameOrEmailExists(string username, string email)
{
    return await context.Users.AnyAsync(u => 
        u.Username == username || u.Email == email
    );
}
```

**Impacto:**
- ✅ Reduce queries a BD en 50%
- ✅ Más rápido (menos latencia)
- ✅ Menos carga en BD

---

### Issue #6: Agregar Validación Explícita de ModelState
**Severidad:** BAJA - Best practice  
**Tiempo Estimado:** 10 minutos

#### Cambio Requerido:
```csharp
// ARCHIVO: /AppWeb/Controllers/UserController.cs

// ANTES:
[HttpPost]
public async Task<IActionResult> CreateUser([FromForm] CreateUserRequest request)
{
    var command = new CreateUserCommand(...);
    var result = await createUserHandler.Handle(command);
    return Ok(result);
}

// DESPUÉS:
[HttpPost]
public async Task<IActionResult> CreateUser([FromForm] CreateUserRequest request)
{
    if (!ModelState.IsValid)
    {
        var errors = ModelState.Values.SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage);
        return BadRequest(new 
        { 
            Message = "Datos inválidos",
            Errors = errors 
        });
    }

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
    return CreatedAtAction(nameof(CreateUser), result);
}
```

**Mejoras:**
- ✅ Validación explícita
- ✅ Mensaje de error consistente
- ✅ Status code 201 Created (mejor que 200 Ok)
- ✅ Mejor documentación de flujo

---

## 🟡 MEDIUM (Sprint siguiente)

### Issue #7: Documentación XML Completa
**Severidad:** BAJA - Documentación  
**Tiempo Estimado:** 1 hora

#### Ejemplo:
```csharp
// ARCHIVO: /Application/Modules/User/CreateUser/CreateUserCommand.cs

/// <summary>
/// Comando para crear un nuevo usuario con información de autenticación y perfil
/// </summary>
public class CreateUserCommand(
    [Required] string username, 
    [Required] string email, 
    [Required] string password, 
    [Required] string firstName, 
    [Required] string lastName, 
    Stream? profilePicture = null, 
    string? profilePictureFileName = null, 
    string? profilePictureContentType = null)
{
    /// <summary>
    /// Nombre de usuario único, alfanumérico con 10-150 caracteres
    /// </summary>
    public string Username { get; set; } = username;
    
    /// <summary>
    /// Email único del usuario, debe ser una dirección válida
    /// </summary>
    public string Email { get; set; } = email;
    
    /// <summary>
    /// Contraseña del usuario (mínimo 8 caracteres, mayúscula, minúscula, número, especial)
    /// </summary>
    public string Password { get; set; } = password;
    
    /// <summary>
    /// Primer nombre del usuario
    /// </summary>
    public string FirstName { get; set; } = firstName;
    
    /// <summary>
    /// Apellido del usuario
    /// </summary>
    public string LastName { get; set; } = lastName;
    
    /// <summary>
    /// Stream de la imagen de perfil (opcional, máximo 1MB)
    /// </summary>
    public Stream? ProfilePicture { get; init; } = profilePicture;
    
    /// <summary>
    /// Nombre del archivo de la imagen de perfil
    /// </summary>
    public string? ProfilePictureFileName { get; init; } = profilePictureFileName;
    
    /// <summary>
    /// Tipo MIME del archivo (image/jpeg, image/png, image/gif)
    /// </summary>
    public string? ProfilePictureContentType { get; init; } = profilePictureContentType;
}
```

---

### Issue #8: Preparar Estructura para Tests
**Severidad:** MEDIA - Quality assurance  
**Tiempo Estimado:** 1 hora

```csharp
// ARCHIVO: /Test/Application/CreateUserHandlerTests.cs (CREAR)

using Xunit;
using Moq;
using Application.Modules.User.CreateUser;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Exceptions;

namespace Test.Application;

public class CreateUserHandlerTests
{
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICloudStorage> _cloudStorageMock;
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerTests()
    {
        _passwordServiceMock = new Mock<IPasswordService>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _cloudStorageMock = new Mock<ICloudStorage>();
        _handler = new CreateUserHandler(
            _passwordServiceMock.Object,
            _userRepositoryMock.Object,
            _cloudStorageMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateUserCommand(
            username: "testuser",
            email: "test@example.com",
            password: "Test@1234",
            firstName: "John",
            lastName: "Doe"
        );

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ThrowsAlreadyExistsException()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.UsernameOrEmailExists(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var command = new CreateUserCommand(...);

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistsException>(
            () => _handler.Handle(command)
        );
    }
}
```

---

## ✅ CHECKLIST DE IMPLEMENTACIÓN

### Esta Semana:
- [ ] Issue #1: Renombrar archivos con typos
- [ ] Issue #2: Mejorar validación de contraseña
- [ ] Issue #3: Revisar SameSite en JWT

### Próxima Semana:
- [ ] Issue #4: Agregar versionado v1
- [ ] Issue #5: Optimizar query duplicada
- [ ] Issue #6: Validación explícita ModelState

### Post-MVP:
- [ ] Issue #7: Documentación XML
- [ ] Issue #8: Agregar tests

---

## 📊 RESUMEN DE TODOS LOS ISSUES

| # | Título | Severidad | Tiempo | Estado | Responsable |
|---|---|---|---|---|---|
| 1 | Typos en nombres | MEDIA | 30 min | PENDIENTE | - |
| 2 | Password validation | ALTA | 15 min | PENDIENTE | - |
| 3 | JWT SameSite | MEDIA | 5 min | PENDIENTE | - |
| 4 | API Versioning | MEDIA | 20 min | PENDIENTE | - |
| 5 | Query optimization | BAJA | 5 min | PENDIENTE | - |
| 6 | ModelState validation | BAJA | 10 min | PENDIENTE | - |
| 7 | XML Documentation | BAJA | 1 hora | PENDIENTE | - |
| 8 | Tests setup | MEDIA | 1 hora | PENDIENTE | - |

**Total:** 8 issues, ~3 horas de trabajo

---

## 🚀 DEPLOYMENT CHECKLIST

Antes de pasar a QA:
- [ ] Todos los issues CRITICAL cerrados
- [ ] Todos los issues HIGH resueltos
- [ ] Código compila sin warnings
- [ ] Pruebas manuales de endpoints actualizados
- [ ] Frontend actualizado con nuevas rutas (v1)
- [ ] Variables de entorno actualizadas
- [ ] Build aceptable en CI/CD

---

**Documento generado:** 14 de Abril de 2026  
**Estimated completion:** 21 de Abril de 2026
