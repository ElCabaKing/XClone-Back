using Application.Modules.Auth.Login;
using AppWeb.Requests.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        LoginHandler loginHandler
    ) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
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
    }
}
