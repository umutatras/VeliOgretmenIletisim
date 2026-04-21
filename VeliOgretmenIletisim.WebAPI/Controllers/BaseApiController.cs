using MediatR;
using Microsoft.AspNetCore.Mvc;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private IMediator? _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected ActionResult HandleResult(Result result)
    {
        return result.StatusCode switch
        {
            200 => Ok(result),
            201 => Created("", result),
            204 => NoContent(),
            400 => BadRequest(result),
            401 => Unauthorized(result),
            403 => Forbid(),
            404 => NotFound(result),
            _ => StatusCode(result.StatusCode, result)
        };
    }

    protected ActionResult HandleResult<T>(Result<T> result)
    {
        return result.StatusCode switch
        {
            200 => Ok(result),
            201 => Created("", result),
            204 => NoContent(),
            400 => BadRequest(result),
            401 => Unauthorized(result),
            403 => Forbid(),
            404 => NotFound(result),
            _ => StatusCode(result.StatusCode, result)
        };
    }
}
