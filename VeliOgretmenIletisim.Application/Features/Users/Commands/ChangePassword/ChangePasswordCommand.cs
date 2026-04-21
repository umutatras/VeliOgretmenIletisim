using MediatR;
using Microsoft.AspNetCore.Identity;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<Result>
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(UserManager<AppUser> userManager, ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Result.Failure("Unauthorized.", 401);

        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user == null)
            return Result.Failure("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        
        if (!result.Succeeded)
            return Result.Failure(result.Errors.Select(e => e.Description).ToList());

        return Result.Success("Password updated successfully.");
    }
}
