using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Appointments.Queries.GetTeacherAppointments;

public record GetTeacherAppointmentsQuery : IRequest<Result<List<TeacherAppointmentDto>>>;

public record TeacherAppointmentDto(
    Guid Id,
    string ParentName,
    string StudentName,
    DateTime StartTime,
    DateTime EndTime,
    AppointmentStatus Status,
    string? Note,
    string? TeacherNote
);
