using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Students.Queries.GetMyStudents;

public class GetMyStudentsQueryHandler : IRequestHandler<GetMyStudentsQuery, Result<List<MyStudentDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public GetMyStudentsQueryHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<MyStudentDto>>> Handle(GetMyStudentsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        
        // Find the parent profile linked to this user
        var parent = await _uow.GetRepository<Parent>()
            .GetAll()
            .Include(p => p.Children)
            .ThenInclude(c => c.Teacher)
            .ThenInclude(t => t.AppUser)
            .FirstOrDefaultAsync(p => p.AppUserId == userId, cancellationToken);

        if (parent == null)
            return Result<List<MyStudentDto>>.Failure("Veli profili bulunamadı.");

        var children = parent.Children.Select(c => new MyStudentDto(
            c.Id,
            $"{c.FirstName} {c.LastName}",
            c.StudentNumber,
            c.TeacherId,
            c.Teacher?.AppUser?.FullName ?? "Atanmamış"
        )).ToList();

        return Result<List<MyStudentDto>>.Success(children);
    }
}
