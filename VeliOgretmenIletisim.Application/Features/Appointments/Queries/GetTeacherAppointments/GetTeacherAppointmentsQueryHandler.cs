using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Appointments.Queries.GetTeacherAppointments;

public class GetTeacherAppointmentsQueryHandler : IRequestHandler<GetTeacherAppointmentsQuery, Result<List<TeacherAppointmentDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public GetTeacherAppointmentsQueryHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<TeacherAppointmentDto>>> Handle(GetTeacherAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var teacher = await _uow.GetRepository<Teacher>()
            .Where(t => t.AppUserId == currentUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (teacher == null)
            return Result<List<TeacherAppointmentDto>>.Failure("Öğretmen profili bulunamadı.");

        var appointments = await _uow.GetRepository<Appointment>()
            .Where(a => a.TeacherId == teacher.Id)
            .Include(a => a.Parent).ThenInclude(p => p.AppUser)
            .Include(a => a.Student)
            .Include(a => a.Availability)
            .OrderByDescending(a => a.Availability.StartTime)
            .Select(a => new TeacherAppointmentDto(
                a.Id,
                a.Parent.AppUser.FirstName + " " + a.Parent.AppUser.LastName,
                a.Student != null ? a.Student.FirstName + " " + a.Student.LastName : "Belirtilmedi",
                a.Availability.StartTime,
                a.Availability.EndTime,
                a.Status,
                a.Note,
                a.TeacherNote
            ))
            .ToListAsync(cancellationToken);

        return Result<List<TeacherAppointmentDto>>.Success(appointments);
    }
}
