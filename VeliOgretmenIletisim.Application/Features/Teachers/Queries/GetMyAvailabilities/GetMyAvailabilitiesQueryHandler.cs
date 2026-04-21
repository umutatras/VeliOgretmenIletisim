using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Teachers.Queries.GetMyAvailabilities;

public class GetMyAvailabilitiesQueryHandler : IRequestHandler<GetMyAvailabilitiesQuery, Result<List<TeacherAvailabilityDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public GetMyAvailabilitiesQueryHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<TeacherAvailabilityDto>>> Handle(GetMyAvailabilitiesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var teacher = await _uow.GetRepository<Teacher>()
            .GetAll()
            .FirstOrDefaultAsync(t => t.AppUserId == userId, cancellationToken);

        if (teacher == null) return Result<List<TeacherAvailabilityDto>>.Failure($"Öğretmen profili bulunamadı. (UserId: {userId})");

        var availabilities = await _uow.GetRepository<Availability>()
            .GetAll()
            .Where(a => a.TeacherId == teacher.Id)
            .OrderBy(a => a.StartTime)
            .Select(a => new TeacherAvailabilityDto(
                a.Id,
                a.StartTime,
                a.EndTime,
                a.MaxCapacity,
                a.IsGroup
            ))
            .ToListAsync(cancellationToken);

        return Result<List<TeacherAvailabilityDto>>.Success(availabilities);
    }
}
