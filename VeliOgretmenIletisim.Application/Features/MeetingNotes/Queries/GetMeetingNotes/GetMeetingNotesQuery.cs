using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.MeetingNotes.Queries.GetMeetingNotes;

public record GetMeetingNotesQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedResult<MeetingNoteDto>>>;

public class MeetingNoteDto
{
    public Guid Id { get; set; }
    public string Note { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public class GetMeetingNotesQueryHandler : IRequestHandler<GetMeetingNotesQuery, Result<PagedResult<MeetingNoteDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public GetMeetingNotesQueryHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResult<MeetingNoteDto>>> Handle(GetMeetingNotesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var query = _uow.GetRepository<MeetingNote>()
            .GetAll()
            .AsNoTracking()
            .Include(m => m.Teacher).ThenInclude(t => t.AppUser)
            .Include(m => m.Parent).ThenInclude(p => p.AppUser)
            .Where(m => m.Teacher.AppUserId == userId || m.Parent.AppUserId == userId) // Filter by user context
            .OrderByDescending(m => m.CreatedDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MeetingNoteDto
            {
                Id = m.Id,
                Note = m.Note,
                TeacherName = $"{m.Teacher.AppUser.FirstName} {m.Teacher.AppUser.LastName}",
                ParentName = $"{m.Parent.AppUser.FirstName} {m.Parent.AppUser.LastName}",
                CreatedDate = m.CreatedDate
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<MeetingNoteDto>(items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<MeetingNoteDto>>.Success(pagedResult);
    }
}
