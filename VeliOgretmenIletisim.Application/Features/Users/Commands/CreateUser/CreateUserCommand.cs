using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<Result<Guid>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
