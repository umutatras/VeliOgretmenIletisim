using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Appointments.Commands.UpdateAppointmentStatus;

public class UpdateAppointmentStatusCommandHandler : IRequestHandler<UpdateAppointmentStatusCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;

    public UpdateAppointmentStatusCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService, INotificationService notificationService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var teacher = await _uow.GetRepository<Teacher>()
            .Where(t => t.AppUserId == currentUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (teacher == null)
            return Result.Failure("Öğretmen profili bulunamadı.");

        var appointment = await _uow.GetRepository<Appointment>()
            .Where(a => a.Id == request.AppointmentId)
            .Include(a => a.Availability)
            .FirstOrDefaultAsync(cancellationToken);

        if (appointment == null)
            return Result.Failure("Randevu bulunamadı.");

        // Check ownership
        if (appointment.Availability.TeacherId != teacher.Id)
            return Result.Failure("Bu randevuyu güncelleme yetkiniz bulunmamaktadır.", 403);

        appointment.Status = request.Status;
        appointment.TeacherNote = request.TeacherNote;

        await _uow.SaveChangesAsync(cancellationToken);

        // Notify Parent
        var parent = await _uow.GetRepository<Parent>().FirstOrDefaultAsync(p => p.Id == appointment.ParentId);
        if (parent != null)
        {
            var statusTextNotif = request.Status == Domain.Enums.AppointmentStatus.Approved ? "Onaylandı" : "Reddedildi";
            await _notificationService.SendToUserAsync(parent.AppUserId.ToString(), "Randevu Durumu Güncellendi", $"{statusTextNotif}: {appointment.Availability.StartTime:dd.MM.yyyy HH:mm} tarihli randevunuzun durumu güncellendi.", "AppointmentStatusChanged");
        }

        var statusText = request.Status == Domain.Enums.AppointmentStatus.Approved ? "onaylandı" : "reddedildi";
        return Result.Success($"Randevu başarıyla {statusText}.");
    }
}
