using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Appointments.Queries.GetAvailabilitiesForParent;

public class GetAvailabilitiesForParentQueryHandler : IRequestHandler<GetAvailabilitiesForParentQuery, Result<List<TeacherAvailabilityDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public GetAvailabilitiesForParentQueryHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<TeacherAvailabilityDto>>> Handle(GetAvailabilitiesForParentQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        // Get Parent Id
        var parent = await _uow.GetRepository<Parent>()
            .GetAll()
            .FirstOrDefaultAsync(p => p.AppUserId == currentUserId, cancellationToken);

        if (parent == null)
            return Result<List<TeacherAvailabilityDto>>.Failure("Veli bulunamadı.");

        // Get Teachers of Parent's students
        var teacherIds = await _uow.GetRepository<Student>()
            .Where(s => s.ParentId == parent.Id)
            .SelectMany(s => s.StudentTeachers)
            .Select(st => st.TeacherId)
            .Distinct()
            .ToListAsync(cancellationToken);

        // Get Availabilities of these teachers
        var now = DateTime.Now;
        var availabilities = await _uow.GetRepository<Availability>()
            .Where(a => teacherIds.Contains(a.TeacherId) && a.StartTime > now)
            .Include(a => a.Teacher)
            .ThenInclude(t => t.AppUser)
            .Include(a => a.Appointments)
            .Select(a => new TeacherAvailabilityDto(
                a.Id,
                a.TeacherId,
                a.Teacher.AppUser.FirstName + " " + a.Teacher.AppUser.LastName,
                a.StartTime,
                a.EndTime,
                a.MaxCapacity,
                a.Appointments.Count(ap => ap.Status != AppointmentStatus.Cancelled),
                a.IsGroup,
                a.Appointments.Count(ap => ap.Status != AppointmentStatus.Cancelled) >= a.MaxCapacity
            ))
            .ToListAsync(cancellationToken);

        return Result<List<TeacherAvailabilityDto>>.Success(availabilities);
    }
}
