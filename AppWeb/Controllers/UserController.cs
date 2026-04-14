using Application.Modules.User.CreateUser;
using AppWeb.Requests.User;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController
    (
        CreateUserHandler createUserHandler
    ) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserRequest request)
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
            var result = await createUserHandler.Handle(command);
            return Ok(result);
        }
    }
}
