
using Application.Modules.Users.CreateUser;
using Application.Modules.Users.UpdateUser;
using AppWeb.Requests.User;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController
    (
        CreateUserHandler createUserHandler,
        UpdateUserHandler updateUserHandler
    ) : ControllerBase
    {

        [HttpPost("create")]
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
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateUserCommand(
                request.UserId,
                request.Username,
                request.Email,
                request.FirstName,
                request.LastName
            );
            var result = await updateUserHandler.Handle(command);
            return Ok(result);
        }
    }
}
