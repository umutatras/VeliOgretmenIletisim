using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<Result>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; } // Sadece Teacher veya Parent olabilir
}
