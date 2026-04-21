using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeliOgretmenIletisim.Application.Features.Users.Commands.CreateUser;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

public class UsersController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        return HandleResult(await Mediator.Send(new VeliOgretmenIletisim.Application.Features.Users.Queries.GetProfile.GetProfileQuery()));
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] VeliOgretmenIletisim.Application.Features.Users.Commands.ChangePassword.ChangePasswordCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [Authorize]
    [HttpPost("profile-picture")]
    public async Task<IActionResult> UpdateProfilePicture([FromBody] VeliOgretmenIletisim.Application.Features.Users.Commands.UpdateProfilePicture.UpdateProfilePictureCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }
}
