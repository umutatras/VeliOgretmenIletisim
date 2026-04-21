using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Teachers.Queries.GetMyStudentsForTeacher;

public record TeacherStudentDto(Guid StudentId, string StudentName, string StudentNumber, Guid ParentId, string ParentName);

public class GetMyStudentsForTeacherQuery : IRequest<Result<List<TeacherStudentDto>>>
{
}
