using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Teachers.Commands.AssignDepartment;

public class AssignDepartmentCommand : IRequest<Result>
{
    public Guid TeacherId { get; set; }
    public Guid DepartmentId { get; set; }
}
