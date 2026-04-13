using Application.Modules.User.CreateUser;
using AppWeb.Requests.User;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;

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
            if (request.ProfilePicture is not null)
            {
                if (request.ProfilePicture.Length > MediaConstants.MAX_PROFILE_PICTURE_SIZE_BYTES)
                    throw new BadRequestException(MediaConstants.PROFILE_TOO_LARGE);
                if (!MediaConstants.ALLOWED_PROFILE_PICTURE_TYPES.Contains(request.ProfilePicture.ContentType))
                    throw new BadRequestException(MediaConstants.PROFILE_INVALID_TYPE);
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
            return Ok(result);
        }
    }
}
