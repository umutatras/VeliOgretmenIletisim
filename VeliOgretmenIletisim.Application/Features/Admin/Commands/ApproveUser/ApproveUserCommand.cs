using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Admin.Commands.ApproveUser;

public class ApproveUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
}
