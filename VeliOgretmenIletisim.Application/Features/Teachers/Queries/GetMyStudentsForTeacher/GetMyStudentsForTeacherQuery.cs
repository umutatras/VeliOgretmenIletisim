using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Teachers.Queries.GetMyStudentsForTeacher;

public record TeacherStudentDto(Guid Id, string FirstName, string LastName, string StudentNumber, Guid ParentId, string ParentName);

public class GetMyStudentsForTeacherQuery : IRequest<Result<List<TeacherStudentDto>>>
{
}
