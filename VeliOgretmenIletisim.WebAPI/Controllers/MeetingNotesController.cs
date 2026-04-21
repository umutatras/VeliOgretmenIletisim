using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeliOgretmenIletisim.Application.Features.MeetingNotes.Commands.CreateMeetingNote;
using VeliOgretmenIletisim.Application.Features.MeetingNotes.Queries.GetMeetingNotes;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

[Authorize]
public class MeetingNotesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return HandleResult(await Mediator.Send(new GetMeetingNotesQuery(pageNumber, pageSize)));
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Create([FromBody] CreateMeetingNoteCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }
}
