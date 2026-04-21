using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Auth.Queries.Login;

public class LoginQuery : IRequest<Result<LoginResponseDto>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginResponseDto
{
    public string Token { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
    public string? ProfilePicturePath { get; set; }
}
