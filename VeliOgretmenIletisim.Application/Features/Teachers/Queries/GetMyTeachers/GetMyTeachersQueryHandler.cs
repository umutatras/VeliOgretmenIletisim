using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;


namespace VeliOgretmenIletisim.Application.Features.Teachers.Queries.GetMyTeachers;

public class GetMyTeachersQuery : IRequest<Result<List<TeacherDto>>> { }

public class TeacherDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string DepartmentName { get; set; }
}

public class GetMyTeachersQueryHandler : IRequestHandler<GetMyTeachersQuery, Result<List<TeacherDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public GetMyTeachersQueryHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<TeacherDto>>> Handle(GetMyTeachersQuery request, CancellationToken cancellationToken)
    {
        // 1. Mevcut AppUser ID'sini kullanarak Parent ID'yi bul
        var userId = _currentUserService.UserId;
        var parent = await _uow.GetRepository<Parent>()
            .Where(x => x.AppUserId == userId)
            .Select(x => new { x.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (parent == null) return Result<List<TeacherDto>>.Failure("Parent profile not found.");

        // 2. Velinin çocuklarının öğretmenlerini Select Projection ile çek (Performans: AsNoTracking + Select)
        var teachers = await _uow.GetRepository<Student>()
            .Where(s => s.ParentId == parent.Id)
            .SelectMany(s => s.StudentTeachers)
            .Select(st => st.Teacher)
            .Distinct()
            .Select(t => new TeacherDto
            {
                Id = t.Id,
                FullName = $"{t.AppUser.FirstName} {t.AppUser.LastName}",
                DepartmentName = t.Department.Name
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result<List<TeacherDto>>.Success(teachers);
    }
}
