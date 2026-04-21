using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Appointments.Queries.GetAppointments;

public record AppointmentDto(Guid Id, string ParentName, string StudentName, string TeacherName, DateTime AppointmentDate, string Status, string? Note);

public class GetAppointmentsQuery : IRequest<Result<PagedResult<AppointmentDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public GetAppointmentsQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
