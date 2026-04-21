using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Notifications;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Admin.Commands.ApproveUser;

public class ApproveUserCommandHandler : IRequestHandler<ApproveUserCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly INotificationDispatcher _notificationDispatcher;

    public ApproveUserCommandHandler(IUnitOfWork uow, INotificationDispatcher notificationDispatcher)
    {
        _uow = uow;
        _notificationDispatcher = notificationDispatcher;
    }

    public async Task<Result> Handle(ApproveUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _uow.GetRepository<AppUser>().GetByIdAsync(request.UserId);

        if (user == null)
            return Result.Failure("User not found.");

        if (user.IsApproved)
            return Result.Success("User is already approved.");

        user.IsApproved = true;
        user.IsActive = true;

        await _uow.SaveChangesAsync(cancellationToken);

        // Bildirim: Arka planda Email gönderimi
        _notificationDispatcher.ScheduleEmail(
            user.Email,
            "Account Approved",
            $"Hello {user.FirstName}, your account has been approved by the administration. You can now log in.");

        return Result.Success($"User {user.Email} has been approved.");
    }
}
