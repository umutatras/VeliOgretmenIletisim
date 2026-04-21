using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Availabilities.Commands.DeleteAvailability;

public class DeleteAvailabilityCommandHandler : IRequestHandler<DeleteAvailabilityCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public DeleteAvailabilityCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var availability = await _uow.GetRepository<Availability>().GetByIdAsync(request.Id);

        if (availability == null)
            return Result.Failure("Müsaitlik kaydı bulunamadı.", 404);

        var userId = _currentUserService.UserId;
        var teacher = await _uow.GetRepository<Teacher>()
            .Where(t => t.AppUserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (teacher == null || availability.TeacherId != teacher.Id)
            return Result.Failure("Bu kaydı silme yetkiniz bulunmamaktadır.", 403);

        _uow.GetRepository<Availability>().Delete(availability);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success("Müsaitlik kaydı başarıyla silindi.");
    }
}
