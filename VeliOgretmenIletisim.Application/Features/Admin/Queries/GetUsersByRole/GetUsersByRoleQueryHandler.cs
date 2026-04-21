using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Admin.Queries.GetUsersByRole;

public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, Result<List<UserBriefDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetUsersByRoleQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<List<UserBriefDto>>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        if (request.Role == UserRole.Teacher)
        {
            var teachers = await _uow.GetRepository<Teacher>()
                .GetAll()
                .Include(t => t.AppUser)
                .Where(t => t.AppUser.IsActive)
                .OrderBy(t => t.AppUser.FirstName)
                .Select(t => new UserBriefDto(t.Id, $"{t.AppUser.FirstName} {t.AppUser.LastName}", t.AppUser.Email!))
                .ToListAsync(cancellationToken);

            return Result<List<UserBriefDto>>.Success(teachers);
        }

        if (request.Role == UserRole.Parent)
        {
            var parents = await _uow.GetRepository<Parent>()
                .GetAll()
                .Include(p => p.AppUser)
                .Where(p => p.AppUser.IsActive)
                .OrderBy(p => p.AppUser.FirstName)
                .Select(p => new UserBriefDto(p.Id, $"{p.AppUser.FirstName} {p.AppUser.LastName}", p.AppUser.Email!))
                .ToListAsync(cancellationToken);

            return Result<List<UserBriefDto>>.Success(parents);
        }

        // Default or Admin case
        var users = await _uow.GetRepository<AppUser>().Where(u => u.Role == request.Role && u.IsActive)
            .OrderBy(u => u.FirstName)
            .Select(u => new UserBriefDto(u.Id, $"{u.FirstName} {u.LastName}", u.Email!))
            .ToListAsync(cancellationToken);

        return Result<List<UserBriefDto>>.Success(users);
    }
}
