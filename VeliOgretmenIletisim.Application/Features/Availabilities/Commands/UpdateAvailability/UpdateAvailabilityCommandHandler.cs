using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Availabilities.Commands.UpdateAvailability;

public class UpdateAvailabilityCommandHandler : IRequestHandler<UpdateAvailabilityCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAvailabilityCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var availability = await _uow.GetRepository<Availability>().GetByIdAsync(request.Id);

        if (availability == null)
            return Result.Failure("Müsaitlik kaydı bulunamadı.", 404);

        var userId = _currentUserService.UserId;
        var teacher = await _uow.GetRepository<Teacher>()
            .Where(t => t.AppUserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (teacher == null || availability.TeacherId != teacher.Id)
            return Result.Failure("Bu kaydı güncelleme yetkiniz bulunmamaktadır.", 403);

        availability.StartTime = request.StartTime;
        availability.EndTime = request.EndTime;
        availability.MaxCapacity = request.MaxCapacity;
        availability.IsGroup = request.IsGroup;

        _uow.GetRepository<Availability>().Update(availability);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success("Müsaitlik kaydı başarıyla güncellendi.");
    }
}
