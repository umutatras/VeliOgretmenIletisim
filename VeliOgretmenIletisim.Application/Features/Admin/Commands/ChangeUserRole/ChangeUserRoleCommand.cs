using MediatR;
using Microsoft.AspNetCore.Identity;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Domain.Entities;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Admin.Commands.ChangeUserRole;

public record ChangeUserRoleCommand(Guid UserId, string NewRole) : IRequest<Result>;

public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;

    public ChangeUserRoleCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return Result.Failure("User not found.");

        // Get current roles
        var currentRoles = await _userManager.GetRolesAsync(user);

        // Remove from current roles
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // Add to new role
        var result = await _userManager.AddToRoleAsync(user, request.NewRole);

        if (!result.Succeeded)
            return Result.Failure(result.Errors.Select(e => e.Description).ToList());

        // Also update the Enum Role in AppUser for business logic consistency
        if (Enum.TryParse<UserRole>(request.NewRole, out var roleEnum))
        {
            user.Role = roleEnum;
            await _userManager.UpdateAsync(user);
        }

        return Result.Success($"Role changed to {request.NewRole} successfully.");
    }
}
