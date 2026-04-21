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
    private readonly INotificationDispatcher _notificationDispatcher;

    public AssignAppointmentCommandHandler(IUnitOfWork uow, INotificationDispatcher notificationDispatcher)
    {
        _uow = uow;
        _notificationDispatcher = notificationDispatcher;
    }

    public async Task<Result> Handle(AssignAppointmentCommand request, CancellationToken cancellationToken)
    {
        var availability = await _uow.GetRepository<Availability>().GetByIdAsync(request.AvailabilityId);
        var parent = await _uow.GetRepository<Parent>()
            .GetAll()
            .Include(x => x.AppUser)
            .FirstOrDefaultAsync(x => x.Id == request.ParentId, cancellationToken);

        if (availability == null || parent == null)
            return Result.Failure("Availability or Parent not found.");

        // Kapasite kontrolü
        var currentAppointments = await _uow.GetRepository<Appointment>()
            .Where(x => x.AvailabilityId == request.AvailabilityId && x.Status != AppointmentStatus.Cancelled)
            .CountAsync(cancellationToken);

        if (currentAppointments >= availability.MaxCapacity)
            return Result.Failure("This slot is full.");

        var appointment = new Appointment
        {
            AvailabilityId = request.AvailabilityId,
            ParentId = request.ParentId,
            Status = AppointmentStatus.Approved
        };

        await _uow.GetRepository<Appointment>().AddAsync(appointment);
        await _uow.SaveChangesAsync(cancellationToken);

        // Background Job Dispatching
        _notificationDispatcher.ScheduleEmail(
            parent.AppUser.Email,
            "New Appointment Assigned",
            $"You have a new appointment on {availability.StartTime:dd/MM/yyyy HH:mm}");

        if (!string.IsNullOrEmpty(parent.AppUser.PhoneNumber))
        {
            _notificationDispatcher.ScheduleSms(
                parent.AppUser.PhoneNumber,
                $"Appointment assigned: {availability.StartTime}");
        }

        return Result.Success("Appointment assigned and notifications scheduled.");
    }
}
