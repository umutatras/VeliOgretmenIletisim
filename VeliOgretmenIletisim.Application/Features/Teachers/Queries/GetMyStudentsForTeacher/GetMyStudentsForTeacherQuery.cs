using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Teachers.Queries.GetMyStudentsForTeacher;

public record TeacherStudentDto(
    Guid Id, 
    string FirstName, 
    string LastName, 
    string StudentNumber, 
    Guid ParentId, 
    string ParentName,
    List<string> TeacherNames,
    List<Guid> TeacherIds
);

public class GetMyStudentsForTeacherQuery : IRequest<Result<PagedResult<TeacherStudentDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SearchTerm { get; set; }

    public GetMyStudentsForTeacherQuery(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }
}
