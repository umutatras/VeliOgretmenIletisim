using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeliOgretmenIletisim.Application.Features.Complaints.Commands.AnswerComplaint;
using VeliOgretmenIletisim.Application.Features.Complaints.Commands.CreateComplaint;
using VeliOgretmenIletisim.Application.Features.Complaints.Queries.GetComplaints;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

[Authorize]
public class ComplaintsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return HandleResult(await Mediator.Send(new GetComplaintsQuery(pageNumber, pageSize)));
    }

    [HttpPost]
    [Authorize(Roles = "Parent")]
    public async Task<IActionResult> Create([FromBody] CreateComplaintCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [HttpPost("answer")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Answer([FromBody] AnswerComplaintCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }
}
