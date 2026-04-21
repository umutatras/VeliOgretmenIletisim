using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Notifications;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Complaints.Commands.AnswerComplaint;

public record AnswerComplaintCommand(Guid ComplaintId, string Response, ComplaintStatus Status) : IRequest<Result>;

public class AnswerComplaintCommandHandler : IRequestHandler<AnswerComplaintCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly INotificationService _notificationService;

    public AnswerComplaintCommandHandler(IUnitOfWork uow, INotificationService notificationService)
    {
        _uow = uow;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(AnswerComplaintCommand request, CancellationToken cancellationToken)
    {
        var repository = _uow.GetRepository<ComplaintRequest>();
        var complaint = await repository.GetAll()
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == request.ComplaintId, cancellationToken);

        if (complaint == null)
            return Result.Failure("Complaint not found.");

        complaint.AdminResponse = request.Response;
        complaint.Status = request.Status;
        complaint.IsRead = true;

        repository.Update(complaint);
        await _uow.SaveChangesAsync(cancellationToken);

        // Real-time notification to Parent
        await _notificationService.SendToUserAsync(complaint.Parent.AppUserId, 
            $"Your complaint '{complaint.Subject}' has been answered by the Admin.", "ComplaintAnswered");

        return Result.Success("Complaint answered successfully.");
    }
}
