using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeliOgretmenIletisim.Application.Features.Complaints.Commands.CreateComplaint;
using VeliOgretmenIletisim.Application.Features.Teachers.Queries.GetMyTeachers;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

[Authorize(Roles = "Parent")]
public class ParentsController : BaseApiController
{
    [HttpGet("my-teachers")]
    public async Task<IActionResult> GetMyTeachers()
    {
        return HandleResult(await Mediator.Send(new GetMyTeachersQuery()));
    }

    [HttpGet("my-students")]
    public async Task<IActionResult> GetMyStudents()
    {
        return HandleResult(await Mediator.Send(new VeliOgretmenIletisim.Application.Features.Students.Queries.GetMyStudents.GetMyStudentsQuery()));
    }

    [HttpPost("complaints")]
    public async Task<IActionResult> CreateComplaint([FromBody] CreateComplaintCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }
}
