using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Appointments.Queries.GetAvailabilitiesForParent;

public record GetAvailabilitiesForParentQuery : IRequest<Result<List<TeacherAvailabilityDto>>>;

public record TeacherAvailabilityDto(
    Guid Id,
    Guid TeacherId,
    string TeacherName,
    DateTime StartTime,
    DateTime EndTime,
    int MaxCapacity,
    int CurrentCount,
    bool IsGroup,
    bool IsFull
);
