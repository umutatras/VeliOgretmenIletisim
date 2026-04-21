using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Availabilities.Commands.UpdateAvailability;

public class UpdateAvailabilityCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxCapacity { get; set; }
    public bool IsGroup { get; set; }
}
