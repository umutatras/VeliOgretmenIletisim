using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeliOgretmenIletisim.Application.Features.Appointments.Commands.AssignAppointment;
using VeliOgretmenIletisim.Application.Features.Appointments.Queries.GetAppointments;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

[Authorize]
public class AppointmentsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return HandleResult(await Mediator.Send(new GetAppointmentsQuery(pageNumber, pageSize)));
    }

    [HttpPost]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> Create([FromBody] AssignAppointmentCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }
}
