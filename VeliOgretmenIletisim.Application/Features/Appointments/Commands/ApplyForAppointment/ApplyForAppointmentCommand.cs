using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Appointments.Commands.ApplyForAppointment;

public class ApplyForAppointmentCommand : IRequest<Result<Guid>>
{
    public Guid AvailabilityId { get; set; }
    public Guid? StudentId { get; set; }
    public string? Note { get; set; }
}
