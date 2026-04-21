using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeliOgretmenIletisim.Application.Features.Appointments.Commands.AssignAppointment;
using VeliOgretmenIletisim.Application.Features.Appointments.Queries.GetAppointments;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

[Authorize]
public class AppointmentsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        return HandleResult(await Mediator.Send(new GetAppointmentsQuery(pageNumber, pageSize, searchTerm)));
    }

    [HttpGet("availabilities-for-parent")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> GetAvailabilitiesForParent()
    {
        return HandleResult(await Mediator.Send(new VeliOgretmenIletisim.Application.Features.Appointments.Queries.GetAvailabilitiesForParent.GetAvailabilitiesForParentQuery()));
    }

    [HttpPost("apply")]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> Apply([FromBody] VeliOgretmenIletisim.Application.Features.Appointments.Commands.ApplyForAppointment.ApplyForAppointmentCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [HttpGet("teacher-requests")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetTeacherRequests()
    {
        return HandleResult(await Mediator.Send(new VeliOgretmenIletisim.Application.Features.Appointments.Queries.GetTeacherAppointments.GetTeacherAppointmentsQuery()));
    }

    [HttpPut("status")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UpdateStatus([FromBody] VeliOgretmenIletisim.Application.Features.Appointments.Commands.UpdateAppointmentStatus.UpdateAppointmentStatusCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }
}
