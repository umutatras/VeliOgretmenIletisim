using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Teachers.Queries.GetMyStudentsForTeacher;

public class GetMyStudentsForTeacherQueryHandler : IRequestHandler<GetMyStudentsForTeacherQuery, Result<PagedResult<TeacherStudentDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public GetMyStudentsForTeacherQueryHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResult<TeacherStudentDto>>> Handle(GetMyStudentsForTeacherQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var studentsQuery = _uow.GetRepository<Student>()
            .GetAll()
            .AsNoTracking()
            .Include(s => s.Parent)
                .ThenInclude(p => p.AppUser)
            .Include(s => s.StudentTeachers)
                .ThenInclude(st => st.Teacher)
                    .ThenInclude(t => t.AppUser)
            .AsQueryable();

        if (_currentUserService.Role != "Admin")
        {
            var teacher = await _uow.GetRepository<Teacher>()
                .GetAll()
                .FirstOrDefaultAsync(t => t.AppUserId == currentUserId, cancellationToken);

            if (teacher == null)
                return Result<PagedResult<TeacherStudentDto>>.Failure("Öğretmen profili bulunamadı.");

            studentsQuery = studentsQuery.Where(s => s.StudentTeachers.Any(st => st.TeacherId == teacher.Id));
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            studentsQuery = studentsQuery.Where(s =>
                s.FirstName.ToLower().Contains(searchTerm) ||
                s.LastName.ToLower().Contains(searchTerm) ||
                s.StudentNumber.ToLower().Contains(searchTerm));
        }

        var totalCount = await studentsQuery.CountAsync(cancellationToken);

        var students = await studentsQuery
            .OrderBy(s => s.FirstName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new TeacherStudentDto(
                s.Id,
                s.FirstName,
                s.LastName,
                s.StudentNumber,
                s.PhoneNumber,
                s.ParentId,
                s.Parent != null && s.Parent.AppUser != null
                    ? s.Parent.AppUser.FirstName + " " + s.Parent.AppUser.LastName
                    : "Bilinmiyor",
                s.StudentTeachers
                    .Select(st => st.Teacher != null && st.Teacher.AppUser != null
                        ? st.Teacher.AppUser.FirstName + " " + st.Teacher.AppUser.LastName
                        : "Bilinmiyor")
                    .ToList(),
                s.StudentTeachers.Select(st => st.TeacherId).ToList()
            ))
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<TeacherStudentDto>(students, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<TeacherStudentDto>>.Success(pagedResult);
    }
}
