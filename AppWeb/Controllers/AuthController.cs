using Application.Modules.Auth.Login;
using AppWeb.Requests.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AppWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        LoginHandler loginHandler
    ) : ControllerBase
    {
        [EnableRateLimiting("Fixed")]
        [HttpPost("login")]
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
    }
}
