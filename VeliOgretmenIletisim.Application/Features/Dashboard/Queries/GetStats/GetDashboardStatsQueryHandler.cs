using MediatR;
using Microsoft.EntityFrameworkCore;
using VeliOgretmenIletisim.Application.Common.Models;
using VeliOgretmenIletisim.Application.Interfaces.Repositories;
using VeliOgretmenIletisim.Domain.Entities;
using VeliOgretmenIletisim.Domain.Enums;

namespace VeliOgretmenIletisim.Application.Features.Dashboard.Queries.GetStats;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
{
    private readonly IUnitOfWork _uow;

    public GetDashboardStatsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = new DashboardStatsDto
        {
            ActiveAnnouncementsCount = await _uow.GetRepository<Announcement>().GetAll().CountAsync(cancellationToken),
            PendingAppointmentsCount = await _uow.GetRepository<Appointment>().GetAll().CountAsync(a => a.Status == AppointmentStatus.Pending, cancellationToken),
            CompletedComplaintsCount = await _uow.GetRepository<ComplaintRequest>().GetAll().CountAsync(c => c.Status == ComplaintStatus.Resolved, cancellationToken),
            TotalStudentsCount = await _uow.GetRepository<Student>().GetAll().CountAsync(cancellationToken),
            RecentAnnouncements = await _uow.GetRepository<Announcement>().GetAll()
                .OrderByDescending(a => a.CreatedDate)
                .Take(5)
                .Select(a => new RecentAnnouncementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    CreatedDate = a.CreatedDate
                })
                .ToListAsync(cancellationToken)
        };

        return Result<DashboardStatsDto>.Success(stats);
    }
}
