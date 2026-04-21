using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Students.Commands.DeleteStudent;

public record DeleteStudentCommand(Guid Id) : IRequest<Result>;
