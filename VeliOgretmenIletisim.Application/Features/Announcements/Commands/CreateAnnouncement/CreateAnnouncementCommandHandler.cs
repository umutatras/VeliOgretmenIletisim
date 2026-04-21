using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Announcements.Commands.CreateAnnouncement;

public class CreateAnnouncementCommandHandler : IRequestHandler<CreateAnnouncementCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public CreateAnnouncementCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(CreateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _uow.GetRepository<Teacher>()
            .Where(x => x.AppUserId == _currentUserService.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (teacher == null)
            return Result.Failure("Teacher profile not found.");

        var announcement = new Announcement
        {
            Title = request.Title,
            Content = request.Content,
            AttachmentPath = request.AttachmentPath,
            TeacherId = teacher.Id
        };

        await _uow.GetRepository<Announcement>().AddAsync(announcement);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success("Announcement published successfully.");
    }
}
