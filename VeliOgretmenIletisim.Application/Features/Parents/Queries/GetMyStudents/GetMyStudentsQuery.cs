using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Parents.Queries.GetMyStudents;

public record GetMyStudentsQuery : IRequest<Result<List<ParentStudentDto>>>;

public record ParentStudentDto(
    Guid Id,
    string FirstName,
    string LastName,
    string StudentNumber,
    string PhoneNumber
);
