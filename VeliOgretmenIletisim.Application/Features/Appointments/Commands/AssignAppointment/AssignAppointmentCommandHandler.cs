using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Notifications;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Appointments.Commands.AssignAppointment;

public class AssignAppointmentCommandHandler : IRequestHandler<AssignAppointmentCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly INotificationService _notificationService;

    public AssignAppointmentCommandHandler(IUnitOfWork uow, INotificationService notificationService)
    {
        _uow = uow;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(AssignAppointmentCommand request, CancellationToken cancellationToken)
    {
        var availability = await _uow.GetRepository<Availability>()
            .GetAll()
            .Include(a => a.Teacher)
            .ThenInclude(t => t.AppUser)
            .FirstOrDefaultAsync(a => a.Id == request.AvailabilityId, cancellationToken);

        var student = await _uow.GetRepository<Student>()
            .GetAll()
            .Include(s => s.Parent)
            .ThenInclude(p => p.AppUser)
            .FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken);

        if (availability == null || student == null)
            return Result.Failure("Müsaitlik veya Öğrenci bulunamadı.");

        var currentAppointments = await _uow.GetRepository<Appointment>()
            .Where(x => x.AvailabilityId == request.AvailabilityId && x.Status != AppointmentStatus.Cancelled)
            .CountAsync(cancellationToken);

        if (currentAppointments >= availability.MaxCapacity)
            return Result.Failure("Bu zaman dilimi dolmuştur.");

        var appointment = new Appointment
        {
            AvailabilityId = request.AvailabilityId,
            ParentId = student.ParentId,
            StudentId = request.StudentId,
            Status = AppointmentStatus.Pending,
            Note = request.Note
        };

        await _uow.GetRepository<Appointment>().AddAsync(appointment);
        await _uow.SaveChangesAsync(cancellationToken);

        await _notificationService.SendToUserAsync(availability.Teacher.AppUserId,
            $"Yeni bir randevu talebi var: {student.FirstName} {student.LastName} - {availability.StartTime:dd/MM/yyyy HH:mm}", "Randevu");

        return Result.Success("Randevu başarıyla oluşturuldu.");
    }
}
