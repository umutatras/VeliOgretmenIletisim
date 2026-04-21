using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Availabilities.Commands.CreateAvailability;

public class CreateAvailabilityCommandHandler : IRequestHandler<CreateAvailabilityCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public CreateAvailabilityCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(CreateAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _uow.GetRepository<Teacher>()
            .Where(x => x.AppUserId == _currentUserService.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (teacher == null)
            return Result.Failure("Teacher profile not found.");

        if (request.StartTime >= request.EndTime)
            return Result.Failure("Start time must be before end time.");

        var availability = new Availability
        {
            TeacherId = teacher.Id,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            MaxCapacity = request.IsGroup ? request.MaxCapacity : 1,
            IsGroup = request.IsGroup
        };

        await _uow.GetRepository<Availability>().AddAsync(availability);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success("Availability defined successfully.");
    }
}
