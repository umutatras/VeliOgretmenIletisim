using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Users.Commands.UpdateProfilePicture;

public class UpdateProfilePictureCommand : IRequest<Result<string>>
{
    public string ProfilePicturePath { get; set; } = string.Empty;
}
