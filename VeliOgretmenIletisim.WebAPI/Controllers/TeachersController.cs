using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeliOgretmenIletisim.Application.Features.Announcements.Commands.CreateAnnouncement;
using VeliOgretmenIletisim.Application.Features.Availabilities.Commands.CreateAvailability;
using VeliOgretmenIletisim.Application.Features.Availabilities.Commands.UpdateAvailability;
using VeliOgretmenIletisim.Application.Features.Availabilities.Commands.DeleteAvailability;
using VeliOgretmenIletisim.Application.Features.MeetingNotes.Commands.CreateMeetingNote;
using VeliOgretmenIletisim.Application.Features.Students.Commands.TeacherAddStudent;
using VeliOgretmenIletisim.Application.Features.Teachers.Queries.GetMyStudentsForTeacher;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

[Authorize(Roles = "Teacher")]
public class TeachersController : BaseApiController
{
    [HttpGet("my-availabilities")]
    public async Task<IActionResult> GetMyAvailabilities()
    {
        return HandleResult(await Mediator.Send(new VeliOgretmenIletisim.Application.Features.Teachers.Queries.GetMyAvailabilities.GetMyAvailabilitiesQuery()));
    }

    [HttpGet("my-students")]
    public async Task<IActionResult> GetMyStudents()
    {
        return HandleResult(await Mediator.Send(new GetMyStudentsForTeacherQuery()));
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

    [HttpPut("availabilities")]
    public async Task<IActionResult> UpdateAvailability([FromBody] UpdateAvailabilityCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [HttpDelete("availabilities/{id}")]
    public async Task<IActionResult> DeleteAvailability(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteAvailabilityCommand(id)));
    }

    [HttpPost("meeting-notes")]
    public async Task<IActionResult> CreateMeetingNote([FromBody] CreateMeetingNoteCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [HttpPost("students")]
    public async Task<IActionResult> AddStudent([FromBody] TeacherAddStudentCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }
}
