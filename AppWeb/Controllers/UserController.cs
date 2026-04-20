
using Application.Modules.Users.UpdateProfile;
using AppWeb.Requests.User;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController
    (
        UpdateProfileHandler updateProfileHandler
    ) : ControllerBase
    {

       
        [HttpPut("update")]
        [EndpointSummary ("Actualización de usuario")]
        [EndpointDescription("Permite actualizar los datos de un usuario existente.")]
        [ProducesResponseType(typeof(UpdateProfileResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateProfileCommand(
                request.UserId,
                request.Username,
                request.Email,
                request.FirstName,
                request.LastName
            );
            var result = await updateProfileHandler.Handle(command);
            return Ok(result);
        }
    }
}
