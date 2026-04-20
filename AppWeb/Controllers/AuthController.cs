using Application.Modules.Auth.Login;
using Application.Modules.Auth.RecoveryPassword;
using Application.Modules.Auth.Register;
using AppWeb.Requests.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AppWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        LoginHandler loginHandler,
        RegisterHandler registerHandler,
        RecoveryPasswordHandler recoveryPasswordHandler
    ) : ControllerBase
    {
        [EnableRateLimiting("Fixed")]
        [HttpPost("login")]
        [EndpointSummary("Login de usuario")]
        [EndpointDescription("Permite a un usuario autenticarse y obtener un token de acceso.")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new LoginCommand(
                request.Username,
                request.Password
            );
            var result = await loginHandler.Handle(command);
            Response.Cookies.Append("accessToken", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(5)
            });
            return Ok(result);
        }

        [HttpPost("create")]
        [EndpointSummary("Registro de usuario")]
        [EndpointDescription("Permite crear un nuevo usuario con los datos proporcionados.")]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateUser([FromForm] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
            var result = await registerHandler.Handle(command);
            return Created("/users/" + result.Data.Id, result);
        }

        [HttpPost("recover")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoveryPasswordRequest request)
        {
            var command = new RecoveryPasswordCommand(request.Email);
            var result = await recoveryPasswordHandler.Handle(command);
            return Ok(result);
        }
    }
}
