using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Application.Interfaces.Notifications;
using VeliOgretmenIletisim.Domain.Entities;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Appointments.Commands.ApplyForAppointment;

public class ApplyForAppointmentCommandHandler : IRequestHandler<ApplyForAppointmentCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;

    public ApplyForAppointmentCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService, INotificationService notificationService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
    }

    public async Task<Result<Guid>> Handle(ApplyForAppointmentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var parent = await _uow.GetRepository<Parent>()
            .Where(p => p.AppUserId == currentUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (parent == null)
            return Result<Guid>.Failure("Veli bulunamadı.");

        var availability = await _uow.GetRepository<Availability>()
            .Where(a => a.Id == request.AvailabilityId)
            .Include(a => a.Appointments)
            .FirstOrDefaultAsync(cancellationToken);

        if (availability == null)
            return Result<Guid>.Failure("Müsaitlik bulunamadı.");

        // Check Capacity
        var activeAppointments = availability.Appointments.Count(a => a.Status != AppointmentStatus.Cancelled);
        if (activeAppointments >= availability.MaxCapacity)
            return Result<Guid>.Failure("Seçilen saat dilimi dolmuştur.");

        // Check if already applied
        var exists = availability.Appointments.Any(a => a.ParentId == parent.Id && a.Status != AppointmentStatus.Cancelled);
        if (exists)
            return Result<Guid>.Failure("Seçilen öğretmen ve saat dilimi için zaten aktif bir başvurunuz bulunmaktadır.");

        var appointment = new Appointment
        {
            AvailabilityId = availability.Id,
            TeacherId = availability.TeacherId,
            ParentId = parent.Id,
            StudentId = request.StudentId,
            Status = AppointmentStatus.Pending,
            Note = request.Note
        };

        await _uow.GetRepository<Appointment>().AddAsync(appointment);
        await _uow.SaveChangesAsync(cancellationToken);

        // Send Light Signal (No data push, just a "hey" signal)
        var teacher = await _uow.GetRepository<Teacher>().GetAll().Include(t => t.AppUser).FirstOrDefaultAsync(t => t.Id == availability.TeacherId);
        if (teacher != null)
        {
            await _notificationService.SendToUserAsync(teacher.AppUserId, "Yeni bir randevu talebiniz var.", "AppointmentRequest");
        }

        return Result<Guid>.Success(appointment.Id, "Randevu başvurusu başarıyla alındı. Öğretmenin onayı bekleniyor.");
    }
}
