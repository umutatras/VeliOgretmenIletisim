using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Complaints.Queries.GetComplaints;

public record GetComplaintsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedResult<ComplaintDto>>>;

public class ComplaintDto
{
    public Guid Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public string? AdminResponse { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class GetComplaintsQueryHandler : IRequestHandler<GetComplaintsQuery, Result<PagedResult<ComplaintDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public GetComplaintsQueryHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResult<ComplaintDto>>> Handle(GetComplaintsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var isAdmin = _currentUserService.Role == "Admin";

        var query = _uow.GetRepository<ComplaintRequest>()
            .GetAll()
            .AsNoTracking()
            .Include(c => c.Parent).ThenInclude(p => p.AppUser)
            .OrderByDescending(c => c.CreatedDate)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(c => c.Parent.AppUserId == userId);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ComplaintDto
            {
                Id = c.Id,
                Subject = c.Subject,
                Description = c.Description,
                Status = c.Status.ToString(),
                ParentName = $"{c.Parent.AppUser.FirstName} {c.Parent.AppUser.LastName}",
                AdminResponse = c.AdminResponse,
                CreatedDate = c.CreatedDate
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<ComplaintDto>(items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<ComplaintDto>>.Success(pagedResult);
    }
}
