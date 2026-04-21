using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Announcements.Commands.CreateAnnouncement;

public class CreateAnnouncementCommand : IRequest<Result>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? AttachmentPath { get; set; }
}
