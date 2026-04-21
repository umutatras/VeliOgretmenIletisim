using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.MeetingNotes.Commands.CreateMeetingNote;

public class CreateMeetingNoteCommandHandler : IRequestHandler<CreateMeetingNoteCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public CreateMeetingNoteCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(CreateMeetingNoteCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _uow.GetRepository<Teacher>()
            .Where(x => x.AppUserId == _currentUserService.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (teacher == null) return Result.Failure("Teacher profile not found.");

        // İş Kuralı: Öğretmen sadece kendi öğrencisinin velisine not yazabilir mi?
        // Velinin bu öğretmenin öğrencisine sahip olduğunu kontrol et
        var isLinkExists = await _uow.GetRepository<Student>()
            .Where(s => s.TeacherId == teacher.Id && s.ParentId == request.ParentId)
            .AnyAsync(cancellationToken);

        if (!isLinkExists)
            return Result.Failure("You can only send notes to your own students' parents.");

        var meetingNote = new MeetingNote
        {
            TeacherId = teacher.Id,
            ParentId = request.ParentId,
            Note = request.Note
        };

        await _uow.GetRepository<MeetingNote>().AddAsync(meetingNote);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success("Meeting note shared successfully.");
    }
}
