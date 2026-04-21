using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Domain.Entities;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Admin.Queries.GetUsersByRole;

public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, Result<List<UserBriefDto>>>
{
    private readonly UserManager<AppUser> _userManager;

    public GetUsersByRoleQueryHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<List<UserBriefDto>>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users
            .Where(u => u.Role == request.Role && u.IsActive)
            .OrderBy(u => u.FirstName)
            .Select(u => new UserBriefDto(u.Id, u.FullName, u.Email!))
            .ToListAsync(cancellationToken);

        return Result<List<UserBriefDto>>.Success(users);
    }
}
