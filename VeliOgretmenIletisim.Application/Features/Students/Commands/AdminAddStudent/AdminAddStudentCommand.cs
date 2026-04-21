using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Students.Commands.AdminAddStudent;

public class AdminAddStudentCommand : IRequest<Result<Guid>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string StudentNumber { get; set; }
    public Guid ParentId { get; set; }
    public Guid? TeacherId { get; set; }
}
