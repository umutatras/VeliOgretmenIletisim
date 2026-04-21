using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Students.Commands.UpdateStudent;

public class UpdateStudentCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public Guid ParentId { get; set; }
    public List<Guid> TeacherIds { get; set; } = new();
}
