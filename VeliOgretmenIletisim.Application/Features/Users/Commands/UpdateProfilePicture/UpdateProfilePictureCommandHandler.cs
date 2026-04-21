using MediatR;
using Microsoft.AspNetCore.Identity;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Users.Commands.UpdateProfilePicture;

public class UpdateProfilePictureCommandHandler : IRequestHandler<UpdateProfilePictureCommand, Result<string>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProfilePictureCommandHandler(UserManager<AppUser> userManager, ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<Result<string>> Handle(UpdateProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
        {
            return Result<string>.Failure("Kullanıcı oturumu bulunamadı.");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result<string>.Failure("Kullanıcı bulunamadı.");
        }

        user.ProfilePicturePath = request.ProfilePicturePath;
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return Result<string>.Success(user.ProfilePicturePath, "Profil resmi başarıyla güncellendi.");
        }

        return Result<string>.Failure("Profil resmi güncellenirken bir hata oluştu.");
    }
}
