using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Application.Interfaces.Security;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Application.Features.Appointments.Queries.GetAppointments;

public class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, Result<PagedResult<AppointmentDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public GetAppointmentsQueryHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResult<AppointmentDto>>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var query = _uow.GetRepository<Appointment>().GetAll()
            .Include(a => a.Parent).ThenInclude(p => p.AppUser)
            .Include(a => a.Student)
            .Include(a => a.Availability).ThenInclude(v => v.Teacher).ThenInclude(t => t.AppUser)
            .AsQueryable();

        // Role-based filtering
        var userRole = _currentUserService.Role;
        if (userRole == "Parent")
        {
            query = query.Where(a => a.Parent.AppUserId == userId);
        }
        else if (userRole == "Teacher")
        {
            query = query.Where(a => a.Availability.Teacher.AppUserId == userId);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(a => a.Availability.StartTime)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AppointmentDto(
                a.Id,
                a.Parent.AppUser.FullName,
                a.Student != null ? $"{a.Student.FirstName} {a.Student.LastName}" : "N/A",
                a.Availability.Teacher.AppUser.FullName,
                a.Availability.StartTime,
                a.Status.ToString(),
                a.Note
            ))
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<AppointmentDto>(items, totalCount, request.PageNumber, request.PageSize);
        return Result<PagedResult<AppointmentDto>>.Success(pagedResult);
    }
}
