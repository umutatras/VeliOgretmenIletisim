using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Teachers.Queries.GetMyStudentsForTeacher;

public class GetMyStudentsForTeacherQueryHandler : IRequestHandler<GetMyStudentsForTeacherQuery, Result<List<TeacherStudentDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public GetMyStudentsForTeacherQueryHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<TeacherStudentDto>>> Handle(GetMyStudentsForTeacherQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var teacher = await _uow.GetRepository<Teacher>()
            .GetAll()
            .FirstOrDefaultAsync(t => t.AppUserId == currentUserId, cancellationToken);

        if (teacher == null)
            return Result<List<TeacherStudentDto>>.Failure("Öğretmen profili bulunamadı.");

        var students = await _uow.GetRepository<Student>()
            .GetAll()
            .Include(s => s.Parent)
            .ThenInclude(p => p.AppUser)
            .Where(s => s.TeacherId == teacher.Id)
            .Select(s => new TeacherStudentDto(
                s.Id,
                $"{s.FirstName} {s.LastName}",
                s.StudentNumber,
                s.ParentId,
                $"{s.Parent.AppUser.FirstName} {s.Parent.AppUser.LastName}"
            ))
            .ToListAsync(cancellationToken);

        return Result<List<TeacherStudentDto>>.Success(students);
    }
}
