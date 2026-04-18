
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
        [EndpointSummary ("Creación de usuario")]
        [EndpointDescription("Permite crear un nuevo usuario con los datos proporcionados.")]
        [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status200OK)]
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
            return Created("/users/" + result.Data.Id, result);
        }
        [HttpPut("update")]
        [EndpointSummary ("Actualización de usuario")]
        [EndpointDescription("Permite actualizar los datos de un usuario existente.")]
        [ProducesResponseType(typeof(UpdateUserResponse), StatusCodes.Status200OK)]
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
