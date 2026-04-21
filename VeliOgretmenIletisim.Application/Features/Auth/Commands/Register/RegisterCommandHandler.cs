using MediatR;
using Microsoft.AspNetCore.Identity;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _uow;

    public RegisterCommandHandler(UserManager<AppUser> userManager, IUnitOfWork uow)
    {
        _userManager = userManager;
        _uow = uow;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return Result.Failure("Email adresi zaten kullanımda.");

        if (request.Role == UserRole.Admin)
            return Result.Failure("Bu işlem ile Admin hesabı oluşturulamaz.");

        var appUser = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            Role = request.Role,
            IsActive = false, // Onaylanana kadar pasif
            IsApproved = false // Admin onayı bekliyor
        };

        var result = await _userManager.CreateAsync(appUser, request.Password);

        if (!result.Succeeded)
        {
            return Result.Failure(result.Errors.Select(e => e.Description).FirstOrDefault() ?? "Kayıt sırasında hata oluştu.");
        }

        // Role göre profil oluşturma
        if (request.Role == UserRole.Teacher)
        {
            var teacher = new Teacher { AppUserId = appUser.Id };
            await _uow.GetRepository<Teacher>().AddAsync(teacher);
        }
        else if (request.Role == UserRole.Parent)
        {
            var parent = new Parent { AppUserId = appUser.Id };
            await _uow.GetRepository<Parent>().AddAsync(parent);
        }

        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success("Kayıt işleminiz başarılı. Lütfen üyeliğinizin yönetici tarafından onaylanmasını bekleyiniz.");
    }
}
