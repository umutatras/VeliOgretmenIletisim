using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; }
}
