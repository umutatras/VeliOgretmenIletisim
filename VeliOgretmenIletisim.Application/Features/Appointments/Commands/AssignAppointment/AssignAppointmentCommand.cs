using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Appointments.Commands.AssignAppointment;

public class AssignAppointmentCommand : IRequest<Result>
{
    public Guid AvailabilityId { get; set; }
    public Guid StudentId { get; set; }
    public string? Note { get; set; }
}
