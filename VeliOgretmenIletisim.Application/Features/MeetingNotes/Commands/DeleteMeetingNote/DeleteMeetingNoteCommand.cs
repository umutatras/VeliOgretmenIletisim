using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.MeetingNotes.Commands.DeleteMeetingNote;

public record DeleteMeetingNoteCommand(Guid Id) : IRequest<Result>;
