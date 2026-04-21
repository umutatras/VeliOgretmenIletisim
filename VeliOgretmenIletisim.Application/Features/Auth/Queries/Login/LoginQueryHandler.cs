using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Auth.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<LoginResponseDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtProvider _jwtProvider;

    public LoginQueryHandler(UserManager<AppUser> userManager, IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !user.IsActive)
        {
            return Result<LoginResponseDto>.Failure("Invalid email or password.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        
        if (!isPasswordValid)
        {
            return Result<LoginResponseDto>.Failure("Invalid email or password.");
        }

        if (!user.IsApproved && user.Role != Domain.Enums.UserRole.Admin)
        {
            return Result<LoginResponseDto>.Failure("Your account is pending admin approval.");
        }

        var token = _jwtProvider.GenerateToken(user);

        return Result<LoginResponseDto>.Success(new LoginResponseDto
        {
            Token = token,
            FullName = $"{user.FirstName} {user.LastName}",
            Role = user.Role.ToString(),
            ProfilePicturePath = user.ProfilePicturePath
        });
    }
}
