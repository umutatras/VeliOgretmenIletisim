using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeliOgretmenIletisim.Application.Features.Announcements.Commands.CreateAnnouncement;
using VeliOgretmenIletisim.Application.Features.Announcements.Commands.DeleteAnnouncement;
using VeliOgretmenIletisim.Application.Features.Announcements.Queries.GetAllAnnouncements;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

public class AnnouncementsController : BaseApiController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        return HandleResult(await Mediator.Send(new GetAllAnnouncementsQuery(pageNumber, pageSize, searchTerm)));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Create([FromBody] CreateAnnouncementCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteAnnouncementCommand(id)));
    }
}
