using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeliOgretmenIletisim.Application.Features.Admin.Commands.ApproveUser;
using VeliOgretmenIletisim.Application.Features.Departments.Commands.CreateDepartment;
using VeliOgretmenIletisim.Application.Features.Departments.Commands.DeleteDepartment;
using VeliOgretmenIletisim.Application.Features.Departments.Queries.GetAllDepartments;
using VeliOgretmenIletisim.Application.Features.Students.Commands.AdminAddStudent;
using VeliOgretmenIletisim.Application.Features.Students.Queries.GetAllStudents;
using VeliOgretmenIletisim.Application.Features.Teachers.Commands.AssignDepartment;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : BaseApiController
{
    [HttpPost("approve-user")]
    public async Task<IActionResult> ApproveUser([FromBody] ApproveUserCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [HttpPost("change-role")]
    public async Task<IActionResult> ChangeRole([FromBody] VeliOgretmenIletisim.Application.Features.Admin.Commands.ChangeUserRole.ChangeUserRoleCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        return HandleResult(await Mediator.Send(new VeliOgretmenIletisim.Application.Features.Admin.Queries.GetAuditLogs.GetAuditLogsQuery(pageNumber, pageSize)));
    }

    // Departments
    [HttpGet("departments")]
    public async Task<IActionResult> GetDepartments()
    {
        return HandleResult(await Mediator.Send(new GetAllDepartmentsQuery()));
    }

    [HttpPost("departments")]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [HttpDelete("departments/{id}")]
    public async Task<IActionResult> DeleteDepartment(Guid id)
    {
        return HandleResult(await Mediator.Send(new DeleteDepartmentCommand(id)));
    }

    // Students
    [HttpGet("students")]
    public async Task<IActionResult> GetStudents([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return HandleResult(await Mediator.Send(new GetAllStudentsQuery(pageNumber, pageSize)));
    }

    [HttpPost("students")]
    public async Task<IActionResult> AddStudent([FromBody] AdminAddStudentCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [HttpPost("teacher-department")]
    public async Task<IActionResult> AssignDepartment([FromBody] AssignDepartmentCommand command)
    {
        return HandleResult(await Mediator.Send(command));
    }
}
