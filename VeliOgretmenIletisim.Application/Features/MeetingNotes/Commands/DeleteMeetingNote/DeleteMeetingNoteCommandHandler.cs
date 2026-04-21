using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.MeetingNotes.Commands.DeleteMeetingNote;

public class DeleteMeetingNoteCommandHandler : IRequestHandler<DeleteMeetingNoteCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public DeleteMeetingNoteCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteMeetingNoteCommand request, CancellationToken cancellationToken)
    {
        var meetingNote = await _uow.GetRepository<MeetingNote>().GetByIdAsync(request.Id);

        if (meetingNote == null)
            return Result.Failure("Görüşme notu bulunamadı.", 404);

        // Güvenlik: Sadece notu yazan öğretmen silebilir
        var userId = _currentUserService.UserId;
        var teacher = await _uow.GetRepository<Teacher>()
            .Where(t => t.AppUserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (teacher == null || meetingNote.TeacherId != teacher.Id)
            return Result.Failure("Bu notu silme yetkiniz bulunmamaktadır.", 403);

        _uow.GetRepository<MeetingNote>().Delete(meetingNote);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success("Görüşme notu başarıyla silindi.");
    }
}
