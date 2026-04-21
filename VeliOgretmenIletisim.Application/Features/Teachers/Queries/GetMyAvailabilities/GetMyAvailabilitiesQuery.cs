using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Teachers.Queries.GetMyAvailabilities;

public record TeacherAvailabilityDto(Guid Id, DateTime StartTime, DateTime EndTime, int MaxCapacity, bool IsGroup);

public class GetMyAvailabilitiesQuery : IRequest<Result<List<TeacherAvailabilityDto>>>
{
}
