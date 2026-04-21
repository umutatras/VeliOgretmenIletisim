using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public RegisterCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _uow.GetRepository<AppUser>().Where(x => x.Email == request.Email).AnyAsync(cancellationToken))
            return Result.Failure("Email is already in use.");

        if (request.Role == UserRole.Admin)
            return Result.Failure("Cannot register an admin user through this endpoint.");

        var appUser = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            IsActive = false, // Onaylanana kadar pasif
            IsApproved = false // Onay bekliyor
        };

        await _uow.GetRepository<AppUser>().AddAsync(appUser);

        // Role göre profil oluşturma
        if (request.Role == UserRole.Teacher)
        {
            var teacher = new Teacher { AppUser = appUser };
            await _uow.GetRepository<Teacher>().AddAsync(teacher);
        }
        else if (request.Role == UserRole.Parent)
        {
            var parent = new Parent { AppUser = appUser };
            await _uow.GetRepository<Parent>().AddAsync(parent);
        }

        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success("Registration successful. Please wait for admin approval.");
    }
}
