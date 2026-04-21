using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Students.Commands.UpdateStudent;

public class UpdateStudentCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public Guid ParentId { get; set; }
    public Guid? TeacherId { get; set; } // Admin can update this, Teacher cannot (or it's restricted)
}
