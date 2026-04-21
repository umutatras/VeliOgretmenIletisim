using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Admin.Queries.GetPendingApprovals;

public record GetPendingApprovalsQuery() : IRequest<Result<List<PendingUserDto>>>;

public record PendingUserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    DateTime CreatedDate
);

public class GetPendingApprovalsQueryHandler : IRequestHandler<GetPendingApprovalsQuery, Result<List<PendingUserDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetPendingApprovalsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<List<PendingUserDto>>> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
    {
        var users = await _uow.GetRepository<AppUser>()
            .Where(u => !u.IsApproved)
            .OrderByDescending(u => u.CreatedDate)
            .Select(u => new PendingUserDto(
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email,
                u.Role.ToString(),
                u.CreatedDate
            ))
            .ToListAsync(cancellationToken);

        return Result<List<PendingUserDto>>.Success(users);
    }
}
