using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Appointments.Commands.UpdateAppointmentStatus;

public class UpdateAppointmentStatusCommand : IRequest<Result>
{
    public Guid AppointmentId { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? TeacherNote { get; set; }
}
