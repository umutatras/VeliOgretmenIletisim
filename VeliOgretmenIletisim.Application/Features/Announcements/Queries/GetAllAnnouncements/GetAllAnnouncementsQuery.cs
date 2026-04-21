using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Announcements.Queries.GetAllAnnouncements;

public record GetAllAnnouncementsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedResult<AnnouncementDto>>>;

public class AnnouncementDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? AttachmentPath { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public class GetAllAnnouncementsQueryHandler : IRequestHandler<GetAllAnnouncementsQuery, Result<PagedResult<AnnouncementDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetAllAnnouncementsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PagedResult<AnnouncementDto>>> Handle(GetAllAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var query = _uow.GetRepository<Announcement>()
            .GetAll()
            .AsNoTracking()
            .Include(a => a.Teacher)
            .ThenInclude(t => t.AppUser)
            .OrderByDescending(a => a.CreatedDate);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AnnouncementDto
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                AttachmentPath = a.AttachmentPath,
                TeacherName = $"{a.Teacher.AppUser.FirstName} {a.Teacher.AppUser.LastName}",
                CreatedDate = a.CreatedDate
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<AnnouncementDto>(items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<AnnouncementDto>>.Success(pagedResult);
    }
}
