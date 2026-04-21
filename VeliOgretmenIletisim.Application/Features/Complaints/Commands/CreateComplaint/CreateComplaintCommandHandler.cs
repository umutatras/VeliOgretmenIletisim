using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Complaints.Commands.CreateComplaint;

public class CreateComplaintCommandHandler : IRequestHandler<CreateComplaintCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public CreateComplaintCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(CreateComplaintCommand request, CancellationToken cancellationToken)
    {
        var parent = await _uow.GetRepository<Parent>()
            .Where(x => x.AppUserId == _currentUserService.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (parent == null)
            return Result.Failure("Parent profile not found.");

        var complaint = new ComplaintRequest
        {
            ParentId = parent.Id,
            Subject = request.Subject,
            Description = request.Description,
            Status = ComplaintStatus.Pending,
            IsRead = false
        };

        await _uow.GetRepository<ComplaintRequest>().AddAsync(complaint);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success("Complaint/Request submitted successfully.");
    }
}
