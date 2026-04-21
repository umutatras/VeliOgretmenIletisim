using MediatR;
using Microsoft.AspNetCore.Identity;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Users.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProfileCommandHandler(UserManager<AppUser> userManager, ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(_currentUserService.UserId.ToString());
        if (user == null)
            return Result.Failure("Kullanıcı bulunamadı.");

        user.FirstName = request.FullName.Split(' ').FirstOrDefault() ?? "";
        user.LastName = request.FullName.Contains(' ') ? request.FullName.Substring(request.FullName.IndexOf(' ') + 1) : "";
        user.PhoneNumber = request.PhoneNumber;
        
        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            var emailExists = await _userManager.FindByEmailAsync(request.Email);
            if (emailExists != null)
                return Result.Failure("Bu e-posta adresi zaten kullanımda.");
            
            user.Email = request.Email;
            user.UserName = request.Email;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));

        return Result.Success("Profil başarıyla güncellendi.");
    }
}
