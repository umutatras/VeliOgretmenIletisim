using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Parents.Queries.GetMyStudents;

public class GetMyStudentsQueryHandler : IRequestHandler<GetMyStudentsQuery, Result<List<ParentStudentDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public GetMyStudentsQueryHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<ParentStudentDto>>> Handle(GetMyStudentsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var parent = await _uow.GetRepository<Parent>()
            .GetAll()
            .FirstOrDefaultAsync(p => p.AppUserId == currentUserId, cancellationToken);

        if (parent == null)
            return Result<List<ParentStudentDto>>.Failure("Veli profili bulunamadı.");

        var students = await _uow.GetRepository<Student>()
            .Where(s => s.ParentId == parent.Id)
            .Select(s => new ParentStudentDto(
                s.Id,
                s.FirstName,
                s.LastName,
                s.StudentNumber,
                s.PhoneNumber
            ))
            .ToListAsync(cancellationToken);

        return Result<List<ParentStudentDto>>.Success(students);
    }
}
