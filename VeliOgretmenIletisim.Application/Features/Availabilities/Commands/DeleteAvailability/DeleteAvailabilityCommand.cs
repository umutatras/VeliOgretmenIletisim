using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Availabilities.Commands.DeleteAvailability;

public record DeleteAvailabilityCommand(Guid Id) : IRequest<Result>;
