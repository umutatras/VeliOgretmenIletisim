using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.MeetingNotes.Commands.CreateMeetingNote;

public class CreateMeetingNoteCommand : IRequest<Result>
{
    public Guid ParentId { get; set; }
    public string Note { get; set; }
}
