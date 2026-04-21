using Microsoft.AspNetCore.Mvc;
using VeliOgretmenIletisim.Application.Features.Auth.Queries.Login;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

public class AuthController : BaseApiController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginQuery query)
    {
        return HandleResult(await Mediator.Send(query));
    }
}
