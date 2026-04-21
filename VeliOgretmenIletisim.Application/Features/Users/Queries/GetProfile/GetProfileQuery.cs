using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Users.Queries.GetProfile;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfilePicturePath { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class GetProfileQuery : IRequest<Result<UserProfileDto>>
{
}
