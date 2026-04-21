using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Students.Queries.GetMyStudents;

public record MyStudentDto(Guid Id, string FullName, string StudentNumber, Guid TeacherId, string TeacherName);

public class GetMyStudentsQuery : IRequest<Result<List<MyStudentDto>>>
{
}
