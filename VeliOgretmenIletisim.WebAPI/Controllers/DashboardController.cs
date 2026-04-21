using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeliOgretmenIletisim.Application.Features.Dashboard.Queries.GetStats;

namespace VeliOgretmenIletisim.WebAPI.Controllers;

[Authorize]
public class DashboardController : BaseApiController
{
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        return HandleResult(await Mediator.Send(new GetDashboardStatsQuery()));
    }
}
