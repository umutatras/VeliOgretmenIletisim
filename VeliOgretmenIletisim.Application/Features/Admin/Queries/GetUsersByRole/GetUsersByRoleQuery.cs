using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Admin.Queries.GetUsersByRole;

public record UserBriefDto(Guid Id, string FullName, string Email);

public class GetUsersByRoleQuery : IRequest<Result<List<UserBriefDto>>>
{
    public UserRole Role { get; set; }

    public GetUsersByRoleQuery(UserRole role)
    {
        Role = role;
    }
}
