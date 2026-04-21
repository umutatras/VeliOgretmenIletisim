using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Students.Commands.TeacherAddStudent;

public class TeacherAddStudentCommand : IRequest<Result<Guid>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string StudentNumber { get; set; }
    public Guid ParentId { get; set; }
    public Guid? TeacherId { get; set; }
}
