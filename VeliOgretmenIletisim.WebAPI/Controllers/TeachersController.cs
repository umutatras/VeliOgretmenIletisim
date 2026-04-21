using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeliOgretmenIletisim.Application.Features.Announcements.Commands.CreateAnnouncement;
using VeliOgretmenIletisim.Application.Features.Availabilities.Commands.CreateAvailability;
using VeliOgretmenIletisim.Application.Features.MeetingNotes.Commands.CreateMeetingNote;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

[Authorize(Roles = "Teacher")]
public class TeachersController : BaseApiController
{
    [HttpGet("my-availabilities")]
    public async Task<IActionResult> GetMyAvailabilities()
    {
        return HandleResult(await Mediator.Send(new VeliOgretmenIletisim.Application.Features.Teachers.Queries.GetMyAvailabilities.GetMyAvailabilitiesQuery()));
    }

    [HttpPost("announcements")]
    public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [HttpPost("availabilities")]
    public async Task<IActionResult> CreateAvailability([FromBody] CreateAvailabilityCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [HttpPost("meeting-notes")]
    public async Task<IActionResult> CreateMeetingNote([FromBody] CreateMeetingNoteCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }
}
