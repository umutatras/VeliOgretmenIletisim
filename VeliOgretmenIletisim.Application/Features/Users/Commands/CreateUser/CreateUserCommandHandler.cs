using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;

    public CreateUserCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Manual Mapping: Command -> Entity
        var user = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true
        };

        await _uow.GetRepository<AppUser>().AddAsync(user);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(user.Id, "User created successfully.", 201);
    }
}
