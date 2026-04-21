using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Availabilities.Commands.CreateAvailability;

public class CreateAvailabilityCommand : IRequest<Result>
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxCapacity { get; set; } = 15;
    public bool IsGroup { get; set; }
}
