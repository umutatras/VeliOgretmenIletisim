using MediatR;
using VeliOgretmenIletisim.Application.Common.Models;

namespace VeliOgretmenIletisim.Application.Features.Dashboard.Queries.GetStats;

public class DashboardStatsDto
{
    public int ActiveAnnouncementsCount { get; set; }
    public int PendingAppointmentsCount { get; set; }
    public int CompletedComplaintsCount { get; set; }
    public int TotalStudentsCount { get; set; }
    public List<RecentAnnouncementDto> RecentAnnouncements { get; set; } = new();
}

public class RecentAnnouncementDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public class GetDashboardStatsQuery : IRequest<Result<DashboardStatsDto>>
{
}
